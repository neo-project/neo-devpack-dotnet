using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class UnitTest_NamedArguments
{
    [TestMethod]
    public void NamedArgument_Uses_Default_For_Omitted_Earlier_Parameter()
    {
        Helper.AssertClassCompilationSucceeds("""
private static int Add(int a = 1, int b = 2) => a + b;

public static int Test() => Add(b: 5);
""", "Named arguments should allow omitted earlier default parameters.");
    }

    [TestMethod]
    public void NamedArgument_Preserves_Positional_Arguments_Before_Default()
    {
        Helper.AssertClassCompilationSucceeds("""
private static int Add(int a, int b = 2, int c = 3) => a + b + c;

public static int Test() => Add(4, c: 7);
""", "Named arguments should preserve positional arguments while using defaults.");
    }
}
