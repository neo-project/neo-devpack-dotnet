using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Company.SmartContract
{
    [DisplayName("MultiSigContract")]
    [ManifestExtra("Author", "Your Name")]
    [ManifestExtra("Email", "your.email@example.com")]
    [ManifestExtra("Description", "A Multi-Signature Wallet Contract")]
    [ContractPermission("*", "*")]
    public class MultiSigContract : SmartContract
    {
        #region Storage Prefixes

        private const byte Prefix_Owner = 0x01;
        private const byte Prefix_Transaction = 0x02;
        private const byte Prefix_Confirmation = 0x03;
        private const byte Prefix_TransactionCount = 0x04;
        private const byte Prefix_RequiredConfirmations = 0x05;

        #endregion

        #region Events

        [DisplayName("TransactionSubmitted")]
        public static event Action<BigInteger, UInt160, UInt160, BigInteger, ByteString> OnTransactionSubmitted;

        [DisplayName("TransactionConfirmed")]
        public static event Action<BigInteger, UInt160> OnTransactionConfirmed;

        [DisplayName("TransactionExecuted")]
        public static event Action<BigInteger> OnTransactionExecuted;

        [DisplayName("OwnerAdded")]
        public static event Action<UInt160> OnOwnerAdded;

        [DisplayName("OwnerRemoved")]
        public static event Action<UInt160> OnOwnerRemoved;

        [DisplayName("RequirementChanged")]
        public static event Action<BigInteger> OnRequirementChanged;

        #endregion

        #region Deployment

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            // Initialize with transaction creator as first owner
            var tx = (Transaction)Runtime.ScriptContainer;
            var creator = tx.Sender;
            
            // Set initial owners and requirements (customize as needed)
            UInt160[] initialOwners = { creator };
            BigInteger required = 1;
            
            InitializeContract(initialOwners, required);
        }

        private static void InitializeContract(UInt160[] owners, BigInteger required)
        {
            if (owners.Length == 0)
                throw new Exception("At least one owner is required");
            
            if (required > owners.Length || required <= 0)
                throw new Exception("Invalid required confirmations");

            // Store owners
            for (int i = 0; i < owners.Length; i++)
            {
                if (!owners[i].IsValid)
                    throw new Exception("Invalid owner address");
                
                var ownerKey = Prefix_Owner.ToByteArray().Concat(owners[i]);
                Storage.Put(Storage.CurrentContext, ownerKey, true);
            }

            // Store required confirmations
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_RequiredConfirmations }, required);
            
            // Initialize transaction counter
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TransactionCount }, 0);
        }

        #endregion

        #region Owner Management

        [Safe]
        public static bool IsOwner(UInt160 address)
        {
            var ownerKey = Prefix_Owner.ToByteArray().Concat(address);
            return Storage.Get(Storage.CurrentContext, ownerKey) != null;
        }

        [Safe]
        public static BigInteger GetRequiredConfirmations()
        {
            return Storage.Get(Storage.CurrentContext, new byte[] { Prefix_RequiredConfirmations });
        }

        public static void AddOwner(UInt160 newOwner)
        {
            if (!newOwner.IsValid)
                throw new Exception("Invalid owner address");
            
            if (IsOwner(newOwner))
                throw new Exception("Address is already an owner");

            // This should be called through a multi-sig transaction
            var ownerKey = Prefix_Owner.ToByteArray().Concat(newOwner);
            Storage.Put(Storage.CurrentContext, ownerKey, true);
            
            OnOwnerAdded(newOwner);
        }

        public static void RemoveOwner(UInt160 owner)
        {
            if (!IsOwner(owner))
                throw new Exception("Address is not an owner");

            // This should be called through a multi-sig transaction
            var ownerKey = Prefix_Owner.ToByteArray().Concat(owner);
            Storage.Delete(Storage.CurrentContext, ownerKey);
            
            OnOwnerRemoved(owner);
        }

        public static void ChangeRequirement(BigInteger required)
        {
            if (required <= 0)
                throw new Exception("Invalid requirement");

            // This should be called through a multi-sig transaction
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_RequiredConfirmations }, required);
            
            OnRequirementChanged(required);
        }

        #endregion

        #region Transaction Management

        public static BigInteger SubmitTransaction(UInt160 to, BigInteger value, ByteString data)
        {
            var sender = (Transaction)Runtime.ScriptContainer;
            var from = sender.Sender;
            
            if (!IsOwner(from))
                throw new Exception("Only owners can submit transactions");

            if (!to.IsValid)
                throw new Exception("Invalid recipient address");

            if (value < 0)
                throw new Exception("Invalid value");

            // Get next transaction ID
            var transactionCount = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_TransactionCount });
            var txId = (BigInteger)transactionCount + 1;
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TransactionCount }, txId);

            // Store transaction
            var txKey = Prefix_Transaction.ToByteArray().Concat(txId.ToByteArray());
            var transaction = new Map<string, object>
            {
                ["to"] = to,
                ["value"] = value,
                ["data"] = data,
                ["executed"] = false,
                ["confirmations"] = 0
            };
            Storage.Put(Storage.CurrentContext, txKey, StdLib.Serialize(transaction));

            // Auto-confirm by submitter
            ConfirmTransaction(txId);

            OnTransactionSubmitted(txId, from, to, value, data);
            return txId;
        }

        public static void ConfirmTransaction(BigInteger transactionId)
        {
            var sender = (Transaction)Runtime.ScriptContainer;
            var from = sender.Sender;
            
            if (!IsOwner(from))
                throw new Exception("Only owners can confirm transactions");

            var txKey = Prefix_Transaction.ToByteArray().Concat(transactionId.ToByteArray());
            var txData = Storage.Get(Storage.CurrentContext, txKey);
            if (txData == null)
                throw new Exception("Transaction does not exist");

            var transaction = (Map<string, object>)StdLib.Deserialize(txData);
            if ((bool)transaction["executed"])
                throw new Exception("Transaction already executed");

            var confirmKey = Prefix_Confirmation.ToByteArray().Concat(transactionId.ToByteArray()).Concat(from);
            if (Storage.Get(Storage.CurrentContext, confirmKey) != null)
                throw new Exception("Transaction already confirmed by this owner");

            // Record confirmation
            Storage.Put(Storage.CurrentContext, confirmKey, true);
            
            // Update confirmation count
            var confirmations = (BigInteger)transaction["confirmations"] + 1;
            transaction["confirmations"] = confirmations;
            Storage.Put(Storage.CurrentContext, txKey, StdLib.Serialize(transaction));

            OnTransactionConfirmed(transactionId, from);

            // Execute if enough confirmations
            var required = GetRequiredConfirmations();
            if (confirmations >= required)
            {
                ExecuteTransaction(transactionId, transaction);
            }
        }

        private static void ExecuteTransaction(BigInteger transactionId, Map<string, object> transaction)
        {
            var to = (UInt160)transaction["to"];
            var value = (BigInteger)transaction["value"];
            var data = (ByteString)transaction["data"];

            // Mark as executed
            transaction["executed"] = true;
            var txKey = Prefix_Transaction.ToByteArray().Concat(transactionId.ToByteArray());
            Storage.Put(Storage.CurrentContext, txKey, StdLib.Serialize(transaction));

            // Execute the transaction
            if (value > 0)
            {
                // Transfer GAS or NEO
                if (!GAS.Transfer(Runtime.ExecutingScriptHash, to, value, null))
                    throw new Exception("Transfer failed");
            }

            if (data.Length > 0)
            {
                // Call contract method if data provided
                Contract.Call(to, "onReceive", CallFlags.All, Runtime.ExecutingScriptHash, value, data);
            }

            OnTransactionExecuted(transactionId);
        }

        #endregion

        #region View Methods

        [Safe]
        public static Map<string, object> GetTransaction(BigInteger transactionId)
        {
            var txKey = Prefix_Transaction.ToByteArray().Concat(transactionId.ToByteArray());
            var txData = Storage.Get(Storage.CurrentContext, txKey);
            if (txData == null)
                return null;

            return (Map<string, object>)StdLib.Deserialize(txData);
        }

        [Safe]
        public static bool IsConfirmed(BigInteger transactionId, UInt160 owner)
        {
            var confirmKey = Prefix_Confirmation.ToByteArray().Concat(transactionId.ToByteArray()).Concat(owner);
            return Storage.Get(Storage.CurrentContext, confirmKey) != null;
        }

        [Safe]
        public static BigInteger GetTransactionCount()
        {
            return Storage.Get(Storage.CurrentContext, new byte[] { Prefix_TransactionCount });
        }

        #endregion

        #region Token Handling

        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            // Accept NEP-17 tokens
        }

        public static void OnNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
        {
            // Accept NEP-11 tokens
        }

        #endregion

        #region Admin

        public static void Update(ByteString nefFile, string manifest)
        {
            // This should be called through a multi-sig transaction
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void Destroy()
        {
            // This should be called through a multi-sig transaction
            ContractManagement.Destroy();
        }

        #endregion
    }
}