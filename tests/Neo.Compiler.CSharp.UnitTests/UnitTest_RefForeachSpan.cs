using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RefForeachSpan
{
    [TestMethod]
    public void IncrementAll_updates_every_element()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefForeachSpan>(Contract_RefForeachSpan.Nef, Contract_RefForeachSpan.Manifest);

        var result = contract.IncrementAll(1, 3, 5);
        Assert.AreEqual(15, result);
    }

    [TestMethod]
    public void IncrementArrayValues_handles_array_sources()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefForeachSpan>(Contract_RefForeachSpan.Nef, Contract_RefForeachSpan.Manifest);

        Assert.AreEqual(12, contract.IncrementArrayValues(2, 3, 5));
    }

    [TestMethod]
    public void SegmentDouble_processes_array_segments()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefForeachSpan>(Contract_RefForeachSpan.Nef, Contract_RefForeachSpan.Manifest);

        Assert.AreEqual(12, contract.SegmentDouble(2, 4));
    }
}
