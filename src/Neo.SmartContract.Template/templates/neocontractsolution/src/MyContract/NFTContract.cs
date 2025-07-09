using System;
using System.ComponentModel;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace MyContract
{
    [DisplayName("MyContract")]
    [ManifestExtra("Author", "MyContract Author")]
    [ManifestExtra("Email", "developer@mycontract.com")]
    [ManifestExtra("Description", "NEP-11 Non-Divisible NFT Implementation")]
    [ContractSourceCode("https://github.com/mycompany/mycontract")]
    [SupportedStandards("NEP-11")]
    [ContractPermission("*", "onNEP11Payment")]
    public class NFTContract : SmartContract
    {
        // NFT settings
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => "MYNFT";

        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => 0; // NFTs are non-divisible

        // Storage prefixes
        private const byte Prefix_TotalSupply = 0x00;
        private const byte Prefix_Balance = 0x01;
        private const byte Prefix_TokenOwner = 0x02;
        private const byte Prefix_Token = 0x03;
        private const byte Prefix_AccountToken = 0x04;
        private const byte Prefix_NextTokenId = 0x05;

        #if (enableSecurityFeatures)
        // Security: Contract owner for administrative functions
        private static readonly UInt160 Owner = "NYourOwnerAddressHere".ToScriptHash();
        
        // Security: Minter role
        private const byte Prefix_Minter = 0x06;
        private const byte Prefix_Paused = 0x07;

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

        private static void RequireNotPaused()
        {
            if (Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Paused }).Length > 0)
                throw new Exception("Contract is paused");
        }

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
        #endif

        // Deploy
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, 0);
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextTokenId }, 1);
        }

        // NEP-11 Methods
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

        [DisplayName("transfer")]
        public static bool Transfer(UInt160 to, ByteString tokenId, object? data = null)
        {
            #if (enableSecurityFeatures)
            RequireNotPaused();
            #endif

            if (!to.IsValid || to.IsZero)
                throw new Exception("Invalid recipient");

            var owner = GetTokenOwner(tokenId);
            if (owner == null)
                throw new Exception("Token does not exist");

            if (!Runtime.CheckWitness(owner))
                return false;

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

        [DisplayName("ownerOf")]
        [Safe]
        public static UInt160? OwnerOf(ByteString tokenId)
        {
            return GetTokenOwner(tokenId);
        }

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

        [DisplayName("tokens")]
        [Safe]
        public static Iterator<ByteString> Tokens()
        {
            var key = new byte[] { Prefix_TokenOwner };
            return Storage.Find(Storage.CurrentContext, key, FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        // Minting
        [DisplayName("mint")]
        public static ByteString Mint(UInt160 to, Map<string, object> properties)
        {
            #if (enableSecurityFeatures)
            RequireMinter();
            RequireNotPaused();
            #endif

            if (!to.IsValid || to.IsZero)
                throw new Exception("Invalid recipient");

            // Get next token ID
            var tokenIdBytes = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_NextTokenId });
            var tokenId = (BigInteger)tokenIdBytes;
            
            // Create token
            var tokenIdByteString = tokenId.ToByteArray();
            SetTokenOwner(tokenIdByteString, to);
            
            // Store properties
            properties["tokenId"] = tokenId;
            properties["mintTime"] = Runtime.Time;
            var tokenKey = CreateTokenKey(tokenIdByteString);
            Storage.Put(Storage.CurrentContext, tokenKey, StdLib.Serialize(properties));

            // Update balances and supply
            UpdateBalance(to, 1);
            AddAccountToken(to, tokenIdByteString);
            
            var currentSupply = TotalSupply();
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, currentSupply + 1);
            
            // Increment token ID counter
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextTokenId }, tokenId + 1);

            OnTransfer(null, to, 1, tokenIdByteString);

            return tokenIdByteString;
        }

        #if (enableSecurityFeatures)
        [DisplayName("burn")]
        public static void Burn(ByteString tokenId)
        {
            RequireNotPaused();
            
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
        }
        #endif

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

        #if (enableSecurityFeatures)
        [DisplayName("MinterUpdated")]
        public static event Action<UInt160, bool> OnMinterUpdated;
        #endif

        // Update contract
        [DisplayName("update")]
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            #if (enableSecurityFeatures)
            RequireOwner();
            #endif
            
            ContractManagement.Update(nefFile, manifest, data);
        }

        // Destroy contract
        [DisplayName("destroy")]
        public static void Destroy()
        {
            #if (enableSecurityFeatures)
            RequireOwner();
            #endif
            
            ContractManagement.Destroy();
        }
    }
}