using System;

namespace Neo.Compiler.MSIL.Utils
{
    internal class DefLogger : ILogger
    {
        public void Log(string log)
        {
            Console.WriteLine(log);
        }
    }
}
