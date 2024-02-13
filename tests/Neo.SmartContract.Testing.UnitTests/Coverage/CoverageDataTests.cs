using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Testing.Coverage;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests.Coverage
{
    [TestClass]
    public class CoverageDataTests
    {
        [TestMethod]
        public void TestCoverageByEngine()
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

            Assert.AreEqual(engine.Native.NEO.Hash, engine.GetCoverage(engine.Native.NEO)?.Hash);
            Assert.AreEqual(133, engine.GetCoverage(engine.Native.NEO)?.Script.Length);
            Assert.AreEqual(57, engine.GetCoverage(engine.Native.NEO)?.TotalInstructions);
            Assert.AreEqual(3, engine.GetCoverage(engine.Native.NEO)?.CoveredInstructions);
            Assert.AreEqual(3, engine.GetCoverage(engine.Native.NEO)?.HitsInstructions);

            // Check balanceOf

            Assert.AreEqual(0, engine.Native.NEO.BalanceOf(engine.Native.NEO.Hash));

            Assert.AreEqual(57, engine.GetCoverage(engine.Native.NEO)?.TotalInstructions);
            Assert.AreEqual(6, engine.GetCoverage(engine.Native.NEO)?.CoveredInstructions);
            Assert.AreEqual(6, engine.GetCoverage(engine.Native.NEO)?.HitsInstructions);

            // Check coverage by method and expression

            var methodCovered = engine.GetCoverage(engine.Native.Oracle, o => o.Finish());
            Assert.IsNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.NEO, o => o.TotalSupply);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, o => o.BalanceOf(It.IsAny<UInt160>()));
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, o => o.Transfer(It.IsAny<UInt160>(), It.IsAny<UInt160>(), It.IsAny<BigInteger>(), It.IsAny<object>()));
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(0, methodCovered?.CoveredInstructions);

            // Check coverage by raw method

            methodCovered = engine.GetCoverage(engine.Native.Oracle)?.GetCoverage("finish", 0);
            Assert.IsNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.NEO)?.GetCoverage("totalSupply", 0);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO)?.GetCoverage("balanceOf", 1);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO)?.GetCoverage("transfer", 4);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(0, methodCovered?.CoveredInstructions);
        }

        [TestMethod]
        public void TestCoverageByExtension()
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

            Assert.IsNull(engine.Native.NEO.GetCoverage());
            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);

            Assert.AreEqual(engine.Native.NEO.Hash, engine.Native.NEO.GetCoverage()?.Hash);
            Assert.AreEqual(133, engine.Native.NEO.GetCoverage()?.Script.Length);
            Assert.AreEqual(57, engine.Native.NEO.GetCoverage()?.TotalInstructions);
            Assert.AreEqual(3, engine.Native.NEO.GetCoverage()?.CoveredInstructions);
            Assert.AreEqual(3, engine.Native.NEO.GetCoverage()?.HitsInstructions);

            // Check balanceOf

            Assert.AreEqual(0, engine.Native.NEO.BalanceOf(engine.Native.NEO.Hash));

            Assert.AreEqual(57, engine.Native.NEO.GetCoverage()?.TotalInstructions);
            Assert.AreEqual(6, engine.Native.NEO.GetCoverage()?.CoveredInstructions);
            Assert.AreEqual(6, engine.Native.NEO.GetCoverage()?.HitsInstructions);

            // Check coverage by method and expression

            var methodCovered = engine.Native.Oracle.GetCoverage(o => o.Finish());
            Assert.IsNull(methodCovered);

            methodCovered = engine.Native.NEO.GetCoverage(o => o.TotalSupply);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.Native.NEO.GetCoverage(o => o.BalanceOf(It.IsAny<UInt160>()));
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.Native.NEO.GetCoverage(o => o.Transfer(It.IsAny<UInt160>(), It.IsAny<UInt160>(), It.IsAny<BigInteger>(), It.IsAny<object>()));
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(0, methodCovered?.CoveredInstructions);

            // Check coverage by raw method

            methodCovered = engine.Native.Oracle.GetCoverage()?.GetCoverage("finish", 0);
            Assert.IsNull(methodCovered);

            methodCovered = engine.Native.NEO.GetCoverage()?.GetCoverage("totalSupply", 0);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.Native.NEO.GetCoverage()?.GetCoverage("balanceOf", 1);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.Native.NEO.GetCoverage()?.GetCoverage("transfer", 4);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(0, methodCovered?.CoveredInstructions);
        }

        [TestMethod]
        public void TestHits()
        {
            var coverage = new CoverageData(0);

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
