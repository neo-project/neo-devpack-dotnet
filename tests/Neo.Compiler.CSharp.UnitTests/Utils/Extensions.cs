using System.IO;

namespace Neo.Compiler.CSharp.UnitTests.Utils
{
    static class Extensions
    {
        public static readonly string TestContractRoot = Path.GetFullPath("../../../../Neo.Compiler.CSharp.TestContracts/Working") + "/";
        public static readonly string NotWorkingTestContractRoot = Path.GetFullPath("../../../../Neo.Compiler.CSharp.TestContracts/NotWorking") + "/";
    }
}
