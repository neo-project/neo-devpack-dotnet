namespace AntShares
{

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// this class generate config / xml file by template
    /// </summary>
    public class ConvertTask : Task
    {
        [Required]
        public ITaskItem DataSource { get; set; }

        /// <summary>
        /// execute replace logic
        /// </summary>
        /// <returns>ture successful, false failure</returns>
        public override bool Execute()
        {
            var srcdll = this.DataSource.ToString();

            this.Log.LogMessage("LogMessage AntShares IL Convert 0.01.");
            this.Log.LogMessageFromText("LogMessageFromText AntShares IL Convert 0.01.", MessageImportance.High);
            this.Log.LogWarning("LogWarning AntShares IL Convert 0.01.", MessageImportance.High);

            this.Log.LogWarning("warn", "warn code", "", "abc.cs", 123, 0, 123, 0, "LogWarning with code: AntShares IL Convert 0.01.");

            this.Log.LogMessageFromText("src=" + this.DataSource.ToString(), MessageImportance.High);

            ProcessStartInfo pinfo = new ProcessStartInfo();
            pinfo.FileName = "cmd.exe";
            pinfo.WorkingDirectory = System.IO.Path.GetDirectoryName(srcdll);
            pinfo.UseShellExecute = false;
            pinfo.RedirectStandardInput = true;
            pinfo.RedirectStandardOutput = true;
            pinfo.CreateNoWindow = true;
            Process p = Process.Start(pinfo);
            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine("tsc");
            p.StandardInput.WriteLine("exit");

            List<string> lines = new List<string>();
            while(p.StandardOutput.EndOfStream==false)
            {
                lines.Add(p.StandardOutput.ReadLine());
            }
            string all=           p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}