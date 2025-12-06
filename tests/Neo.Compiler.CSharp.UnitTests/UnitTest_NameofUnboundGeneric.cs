using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NameofUnboundGeneric
{
    [TestMethod]
    public void DescribeDictionary_returns_generic_type_names()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NameofUnboundGeneric>(Contract_NameofUnboundGeneric.Nef, Contract_NameofUnboundGeneric.Manifest);
        Assert.AreEqual("Dictionary:Func", contract.Dictionary());
    }

    [TestMethod]
    public void DescribeNested_returns_nested_generic_names()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NameofUnboundGeneric>(Contract_NameofUnboundGeneric.Nef, Contract_NameofUnboundGeneric.Manifest);
        Assert.AreEqual("Enumerator|KeyValuePair", contract.DescribeNested());
    }

    [TestMethod]
    public void DescribeLists_returns_open_generic_names()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NameofUnboundGeneric>(Contract_NameofUnboundGeneric.Nef, Contract_NameofUnboundGeneric.Manifest);
        Assert.AreEqual("List:ValueTuple", contract.List());
    }

    [TestMethod]
    public void DescribeNestedMembers_handles_member_types()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_NameofUnboundGeneric>(Contract_NameofUnboundGeneric.Nef, Contract_NameofUnboundGeneric.Manifest);
        Assert.AreEqual("KeyCollection|Enumerator", contract.DescribeNestedMembers());
    }
}
