using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

using System;
using System.ComponentModel;

namespace ProjectName
{
    [DisplayName(nameof(Contract1))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ManifestExtra("Description", "<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template")]
    [ContractPermission(ByteString.Wildcard, ByteString.Wildcard)]
    public class Contract1 : SmartContract
    {
        private const byte Prefix_Owner = 0xff;

        public delegate void OnSetOwnerDelegate(UInt160 newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner;

        // TODO: Replace it with your own address.
        [InitialValue("<Your Address Here>", ContractParameterType.Hash160)]
        private static readonly UInt160 InitialOwner = default;

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

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

        public static void Destroy()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Destroy();
        }

        // Safe is for read operations Or Safe to call by everyone
        [Safe]
        public static UInt160 GetOwner()
        {
            var currentOwner = Storage.Get(new[] { Prefix_Owner });

            if (currentOwner == null)
                return InitialOwner;

            return (UInt160)currentOwner;
        }

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
    }
}
