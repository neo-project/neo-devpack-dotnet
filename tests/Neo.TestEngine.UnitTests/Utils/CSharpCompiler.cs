namespace Neo.TestEngine.UnitTests.Utils
{
    public class CSharpCompiler
    {
        public static void Compile(string filepath)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = string.Format("/C nccs {0}", filepath);
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
