using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{NameSpace}}
{
    /// <summary>
    /// Updatable NEP-17 Token Contract Template
    /// 
    /// This template provides a fully functional NEP-17 token with update capability.
    /// Customize the authorization logic in the Update method based on your requirements.
    /// </summary>
    [DisplayName("{{ContractName}}")]
    [ContractSourceCode("{{SourceUrl}}")]
    [ManifestExtra("Author", "{{Author}}")]
    [ManifestExtra("Email", "{{Email}}")]
    [ManifestExtra("Description", "{{Description}}")]
    [ManifestExtra("Version", "{{Version}}")]
    [ContractPermission("*", "*")]
    [SupportedStandards("NEP-17")]
    public class {{ContractClassName}} : SmartContract
    {
        #region Token Configuration
        
        // Token metadata - customize these values
        private const string TOKEN_NAME = "{{TokenName}}";
        private const string TOKEN_SYMBOL = "{{TokenSymbol}}";
        private const byte DECIMALS = {{Decimals}};
        private static readonly BigInteger TOTAL_SUPPLY = {{TotalSupply}}_{{DecimalZeros}}; // {{TotalSupplyDescription}}
        private const int CONTRACT_VERSION = 1;
        
        #endregion
        
        #region Storage Keys
        
        private const byte PREFIX_BALANCE = 0x10;
        private const byte PREFIX_OWNER = 0x11;
        private const byte PREFIX_TOTAL_SUPPLY = 0x12;
        private const byte PREFIX_METADATA = 0x13;
        private const byte PREFIX_UPDATE_HISTORY = 0x14;
        
        #endregion
        
        #region Events
        
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger> OnTransfer;
        
        [DisplayName("ContractUpdated")]
        public static event Action<UInt160, int, UInt256> OnContractUpdated;
        
        #endregion
        
        #region Deployment and Initialization
        
        /// <summary>
        /// Contract deployment/update method
        /// </summary>
        [DisplayName("_deploy")]
        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // Authorization check - customize this based on your requirements
                if (!IsAuthorizedToUpdate())
                {
                    throw new Exception("Unauthorized update attempt");
                }
                
                // Store update history
                RecordUpdate();
                
                // Emit update event
                OnContractUpdated(Runtime.CallingScriptHash, CONTRACT_VERSION, Runtime.TransactionId);
                
                // Optional: State migration if needed
                // MigrateState(data);
                
                Runtime.Log("Contract updated successfully");
                return;
            }
            
            // Initialize contract on first deployment
            var deployer = (UInt160)Runtime.CallingScriptHash;
            
            // Set contract owner
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_OWNER }, deployer);
            
            // Set initial total supply
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_TOTAL_SUPPLY }, TOTAL_SUPPLY);
            
            // Give all tokens to deployer initially
            var balanceKey = Helper.Concat(new byte[] { PREFIX_BALANCE }, deployer);
            Storage.Put(Storage.CurrentContext, balanceKey, TOTAL_SUPPLY);
            
            // Store deployment metadata
            var metadata = new Map<string, object>
            {
                ["deployedAt"] = Runtime.Time,
                ["deployedBy"] = deployer,
                ["version"] = CONTRACT_VERSION
            };
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_METADATA }, StdLib.Serialize(metadata));
            
            // Emit transfer event for initial mint
            OnTransfer(UInt160.Zero, deployer, TOTAL_SUPPLY);
        }
        
        #endregion
        
        #region NEP-17 Standard Methods
        
        /// <summary>
        /// Get token symbol
        /// </summary>
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => TOKEN_SYMBOL;
        
        /// <summary>
        /// Get token decimals
        /// </summary>
        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => DECIMALS;
        
        /// <summary>
        /// Get total supply
        /// </summary>
        [DisplayName("totalSupply")]
        [Safe]
        public static BigInteger TotalSupply()
        {
            var supply = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_TOTAL_SUPPLY });
            return supply?.Length > 0 ? (BigInteger)supply : 0;
        }
        
        /// <summary>
        /// Get balance of an account
        /// </summary>
        [DisplayName("balanceOf")]
        [Safe]
        public static BigInteger BalanceOf(UInt160 account)
        {
            if (!account.IsValid)
                throw new Exception("Invalid account");
                
            var key = Helper.Concat(new byte[] { PREFIX_BALANCE }, account);
            var balance = Storage.Get(Storage.CurrentContext, key);
            return balance?.Length > 0 ? (BigInteger)balance : 0;
        }
        
        /// <summary>
        /// Transfer tokens
        /// </summary>
        [DisplayName("transfer")]
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            if (!from.IsValid || !to.IsValid)
                throw new Exception("Invalid address");
                
            if (amount <= 0)
                throw new Exception("Invalid amount");
                
            if (!Runtime.CheckWitness(from))
                return false;
                
            var fromBalance = BalanceOf(from);
            if (fromBalance < amount)
                return false;
                
            if (from != to)
            {
                // Update balances
                UpdateBalance(from, fromBalance - amount);
                UpdateBalance(to, BalanceOf(to) + amount);
            }
            
            OnTransfer(from, to, amount);
            
            // Call onNEP17Payment if recipient is a contract
            if (ContractManagement.GetContract(to) != null)
            {
                Contract.Call(to, "onNEP17Payment", CallFlags.All, from, amount, data);
            }
            
            return true;
        }
        
        #endregion
        
        #region Update Helper Methods
        
        /// <summary>
        /// Check if caller is authorized to update the contract
        /// Customize this method based on your authorization requirements
        /// </summary>
        private static bool IsAuthorizedToUpdate()
        {
            // Option 1: Owner-only updates
            var owner = GetOwner();
            if (Runtime.CheckWitness(owner))
                return true;
            
            // Option 2: Multi-sig committee (uncomment if needed)
            // var committee = GetCommittee();
            // if (committee != null && Runtime.CheckWitness(committee))
            //     return true;
            
            // Option 3: Time-locked updates (uncomment if needed)
            // if (HasValidUpdateProposal())
            //     return true;
            
            return false;
        }
        
        /// <summary>
        /// Optional: Migrate state during update
        /// </summary>
        private static void MigrateState(object data)
        {
            // Example state migration logic
            var currentVersion = Storage.Get(Storage.CurrentContext, "version");
            if (currentVersion == null || (int)currentVersion < CONTRACT_VERSION)
            {
                // Perform migration steps
                Storage.Put(Storage.CurrentContext, "version", CONTRACT_VERSION);
                
                // Add any data structure migrations here
            }
        }
        
        /// <summary>
        /// Record update in contract history
        /// </summary>
        private static void RecordUpdate()
        {
            var updateRecord = new Map<string, object>
            {
                ["timestamp"] = Runtime.Time,
                ["updatedBy"] = Runtime.CallingScriptHash,
                ["txHash"] = Runtime.TransactionId,
                ["version"] = CONTRACT_VERSION
            };
            
            var key = Helper.Concat(new byte[] { PREFIX_UPDATE_HISTORY }, ((BigInteger)Runtime.Time).ToByteArray());
            Storage.Put(Storage.CurrentContext, key, StdLib.Serialize(updateRecord));
        }
        
        #endregion
        
        #region Administrative Methods
        
        /// <summary>
        /// Get contract owner
        /// </summary>
        [DisplayName("getOwner")]
        [Safe]
        public static UInt160 GetOwner()
        {
            var owner = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_OWNER });
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }
        
        /// <summary>
        /// Transfer ownership (owner only)
        /// </summary>
        [DisplayName("transferOwnership")]
        public static bool TransferOwnership(UInt160 newOwner)
        {
            if (!Runtime.CheckWitness(GetOwner()))
                throw new Exception("Only owner can transfer ownership");
                
            if (!newOwner.IsValid)
                throw new Exception("Invalid new owner address");
                
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_OWNER }, newOwner);
            return true;
        }
        
        /// <summary>
        /// Get contract version
        /// </summary>
        [DisplayName("getVersion")]
        [Safe]
        public static int GetVersion() => CONTRACT_VERSION;
        
        /// <summary>
        /// Get last update time
        /// </summary>
        [DisplayName("getLastUpdateTime")]
        [Safe]
        public static BigInteger GetLastUpdateTime()
        {
            var metadata = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_METADATA });
            if (metadata == null) return 0;
            
            var data = (Map<string, object>)StdLib.Deserialize(metadata);
            return data.ContainsKey("lastUpdate") ? (BigInteger)data["lastUpdate"] : (BigInteger)data["deployedAt"];
        }
        
        /// <summary>
        /// Get deployment information
        /// </summary>
        [DisplayName("getDeploymentInfo")]
        [Safe]
        public static Map<string, object> GetDeploymentInfo()
        {
            var metadata = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_METADATA });
            return metadata != null ? (Map<string, object>)StdLib.Deserialize(metadata) : new Map<string, object>();
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Update account balance
        /// </summary>
        private static void UpdateBalance(UInt160 account, BigInteger balance)
        {
            var key = Helper.Concat(new byte[] { PREFIX_BALANCE }, account);
            if (balance == 0)
            {
                Storage.Delete(Storage.CurrentContext, key);
            }
            else
            {
                Storage.Put(Storage.CurrentContext, key, balance);
            }
        }
        
        #endregion
    }
}