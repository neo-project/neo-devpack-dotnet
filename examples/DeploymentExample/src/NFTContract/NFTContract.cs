using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace DeploymentExample.Contract
{
    [DisplayName("DeploymentExample.NFTContract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [ManifestExtra("Author", "Neo Community")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Non-Fungible Token Contract")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractPermission("*", "*")]
    [SupportedStandards("NEP-11")]
    public class NFTContract : SmartContract
    {
        // Token metadata
        private const string TOKEN_NAME = "Example NFT";
        private const string TOKEN_SYMBOL = "EXNFT";

        // Storage keys
        private const byte PREFIX_OWNER = 0x20;
        private const byte PREFIX_BALANCE = 0x21;
        private const byte PREFIX_TOKEN_OWNER = 0x22;
        private const byte PREFIX_TOKEN_URI = 0x23;
        private const byte PREFIX_TOKEN_PROPERTIES = 0x24;
        private const byte PREFIX_NEXT_TOKEN_ID = 0x25;
        private const byte PREFIX_GOVERNANCE = 0x26;
        private const byte PREFIX_MINT_PRICE = 0x27;
        private const byte PREFIX_TOKEN_CONTRACT = 0x28;

        // Events
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger, ByteString> OnTransfer;

        [DisplayName("Minted")]
        public static event Action<UInt160, ByteString, string> OnMinted;

        /// <summary>
        /// Deploy the NFT contract
        /// </summary>
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (!update)
            {
                var args = (object[])data;
                var owner = (UInt160)args[0];
                var tokenContract = args.Length > 1 ? (UInt160)args[1] : UInt160.Zero;
                var mintPrice = args.Length > 2 ? (BigInteger)args[2] : 10_00000000; // 10 tokens default
                
                if (!owner.IsValid || owner.IsZero)
                {
                    throw new Exception("Invalid owner address");
                }
                
                // Set contract owner
                Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_OWNER }, owner);
                
                // Set token contract for payments
                if (tokenContract.IsValid && !tokenContract.IsZero)
                {
                    Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_TOKEN_CONTRACT }, tokenContract);
                }
                
                // Set mint price
                Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_MINT_PRICE }, mintPrice);
                
                // Initialize token ID counter
                Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_NEXT_TOKEN_ID }, 1);
            }
        }

        /// <summary>
        /// Get token symbol
        /// </summary>
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => TOKEN_SYMBOL;

        /// <summary>
        /// Get decimals (NFTs have 0 decimals)
        /// </summary>
        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => 0;

        /// <summary>
        /// Get total supply
        /// </summary>
        [DisplayName("totalSupply")]
        [Safe]
        public static BigInteger TotalSupply()
        {
            var nextId = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_NEXT_TOKEN_ID });
            return nextId?.Length > 0 ? (BigInteger)nextId - 1 : 0;
        }

        /// <summary>
        /// Get balance of an account
        /// </summary>
        [DisplayName("balanceOf")]
        [Safe]
        public static BigInteger BalanceOf(UInt160 account)
        {
            if (!account.IsValid || account.IsZero)
            {
                throw new Exception("Invalid account");
            }

            var key = Helper.Concat(new byte[] { PREFIX_BALANCE }, account);
            var balance = Storage.Get(Storage.CurrentContext, key);
            return balance?.Length > 0 ? (BigInteger)balance : 0;
        }

        /// <summary>
        /// Get owner of a token
        /// </summary>
        [DisplayName("ownerOf")]
        [Safe]
        public static UInt160 OwnerOf(ByteString tokenId)
        {
            var key = Helper.Concat(new byte[] { PREFIX_TOKEN_OWNER }, tokenId);
            var owner = Storage.Get(Storage.CurrentContext, key);
            if (owner == null || owner.Length != 20)
            {
                throw new Exception("Token does not exist");
            }
            return (UInt160)owner;
        }

        /// <summary>
        /// Get all tokens owned by an account
        /// </summary>
        [DisplayName("tokensOf")]
        [Safe]
        public static Iterator TokensOf(UInt160 account)
        {
            if (!account.IsValid || account.IsZero)
            {
                throw new Exception("Invalid account");
            }

            return Storage.Find(Storage.CurrentContext, new byte[] { PREFIX_TOKEN_OWNER }, FindOptions.ValuesOnly);
        }

        /// <summary>
        /// Transfer a token
        /// </summary>
        [DisplayName("transfer")]
        public static bool Transfer(UInt160 to, ByteString tokenId, object data)
        {
            if (!to.IsValid || to.IsZero)
            {
                throw new Exception("Invalid recipient");
            }

            var from = OwnerOf(tokenId);
            if (!Runtime.CheckWitness(from))
            {
                return false;
            }

            // Update token owner
            var ownerKey = Helper.Concat(new byte[] { PREFIX_TOKEN_OWNER }, tokenId);
            Storage.Put(Storage.CurrentContext, ownerKey, to);

            // Update balances
            if (from != to)
            {
                // Decrease from balance
                var fromBalanceKey = Helper.Concat(new byte[] { PREFIX_BALANCE }, from);
                var fromBalance = BalanceOf(from);
                if (fromBalance <= 1)
                {
                    Storage.Delete(Storage.CurrentContext, fromBalanceKey);
                }
                else
                {
                    Storage.Put(Storage.CurrentContext, fromBalanceKey, fromBalance - 1);
                }

                // Increase to balance
                var toBalanceKey = Helper.Concat(new byte[] { PREFIX_BALANCE }, to);
                var toBalance = BalanceOf(to);
                Storage.Put(Storage.CurrentContext, toBalanceKey, toBalance + 1);
            }

            OnTransfer(from, to, 1, tokenId);

            // Call onNEP11Payment if recipient is a contract
            if (ContractManagement.GetContract(to) != null)
            {
                Neo.SmartContract.Framework.Services.Contract.Call(to, "onNEP11Payment", CallFlags.All, from, 1, tokenId, data);
            }

            return true;
        }

        /// <summary>
        /// Mint a new NFT
        /// </summary>
        [DisplayName("mint")]
        public static ByteString Mint(string tokenURI, Map<string, object> properties)
        {
            var minter = Runtime.CallingScriptHash;
            if (minter == null || !Runtime.CheckWitness(minter))
            {
                throw new Exception("Unauthorized");
            }

            // Check payment if token contract is set
            var tokenContract = GetTokenContract();
            if (tokenContract != UInt160.Zero)
            {
                var mintPrice = GetMintPrice();
                var paymentSuccess = (bool)Neo.SmartContract.Framework.Services.Contract.Call(tokenContract, "transfer", CallFlags.All, 
                    minter, GetOwner(), mintPrice, "NFT mint payment");
                if (!paymentSuccess)
                {
                    throw new Exception("Payment failed");
                }
            }

            // Get next token ID
            var nextIdKey = new byte[] { PREFIX_NEXT_TOKEN_ID };
            var nextId = Storage.Get(Storage.CurrentContext, nextIdKey);
            var tokenId = nextId?.Length > 0 ? (BigInteger)nextId : 1;
            
            // Create token ID as ByteString
            var tokenIdBytes = tokenId.ToByteArray();

            // Set token owner
            var ownerKey = Helper.Concat(new byte[] { PREFIX_TOKEN_OWNER }, tokenIdBytes);
            Storage.Put(Storage.CurrentContext, ownerKey, minter);

            // Set token URI
            var uriKey = Helper.Concat(new byte[] { PREFIX_TOKEN_URI }, tokenIdBytes);
            Storage.Put(Storage.CurrentContext, uriKey, tokenURI);

            // Set token properties
            var propsKey = Helper.Concat(new byte[] { PREFIX_TOKEN_PROPERTIES }, tokenIdBytes);
            Storage.Put(Storage.CurrentContext, propsKey, StdLib.Serialize(properties));

            // Update balance
            var balanceKey = Helper.Concat(new byte[] { PREFIX_BALANCE }, minter);
            var balance = BalanceOf(minter);
            Storage.Put(Storage.CurrentContext, balanceKey, balance + 1);

            // Update next token ID
            Storage.Put(Storage.CurrentContext, nextIdKey, tokenId + 1);

            OnTransfer(UInt160.Zero, minter, 1, (ByteString)tokenIdBytes);
            OnMinted(minter, (ByteString)tokenIdBytes, tokenURI);

            return (ByteString)tokenIdBytes;
        }

        /// <summary>
        /// Get token URI
        /// </summary>
        [DisplayName("tokenURI")]
        [Safe]
        public static string TokenURI(ByteString tokenId)
        {
            var key = Helper.Concat(new byte[] { PREFIX_TOKEN_URI }, tokenId);
            var uri = Storage.Get(Storage.CurrentContext, key);
            if (uri == null)
            {
                throw new Exception("Token does not exist");
            }
            return uri;
        }

        /// <summary>
        /// Get token properties
        /// </summary>
        [DisplayName("properties")]
        [Safe]
        public static Map<string, object> Properties(ByteString tokenId)
        {
            var key = Helper.Concat(new byte[] { PREFIX_TOKEN_PROPERTIES }, tokenId);
            var props = Storage.Get(Storage.CurrentContext, key);
            if (props == null)
            {
                throw new Exception("Token does not exist");
            }
            return (Map<string, object>)StdLib.Deserialize(props);
        }

        /// <summary>
        /// Get owner
        /// </summary>
        [DisplayName("getOwner")]
        [Safe]
        public static UInt160 GetOwner()
        {
            var owner = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_OWNER });
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }

        /// <summary>
        /// Get token contract for payments
        /// </summary>
        [DisplayName("getTokenContract")]
        [Safe]
        public static UInt160 GetTokenContract()
        {
            var contract = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_TOKEN_CONTRACT });
            return contract?.Length == 20 ? (UInt160)contract : UInt160.Zero;
        }

        /// <summary>
        /// Get mint price
        /// </summary>
        [DisplayName("getMintPrice")]
        [Safe]
        public static BigInteger GetMintPrice()
        {
            var price = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_MINT_PRICE });
            return price?.Length > 0 ? (BigInteger)price : 0;
        }

        /// <summary>
        /// Set mint price (owner only)
        /// </summary>
        [DisplayName("setMintPrice")]
        public static bool SetMintPrice(BigInteger newPrice)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new Exception("Only owner can set mint price");
            }

            if (newPrice < 0)
            {
                throw new Exception("Invalid price");
            }

            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_MINT_PRICE }, newPrice);
            return true;
        }

        /// <summary>
        /// Update the contract (owner only)
        /// </summary>
        [DisplayName("update")]
        public static bool Update(ByteString nefFile, string manifest, object data)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new Exception("Only owner can update contract");
            }

            ContractManagement.Update(nefFile, manifest, data);
            return true;
        }
    }
}