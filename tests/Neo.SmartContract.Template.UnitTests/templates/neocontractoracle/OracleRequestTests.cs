using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM;
using System.Text;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractoracle
{

    /// <summary>
    /// You need to build the solution to resolve OracleRequest class.
    /// </summary>
    [TestClass]
    public class OracleRequestTests : TestBase<OracleRequest>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OracleRequestTests() : base(OracleRequest.Nef, OracleRequest.Manifest) { }

        [TestMethod]
        public void TestGetResponse()
        {
            Assert.IsNull(Contract.Response);
        }

        [TestMethod]
        public void TestDoRequestAndFinish()
        {
            // Check without being oracle

            Assert.ThrowsException<VMUnhandledException>(() => Contract.OnOracleResponse(null, null, null, null));

            // Check empty

            Assert.IsNull(Contract.Response);

            // Create request

            string response = "";
            ulong? requestId = null;
            JToken data = JToken.Parse(@"
{
    ""id"": ""6520ad3c12a5d3765988542a"",
    ""record"": {
        ""propertyName"": ""Hello World!""
    }
}")!;

            Engine.Native.Oracle.OnOracleRequest += (ulong id, UInt160 requestContract, string url, string filter) =>
            {
                requestId = id;
                response = data.JsonPath(filter).ToString(false);
            };

            Contract.DoRequest();
            Assert.IsNotNull(requestId);

            // Execute error

            Engine.Transaction.Attributes = new[]{
                new OracleResponse()
                {
                     Code = OracleResponseCode.Error,
                     Id = requestId.Value,
                     Result = Encoding.UTF8.GetBytes(response),
                }
            };
            Assert.ThrowsException<VMUnhandledException>(Engine.Native.Oracle.Finish);

            // Execute finish

            Engine.Transaction.Attributes = new[]{
                new OracleResponse()
                {
                     Code = OracleResponseCode.Success,
                     Id = requestId.Value,
                     Result = Encoding.UTF8.GetBytes(response),
                }
            };

            Engine.Native.Oracle.Finish();

            // Check result

            Assert.AreEqual("Hello World!", Contract.Response);
        }
    }
}
