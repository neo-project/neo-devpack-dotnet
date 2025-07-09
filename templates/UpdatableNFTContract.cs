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
    /// Updatable NEP-11 NFT Contract Template
    /// 
    /// This template provides a fully functional NEP-11 NFT contract with update capability.
    /// Supports both divisible and non-divisible NFTs based on configuration.
    /// </summary>
    [DisplayName("{{ContractName}}")]
    [ContractSourceCode("{{SourceUrl}}")]
    [ManifestExtra("Author", "{{Author}}")]
    [ManifestExtra("Email", "{{Email}}")]
    [ManifestExtra("Description", "{{Description}}")]
    [ManifestExtra("Version", "{{Version}}")]
    [ContractPermission("*", "*")]
    [SupportedStandards("NEP-11")]
    public class {{ContractClassName}} : SmartContract
    {
        #region Contract Configuration
        
        private const string TOKEN_SYMBOL = "{{TokenSymbol}}";
        private const byte DECIMALS = {{NFTDecimals}}; // 0 for non-divisible, >0 for divisible
        private const int CONTRACT_VERSION = 1;
        private const BigInteger MAX_SUPPLY = {{MaxSupply}}; // Maximum number of NFTs
        
        #endregion
        
        #region Storage Keys
        
        private const byte PREFIX_TOKEN_ID = 0x10;
        private const byte PREFIX_OWNER = 0x11;
        private const byte PREFIX_BALANCE = 0x12;
        private const byte PREFIX_TOKEN_METADATA = 0x13;
        private const byte PREFIX_APPROVED = 0x14;
        private const byte PREFIX_APPROVED_FOR_ALL = 0x15;
        private const byte PREFIX_TOTAL_SUPPLY = 0x16;
        private const byte PREFIX_CONTRACT_OWNER = 0x17;
        private const byte PREFIX_UPDATE_HISTORY = 0x18;
        private const byte PREFIX_MINTING_CONFIG = 0x19;
        
        #endregion
        
        #region Events
        
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger, ByteString> OnTransfer;
        
        [DisplayName("Approval")]
        public static event Action<UInt160, UInt160, BigInteger, ByteString> OnApproval;
        
        [DisplayName("ApprovalForAll")]
        public static event Action<UInt160, UInt160, bool> OnApprovalForAll;
        
        [DisplayName("Mint")]
        public static event Action<UInt160, ByteString> OnMint;
        
        [DisplayName("ContractUpdated")]
        public static event Action<UInt160, int, UInt256> OnContractUpdated;
        
        #endregion
        
        #region Deployment and Initialization
        
        [DisplayName("_deploy")]
        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // Authorization check - only contract owner can update
                if (!Runtime.CheckWitness(GetContractOwner()))
                {
                    throw new Exception("Only owner can update contract");
                }
                
                // Record update history
                RecordUpdate();
                
                // Emit update event
                OnContractUpdated(Runtime.CallingScriptHash, CONTRACT_VERSION, Runtime.TransactionId);
                
                // Optional: State migration if needed
                // MigrateState(data);
                
                Runtime.Log("NFT Contract updated successfully");
                return;
            }
            
            // Initial deployment
            var deployer = (UInt160)Runtime.CallingScriptHash;
            
            // Set contract owner
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_CONTRACT_OWNER }, deployer);
            
            // Initialize total supply
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_TOTAL_SUPPLY }, 0);
            
            // Initialize minting configuration
            var mintingConfig = new Map<string, object>
            {
                ["mintingEnabled"] = true,
                ["mintPrice"] = 0, // Free minting initially
                ["maxPerAddress"] = 10 // Max NFTs per address
            };
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_MINTING_CONFIG }, StdLib.Serialize(mintingConfig));
            
            Runtime.Log("NFT Contract deployed successfully");
        }
        
        #endregion
        
        #region NEP-11 Standard Methods
        
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => TOKEN_SYMBOL;
        
        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => DECIMALS;
        
        [DisplayName("totalSupply")]
        [Safe]
        public static BigInteger TotalSupply()
        {
            var supply = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_TOTAL_SUPPLY });
            return supply?.Length > 0 ? (BigInteger)supply : 0;
        }
        
        [DisplayName("balanceOf")]
        [Safe]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            if (!owner.IsValid)
                throw new Exception("Invalid owner address");
                
            var key = Helper.Concat(new byte[] { PREFIX_BALANCE }, owner);
            var balance = Storage.Get(Storage.CurrentContext, key);
            return balance?.Length > 0 ? (BigInteger)balance : 0;
        }
        
        [DisplayName("tokensOf")]
        [Safe]
        public static Iterator TokensOf(UInt160 owner)
        {
            if (!owner.IsValid)
                throw new Exception("Invalid owner address");
                
            var prefix = Helper.Concat(new byte[] { PREFIX_TOKEN_ID }, owner);
            return Storage.Find(Storage.CurrentContext, prefix, FindOptions.RemovePrefix | FindOptions.KeysOnly);
        }
        
        [DisplayName("ownerOf")]
        [Safe]
        public static UInt160 OwnerOf(ByteString tokenId)
        {
            var key = Helper.Concat(new byte[] { PREFIX_TOKEN_ID }, tokenId);
            var owner = Storage.Get(Storage.CurrentContext, key);
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }
        
        [DisplayName("transfer")]
        public static bool Transfer(UInt160 to, ByteString tokenId, object data)
        {
            var owner = OwnerOf(tokenId);
            if (owner == UInt160.Zero)
                throw new Exception("Token does not exist");
                
            if (!Runtime.CheckWitness(owner))
                throw new Exception("No authorization");
                
            return DoTransfer(owner, to, tokenId, data);
        }
        
        [DisplayName("transferFrom")]
        public static bool TransferFrom(UInt160 from, UInt160 to, ByteString tokenId, object data)
        {
            var owner = OwnerOf(tokenId);
            if (owner != from)
                throw new Exception("From address is not the owner");
                
            if (!Runtime.CheckWitness(from) && !IsApprovedFor(tokenId, Runtime.CallingScriptHash))
                throw new Exception("No authorization");
                
            return DoTransfer(from, to, tokenId, data);
        }
        
        #endregion
        
        #region Approval Methods
        
        [DisplayName("approve")]
        public static void Approve(UInt160 approved, ByteString tokenId)
        {
            var owner = OwnerOf(tokenId);
            if (owner == UInt160.Zero)
                throw new Exception("Token does not exist");
                
            if (!Runtime.CheckWitness(owner))
                throw new Exception("No authorization");
                
            var key = Helper.Concat(new byte[] { PREFIX_APPROVED }, tokenId);
            Storage.Put(Storage.CurrentContext, key, approved);
            
            OnApproval(owner, approved, 1, tokenId);
        }
        
        [DisplayName("setApprovalForAll")]
        public static void SetApprovalForAll(UInt160 operator, bool approved)
        {
            var owner = Runtime.CallingScriptHash;
            var key = Helper.Concat(Helper.Concat(new byte[] { PREFIX_APPROVED_FOR_ALL }, owner), operator);
            
            if (approved)
            {
                Storage.Put(Storage.CurrentContext, key, 1);
            }
            else
            {
                Storage.Delete(Storage.CurrentContext, key);
            }
            
            OnApprovalForAll(owner, operator, approved);
        }
        
        [DisplayName("getApproved")]
        [Safe]
        public static UInt160 GetApproved(ByteString tokenId)
        {
            var key = Helper.Concat(new byte[] { PREFIX_APPROVED }, tokenId);
            var approved = Storage.Get(Storage.CurrentContext, key);
            return approved?.Length == 20 ? (UInt160)approved : UInt160.Zero;
        }
        
        [DisplayName("isApprovedForAll")]
        [Safe]
        public static bool IsApprovedForAll(UInt160 owner, UInt160 operator)
        {
            var key = Helper.Concat(Helper.Concat(new byte[] { PREFIX_APPROVED_FOR_ALL }, owner), operator);
            var approval = Storage.Get(Storage.CurrentContext, key);
            return approval?.Length > 0 && (BigInteger)approval == 1;
        }
        
        #endregion
        
        #region Minting Methods
        
        [DisplayName("mint")]
        public static ByteString Mint(UInt160 to, ByteString tokenId, Map<string, object> properties)
        {
            if (!IsMintingEnabled())
                throw new Exception("Minting is disabled");
                
            if (!IsAuthorizedToMint())
                throw new Exception("Unauthorized minting");
                
            if (!to.IsValid)
                throw new Exception("Invalid recipient address");
                
            if (OwnerOf(tokenId) != UInt160.Zero)
                throw new Exception("Token already exists");
                
            var currentSupply = TotalSupply();
            if (currentSupply >= MAX_SUPPLY)
                throw new Exception("Maximum supply reached");
                
            // Check per-address minting limits
            var config = GetMintingConfig();
            var maxPerAddress = (BigInteger)config["maxPerAddress"];
            if (BalanceOf(to) >= maxPerAddress)
                throw new Exception("Address minting limit reached");
                
            // Set token owner
            var tokenKey = Helper.Concat(new byte[] { PREFIX_TOKEN_ID }, tokenId);
            Storage.Put(Storage.CurrentContext, tokenKey, to);
            
            // Store token metadata
            if (properties != null && properties.Count > 0)
            {
                var metadataKey = Helper.Concat(new byte[] { PREFIX_TOKEN_METADATA }, tokenId);
                Storage.Put(Storage.CurrentContext, metadataKey, StdLib.Serialize(properties));
            }
            
            // Update balances and supply
            UpdateBalance(to, BalanceOf(to) + 1);
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_TOTAL_SUPPLY }, currentSupply + 1);
            
            OnTransfer(UInt160.Zero, to, 1, tokenId);
            OnMint(to, tokenId);
            
            return tokenId;
        }
        
        [DisplayName("mintTo")]
        public static ByteString MintTo(UInt160 to, Map<string, object> properties)
        {
            var tokenId = GenerateTokenId();
            return Mint(to, tokenId, properties);
        }
        
        #endregion
        
        #region Metadata Methods
        
        [DisplayName("properties")]
        [Safe]
        public static Map<string, object> Properties(ByteString tokenId)
        {
            if (OwnerOf(tokenId) == UInt160.Zero)
                throw new Exception("Token does not exist");
                
            var key = Helper.Concat(new byte[] { PREFIX_TOKEN_METADATA }, tokenId);
            var metadata = Storage.Get(Storage.CurrentContext, key);
            
            if (metadata != null)
            {
                return (Map<string, object>)StdLib.Deserialize(metadata);
            }
            
            return new Map<string, object>();
        }
        
        [DisplayName("updateProperties")]
        public static void UpdateProperties(ByteString tokenId, Map<string, object> properties)
        {
            var owner = OwnerOf(tokenId);
            if (owner == UInt160.Zero)
                throw new Exception("Token does not exist");
                
            if (!Runtime.CheckWitness(owner) && !Runtime.CheckWitness(GetContractOwner()))
                throw new Exception("No authorization");
                
            var key = Helper.Concat(new byte[] { PREFIX_TOKEN_METADATA }, tokenId);
            Storage.Put(Storage.CurrentContext, key, StdLib.Serialize(properties));
        }
        
        #endregion
        
        #region Update Helper Methods
        
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
        
        [DisplayName("getContractOwner")]
        [Safe]
        public static UInt160 GetContractOwner()
        {
            var owner = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_CONTRACT_OWNER });
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }
        
        [DisplayName("transferContractOwnership")]
        public static bool TransferContractOwnership(UInt160 newOwner)
        {
            if (!Runtime.CheckWitness(GetContractOwner()))
                throw new Exception("Only contract owner can transfer ownership");
                
            if (!newOwner.IsValid)
                throw new Exception("Invalid new owner address");
                
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_CONTRACT_OWNER }, newOwner);
            return true;
        }
        
        [DisplayName("setMintingEnabled")]
        public static void SetMintingEnabled(bool enabled)
        {
            if (!Runtime.CheckWitness(GetContractOwner()))
                throw new Exception("Only contract owner can modify minting settings");
                
            var config = GetMintingConfig();
            config["mintingEnabled"] = enabled;
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_MINTING_CONFIG }, StdLib.Serialize(config));
        }
        
        [DisplayName("setMintPrice")]
        public static void SetMintPrice(BigInteger price)
        {
            if (!Runtime.CheckWitness(GetContractOwner()))
                throw new Exception("Only contract owner can modify minting settings");
                
            var config = GetMintingConfig();
            config["mintPrice"] = price;
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_MINTING_CONFIG }, StdLib.Serialize(config));
        }
        
        [DisplayName("getVersion")]
        [Safe]
        public static int GetVersion() => CONTRACT_VERSION;
        
        [DisplayName("getLastUpdateTime")]
        [Safe]
        public static BigInteger GetLastUpdateTime()
        {
            // Find the most recent update record
            var iterator = Storage.Find(Storage.CurrentContext, new byte[] { PREFIX_UPDATE_HISTORY }, FindOptions.Backward);
            if (iterator.Next())
            {
                var record = (Map<string, object>)StdLib.Deserialize(iterator.Value);
                return (BigInteger)record["timestamp"];
            }
            return 0; // No updates recorded
        }
        
        #endregion
        
        #region Helper Methods
        
        private static bool DoTransfer(UInt160 from, UInt160 to, ByteString tokenId, object data)
        {
            if (!to.IsValid)
                throw new Exception("Invalid recipient address");
                
            // Update token owner
            var tokenKey = Helper.Concat(new byte[] { PREFIX_TOKEN_ID }, tokenId);
            Storage.Put(Storage.CurrentContext, tokenKey, to);
            
            // Update balances
            UpdateBalance(from, BalanceOf(from) - 1);
            UpdateBalance(to, BalanceOf(to) + 1);
            
            // Clear approvals
            var approvalKey = Helper.Concat(new byte[] { PREFIX_APPROVED }, tokenId);
            Storage.Delete(Storage.CurrentContext, approvalKey);
            
            OnTransfer(from, to, 1, tokenId);
            
            // Call onNEP11Payment if recipient is a contract
            if (ContractManagement.GetContract(to) != null)
            {
                Contract.Call(to, "onNEP11Payment", CallFlags.All, from, 1, tokenId, data);
            }
            
            return true;
        }
        
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
        
        private static bool IsApprovedFor(ByteString tokenId, UInt160 spender)
        {
            var owner = OwnerOf(tokenId);
            if (owner == spender)
                return true;
                
            var approved = GetApproved(tokenId);
            if (approved == spender)
                return true;
                
            return IsApprovedForAll(owner, spender);
        }
        
        private static bool IsMintingEnabled()
        {
            var config = GetMintingConfig();
            return (bool)config["mintingEnabled"];
        }
        
        private static bool IsAuthorizedToMint()
        {
            var owner = GetContractOwner();
            return Runtime.CheckWitness(owner);
        }
        
        private static Map<string, object> GetMintingConfig()
        {
            var config = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_MINTING_CONFIG });
            return config != null ? (Map<string, object>)StdLib.Deserialize(config) : new Map<string, object>();
        }
        
        private static ByteString GenerateTokenId()
        {
            var currentSupply = TotalSupply();
            return (currentSupply + 1).ToByteArray();
        }
        
        #endregion
    }
}