using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class CheckWitnessTests : DebugAndTestBase<Contract_CheckWitness>
    {
        [TestMethod]
        public void Test_CheckWitness()
        {
            CheckWitnessAnalyzer.CheckWitnessVulnerability result = CheckWitnessAnalyzer.AnalyzeCheckWitness(NefFile, Manifest, null);
            Assert.AreEqual(result.droppedCheckWitnessResults.Count, 1);
        }
    }
}
