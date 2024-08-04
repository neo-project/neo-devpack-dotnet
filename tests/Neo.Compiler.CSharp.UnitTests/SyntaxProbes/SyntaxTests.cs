using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class SyntaxTests
{
    [TestMethod]
    public void CSharpSyntaxProbe()
    {
        Helper.TestCodeBlock("var a = 1+1;");
        Helper.TestCodeBlock("var b = \"string test\";\nb.ToString();");
        Helper.TestCodeBlock("var b = new object();\nb.ToString();");
    }
}
