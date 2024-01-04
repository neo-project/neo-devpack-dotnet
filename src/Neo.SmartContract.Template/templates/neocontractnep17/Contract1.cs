using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

using System;
using System.ComponentModel;
using System.Numerics;

namespace ProjectName
{
    [DisplayName(nameof(Contract1))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ManifestExtra("Description", "<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template")]
    [ContractPermission("*", "*")]
    [SupportedStandards("NEP-17")]
    public class Contract1 : Nep17Token
    {
        #region Owner

        private const byte Prefix_Owner = 0xff;

        // TODO: Replace it with your own address.
        [InitialValue("<Your Address Here>", Neo.SmartContract.ContractParameterType.Hash160)]
        private static readonly UInt160 InitialOwner = default;

        [Safe]
        public static UInt160 GetOwner()
        {
            var currentOwner = Storage.Get(new[] { Prefix_Owner });

            if (currentOwner == null)
                return InitialOwner;

            return (UInt160)currentOwner;
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");
            if (newOwner != null && newOwner.IsValid)
            {
                Storage.Put(new[] { Prefix_Owner }, newOwner);
                OnSetOwner(newOwner);
            }
        }

        #endregion

        #region NEP17

        // NOTE: Valid Range 0-31
        [Safe]
        public override byte Decimals() => 8;

        // TODO: Replace "EXAMPLE" with a short name all UPPERCASE 3-8 characters
        [Safe]
        public override string Symbol() => "EXAMPLE";

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

        #region Payment

        public static bool Withdraw(UInt160 token, UInt160 to, BigInteger amount)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (to == null || to.IsValid == false)
                throw new ArgumentException("Invalid Address!");
            if (token == null || token.IsValid == false)
                throw new ArgumentException("Invalid Token Address!");
            if (ContractManagement.GetContract(token) == null)
                throw new ArgumentException("Token Not A Contract!");
            // TODO: Add logic
            return true;
        }

        // NOTE: Allows ALL NEP-17 tokens to be received for this contract
        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            // TODO: Add logic for specific NEP-17 contract tokens
            if (Runtime.CallingScriptHash == NEO.Hash)
            {
                // TODO: Add logic (Burn, Mint, Transfer, Etc)
            }
            if (Runtime.CallingScriptHash == GAS.Hash)
            {
                // TODO: Add logic (Burn, Mint, Transfer, Etc)
            }
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

        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // This will be executed during update
                return;
            }

            // This will be executed during deploy
            Storage.Put(Storage.CurrentContext, "Hello", "World");
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Update(nefFile, manifest, null);
        }

        // NOTE: NEP-17 contracts "SHOULD NOT" have "Destroy" method
    }
}
