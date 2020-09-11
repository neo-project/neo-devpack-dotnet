using Neo.VM;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("Neo.SmartContract.Framework.UnitTests")]
namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    internal static class NeonTestTool
    {
        /// <summary>
        /// Is not the official script hash, just a unique hash related to the script used for unit test purpose
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>UInt160</returns>
        public static UInt160 ScriptHash(this ExecutionContext context)
        {
            using (var sha = SHA1.Create())
            {
                return new UInt160(sha.ComputeHash(((byte[])context.Script)));
            }
        }

        public static string Bytes2HexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Append(d.ToString("x02"));
            }
            return sb.ToString();
        }

        public static byte[] HexString2Bytes(string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="filename">File</param>
        /// <param name="releaseMode">Release mode (default=false)</param>
        /// <param name="optimizer">Optimize script (default=false)</param>
        /// <returns>BuildScript</returns>
        public static BuildScript BuildScript(string filename, bool releaseMode = false, bool optimizer = false)
        {
            return BuildScript(new string[] { filename }, releaseMode, optimizer);
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="filenames">Files</param>
        /// <param name="releaseMode">Release mode (default=false)</param>
        /// <param name="optimizer">Optimize script (default=false)</param>
        /// <returns>BuildScript</returns>
        public static BuildScript BuildScript(string[] filenames, bool releaseMode = false, bool optimizer = false)
        {
            var ext = System.IO.Path.GetExtension(filenames.First());
            var comp = (ext.ToLowerInvariant()) switch
            {
                ".cs" => Compiler.CompileCSFiles(filenames, new string[0] { }, releaseMode),
                ".vb" => Compiler.CompileVBFiles(filenames, new string[0] { }, releaseMode),
                _ => throw new System.Exception("do not support extname = " + ext),
            };

            using (var streamDll = new MemoryStream(comp.Dll))
            using (var streamPdb = new MemoryStream(comp.Pdb))
            {
                var bs = new BuildScript();
                bs.Build(streamDll, streamPdb, optimizer);

                if (bs.Error != null)
                {
                    throw (bs.Error);
                }

                if (bs.Error != null) throw bs.Error;

                return bs;
            }
        }
    }
}
