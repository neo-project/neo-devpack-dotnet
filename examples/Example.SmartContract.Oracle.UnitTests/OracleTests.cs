using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Linq;
using System.Text;

namespace Example.SmartContract.Oracle.UnitTests;

[TestClass]
public class OracleTests : TestBase<SampleOracle>
{
    [TestMethod]
    public void ResponseStartsEmpty()
    {
        Assert.IsNull(Contract.Response);
    }

    [TestMethod]
    public void DoRequestAndFinishStoresOracleResponse()
    {
        string response = string.Empty;
        ulong? requestId = null;
        JToken data = JToken.Parse("""{"record":{"propertyName":"Hello World!"}}""")!;

        Engine.Native.Oracle.OnOracleRequest += (id, _, _, filter) =>
        {
            requestId = id;
            response = data.JsonPath(filter!).ToString(false);
        };

        Contract.DoRequest();
        Assert.IsNotNull(requestId);
        Assert.IsNull(Contract.Response);

        Engine.Transaction.Attributes =
        [
            new OracleResponse
            {
                Code = OracleResponseCode.Error,
                Id = requestId.Value,
                Result = Encoding.UTF8.GetBytes(response),
            }
        ];
        Assert.ThrowsException<TestException>(Engine.Native.Oracle.Finish);

        Engine.Transaction.Attributes =
        [
            new OracleResponse
            {
                Code = OracleResponseCode.Success,
                Id = requestId.Value,
                Result = Encoding.UTF8.GetBytes(response),
            }
        ];
        Engine.Native.Oracle.Finish();

        Assert.AreEqual("Hello World!", Contract.Response);
    }

    [TestMethod]
    public void CallbackRejectsDirectCalls()
    {
        Assert.ThrowsException<TestException>(() =>
            Contract.OnOracleResponse("https://example.com", null, 0, "[]"));
    }
}
