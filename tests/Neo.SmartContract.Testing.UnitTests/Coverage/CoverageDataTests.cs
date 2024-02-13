using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;

namespace Neo.SmartContract.Testing.UnitTests.Coverage
{
    [TestClass]
    public class CoverageDataTests
    {
        [TestMethod]
        public void TestCoverage()
        {
            // Create the engine initializing the native contracts
            // Native contracts use 3 opcodes per method

            //{
            //    sb.EmitPush(0); //version
            //    sb.EmitSysCall(ApplicationEngine.System_Contract_CallNative);
            //    sb.Emit(OpCode.RET);
            //}

            var engine = new TestEngine(true);

            // Check totalSupply

            Assert.IsNull(engine.GetCoverage(engine.Native.NEO));
            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);
            Assert.AreEqual(57, engine.GetCoverage(engine.Native.NEO)?.TotalInstructions);
            Assert.AreEqual(3, engine.GetCoverage(engine.Native.NEO)?.CoveredInstructions);

            // Check balanceOf

            Assert.AreEqual(0, engine.Native.NEO.BalanceOf(engine.Native.NEO.Hash));

            Assert.AreEqual(57, engine.GetCoverage(engine.Native.NEO)?.TotalInstructions);
            Assert.AreEqual(6, engine.GetCoverage(engine.Native.NEO)?.CoveredInstructions);
        }

        [TestMethod]
        public void TestHits()
        {
            var coverage = new CoverageData();

            Assert.AreEqual(0, coverage.Hits);
            Assert.AreEqual(0, coverage.GasAvg);
            Assert.AreEqual(0, coverage.GasMax);
            Assert.AreEqual(0, coverage.GasMin);
            Assert.AreEqual(0, coverage.GasTotal);

            coverage.Hit(123);

            Assert.AreEqual(1, coverage.Hits);
            Assert.AreEqual(123, coverage.GasAvg);
            Assert.AreEqual(123, coverage.GasMax);
            Assert.AreEqual(123, coverage.GasMin);
            Assert.AreEqual(123, coverage.GasTotal);

            coverage.Hit(377);

            Assert.AreEqual(2, coverage.Hits);
            Assert.AreEqual(250, coverage.GasAvg);
            Assert.AreEqual(377, coverage.GasMax);
            Assert.AreEqual(123, coverage.GasMin);
            Assert.AreEqual(500, coverage.GasTotal);

            coverage.Hit(500);

            Assert.AreEqual(3, coverage.Hits);
            Assert.AreEqual(333, coverage.GasAvg);
            Assert.AreEqual(500, coverage.GasMax);
            Assert.AreEqual(123, coverage.GasMin);
            Assert.AreEqual(1000, coverage.GasTotal);

            coverage.Hit(0);

            Assert.AreEqual(4, coverage.Hits);
            Assert.AreEqual(250, coverage.GasAvg);
            Assert.AreEqual(500, coverage.GasMax);
            Assert.AreEqual(0, coverage.GasMin);
            Assert.AreEqual(1000, coverage.GasTotal);
        }
    }
}
