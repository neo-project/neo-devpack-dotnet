using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;

namespace Example.SmartContract.NeoFS.UnitTests;

[TestClass]
public class NeoFSTests : TestBase<SampleNeoFS>
{
    private const string ExpectedObjectUri =
        "neofs://C3swfg8MiMJ9bXbeFG6dWJTCoHp9hAEZkHezvbSwK1Cc/3nQH1L8u3eM9jt2mZCs6MyjzdjerdSzBkXCYYj4M4Znk";

    [TestMethod]
    public void BuildsNeoFSObjectUri()
    {
        Assert.AreEqual(ExpectedObjectUri, Contract.ObjectUri);
    }

    [TestMethod]
    public void BuildsNeoFSCommandUris()
    {
        Assert.AreEqual(ExpectedObjectUri + "/range/42|128", Contract.GetRangeUri(42, 128));
        Assert.AreEqual(ExpectedObjectUri + "/header", Contract.HeaderUri);
        Assert.AreEqual(ExpectedObjectUri + "/hash", Contract.HashUri);
    }

    [TestMethod]
    public void OracleCallbackStoresPayload()
    {
        Assert.ThrowsException<TestException>(() =>
            Contract.OnOracleResponse(ExpectedObjectUri, null, 0, "payload"));

        Engine.OnGetCallingScriptHash = (current, expected) => Engine.Native.Oracle.Hash;

        Contract.OnOracleResponse(ExpectedObjectUri, null, 0, "payload");

        Assert.AreEqual("payload", Contract.StoredPayload);
    }
}
