using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Math
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Math.cs");
        }

        [TestMethod]
        public void max_test()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("max", 1, 2);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(2, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("max", 3, 1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(3, result.Pop().GetInteger());
        }

        [TestMethod]
        public void min_test()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("min", 1, 2);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin", 3, 1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin1", 3, 1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin2", 3, 1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin3", 3, 1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin4", 3, 1);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin5", 3, 1);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin6", 3, 1);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin7", 3, 1);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testMin8", -3, 1);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(-3, result.Pop().GetInteger());
        }

        [TestMethod]
        public void sign_test()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("sign", 1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("sign", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(-1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testSign", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(-1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testSign1", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(-1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testSign2", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(-1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testSign3", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(-1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("sign", 0);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(0, result.Pop().GetInteger());
        }

        [TestMethod]
        public void abs_test()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("abs", 1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("abs", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("abs", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testAbs", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testAbs2", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testAbs3", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testAbs4", -1);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testAbs", 0);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(0, result.Pop().GetInteger());
        }

        [TestMethod]
        public void pow_test()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testPow", 10, 10);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(10000000000, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testPow1", 10, 10);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(10000000000, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testPow2", 10, 10);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(10000000000, result.Pop().GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testPow2", 10, 0);

            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());
        }
    }
}
