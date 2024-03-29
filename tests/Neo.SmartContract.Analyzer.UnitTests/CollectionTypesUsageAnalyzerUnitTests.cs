using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Neo.SmartContract.Analyzer.CollectionTypesUsageAnalyzer,
    Neo.SmartContract.Analyzer.CollectionTypesUsageCodeFixProvider>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class CollectionTypesUsageAnalyzerUnitTests
    {
        private const string TestNamespace = """
                                             using System.Collections.Generic;

                                                 public class Map<TKey, TValue>
                                                 {
                                                     public Map() { }
                                                 }

                                                 public class List<T>
                                                 {
                                                     public List() { }
                                                 }


                                             """;
        [TestMethod]
        public async Task UnsupportedDictionaryType_ShouldReportDiagnostic_AndFixToMap()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public void TestMethod()
                                           {
                                               Dictionary<int, string> dict = new Dictionary<int, string>();
                                           }
                                       }
                                       """;

            const string fixTest = TestNamespace + """

                                                   class TestClass
                                                   {
                                                       public void TestMethod()
                                                       {
                                                           Map<int, string> dict = new Map<int, string>();
                                                       }
                                                   }
                                                   """;

            var expectedDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(18, 9)
                .WithArguments("System.Collections.Generic.Dictionary<TKey, TValue>", "Map<TKey, TValue>");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixTest);
        }

        [TestMethod]
        public async Task UnsupportedListType_ShouldReportDiagnostic_AndFixToList()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public void TestMethod()
                                           {
                                               Stack<int> stack = new Stack<int>();
                                           }
                                       }
                                       """;

            var fixtest = TestNamespace + """

                                          class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  List<int> stack = new List<int>();
                                              }
                                          }
                                          """;

            var expectedDiagnostic = VerifyCS.Diagnostic(CollectionTypesUsageAnalyzer.DiagnosticId)
                .WithLocation(18, 9)
                .WithArguments("System.Collections.Generic.Stack<T>", "List<T>");

            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, fixtest);
        }

        [TestMethod]
        public async Task SupportedCollectionType_ShouldNotReportDiagnostic()
        {
            var test = TestNamespace + """

                                       class TestClass
                                       {
                                           public void TestMethod()
                                           {
                                               List<int> list = new List<int>();
                                               Map<int, string> map = new Map<int, string>();
                                           }
                                       }
                                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
