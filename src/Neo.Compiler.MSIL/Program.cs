using Neo.Compiler.MSIL;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neo.Compiler
{
    public class Program
    {
        //Console.WriteLine("helo ha:"+args[0]); //普通输出
        //Console.WriteLine("<WARN> 这是一个严重的问题。");//警告输出，黄字
        //Console.WriteLine("<WARN|aaaa.cs(1)> 这是ee一个严重的问题。");//警告输出，带文件名行号
        //Console.WriteLine("<ERR> 这是一个严重的问题。");//错误输出，红字
        //Console.WriteLine("<ERR|aaaa.cs> 这是ee一个严重的问题。");//错误输出，带文件名
        //Console.WriteLine("SUCC");//输出这个表示编译成功
        //控制台输出约定了特别的语法
        public static void Main(string[] args)
        {
            // Set console
            Console.OutputEncoding = Encoding.UTF8;
            var log = new DefLogger();
            log.Log("Neo.Compiler.MSIL console app v" + Assembly.GetEntryAssembly().GetName().Version);

            // Check argmuents
            if (args.Length == 0)
            {
                log.Log("You need a parameter to specify the DLL or the file name of the project.");
                log.Log("Examples: ");
                log.Log("  neon mySmartContract.dll");
                log.Log("  neon mySmartContract.csproj");

                Environment.Exit(-1);
                return;
            }

            var fileInfo = new FileInfo(args[0]);

            // Set current directory
            if (!fileInfo.Exists)
            {
                log.Log("Could not find file " + fileInfo.FullName);
                Environment.Exit(-1);
                return;
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
                    Environment.Exit(-1);
                    return;
                }
            }

            switch (fileInfo.Extension.ToLowerInvariant())
            {
                case ".csproj":
                    {
                        // Compile csproj file

                        log.Log("Compiling from csproj source");
                        var output = Compiler.CompileCSProj(fileInfo.FullName);
                        fs = new MemoryStream(output.Dll);
                        fspdb = new MemoryStream(output.Pdb);
                        break;
                    }
                case ".cs":
                    {
                        // Compile C# files

                        log.Log("Compiling from c# source");
                        var output = Compiler.CompileCSFile(new string[] { fileInfo.FullName }, new string[0]);
                        fs = new MemoryStream(output.Dll);
                        fspdb = new MemoryStream(output.Pdb);
                        break;
                    }
                case ".vb":
                    {
                        // Compile VB files

                        log.Log("Compiling from VB source");
                        var output = Compiler.CompileVBFile(new string[] { fileInfo.FullName }, new string[0]);
                        fs = new MemoryStream(output.Dll);
                        fspdb = new MemoryStream(output.Pdb);
                        break;
                    }
                default:
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
                            return;
                        }
                        break;
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
                return;
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

                try
                {
                    var outjson = vmtool.FuncExport.Export(module, bytes);
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
                return;
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
                return;
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
                return;
            }

            try
            {
                var features = module == null ? ContractFeatures.NoProperty : module.attributes
                    .Where(u => u.AttributeType.Name == "FeaturesAttribute")
                    .Select(u => (ContractFeatures)u.ConstructorArguments.FirstOrDefault().Value)
                    .FirstOrDefault();

                var storage = features.HasFlag(ContractFeatures.HasStorage).ToString().ToLowerInvariant();
                var payable = features.HasFlag(ContractFeatures.Payable).ToString().ToLowerInvariant();

                string manifest = onlyname + ".manifest.json";
                string defManifest =
                    @"{""groups"":[],""features"":{""storage"":" + storage + @",""payable"":" + payable + @"},""abi"":" +
                    jsonstr +
                    @",""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""safeMethods"":[]}";

                File.Delete(manifest);
                File.WriteAllText(manifest, defManifest);
                log.Log("write:" + manifest);
                bSucc++;
            }
            catch (Exception err)
            {
                log.Log("Write manifest Error:" + err.ToString());
                return;
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
            }
        }
    }
}
