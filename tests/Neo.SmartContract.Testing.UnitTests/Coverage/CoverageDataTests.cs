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
        public void TestDump()
        {
            var engine = new TestEngine(true);

            // Check totalSupply

            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);

            Assert.AreEqual(@"
0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5 [5.26%]
┌-───────────────────────────────-┬-───────-┐
│ Method                          │  Line   │
├-───────────────────────────────-┼-───────-┤
│ totalSupply()                   │ 100.00% │
│ balanceOf(account)              │   0.00% │
│ decimals()                      │   0.00% │
│ getAccountState(account)        │   0.00% │
│ getAllCandidates()              │   0.00% │
│ getCandidates()                 │   0.00% │
│ getCandidateVote(pubKey)        │   0.00% │
│ getCommittee()                  │   0.00% │
│ getGasPerBlock()                │   0.00% │
│ getNextBlockValidators()        │   0.00% │
│ getRegisterPrice()              │   0.00% │
│ registerCandidate(pubkey)       │   0.00% │
│ setGasPerBlock(gasPerBlock)     │   0.00% │
│ setRegisterPrice(registerPrice) │   0.00% │
│ symbol()                        │   0.00% │
│ transfer(from,to,amount,data)   │   0.00% │
│ unclaimedGas(account,end)       │   0.00% │
│ unregisterCandidate(pubkey)     │   0.00% │
│ vote(account,voteTo)            │   0.00% │
└-───────────────────────────────-┴-───────-┘
".Trim(), engine.GetCoverage(engine.Native.NEO)?.Dump().Trim());

            Assert.AreEqual(@"
0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5 [5.26%]
┌-─────────────-┬-───────-┐
│ Method        │  Line   │
├-─────────────-┼-───────-┤
│ totalSupply() │ 100.00% │
└-─────────────-┴-───────-┘
".Trim(), (engine.Native.NEO.GetCoverage(o => o.TotalSupply) as CoveredMethod)?.Dump().Trim());
        }

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

            Assert.IsNotNull(engine.GetCoverage(engine.Native.NEO));
            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);

            Assert.AreEqual(engine.Native.NEO.Hash, engine.GetCoverage(engine.Native.NEO)?.Hash);
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
            Assert.IsNotNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.NEO, o => o.TotalSupply);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, o => o.RegisterPrice);
            Assert.AreEqual(6, methodCovered?.TotalInstructions);
            Assert.AreEqual(0, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, o => o.BalanceOf(It.IsAny<UInt160>()));
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, o => o.Transfer(It.IsAny<UInt160>(), It.IsAny<UInt160>(), It.IsAny<BigInteger>(), It.IsAny<object>()));
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(0, methodCovered?.CoveredInstructions);

            // Check coverage by raw method

            methodCovered = engine.GetCoverage(engine.Native.Oracle, "finish", 0);
            Assert.IsNotNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.NEO, "totalSupply", 0);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, "balanceOf", 1);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, "transfer", 4);
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

            Assert.IsNotNull(engine.Native.NEO.GetCoverage());
            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);

            Assert.AreEqual(engine.Native.NEO.Hash, engine.Native.NEO.GetCoverage()?.Hash);
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
            Assert.IsNotNull(methodCovered);

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

            methodCovered = engine.GetCoverage(engine.Native.Oracle, "finish", 0);
            Assert.IsNotNull(methodCovered);

            methodCovered = engine.GetCoverage(engine.Native.NEO, "totalSupply", 0);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, "balanceOf", 1);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(3, methodCovered?.CoveredInstructions);

            methodCovered = engine.GetCoverage(engine.Native.NEO, "transfer", 4);
            Assert.AreEqual(3, methodCovered?.TotalInstructions);
            Assert.AreEqual(0, methodCovered?.CoveredInstructions);
        }

        [TestMethod]
        public void TestHits()
        {
            var coverage = new CoverageHit(0, "test");

            Assert.AreEqual(0, coverage.Hits);
            Assert.AreEqual("test", coverage.Description);
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
