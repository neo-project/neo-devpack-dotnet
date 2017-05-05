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

            //this.Log.LogMessage("LogMessage AntShares IL Convert 0.01.");//can't show
            this.Log.LogMessageFromText("LogMessageFromText AntShares IL Convert 0.01.", MessageImportance.High);



            string dllname = System.IO.Path.GetFileName(srcdll);
            ProcessStartInfo pinfo = new ProcessStartInfo();
            pinfo.FileName = "cmd.exe";
            pinfo.WorkingDirectory = System.IO.Path.GetDirectoryName(srcdll);
            pinfo.UseShellExecute = false;
            pinfo.RedirectStandardInput = true;
            pinfo.RedirectStandardOutput = true;
            pinfo.CreateNoWindow = true;
            Process p = Process.Start(pinfo);
            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine("antshares.compiler.msil " + dllname);
            p.StandardInput.WriteLine("exit");
            
            //前四行后一行不要
            string lastline = null;
            int count = 0;
            bool bSucc = false;
            while (p.StandardOutput.EndOfStream == false)
            {
                if (lastline != null && lastline.Length > 0)
                {
                    if (lastline[0] == '<')
                    {
                        if (lastline.IndexOf("<WARN>") == 0)
                        {
                            this.Log.LogWarning("", "", "", "", 0, 0, 0, 0, lastline.Substring(6));
                            continue;
                        }
                        else if (lastline.IndexOf("<ERR>") == 0)
                        {
                            this.Log.LogError("", "", "", "", 0, 0, 0, 0, lastline.Substring(5));
                            continue;
                        }
                        else if (lastline.IndexOf("<WRANCODE|") == 0)
                        {
                            var l = lastline.Substring(10);
                            var ine = lastline.IndexOf(">");
                            var text = lastline.Substring(ine + 1);
                            var file = lastline.Substring(10, ine - 10);
                            var lines = file.Split(new char[] { '(', ')' });
                            int _line = 0;
                            if (lines.Length > 1)
                            {
                                int.TryParse(lines[1], out _line);
                            }
                            this.Log.LogWarning("", "", "", lines[0], _line, 0, 0, 0, text);
                            continue;
                        }
                        else if (lastline.IndexOf("<ERRCODE|") == 0)
                        {
                            var l = lastline.Substring(9);
                            var ine = lastline.IndexOf(">");
                            var text = lastline.Substring(ine + 1);
                            var file = lastline.Substring(9, ine - 9);
                            var lines = file.Split(new char[] { '(', ')' });
                            int _line = 0;
                            if (lines.Length > 1)
                            {
                                int.TryParse(lines[1], out _line);
                            }
                            this.Log.LogWarning("", "", "", lines[0], _line, 0, 0, 0, text);
                            continue;
                        }
                    }
                    if(lastline.IndexOf("SUCC")==0)
                    {
                        bSucc = true;
                    }
                    this.Log.LogMessageFromText(lastline, MessageImportance.High);

                }
                var line = p.StandardOutput.ReadLine();
                count++;
                if (count <= 4) continue;
                lastline = line;
                //lines.Add(line);
            }

            //this.Log.LogMessageFromText(lastline, MessageImportance.High);

            p.WaitForExit();
            return bSucc;
        }
    }

}