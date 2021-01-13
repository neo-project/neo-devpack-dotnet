using Neo.Compiler.Optimizer;
using Neo.IO.Json;
using Neo.SmartContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    public class BuildScript
    {
        public bool IsBuild { get; protected set; }
        public bool UseOptimizer { get; protected set; }
        public Exception Error { get; protected set; }
        public ILModule modIL { get; private set; }
        public ModuleConverter converterIL { get; private set; }
        public byte[] finalNEFScript { get; protected set; }
        public JObject finalABI { get; protected set; }
        public string finalManifest { get; protected set; }
        public JObject debugInfo { get; private set; }
        public NefFile nefFile { get; private set; }

        public BuildScript()
        {
        }

        public void Build(Stream fs, Stream fspdb, bool optimizer)
        {
            this.IsBuild = false;
            this.Error = null;
            this.UseOptimizer = optimizer;

            var log = new DefLogger();
            this.modIL = new ILModule(log);
            try
            {
                modIL.LoadModule(fs, fspdb);
            }
            catch (Exception err)
            {
                log.Log("LoadModule Error:" + err.ToString());
                this.Error = err;
                return;
            }

            converterIL = new ModuleConverter(log);
            Dictionary<int, int> addrConvTable = null;
            ConvOption option = new ConvOption();
#if NDEBUG
            try

#endif
            {
                converterIL.Convert(modIL, option);
                finalNEFScript = converterIL.outModule.Build();
                if (optimizer)
                {
                    List<int> entryPoints = new List<int>();
                    foreach (var f in converterIL.outModule.mapMethods.Values)
                    {
                        if (!entryPoints.Contains(f.funcaddr))
                            entryPoints.Add(f.funcaddr);
                    }
                    var opbytes = NefOptimizeTool.Optimize(finalNEFScript, entryPoints.ToArray(), out addrConvTable);
                    float ratio = (opbytes.Length * 100.0f) / (float)finalNEFScript.Length;
                    log.Log("optimization ratio = " + ratio + "%");
                    finalNEFScript = opbytes;
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
                finalABI = FuncExport.GenerateAbi(converterIL.outModule, addrConvTable);
            }
            catch (Exception err)
            {
                log.Log("Gen Abi Error:" + err.ToString());
                this.Error = err;
                return;
            }

            try
            {
                debugInfo = DebugExport.Export(converterIL.outModule, finalNEFScript, addrConvTable);
            }
            catch (Exception err)
            {
                log.Log("Gen debugInfo Error:" + err.ToString());
                this.Error = err;
                return;
            }

            try
            {
                finalManifest = FuncExport.GenerateManifest(finalABI, converterIL.outModule);
            }
            catch (Exception err)
            {
                log.Log("Gen Manifest Error:" + err.ToString());
                this.Error = err;
                return;
            }

            try
            {
                nefFile = new NefFile
                {
                    Compiler = "neon",
                    Version = Version.Parse(((AssemblyFileVersionAttribute)Assembly.GetAssembly(typeof(Program))
                        .GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version).ToString(),
                    Tokens = Array.Empty<MethodToken>(),
                    Script = finalNEFScript
                };
                nefFile.CheckSum = NefFile.ComputeChecksum(nefFile);
            }
            catch (Exception err)
            {
                log.Log("Write Bytes Error:" + err.ToString());
                return;
            }
        }

        public string[] GetAllILFunction()
        {
            List<string> lists = new List<string>();
            foreach (var _class in modIL.mapType)
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
            foreach (var _class in modIL.mapType)
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
            foreach (var _class in modIL.mapType)
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
            var neomethod = this.converterIL.methodLink[method];
            return neomethod;
        }
        public NeoMethod[] GetAllNEOVMMethod()
        {
            return new List<NeoMethod>(this.converterIL.methodLink.Values).ToArray();
        }

        public void DumpNEF()
        {
            {
                Console.WriteLine("dump:");
                foreach (var c in this.converterIL.outModule.totalCodes)
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
