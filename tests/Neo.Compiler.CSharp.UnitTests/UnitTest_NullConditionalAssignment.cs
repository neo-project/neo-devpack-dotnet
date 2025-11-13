using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullConditionalAssignment
{
    [TestMethod]
    public void AssignChild_returns_one_when_receiver_exists()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignChild(true));
    }

    [TestMethod]
    public void AssignChild_returns_zero_when_receiver_null()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(0, contract.AssignChild(false));
    }

    [TestMethod]
    public void AssignSibling_handles_field_targets()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignSibling(true));
        Assert.AreEqual(0, contract.AssignSibling(false));
    }

    [TestMethod]
    public void AssignStatic_handles_static_receivers()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignStatic(true));
        Assert.AreEqual(0, contract.AssignStatic(false));
    }

    [TestMethod]
    public void AssignGrandChild_requires_deep_receiver()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignGrandChild(true, true));
        Assert.AreEqual(0, contract.AssignGrandChild(true, false));
        Assert.AreEqual(0, contract.AssignGrandChild(false, false));
    }

    [TestMethod]
    public void AssignSiblingFromOther_returns_value_when_both_receivers_exist()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NullConditional>(Contract_NullConditional.Nef, Contract_NullConditional.Manifest);
        Assert.AreEqual(1, contract.AssignSiblingFromOther(true, true));
        Assert.AreEqual(0, contract.AssignSiblingFromOther(true, false));
        Assert.AreEqual(0, contract.AssignSiblingFromOther(false, true));
    }
}
