using System;
using System.Numerics;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.RuntimeCompilation;
using Neo.Wallets;

namespace Example.SmartContract.Transfer.UnitTests
{
    [TestClass]
    public class TransferTests : ContractProjectTestBase
    {
        private static readonly string OwnerAddress = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

        public TestContext TestContext { get; set; } = default!;

        public TransferTests()
            : base("../Example.SmartContract.Transfer/Example.SmartContract.Transfer.csproj")
        {
        }

        private void InvokeTransfer(params object?[] args)
        {
            var proxyInstance = (object)Contract;
            Assert.IsTrue(Artifacts.ProxyType.IsInstanceOfType(proxyInstance), "Runtime contract instance does not match the generated proxy type.");

            var method = Artifacts.ProxyType.GetMethod("Transfer");
            Assert.IsNotNull(method, "Expected generated proxy type to expose a Transfer method.");

            var parameters = method!.GetParameters();
            var convertedArgs = new object?[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var targetType = parameters[i].ParameterType;
                var argument = args[i];

                if (argument is Neo.UInt160 neoUInt160 && targetType.FullName == "Neo.UInt160")
                {
                    var parseMethod = targetType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[] { typeof(string) }, null);
                    Assert.IsNotNull(parseMethod, "Expected runtime Neo.UInt160 type to expose Parse method.");
                    convertedArgs[i] = parseMethod!.Invoke(null, new object?[] { neoUInt160.ToString() });
                    continue;
                }

                convertedArgs[i] = argument;
            }

            try
            {
                method.Invoke(proxyInstance, convertedArgs);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is { } inner)
            {
                throw inner;
            }
        }

        [TestInitialize]
        public void TestSetup()
        {
            EnsureContractDeployed();
        }

        [TestMethod]
        public void Transfer_RequiresOwnerWitness()
        {
            EnsureContractDeployed();

            // Set a signer that is not the owner to trigger the witness check.
            Engine.SetTransactionSigners(TestEngine.GetNewSigner());
            var recipient = TestEngine.GetNewSigner();

            try
            {
                InvokeTransfer(recipient.Account, new BigInteger?(BigInteger.One));
                Assert.Fail("Expected contract to reject transfer without owner witness.");
            }
            catch (TestException)
            {
                // expected path
            }
            catch (RuntimeBinderException ex)
            {
                Assert.Fail($"Binder exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void Transfer_AllowsOwnerWitness()
        {
            EnsureContractDeployed();

            var owner = OwnerAddress.ToScriptHash(Engine.ProtocolSettings.AddressVersion);
            var recipient = TestEngine.GetNewSigner();

            Engine.SetTransactionSigners(new Signer
            {
                Account = owner,
                Scopes = WitnessScope.CalledByEntry
            });

            // The contract asserts internally; reaching here means the transfer path executed.
            InvokeTransfer(recipient.Account, new BigInteger?(BigInteger.Zero));
            AssertNoLogs();
        }
    }
}
