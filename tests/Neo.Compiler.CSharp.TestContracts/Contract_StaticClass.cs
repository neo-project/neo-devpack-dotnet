namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_StaticClass : SmartContract.Framework.SmartContract
{
    class TestClass
    {
        // Static field of a class will maintain a single value across
        // all instances of the class during the execution of the contract.
        private static int _a1 = 1;

        public int TestStaticClass() => _a1;

        public void TestStaticClassAdd() => _a1 += 1;
    }

    private static readonly TestClass _testClass = new();
    private static readonly TestClass _testClass2 = new();

    public static int TestStaticClass()
    {
        _testClass.TestStaticClassAdd();
        return _testClass2.TestStaticClass();
    }
}
