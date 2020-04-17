using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class OracleTest
    {
        private TestEngineEx _engine;

        class TestEngineEx : TestEngine
        {
            protected override bool OnSysCall(uint method)
            {
                if (method == "Neo.Oracle.Hash".ToInteropMethodHash())
                {
                    Push("0x010203");
                    return true;
                }
                else if (method == "Neo.Oracle.Get".ToInteropMethodHash())
                {
                    if (!CurrentContext.EvaluationStack.TryPop(out PrimitiveType url)) return false;
                    if (!CurrentContext.EvaluationStack.TryPop(out StackItem filter)) return false;
                    if (!CurrentContext.EvaluationStack.TryPop(out StackItem filterMethod)) return false;
                    if (!CurrentContext.EvaluationStack.TryPop(out StackItem filterArgs)) return false;

                    if (url.GetString() != "url") return false;
                    if (filter != StackItem.Null && filter.GetString() != "filter") return false;
                    if (filter != StackItem.Null && filterMethod.GetString() != "filterMethod") return false;
                    if (filter != StackItem.Null && filterArgs.GetString() != "filterArgs") return false;

                    Push("0x01020304");
                    return true;
                }

                return base.OnSysCall(method);
            }
        }

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngineEx();
            _engine.AddEntryScript("./TestClasses/Contract_Oracle.cs");
        }

        [TestMethod]
        public void Test_OracleHash()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getHash");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var ret = result.TryPop(out PrimitiveType data);
            Assert.IsTrue(ret);

            Assert.AreEqual("0x010203", data.GetString());
        }

        [TestMethod]
        public void Test_OracleGet()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("get1", "url", "filter", "filterMethod", "filterArgs");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var ret = result.TryPop(out PrimitiveType data);
            Assert.IsTrue(ret);

            Assert.AreEqual("0x01020304", data.GetString());
        }

        [TestMethod]
        public void Test_OracleGet2()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("get2", "url");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var ret = result.TryPop(out PrimitiveType data);
            Assert.IsTrue(ret);

            Assert.AreEqual("0x01020304", data.GetString());
        }
    }
}
