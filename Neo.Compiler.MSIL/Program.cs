using Neo.Compiler.MSIL;
using System;
using System.IO;
using System.IO.Compression;
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
        public static int Main(string[] args)
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

                return -1;
            }

            bool bCompatible = false;
            FileInfo fileInfo = null;
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    if (args[i] == "--compatible")
                    {
                        bCompatible = true;
                    }

                    // Other option
                }
                else
                {
                    fileInfo = new FileInfo(args[i]);
                }
            }

            if (fileInfo == null)
            {
                log.Log("Need one param for filename (DLL or source)");
                log.Log("[--compatible] disable nep8 function and disable SyscallInteropHash");
                log.Log("Example:neon abc.dll --compatible");
                return 0;
            }

            // Set current directory

            if (!fileInfo.Exists)
            {
                log.Log("Could not find file " + fileInfo.FullName);
                return -1;
            }

            if (bCompatible)
            {
                log.Log("use --compatible no nep8 and no SyscallInteropHash");
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
            bool bSucc;
            string jsonstr = null;
            // Convert and build
            string debugstr = null;
            try
            {
                var conv = new ModuleConverter(log);
                ConvOption option = new ConvOption
                {
                    useNep8 = !bCompatible,
                    useSysCallInteropHash = !bCompatible
                };
                NeoModule am = conv.Convert(mod, option);
                bytes = am.Build();
                log.Log("convert succ");

                try
                {
                    var outjson = vmtool.FuncExport.Export(am, bytes);
                    StringBuilder sb = new StringBuilder();
                    outjson.ConvertToStringWithFormat(sb, 0);
                    jsonstr = sb.ToString();
                    log.Log("gen abi succ");
                }
                catch (Exception err)
                {
                    log.Log("gen abi Error:" + err.ToString());
                }

                try
                {
                    var outjson = DebugExport.Export(am);
                    StringBuilder sb = new StringBuilder();
                    outjson.ConvertToString(sb);
                    debugstr = sb.ToString();
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

                string bytesname = onlyname + ".avm";

                File.Delete(bytesname);
                File.WriteAllBytes(bytesname, bytes);
                log.Log("write:" + bytesname);
                bSucc = true;
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
                bSucc = true;
            }
            catch (Exception err)
            {
                log.Log("Write abi Error:" + err.ToString());
                return -1;
            }

            try
            {
                string debugname = onlyname + ".debug.json";
                string debugzip = onlyname + ".avmdbgnfo";

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
                bSucc = true;
            }
            catch (Exception err)
            {
                log.Log("Write debug Error:" + err.ToString());
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

            if (bSucc)
            {
                log.Log("SUCC");
                return 1;
            }

            return -1;
        }
    }
}
