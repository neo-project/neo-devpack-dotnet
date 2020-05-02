using Mono.Cecil;
using Neo.Compiler.Optimizer;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    public class BuildScript
    {
        public bool IsBuild { get; private set; }
        public bool UseOptimizer { get; private set; }
        public Exception Error { get; private set; }
        public ILModule ModIL { get; private set; }
        public ModuleConverter ConverterIL { get; private set; }
        public byte[] FinalNEF { get; private set; }
        public MyJson.JsonNode_Object FinialABI { get; private set; }
        public string FinalManifest { get; private set; }

        public BuildScript()
        {
        }

        public void Build(Stream fs, Stream fspdb, bool optimizer)
        {
            this.IsBuild = false;
            this.Error = null;
            this.UseOptimizer = optimizer;

            var log = new DefLogger();
            this.ModIL = new ILModule(log);
            try
            {
                ModIL.LoadModule(fs, fspdb);
            }
            catch (Exception err)
            {
                log.Log("LoadModule Error:" + err.ToString());
                this.Error = err;
                return;
            }

            ConverterIL = new ModuleConverter(log);
            Dictionary<int, int> addrConvTable = null;
            ConvOption option = new ConvOption();
#if NDEBUG
            try

#endif
            {
                ConverterIL.Convert(ModIL, option);
                FinalNEF = ConverterIL.outModule.Build();
                if (optimizer)
                {
                    List<int> entryPoints = new List<int>();
                    foreach (var f in ConverterIL.outModule.mapMethods.Values)
                    {
                        entryPoints.Add(f.funcaddr);
                    }
                    var opbytes = NefOptimizeTool.Optimize(FinalNEF, entryPoints.ToArray(), out addrConvTable);
                    float ratio = (opbytes.Length * 100.0f) / (float)FinalNEF.Length;
                    log.Log("optimization ratio = " + ratio + "%");
                    FinalNEF = opbytes;
                }
                IsBuild = true;
            }
#if NDEBUG
            catch (Exception err)
            {
                this.Error = err;
                log.Log("Convert IL->ASM Error:" + err.ToString());
                return;
            }
#endif
            try
            {
                FinialABI = vmtool.FuncExport.Export(ConverterIL.outModule, FinalNEF, addrConvTable);
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                var features = ConverterIL.outModule == null ? ContractFeatures.NoProperty : ConverterIL.outModule.attributes
                    .Where(u => u.AttributeType.Name == "FeaturesAttribute")
                    .Select(u => (ContractFeatures)u.ConstructorArguments.FirstOrDefault().Value)
                    .FirstOrDefault();

                var extraAttributes = ConverterIL.outModule == null ? new List<Mono.Collections.Generic.Collection<CustomAttributeArgument>>()
                    : ConverterIL.outModule.attributes.Where(u => u.AttributeType.Name == "ManifestExtraAttribute").Select(attribute => attribute.ConstructorArguments).ToList();
                var storage = features.HasFlag(ContractFeatures.HasStorage).ToString().ToLowerInvariant();
                var payable = features.HasFlag(ContractFeatures.Payable).ToString().ToLowerInvariant();

                FinalManifest =
                    @"{""groups"":[],""features"":{""storage"":" + storage + @",""payable"":" + payable + @"},""abi"":" +
                    FinialABI +
                    @",""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""safeMethods"":[],""extra"":[]" + "}";
            }
            catch
            {
            }
        }

        public string[] GetAllILFunction()
        {
            List<string> lists = new List<string>();
            foreach (var _class in ModIL.mapType)
            {
                foreach (var method in _class.Value.methods)
                {
                    var name = method.Key;
                    lists.Add(name);
                }
            }
            return lists.ToArray();
        }

        public ILMethod FindMethod(string fromclass, string method)
        {
            foreach (var _class in ModIL.mapType)
            {
                var indexbegin = _class.Key.LastIndexOf(".");
                var classname = _class.Key;
                if (indexbegin > 0)
                    classname = classname.Substring(indexbegin + 1);

                if (classname == fromclass)
                {
                    foreach (var _method in _class.Value.methods)
                    {
                        var indexmethodname = _method.Key.LastIndexOf("::");
                        var methodname = _method.Key.Substring(indexmethodname + 2);
                        var indexparams = methodname.IndexOf("(");
                        if (indexparams > 0)
                        {
                            methodname = methodname.Substring(0, indexparams);
                        }
                        if (methodname == method)
                            return _method.Value;
                    }
                }
            }
            return null;
        }

        public string GetFullMethodName(string fromclass, string method)
        {
            foreach (var _class in ModIL.mapType)
            {
                var indexbegin = _class.Key.LastIndexOf("::");
                var classname = _class.Key.Substring(indexbegin + 2);
                if (classname == fromclass)
                {
                    foreach (var _method in _class.Value.methods)
                    {
                        var indexmethodname = _method.Key.LastIndexOf("::");
                        var methodname = _method.Key.Substring(indexmethodname + 2);
                        if (methodname == method)
                            return _method.Key;
                    }
                }
            }
            return null;
        }

        public NeoMethod GetNEOVMMethod(ILMethod method)
        {
            var neomethod = this.ConverterIL.methodLink[method];
            return neomethod;
        }
        public NeoMethod[] GetAllNEOVMMethod()
        {
            return new List<NeoMethod>(this.ConverterIL.methodLink.Values).ToArray();
        }

        public void DumpNEF()
        {
            {
                Console.WriteLine("dump:");
                foreach (var c in this.ConverterIL.outModule.totalCodes)
                {
                    var line = c.Key.ToString("X04") + "=>" + c.Value.ToString();
                    if (c.Value.bytes != null && c.Value.bytes.Length > 0)
                    {
                        line += " HEX:";
                        foreach (var b in c.Value.bytes)
                        {
                            line += b.ToString("X02");
                        }
                    }
                    Console.WriteLine(line);
                }

            }
        }
        public byte[] NeoMethodToBytes(NeoMethod method)
        {
            List<byte> bytes = new List<byte>();
            foreach (var c in method.body_Codes.Values)
            {
                bytes.Add((byte)c.code);
                if (c.bytes != null)
                    for (var i = 0; i < c.bytes.Length; i++)
                    {
                        bytes.Add(c.bytes[i]);
                    }
            }
            return bytes.ToArray();
        }

    }
}
