using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RefSupport
{
    [TestMethod]
    public void Ref_parameter_updates_local()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefSupport>(Contract_RefSupport.Nef, Contract_RefSupport.Manifest);

        Assert.AreEqual(6, contract.IncrementRefLocal(5));
    }

    [TestMethod]
    public void Ref_parameter_updates_static_field()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefSupport>(Contract_RefSupport.Nef, Contract_RefSupport.Manifest);

        Assert.AreEqual(11, contract.IncrementRefStaticField(10));
    }

    [TestMethod]
    public void Ref_parameter_updates_instance_field()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefSupport>(Contract_RefSupport.Nef, Contract_RefSupport.Manifest);

        Assert.AreEqual(8, contract.IncrementRefInstanceField(7));
    }

    [TestMethod]
    public void Out_parameter_assigns_value()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefSupport>(Contract_RefSupport.Nef, Contract_RefSupport.Manifest);

        Assert.AreEqual(123, contract.ProduceOutValue());
        Assert.AreEqual(123, contract.ProduceOutStaticField());
        Assert.AreEqual(123, contract.ProduceOutInstanceField());
    }

    [TestMethod]
    public void Ref_parameter_allows_swapping()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_RefSupport>(Contract_RefSupport.Nef, Contract_RefSupport.Manifest);

        Assert.AreEqual(21, contract.SwapDigits(1, 2));
    }
}
