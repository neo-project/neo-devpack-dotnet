using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace $safeprojectname$
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a contract example")]
    [Features(ContractFeatures.HasStorage)]
    public class Contract1 : SmartContract
    {
        static readonly byte[] Owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();

        public static bool Main()
        {
            Storage.Put("Hello", "World");
            return true;
        }

        public static bool Destroy()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            Contract.Destroy();
            return true;
        }

        private static bool IsOwner() => Runtime.CheckWitness(Owner);

        public static bool Update(byte[] script, string manifest)
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            // Check empty
            if (script.Length == 0 && manifest.Length == 0) return false;
            Contract.Update(script, manifest);
            return true;
        }

        // When this contract address is included in the transaction signature,
        // this method will be triggered as a VerificationTrigger to verify that the signature is correct.
        // For example, this method needs to be called when withdrawing token from the contract.
        public static bool Verify() => IsOwner();   
    }
}
