using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class UnitTest_CollectionExpressionSupport
{
    [TestMethod]
    public void CollectionExpression_SpreadElement_FailsWithDiagnostic()
    {
        Helper.AssertClassCompilationFails(@"
public static int[] Clone(int[] values)
{
    return [..values];
}", "Collection expression spread elements should be rejected with a diagnostic instead of throwing.");
    }
}
