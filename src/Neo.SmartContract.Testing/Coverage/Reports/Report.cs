using Palmmedia.ReportGenerator.Core;
using System.IO;

namespace Neo.SmartContract.Testing.Coverage.Reports
{
    public class Report
    {
        /// <summary>
        /// Generate report from cobertura
        /// </summary>
        /// <param name="file">Coverage file</param>
        /// <param name="outputDir">Output dir</param>
        /// <returns>True if was success</returns>
        public static bool CreateReport(string file, string outputDir)
        {
            try
            {
                // Reporting

                Program.Main(new string[] { $"-reports:{Path.GetFullPath(file)}", $"-targetdir:{Path.GetFullPath(outputDir)}" });
                return true;
            }
            catch { }

            return false;
        }
    }
}
