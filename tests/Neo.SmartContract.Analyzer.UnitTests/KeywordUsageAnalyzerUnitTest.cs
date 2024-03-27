using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.KeywordUsageAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class KeywordUsageAnalyzerUnitTest
    {
        [TestMethod]
        public async Task TestLockStatement()
        {
            var test = """
                       class MyClass
                       {
                           public void MyMethod()
                           {
                               lock (this)
                               {
                                   // Some code
                               }
                           }
                       }
                       """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(5, 9, 8, 10).WithArguments("lock");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestUnsafeStatement()
        {
            var test = """
                       class MyClass
                       {
                           public void MyMethod()
                           {
                               unsafe
                               {
                                   // Some code
                               }
                           }
                       }
                       """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(5, 9, 8, 10).WithArguments("unsafe");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestStackAllocExpression()
        {
            var test = """
                       unsafe class MyClass
                       {
                           public void MyMethod()
                           {
                               int* arr = stackalloc int[10];
                           }
                       }
                       """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(5, 20, 5, 38).WithArguments("stackalloc");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestAwaitExpression()
        {
            var test = """
                       using System.Threading.Tasks;
                       class MyClass
                       {
                           public async void MyMethod()
                           {
                               await Task.Delay(1000);
                           }
                       }
                       """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(6, 9, 6, 31).WithArguments("await");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestDynamicType()
        {
            var test = """
                       class MyClass
                       {
                           public void MyMethod()
                           {
                               dynamic d = 1;
                           }
                       }
                       """;

            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(5, 9, 5, 16).WithArguments("dynamic");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestUnmanagedModifier()
        {
            var test = """
                       class MyClass
                       {
                           public void MyMethod()
                           {
                               unsafe
                               {
                                   void* Ptr() => null;
                               }
                           }
                       }
                       """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(5, 9, 8, 10).WithArguments("unsafe");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestQueryExpression()
        {
            var test = """
                       using System.Linq;

                       class MyClass
                       {
                           public void MyMethod()
                           {
                               var query = from x in new int[0] select x;
                           }
                       }
                       """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(7, 21, 7, 50).WithArguments("select");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestNameofExpression()
        {
            var test = """
                       class MyClass
                       {
                           public void MyMethod()
                           {
                               var name = nameof(MyClass);
                           }
                       }
                       """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(5, 20, 5, 35).WithArguments("nameof");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestImplicitKeyword()
        {
            var test = """
                       class MyClass
                       {
                           public static implicit operator int(MyClass c) => 0;
                       }
                       """;

            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(3, 5, 3, 57).WithArguments("implicit");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestExplicitKeyword()
        {
            var test = """
                       class MyClass
                       {
                           public static explicit operator MyClass(int i) => null;
                       }
                       """;

            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(3, 5, 3, 60).WithArguments("explicit");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
