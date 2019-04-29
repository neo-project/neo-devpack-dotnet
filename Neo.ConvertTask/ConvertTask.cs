using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace Neo
{
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
            string dllname = System.IO.Path.GetFileName(srcdll);
            ProcessStartInfo pinfo = new ProcessStartInfo();
            pinfo.FileName = "cmd.exe";
            pinfo.WorkingDirectory = System.IO.Path.GetDirectoryName(srcdll);
            pinfo.UseShellExecute = false;
            pinfo.RedirectStandardInput = true;
            pinfo.RedirectStandardOutput = true;
            pinfo.CreateNoWindow = true;
            pinfo.StandardOutputEncoding = System.Text.Encoding.UTF8;

            Process p = Process.Start(pinfo);
            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine($"neon {dllname} --compatible");
            p.StandardInput.WriteLine("exit");

            //前四行后一行不要
            string lastline = null;
            int count = 0;
            bool bSucc = false;
            while (p.StandardOutput.EndOfStream == false)
            {
                var line = p.StandardOutput.ReadLine();
                count++;
                if (count <= 4) continue;

                if (lastline != null && lastline.Length > 0)
                {
                    if (lastline[0] == '<')
                    {
                        if (lastline.IndexOf("<WARN>") == 0)
                        {
                            this.Log.LogWarning(lastline.Substring(6));
                            lastline = line;
                            continue;
                        }
                        else if (lastline.IndexOf("<ERR>") == 0)
                        {
                            this.Log.LogError(lastline.Substring(5));
                            lastline = line;
                            continue;
                        }
                        else if (lastline.IndexOf("<WARN|") == 0)
                        {
                            var l = lastline.Substring(6);
                            var ine = lastline.IndexOf(">");
                            var text = lastline.Substring(ine + 1);
                            var file = lastline.Substring(6, ine - 6);
                            var lines = file.Split(new char[] { '(', ')' });
                            int _line = 0;
                            if (lines.Length > 1)
                            {
                                int.TryParse(lines[1], out _line);
                            }
                            this.Log.LogWarning("", "", "", lines[0], _line, 0, 0, 0, text);
                            lastline = line;
                            continue;
                        }
                        else if (lastline.IndexOf("<ERR|") == 0)
                        {
                            var l = lastline.Substring(5);
                            var ine = lastline.IndexOf(">");
                            var text = lastline.Substring(ine + 1);
                            var file = lastline.Substring(5, ine - 5);
                            var lines = file.Split(new char[] { '(', ')' });
                            int _line = 0;
                            if (lines.Length > 1)
                            {
                                int.TryParse(lines[1], out _line);
                            }
                            this.Log.LogWarning("", "", "", lines[0], _line, 0, 0, 0, text);
                            lastline = line;
                            continue;
                        }
                    }
                    if (lastline.IndexOf("SUCC") == 0)
                    {
                        bSucc = true;
                    }
                    this.Log.LogMessageFromText(lastline, MessageImportance.High);

                }

                lastline = line;
                //lines.Add(line);
            }

            //this.Log.LogMessageFromText(lastline, MessageImportance.High);

            p.WaitForExit();
            return bSucc;
        }
    }

}
