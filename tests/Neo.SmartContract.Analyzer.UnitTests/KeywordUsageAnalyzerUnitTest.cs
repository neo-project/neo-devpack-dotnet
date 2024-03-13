using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Neo.SmartContract.Analyzer.KeywordUsageAnalyzer,
    Neo.SmartContract.Analyzer.RemoveKeywordsCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    public class KeywordUsageAnalyzerUnitTest
    {
        [Fact]
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

            var fixtest = """

                          class MyClass
                          {
                              public void MyMethod()
                              {
                                  {
                                      // Some code
                                  }
                              }
                          }
                          """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(6, 9, 6, 13).WithArguments("lock");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
        public async Task TestFixedStatement()
        {
            var test = """

                       unsafe class MyClass
                       {
                           public void MyMethod()
                           {
                               fixed (int* ptr = &someInt)
                               {
                                   // Some code
                               }
                           }
                       }
                       """;

            var fixtest = """

                          unsafe class MyClass
                          {
                              public void MyMethod()
                              {
                                  {
                                      // Some code
                                  }
                              }
                          }
                          """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId)
                .WithSpan(6, 9, 6, 14).WithArguments("fixed");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
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

            var fixtest = """

                          class MyClass
                          {
                              public void MyMethod()
                              {
                                  // Some code
                              }
                          }
                          """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(6, 9, 6, 15).WithArguments("unsafe");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
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

            var fixtest = """

                          unsafe class MyClass
                          {
                              public void MyMethod()
                              {
                                  int* arr = ;
                              }
                          }
                          """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(6, 20, 6, 30).WithArguments("stackalloc");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
        public async Task TestAwaitExpression()
        {
            var test = """

                       class MyClass
                       {
                           public async void MyMethod()
                           {
                               await Task.Delay(1000);
                           }
                       }
                       """;

            var fixtest = """

                          class MyClass
                          {
                              public async void MyMethod()
                              {
                                  Task.Delay(1000);
                              }
                          }
                          """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(6, 9, 6, 14).WithArguments("await");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
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

            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(6, 9, 6, 16).WithArguments("dynamic");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
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

            var fixtest = """

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
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(6, 9, 6, 15).WithArguments("unsafe");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
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

            var fixtest = """

                          using System.Linq;

                          class MyClass
                          {
                              public void MyMethod()
                              {
                                  var query = ;
                              }
                          }
                          """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(8, 21, 8, 26).WithArguments("select");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
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

            var fixtest = """

                          class MyClass
                          {
                              public void MyMethod()
                              {
                                  var name = "MyClass";
                              }
                          }
                          """;
            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(6, 20, 6, 26).WithArguments("nameof");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Fact]
        public async Task TestImplicitKeyword()
        {
            var test = """

                       class MyClass
                       {
                           public static implicit operator int(MyClass c) => 0;
                       }
                       """;

            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(4, 19, 4, 27).WithArguments("implicit");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task TestExplicitKeyword()
        {
            var test = """

                       class MyClass
                       {
                           public static explicit operator MyClass(int i) => null;
                       }
                       """;

            var expected = VerifyCS.Diagnostic(KeywordUsageAnalyzer.DiagnosticId).WithSpan(4, 19, 4, 27).WithArguments("explicit");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
