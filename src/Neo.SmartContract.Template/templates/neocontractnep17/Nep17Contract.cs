using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Template
{
    [DisplayName(nameof(Nep17Contract))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ContractDescription( "<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs")]
    [ContractPermission(Permission.WildCard, Method.WildCard)]
    [SupportedStandards("NEP-17")]
    public class Nep17Contract : Neo.SmartContract.Framework.Nep17Token
    {
        #region Owner

        private const byte Prefix_Owner = 0xff;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, "owner must be valid");

            UInt160 previous = GetOwner();
            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previous, newOwner);
        }

        #endregion

        #region NEP17

        // TODO: Replace "EXAMPLE" with a short name all UPPERCASE 3-8 characters
        public override string Symbol { [Safe] get => "EXAMPLE"; }

        // NOTE: Valid Range 0-31
        public override byte Decimals { [Safe] get => 8; }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");
            Nep17Token.Burn(account, amount);
        }

        public static new void Mint(UInt160 to, BigInteger amount)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");
            Nep17Token.Mint(to, amount);
        }

        #endregion

        // When this contract address is included in the transaction signature,
        // this method will be triggered as a VerificationTrigger to verify that the signature is correct.
        // For example, this method needs to be called when withdrawing token from the contract.
        [Safe]
        public static bool Verify() => IsOwner();

        // TODO: Replace it with your methods.
        public static string MyMethod()
        {
            return Storage.Get(Storage.CurrentContext, "Hello");
        }

        // This will be executed during deploy
        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // This will be executed during update
                return;
            }

            // Init method, you must deploy the contract with the owner as an argument, or it will take the sender
            if (data is null) data = Runtime.Transaction.Sender;

            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, "owner must exists");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);
            Storage.Put(Storage.CurrentContext, "Hello", "World");
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No authorization.");
            ContractManagement.Update(nefFile, manifest, data);
        }

        // NOTE: NEP-17 contracts "SHOULD NOT" have "Destroy" method
    }
}
