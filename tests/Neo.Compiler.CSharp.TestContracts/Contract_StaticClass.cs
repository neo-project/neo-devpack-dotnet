namespace Neo.Compiler.CSharp.UnitTests.TestClasses;

public class Contract_StaticClass : SmartContract.Framework.SmartContract
{
    private static readonly TestClass _testClass = new();

    public static int TestStaticClass()
    {
        // _testClass.TestStaticClass();
        _testClass.TestStaticClassAdd();
        // _testClass.TestStaticClassAdd();
        return _testClass.TestStaticClass();
    }
}

class TestClass
{
    // Static field of a class will maintain a single value across
    // all instances of the class during the execution of the contract.
    private static int _a1 = 1;

    public int TestStaticClass() => _a1;

    public void TestStaticClassAdd() => _a1 += 1;

    public int TestStaticClass2()
    {
        return _a1;
    }

    public int TestStaticClassAdd2()
    {
        return _a1 += 1;
    }
}
