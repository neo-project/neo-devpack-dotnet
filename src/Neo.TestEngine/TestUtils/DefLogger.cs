using Neo.Compiler;
using System;

namespace Neo.TestingEngine
{
    internal class DefLogger : ILogger
    {
        public void Log(string log)
        {
            Console.WriteLine(log);
        }
    }
}
