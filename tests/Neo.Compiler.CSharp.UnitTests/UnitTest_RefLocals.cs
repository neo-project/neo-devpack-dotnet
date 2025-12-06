using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RefLocals
{
    private static Contract_RefLocals Deploy(TestEngine engine)
        => engine.Deploy<Contract_RefLocals>(Contract_RefLocals.Nef, Contract_RefLocals.Manifest);

    [TestMethod]
    public void Ref_local_updates_value_type()
    {
        var engine = new TestEngine(true);
        var contract = Deploy(engine);

        Assert.AreEqual(10, contract.IncrementViaRefLocal(5));
    }

    [TestMethod]
    public void Ref_local_can_rebind()
    {
        var engine = new TestEngine(true);
        var contract = Deploy(engine);

        Assert.AreEqual(9, contract.RebindRefLocal(1, 7));
    }

    [TestMethod]
    public void Ref_local_tracks_array_element()
    {
        var engine = new TestEngine(true);
        var contract = Deploy(engine);

        Assert.AreEqual(99, contract.RewriteArrayElement(2));
    }

    [TestMethod]
    public void Ref_local_updates_instance_field()
    {
        var engine = new TestEngine(true);
        var contract = Deploy(engine);

        Assert.AreEqual(14, contract.UpdateInstanceField(10));
    }

    [TestMethod]
    public void Ref_local_updates_static_field()
    {
        var engine = new TestEngine(true);
        var contract = Deploy(engine);

        Assert.AreEqual(6, contract.UpdateStaticField(5));
    }

    [TestMethod]
    public void Ref_local_updates_nested_holder()
    {
        var engine = new TestEngine(true);
        var contract = Deploy(engine);

        Assert.AreEqual(13, contract.UpdateNestedHolder(10));
    }
}
