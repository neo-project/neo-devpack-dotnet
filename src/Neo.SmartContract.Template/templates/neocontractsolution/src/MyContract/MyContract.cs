using System;
using System.ComponentModel;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace NeoContractSolution.Contracts
{
    [DisplayName("MyContract")]
    [ManifestExtra("Author", "TemplateAuthor")]
    [ManifestExtra("Email", "TemplateEmail")]
    [ManifestExtra("Description", "TemplateDescription")]
    [ContractPermission("*", "*")]
    public class MyContract : SmartContract
    {
        private const byte Prefix_Owner = 0x01;
        private const byte Prefix_Initialized = 0x02;
        
        [InitialValue("NhNg7GgRV1VUGNqjGxqgJfbgRoPyJgJVPq", ContractParameterType.Hash160)]
        private static readonly UInt160 InitialOwner = default!;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Owner });
        }

        public static bool SetOwner(UInt160 newOwner)
        {
            var owner = GetOwner();
            ExecutionEngine.Assert(Runtime.CheckWitness(owner), "Only owner can set new owner");
            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, "Invalid new owner");
            
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Owner }, newOwner);
            OnOwnerChanged(owner, newOwner);
            return true;
        }

        [Safe]
        public static bool IsInitialized()
        {
            return Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Initialized }).Equals(1);
        }

        public static bool Initialize(object data = null)
        {
            if (IsInitialized())
                throw new Exception("Contract already initialized");

            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Owner }, InitialOwner);
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Initialized }, 1);
            
            OnInitialized(InitialOwner, data);
            return true;
        }

        public static bool Update(ByteString nefFile, string manifest, object? data = null)
        {
            var owner = GetOwner();
            ExecutionEngine.Assert(Runtime.CheckWitness(owner), "Only owner can update");
            
            ContractManagement.Update(nefFile, manifest, data);
            OnUpdated();
            return true;
        }

        public static bool Destroy()
        {
            var owner = GetOwner();
            ExecutionEngine.Assert(Runtime.CheckWitness(owner), "Only owner can destroy");
            
            ContractManagement.Destroy();
            return true;
        }

        [DisplayName("OwnerChanged")]
        public static event Action<UInt160, UInt160> OnOwnerChanged = default!;

        [DisplayName("Initialized")]
        public static event Action<UInt160, object> OnInitialized = default!;

        [DisplayName("Updated")]
        public static event Action OnUpdated = default!;
    }
}