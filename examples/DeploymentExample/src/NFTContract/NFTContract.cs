using System;
using System.ComponentModel;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace DeploymentExample.Contracts
{
    [DisplayName("ExampleNFT")]
    [ManifestExtra("Author", "Neo Deployment Example")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Example NEP-11 Non-Divisible NFT Implementation")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [SupportedStandards("NEP-11")]
    [ContractPermission("*", "onNEP11Payment")]
    public class NFTContract : SmartContract
    {
        // NFT Configuration
        private static readonly string CollectionName = "Example NFT Collection";
        private static readonly string CollectionSymbol = "EXNFT";
        private static readonly string BaseUri = "https://example.com/nft/";
        private static readonly BigInteger MaxSupply = 10000;
        
        // Contract owner
        private static readonly UInt160 Owner = "NiHURp5SrPnxKHVQNvpDcPVHZnCUXn3w7G".ToScriptHash();

        // Storage prefixes
        private const byte Prefix_TotalSupply = 0x00;
        private const byte Prefix_Balance = 0x01;
        private const byte Prefix_TokenOwner = 0x02;
        private const byte Prefix_Token = 0x03;
        private const byte Prefix_AccountToken = 0x04;
        private const byte Prefix_NextTokenId = 0x05;
        private const byte Prefix_Minter = 0x06;
        private const byte Prefix_Royalty = 0x07;
        private const byte Prefix_TokenApproval = 0x08;

        // Helper methods
        private static void RequireOwner()
        {
            if (!Runtime.CheckWitness(Owner))
                throw new Exception("Only owner can perform this action");
        }

        private static void RequireMinter()
        {
            var caller = Runtime.CallingScriptHash;
            var minterKey = new byte[] { Prefix_Minter }.Concat(caller);
            
            if (Storage.Get(Storage.CurrentContext, minterKey).Length == 0 && !Runtime.CheckWitness(Owner))
                throw new Exception("Only minters can perform this action");
        }

        // NEP-11 Token Standard
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => CollectionSymbol;

        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => 0; // NFTs are non-divisible

        [DisplayName("totalSupply")]
        [Safe]
        public static BigInteger TotalSupply()
        {
            return (BigInteger)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_TotalSupply });
        }

        [DisplayName("balanceOf")]
        [Safe]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            if (!owner.IsValid || owner.IsZero)
                throw new Exception("Invalid owner");

            return GetBalance(owner);
        }

        [DisplayName("tokensOf")]
        [Safe]
        public static Iterator<ByteString> TokensOf(UInt160 owner)
        {
            if (!owner.IsValid || owner.IsZero)
                throw new Exception("Invalid owner");

            var key = new byte[] { Prefix_AccountToken }.Concat(owner);
            return Storage.Find(Storage.CurrentContext, key, FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        [DisplayName("ownerOf")]
        [Safe]
        public static UInt160? OwnerOf(ByteString tokenId)
        {
            return GetTokenOwner(tokenId);
        }

        // Deploy
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, 0);
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextTokenId }, 1);
            
            // Set contract owner as initial minter
            var minterKey = new byte[] { Prefix_Minter }.Concat(Owner);
            Storage.Put(Storage.CurrentContext, minterKey, 1);
        }

        // Transfer
        [DisplayName("transfer")]
        public static bool Transfer(UInt160 to, ByteString tokenId, object? data = null)
        {
            if (!to.IsValid || to.IsZero)
                throw new Exception("Invalid recipient");

            var owner = GetTokenOwner(tokenId);
            if (owner == null)
                throw new Exception("Token does not exist");

            // Check if caller is owner or approved
            if (!Runtime.CheckWitness(owner))
            {
                var approvalKey = CreateTokenApprovalKey(tokenId);
                var approved = Storage.Get(Storage.CurrentContext, approvalKey);
                if (approved.Length == 0 || (UInt160)approved != Runtime.CallingScriptHash)
                    return false;
                    
                // Clear approval after use
                Storage.Delete(Storage.CurrentContext, approvalKey);
            }

            if (owner != to)
            {
                // Update token ownership
                SetTokenOwner(tokenId, to);

                // Update balances
                UpdateBalance(owner, -1);
                UpdateBalance(to, 1);

                // Update account tokens
                RemoveAccountToken(owner, tokenId);
                AddAccountToken(to, tokenId);
            }

            OnTransfer(owner, to, 1, tokenId);

            // Call onNEP11Payment if receiver is a contract
            if (ContractManagement.GetContract(to) != null)
                Contract.Call(to, "onNEP11Payment", CallFlags.All, owner, 1, tokenId, data);

            return true;
        }

        // Get token properties
        [DisplayName("properties")]
        [Safe]
        public static Map<string, object> Properties(ByteString tokenId)
        {
            var tokenKey = CreateTokenKey(tokenId);
            var tokenData = Storage.Get(Storage.CurrentContext, tokenKey);
            
            if (tokenData.Length == 0)
                throw new Exception("Token does not exist");

            var properties = (Map<string, object>)StdLib.Deserialize(tokenData);
            return properties;
        }

        // Get all tokens
        [DisplayName("tokens")]
        [Safe]
        public static Iterator<ByteString> Tokens()
        {
            var key = new byte[] { Prefix_TokenOwner };
            return Storage.Find(Storage.CurrentContext, key, FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        // Extended functionality

        // Mint new NFT
        [DisplayName("mint")]
        public static ByteString Mint(UInt160 to, string name, string description, string image, Map<string, object>? attributes = null)
        {
            RequireMinter();

            if (!to.IsValid || to.IsZero)
                throw new Exception("Invalid recipient");

            // Check max supply
            var currentSupply = TotalSupply();
            if (currentSupply >= MaxSupply)
                throw new Exception("Max supply reached");

            // Get next token ID
            var tokenIdBytes = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_NextTokenId });
            var tokenId = (BigInteger)tokenIdBytes;
            
            // Create token
            var tokenIdByteString = tokenId.ToByteArray();
            SetTokenOwner(tokenIdByteString, to);
            
            // Create properties
            var properties = new Map<string, object>();
            properties["tokenId"] = tokenId;
            properties["name"] = name;
            properties["description"] = description;
            properties["image"] = image;
            properties["tokenURI"] = BaseUri + tokenId.ToString();
            properties["mintTime"] = Runtime.Time;
            properties["minter"] = Runtime.CallingScriptHash;
            
            if (attributes != null)
            {
                properties["attributes"] = attributes;
            }

            // Store token data
            var tokenKey = CreateTokenKey(tokenIdByteString);
            Storage.Put(Storage.CurrentContext, tokenKey, StdLib.Serialize(properties));

            // Update balances and supply
            UpdateBalance(to, 1);
            AddAccountToken(to, tokenIdByteString);
            
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, currentSupply + 1);
            
            // Increment token ID counter
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextTokenId }, tokenId + 1);

            OnTransfer(null, to, 1, tokenIdByteString);
            OnMint(to, tokenIdByteString, properties);

            return tokenIdByteString;
        }

        // Batch mint
        [DisplayName("batchMint")]
        public static ByteString[] BatchMint(UInt160 to, string[] names, string[] descriptions, string[] images)
        {
            RequireMinter();

            if (names.Length != descriptions.Length || names.Length != images.Length)
                throw new Exception("Array lengths must match");

            if (names.Length > 10)
                throw new Exception("Maximum 10 NFTs per batch");

            var tokenIds = new ByteString[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                tokenIds[i] = Mint(to, names[i], descriptions[i], images[i]);
            }

            return tokenIds;
        }

        // Burn NFT
        [DisplayName("burn")]
        public static void Burn(ByteString tokenId)
        {
            var owner = GetTokenOwner(tokenId);
            if (owner == null)
                throw new Exception("Token does not exist");

            if (!Runtime.CheckWitness(owner))
                throw new Exception("Only token owner can burn");

            // Remove token
            DeleteTokenOwner(tokenId);
            var tokenKey = CreateTokenKey(tokenId);
            Storage.Delete(Storage.CurrentContext, tokenKey);

            // Update balance and account tokens
            UpdateBalance(owner, -1);
            RemoveAccountToken(owner, tokenId);

            // Update total supply
            var currentSupply = TotalSupply();
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, currentSupply - 1);

            OnTransfer(owner, null, 1, tokenId);
            OnBurn(owner, tokenId);
        }

        // Approve another address to transfer a specific token
        [DisplayName("approve")]
        public static void Approve(UInt160 spender, ByteString tokenId)
        {
            if (!spender.IsValid || spender.IsZero)
                throw new Exception("Invalid spender");

            var owner = GetTokenOwner(tokenId);
            if (owner == null)
                throw new Exception("Token does not exist");

            if (!Runtime.CheckWitness(owner))
                throw new Exception("Only token owner can approve");

            var approvalKey = CreateTokenApprovalKey(tokenId);
            Storage.Put(Storage.CurrentContext, approvalKey, spender);

            OnApproval(owner, spender, tokenId);
        }

        // Get approved address for a token
        [DisplayName("getApproved")]
        [Safe]
        public static UInt160? GetApproved(ByteString tokenId)
        {
            var owner = GetTokenOwner(tokenId);
            if (owner == null)
                return null;

            var approvalKey = CreateTokenApprovalKey(tokenId);
            var approved = Storage.Get(Storage.CurrentContext, approvalKey);
            
            if (approved.Length == 0)
                return null;

            return (UInt160)approved;
        }

        // Minter management
        [DisplayName("setMinter")]
        public static void SetMinter(UInt160 account, bool isMinter)
        {
            RequireOwner();
            
            if (!account.IsValid || account.IsZero)
                throw new Exception("Invalid account");
                
            var key = new byte[] { Prefix_Minter }.Concat(account);
            
            if (isMinter)
                Storage.Put(Storage.CurrentContext, key, 1);
            else
                Storage.Delete(Storage.CurrentContext, key);
                
            OnMinterUpdated(account, isMinter);
        }

        [DisplayName("isMinter")]
        [Safe]
        public static bool IsMinter(UInt160 account)
        {
            if (!account.IsValid || account.IsZero)
                return false;

            var key = new byte[] { Prefix_Minter }.Concat(account);
            return Storage.Get(Storage.CurrentContext, key).Length > 0;
        }

        // Royalty support
        [DisplayName("setRoyalty")]
        public static void SetRoyalty(ByteString tokenId, UInt160 recipient, BigInteger percentage)
        {
            var owner = GetTokenOwner(tokenId);
            if (owner == null)
                throw new Exception("Token does not exist");

            if (!Runtime.CheckWitness(owner))
                throw new Exception("Only token owner can set royalty");

            if (!recipient.IsValid || recipient.IsZero)
                throw new Exception("Invalid royalty recipient");

            if (percentage < 0 || percentage > 1000) // Max 10%
                throw new Exception("Royalty percentage must be between 0 and 1000 (10%)");

            var royaltyKey = new byte[] { Prefix_Royalty }.Concat(tokenId);
            var royaltyData = new Map<string, object>();
            royaltyData["recipient"] = recipient;
            royaltyData["percentage"] = percentage;

            Storage.Put(Storage.CurrentContext, royaltyKey, StdLib.Serialize(royaltyData));
        }

        [DisplayName("getRoyalty")]
        [Safe]
        public static Map<string, object>? GetRoyalty(ByteString tokenId)
        {
            var royaltyKey = new byte[] { Prefix_Royalty }.Concat(tokenId);
            var data = Storage.Get(Storage.CurrentContext, royaltyKey);
            
            if (data.Length == 0)
                return null;

            return (Map<string, object>)StdLib.Deserialize(data);
        }

        // Collection info
        [DisplayName("getCollectionInfo")]
        [Safe]
        public static Map<string, object> GetCollectionInfo()
        {
            var info = new Map<string, object>();
            info["name"] = CollectionName;
            info["symbol"] = CollectionSymbol;
            info["totalSupply"] = TotalSupply();
            info["maxSupply"] = MaxSupply;
            info["baseUri"] = BaseUri;
            info["owner"] = Owner;
            info["nextTokenId"] = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_NextTokenId });

            return info;
        }

        // Helper methods
        private static byte[] CreateBalanceKey(UInt160 owner)
        {
            return new byte[] { Prefix_Balance }.Concat(owner);
        }

        private static byte[] CreateTokenOwnerKey(ByteString tokenId)
        {
            return new byte[] { Prefix_TokenOwner }.Concat(tokenId);
        }

        private static byte[] CreateTokenKey(ByteString tokenId)
        {
            return new byte[] { Prefix_Token }.Concat(tokenId);
        }

        private static byte[] CreateAccountTokenKey(UInt160 owner, ByteString tokenId)
        {
            return new byte[] { Prefix_AccountToken }.Concat(owner).Concat(tokenId);
        }

        private static byte[] CreateTokenApprovalKey(ByteString tokenId)
        {
            return new byte[] { Prefix_TokenApproval }.Concat(tokenId);
        }

        private static BigInteger GetBalance(UInt160 owner)
        {
            var key = CreateBalanceKey(owner);
            return (BigInteger)Storage.Get(Storage.CurrentContext, key);
        }

        private static void UpdateBalance(UInt160 owner, BigInteger change)
        {
            var balance = GetBalance(owner) + change;
            var key = CreateBalanceKey(owner);
            
            if (balance <= 0)
                Storage.Delete(Storage.CurrentContext, key);
            else
                Storage.Put(Storage.CurrentContext, key, balance);
        }

        private static UInt160? GetTokenOwner(ByteString tokenId)
        {
            var key = CreateTokenOwnerKey(tokenId);
            var owner = Storage.Get(Storage.CurrentContext, key);
            
            if (owner.Length == 0)
                return null;
                
            return (UInt160)owner;
        }

        private static void SetTokenOwner(ByteString tokenId, UInt160 owner)
        {
            var key = CreateTokenOwnerKey(tokenId);
            Storage.Put(Storage.CurrentContext, key, owner);
        }

        private static void DeleteTokenOwner(ByteString tokenId)
        {
            var key = CreateTokenOwnerKey(tokenId);
            Storage.Delete(Storage.CurrentContext, key);
        }

        private static void AddAccountToken(UInt160 owner, ByteString tokenId)
        {
            var key = CreateAccountTokenKey(owner, tokenId);
            Storage.Put(Storage.CurrentContext, key, 1);
        }

        private static void RemoveAccountToken(UInt160 owner, ByteString tokenId)
        {
            var key = CreateAccountTokenKey(owner, tokenId);
            Storage.Delete(Storage.CurrentContext, key);
        }

        // Events
        [DisplayName("Transfer")]
        public static event Action<UInt160?, UInt160?, BigInteger, ByteString> OnTransfer;

        [DisplayName("Mint")]
        public static event Action<UInt160, ByteString, Map<string, object>> OnMint;

        [DisplayName("Burn")]
        public static event Action<UInt160, ByteString> OnBurn;

        [DisplayName("Approval")]
        public static event Action<UInt160, UInt160, ByteString> OnApproval;

        [DisplayName("MinterUpdated")]
        public static event Action<UInt160, bool> OnMinterUpdated;

        // Contract management
        [DisplayName("update")]
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            RequireOwner();
            ContractManagement.Update(nefFile, manifest, data);
        }

        [DisplayName("destroy")]
        public static void Destroy()
        {
            RequireOwner();
            ContractManagement.Destroy();
        }
    }
}