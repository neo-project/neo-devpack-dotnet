using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Exception.UnitTests
{
    [TestClass]
    public class ExceptionTests : TestBase<SampleException>
    {

        [TestMethod]
        public void Test()
        {
            Contract.Try01();
            Contract.Try02();
            Contract.Try03();
            Contract.TryFinally();
            Contract.TryNest();
        }
    }
}
