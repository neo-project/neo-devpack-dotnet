using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_FieldKeyword
{
    [TestMethod]
    public void Field_keyword_clamps_negative_values()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_FieldKeyword>(Contract_FieldKeyword.Nef, Contract_FieldKeyword.Manifest);

        contract.Update(0);
        Assert.AreEqual(10, contract.Update(10));
        Assert.AreEqual(0, contract.Update(-5));
    }

    [TestMethod]
    public void Field_keyword_tracks_last_positive_value()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_FieldKeyword>(Contract_FieldKeyword.Nef, Contract_FieldKeyword.Manifest);

        Assert.AreEqual(5, contract.RecordLastPositiveSequence(5, 0), "Zero input should preserve the previous positive value.");
        Assert.AreEqual(8, contract.RecordLastPositiveSequence(0, 8));
    }

    [TestMethod]
    public void Field_keyword_reads_previous_value_inside_setter()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_FieldKeyword>(Contract_FieldKeyword.Nef, Contract_FieldKeyword.Manifest);

        Assert.AreEqual(7, contract.RecordLastNonZeroSequence(7, 0), "Zero input should reuse the stored field value.");
        Assert.AreEqual(3, contract.RecordLastNonZeroSequence(0, 3));
    }

    [TestMethod]
    public void Field_keyword_handles_instance_properties()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_FieldKeyword>(Contract_FieldKeyword.Nef, Contract_FieldKeyword.Manifest);

        Assert.AreEqual(12, contract.AccumulateWallet(5, 7));
        Assert.AreEqual(4, contract.AccumulateWallet(4, -3), "Negative values must not change the balance.");
    }

    [TestMethod]
    public void Field_keyword_tracks_boolean_state()
    {
        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract_FieldKeyword>(Contract_FieldKeyword.Nef, Contract_FieldKeyword.Manifest);

        Assert.IsTrue(contract.TrackPositiveWallet(true, false));
        Assert.IsFalse(contract.TrackPositiveWallet(false, false));
        Assert.IsTrue(contract.TrackPositiveWallet(false, true));
    }
}
