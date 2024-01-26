using System;
using System.Linq;
using Akka.Util.Internal;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests.Utils
{
    static class Extensions
    {
        public static readonly string TestContractRoot = "../../../../Neo.Compiler.CSharp.TestContracts/";

        public static CompilationContext AddEntryScript(this TestEngine engin, params Type[] files)
        {
            var sourceFiles = Neo.SmartContract.TestEngine.Extensions.GetFiles(TestContractRoot, files).ToArray();
            return engin.AddEntryScript(sourceFiles);
        }

        public static CompilationContext AddEntryScript(this TestEngine engin, bool debug, params Type[] files)
        {
            var sourceFiles = files.Select(p => TestContractRoot + p.Name + ".cs").ToArray();
            return engin.AddEntryScript(debug, sourceFiles);
        }

        public static CompilationContext AddEntryScript<T>(this TestEngine engine)
        {
            return engine.AddEntryScript(typeof(T));
        }
    }
}
