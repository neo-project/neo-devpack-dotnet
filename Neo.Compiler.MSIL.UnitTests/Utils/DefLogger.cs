using System;

namespace Neo2.Compiler.MSIL.Utils
{
    internal class DefLogger : ILogger
    {
        public void Log(string log)
        {
            Console.WriteLine(log);
        }
    }
}
