using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.Event.UnitTests
{
    [TestClass]
    public class EventTests : TestBase<SampleEvent>
    {

        [TestMethod]
        public void Test()
        {
            Assert.IsFalse(Contract.Main());
        }
    }
}
