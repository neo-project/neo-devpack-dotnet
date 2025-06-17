using System;

namespace Neo.SmartContract.Fuzzer.Tests.ManualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running manual tests for Neo.SmartContract.Fuzzer...");
            
            // Run the UnifiedConstraintSolverTest
            UnifiedConstraintSolverTest.Main();
            
            Console.WriteLine("All manual tests completed.");
        }
    }
}
