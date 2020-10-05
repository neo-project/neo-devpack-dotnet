using System;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    internal class DefLogger : ILogger
    {
        public void Log(string log)
        {
            Console.WriteLine(log);
        }
    }
}
