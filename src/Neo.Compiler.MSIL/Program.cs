using CommandLine;
using Mono.Cecil;
using Neo.Compiler.MSIL;
using Neo.Compiler.Optimizer;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public static int Compile(Options options, ILogger log = null)
        {
            // Set console
            Console.OutputEncoding = Encoding.UTF8;
            log ??= new DefLogger();
            log.Log("Neo.Compiler.MSIL console app v" + Assembly.GetAssembly(typeof(Program)).GetName().Version);

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
            JObject abi;
            byte[] bytes;
            int bSucc = 0;
            string debugstr = null;
            NeoModule module;

            // Convert and build
            try
            {
                var conv = new ModuleConverter(log);
                ConvOption option = new ConvOption();
                module = conv.Convert(mod, option);
                bytes = module.Build();
                log.Log("convert succ");
                Dictionary<int, int> addrConvTable = null;
                if (options.Optimize)
                {
                    HashSet<int> entryPoints = new HashSet<int>();
                    foreach (var func in module.mapMethods)
                    {
                        entryPoints.Add(func.Value.funcaddr);
                    }
                    var optimize = NefOptimizeTool.Optimize(bytes, entryPoints.ToArray(), out addrConvTable);
                    log.Log("optimization succ " + (((bytes.Length / (optimize.Length + 0.0)) * 100.0) - 100).ToString("0.00 '%'"));
                    bytes = optimize;
                }

                try
                {
                    abi = FuncExport.GenerateAbi(module, addrConvTable);
                    log.Log("gen abi succ");
                }
                catch (Exception err)
                {
                    log.Log("gen abi Error:" + err.ToString());
                    return -1;
                }

                try
                {
                    var outjson = DebugExport.Export(module, bytes, addrConvTable);
                    debugstr = outjson.ToString(false);
                    log.Log("gen debug succ");
                }
                catch (Exception err)
                {
                    log.Log("gen debug Error:" + err.ToString());
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
                    Version = Version.Parse(((AssemblyFileVersionAttribute)Assembly.GetAssembly(typeof(Program))
                        .GetCustomAttribute(typeof(AssemblyFileVersionAttribute))).Version).ToString(),
                    Tokens = Array.Empty<MethodToken>(),
                    Script = bytes
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
                var sbABI = abi.ToString(false);
                string abiname = onlyname + ".abi.json";

                File.Delete(abiname);
                File.WriteAllText(abiname, sbABI.ToString());
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
                string debugname = onlyname + ".debug.json";
                string debugzip = onlyname + ".nefdbgnfo";

                var tempName = Path.GetTempFileName();
                File.Delete(tempName);
                File.WriteAllText(tempName, debugstr);
                File.Delete(debugzip);
                using (var archive = ZipFile.Open(debugzip, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(tempName, Path.GetFileName(debugname));
                }
                File.Delete(tempName);
                log.Log("write:" + debugzip);
                bSucc++;
            }
            catch (Exception err)
            {
                log.Log("Write debug Error:" + err.ToString());
                return -1;
            }

            try
            {
                string manifest = onlyname + ".manifest.json";
                var defManifest = FuncExport.GenerateManifest(abi, module);

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

            if (bSucc == 4)
            {
                log.Log("SUCC");
                return 0;
            }

            return -1;
        }
    }
}
