using CommandLine;
using Mono.Cecil;
using Neo.Compiler.MSIL;
using Neo.Compiler.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neo.Compiler
{
    public class Program
    {
        public class Options
        {
            [Option('f', "file", Required = true, HelpText = "File for compile.")]
            public string File { get; set; }

            [Option('o', "optimize", Required = false, HelpText = "Optimize.")]
            public bool Optimize { get; set; } = false;
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => Environment.ExitCode = Compile(o));
        }

        public static int Compile(Options options)
        {
            // Set console
            Console.OutputEncoding = Encoding.UTF8;
            var log = new DefLogger();
            log.Log("Neo.Compiler.MSIL console app v" + Assembly.GetEntryAssembly().GetName().Version);

            var fileInfo = new FileInfo(options.File);

            // Set current directory
            if (!fileInfo.Exists)
            {
                log.Log("Could not find file " + fileInfo.FullName);
                return -1;
            }

            Stream fs;
            Stream fspdb;
            var onlyname = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var path = fileInfo.Directory.FullName;

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    Directory.SetCurrentDirectory(path);
                }
                catch
                {
                    log.Log("Could not find path: " + path);
                    return -1;
                }
            }

            switch (fileInfo.Extension.ToLowerInvariant())
            {
                case ".csproj":
                    {
                        // Compile csproj file

                        log.Log("Compiling from csproj project");
                        var output = Compiler.CompileCSProj(fileInfo.FullName);
                        fs = new MemoryStream(output.Dll);
                        fspdb = new MemoryStream(output.Pdb);
                        break;
                    }
                case ".vbproj":
                    {
                        // Compile vbproj file

                        log.Log("Compiling from vbproj project");
                        var output = Compiler.CompileVBProj(fileInfo.FullName);
                        fs = new MemoryStream(output.Dll);
                        fspdb = new MemoryStream(output.Pdb);
                        break;
                    }
                case ".cs":
                    {
                        // Compile C# files

                        log.Log("Compiling from c# source");
                        var output = Compiler.CompileCSFiles(new string[] { fileInfo.FullName }, new string[0]);
                        fs = new MemoryStream(output.Dll);
                        fspdb = new MemoryStream(output.Pdb);
                        break;
                    }
                case ".vb":
                    {
                        // Compile VB files

                        log.Log("Compiling from VB source");
                        var output = Compiler.CompileVBFiles(new string[] { fileInfo.FullName }, new string[0]);
                        fs = new MemoryStream(output.Dll);
                        fspdb = new MemoryStream(output.Pdb);
                        break;
                    }
                case ".dll":
                    {
                        string filepdb = onlyname + ".pdb";

                        // Open file
                        try
                        {
                            fs = fileInfo.OpenRead();

                            if (File.Exists(filepdb))
                            {
                                fspdb = File.OpenRead(filepdb);
                            }
                            else
                            {
                                fspdb = null;
                            }
                        }
                        catch (Exception err)
                        {
                            log.Log("Open File Error:" + err.ToString());
                            return -1;
                        }
                        break;
                    }
                default:
                    {
                        log.Log("File format not supported by neon: " + path);
                        return -1;
                    }
            }

            ILModule mod = new ILModule(log);

            // Load module
            try
            {
                mod.LoadModule(fs, fspdb);
            }
            catch (Exception err)
            {
                log.Log("LoadModule Error:" + err.ToString());
                return -1;
            }
            byte[] bytes;
            int bSucc = 0;
            string jsonstr = null;
            NeoModule module = null;

            // Convert and build
            try
            {
                var conv = new ModuleConverter(log);
                ConvOption option = new ConvOption();
                module = conv.Convert(mod, option);
                bytes = module.Build();
                log.Log("convert succ");
                var funcAddrList = module.ConvertFuncAddr();
                if (options.Optimize)
                {
                    var optimize = NefOptimizeTool.Optimize(bytes, funcAddrList);
                    log.Log("optimization succ " + (((bytes.Length / (optimize.Length + 0.0)) * 100.0) - 100).ToString("0.00 '%'"));
                    bytes = optimize;
                }

                try
                {
                    var outjson = vmtool.FuncExport.Export(module, bytes, funcAddrList);
                    StringBuilder sb = new StringBuilder();
                    outjson.ConvertToStringWithFormat(sb, 0);
                    jsonstr = sb.ToString();
                    log.Log("gen abi succ");
                }
                catch (Exception err)
                {
                    log.Log("gen abi Error:" + err.ToString());
                }

            }
            catch (Exception err)
            {
                log.Log("Convert Error:" + err.ToString());
                return -1;
            }

            // Write bytes

            try
            {
                string bytesname = onlyname + ".nef";
                var nef = new NefFile
                {
                    Compiler = "neon",
                    Version = Version.Parse(((AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly()
                        .GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version),
                    Script = bytes,
                    ScriptHash = bytes.ToScriptHash()
                };
                nef.CheckSum = NefFile.ComputeChecksum(nef);

                File.Delete(bytesname);
                using (var stream = File.OpenWrite(bytesname))
                using (var writer = new BinaryWriter(stream))
                {
                    nef.Serialize(writer);
                }
                log.Log("write:" + bytesname);
                bSucc++;
            }
            catch (Exception err)
            {
                log.Log("Write Bytes Error:" + err.ToString());
                return -1;
            }

            try
            {
                string abiname = onlyname + ".abi.json";

                File.Delete(abiname);
                File.WriteAllText(abiname, jsonstr);
                log.Log("write:" + abiname);
                bSucc++;
            }
            catch (Exception err)
            {
                log.Log("Write abi Error:" + err.ToString());
                return -1;
            }

            try
            {
                var features = module == null ? ContractFeatures.NoProperty : module.attributes
                    .Where(u => u.AttributeType.Name == "FeaturesAttribute")
                    .Select(u => (ContractFeatures)u.ConstructorArguments.FirstOrDefault().Value)
                    .FirstOrDefault();

                var extraAttributes = module == null ? new List<Mono.Collections.Generic.Collection<CustomAttributeArgument>>() : module.attributes.Where(u => u.AttributeType.Name == "ManifestExtraAttribute").Select(attribute => attribute.ConstructorArguments).ToList();

                var extra = BuildExtraAttributes(extraAttributes);
                var storage = features.HasFlag(ContractFeatures.HasStorage).ToString().ToLowerInvariant();
                var payable = features.HasFlag(ContractFeatures.Payable).ToString().ToLowerInvariant();

                string manifest = onlyname + ".manifest.json";
                string defManifest =
                    @"{""groups"":[],""features"":{""storage"":" + storage + @",""payable"":" + payable + @"},""abi"":" +
                    jsonstr +
                    @",""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""safeMethods"":[],""extra"":" + extra + "}";

                File.Delete(manifest);
                File.WriteAllText(manifest, defManifest);
                log.Log("write:" + manifest);
                bSucc++;
            }
            catch (Exception err)
            {
                log.Log("Write manifest Error:" + err.ToString());
                return -1;
            }

            try
            {
                fs.Dispose();
                if (fspdb != null)
                    fspdb.Dispose();
            }
            catch
            {
            }

            if (bSucc == 3)
            {
                log.Log("SUCC");
                return 0;
            }

            return -1;
        }

        private static string BuildExtraAttributes(List<Mono.Collections.Generic.Collection<CustomAttributeArgument>> extraAttributes)
        {
            if (extraAttributes.Count == 0)
            {
                return "null";
            }

            string extra = "{";
            foreach (var extraAttribute in extraAttributes)
            {
                var key = extraAttribute[0].Value;
                var value = extraAttribute[1].Value;
                extra += ($"\"{key}\":\"{value}\",");
            }
            extra = extra.Substring(0, extra.Length - 1);
            extra += "}";

            return extra;
        }
    }
}
