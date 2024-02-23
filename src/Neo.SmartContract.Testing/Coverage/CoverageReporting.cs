using Palmmedia.ReportGenerator.Core;
using System.IO;

namespace Neo.SmartContract.Testing.Coverage
{
    public class CoverageReporting
    {
        /// <summary>
        /// Generate report from cobertura
        /// </summary>
        /// <param name="file">Coverage file</param>
        /// <param name="outputDir">Output dir</param>
        /// <param name="license">License</param>
        /// <returns>True if was success</returns>
        public static bool CreateReport(string file, string outputDir, string? license = null)
        {
            try
            {
                // Reporting

                if (string.IsNullOrEmpty(license))
                {
                    Program.Main(new string[] { $"-reports:{Path.GetFullPath(file)}", $"-targetdir:{Path.GetFullPath(outputDir)}" });
                }
                else
                {
                    Program.Main(new string[] { $"-reports:{Path.GetFullPath(file)}", $"-targetdir:{Path.GetFullPath(outputDir)}", $"-license:{license}" });
                }
                return true;
            }
            catch { }

            return false;
        }
    }
}
