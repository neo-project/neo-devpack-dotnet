using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;
using Helper = Neo.SmartContract.Framework.Helper;

namespace NFTContract
{
    /// <summary>
    /// Non-Fungible Token Smart Contract Template
    /// </summary>
    public class NFTContract : SmartContract
    {
        [DisplayName("MintToken")]
        public static event Action<byte[], byte[], byte[]> MintTokenNotify;

        [DisplayName("Transfer")]
        public static event Action<byte[], byte[], BigInteger, byte[]> TransferNotify;

        private static readonly byte[] superAdmin = Helper.ToScriptHash("Nj9Epc1x2sDmd6yH5qJPYwXRqSRf5X6KHE");

        private static StorageContext Context() => Storage.CurrentContext;

        private const byte Prefix_TotalSupply = 10;
        private const byte Prefix_TokenOwner = 11;
        private const byte Prefix_TokenBalance = 12;
        private const byte Prefix_Properties = 13;
        private const byte Prefix_TokensOf = 14;

        private const int TOKEN_DECIMALS = 8;
        private const int FACTOR = 100_000_000;

        public static string Name()
        {
            return "MyNFT";
        }

        public static string Symbol()
        {
            return "MNFT";
        }

        public static string[] SupportedStandards()
        {
            return new string[] { "NEP-10", "NEP-11" };
        }

        public static byte[] CreateStorageKey(byte[] prefix, byte[] key)
        {
            return prefix.Concat(key);
        }

        public static BigInteger TotalSupply()
        {
            return Storage.Get(Context(), Prefix_TotalSupply.ToByteArray()).ToBigInteger();
        }

        public static int Decimals()
        {
            return TOKEN_DECIMALS;
        }

        public static Enumerator<byte[]> OwnerOf(byte[] tokenid)
        {
            return Storage.Find(Context(), CreateStorageKey(Prefix_TokenOwner.ToByteArray(), tokenid)).Values;
        }

        public static Enumerator<byte[]> TokensOf(byte[] owner)
        {
            if (owner.Length != 20) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            return Storage.Find(Context(), CreateStorageKey(Prefix_TokensOf.ToByteArray(), owner)).Values;
        }

        public static string Properties(byte[] tokenid)
        {
            return Storage.Get(Context(), CreateStorageKey(Prefix_Properties.ToByteArray(), tokenid)).AsString();
        }

        public static bool Mint(byte[] tokenId, byte[] owner, byte[] properties)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            if (owner.Length != 20) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            if (properties.Length > 2048) throw new FormatException("The length of 'properties' should be less than 2048.");

            StorageMap tokenOwnerMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenOwner.ToByteArray(), tokenId));
            if (tokenOwnerMap.Get(owner) != null) return false;

            StorageMap tokenOfMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokensOf.ToByteArray(), owner));
            Storage.Put(Context(), CreateStorageKey(Prefix_Properties.ToByteArray(), tokenId), properties);
            tokenOwnerMap.Put(owner, owner);
            tokenOfMap.Put(tokenId, tokenId);

            var totalSupply = Storage.Get(Context(), Prefix_TotalSupply.ToByteArray());
            if (totalSupply is null)
                Storage.Put(Context(), Prefix_TotalSupply.ToByteArray(), 1);
            else
                Storage.Put(Context(), Prefix_TotalSupply.ToByteArray(), totalSupply.ToBigInteger() + 1);

            StorageMap tokenBalanceMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance.ToByteArray(), owner));
            tokenBalanceMap.Put(tokenId, FACTOR);

            // Notify
            MintTokenNotify(owner, tokenId, properties);
            return true;
        }

        public static BigInteger BalanceOf(byte[] owner, byte[] tokenid)
        {
            if (owner.Length != 20) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            if (tokenid is null)
            {
                var iterator = Storage.Find(Context(), CreateStorageKey(Prefix_TokenBalance.ToByteArray(), owner));
                BigInteger result = 0;
                while (iterator.Next())
                    result += iterator.Value.ToBigInteger();
                return result;
            }
            else
                return Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance.ToByteArray(), owner)).Get(tokenid).ToBigInteger();
        }

        public static bool Transfer(byte[] from, byte[] to, BigInteger amount, byte[] tokenId)
        {
            if (from.Length != 20 || to.Length != 20) throw new FormatException("The parameters 'from' and 'to' should be 20-byte addresses.");
            if (amount < 0 || amount > FACTOR) throw new FormatException("The parameters 'amount' is out of range.");
            if (!Runtime.CheckWitness(from)) return false;

            if (from.Equals(to))
            {
                Transfer(from, to, amount, tokenId);
                return true;
            }

            StorageMap fromTokenBalanceMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance.ToByteArray(), from));
            StorageMap toTokenBalanceMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenBalance.ToByteArray(), to));
            StorageMap tokenOwnerMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokenOwner.ToByteArray(), tokenId));
            StorageMap fromTokensOfMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokensOf.ToByteArray(), from));
            StorageMap toTokensOfMap = Storage.CurrentContext.CreateMap(CreateStorageKey(Prefix_TokensOf.ToByteArray(), to));

            var fromTokenBalance = fromTokenBalanceMap.Get(tokenId);
            if (fromTokenBalance == null || fromTokenBalance.ToBigInteger() < amount) return false;
            var fromNewBalance = fromTokenBalance.ToBigInteger() - amount;
            if (fromNewBalance == 0)
            {
                tokenOwnerMap.Delete(from);
                fromTokensOfMap.Delete(tokenId);
            }
            fromTokenBalanceMap.Put(tokenId, fromNewBalance);

            var toTokenBalance = toTokenBalanceMap.Get(tokenId);
            if (toTokenBalance is null && amount > 0)
            {
                tokenOwnerMap.Put(to, to);
                toTokenBalanceMap.Put(tokenId, amount);
                toTokensOfMap.Put(tokenId, tokenId);
            }
            else
            {
                toTokenBalanceMap.Put(tokenId, toTokenBalance.ToBigInteger() + amount);
            }

            // Notify
            TransferNotify(from, to, amount, tokenId);
            return true;
        }

        public static bool Migrate(byte[] script, string manifest)
        {
            if (!Runtime.CheckWitness(superAdmin))
            {
                return false;
            }
            if (script.Length == 0 || manifest.Length == 0)
            {
                return false;
            }
            Contract.Update(script, manifest);
            return true;
        }

        public static bool Destroy()
        {
            if (!Runtime.CheckWitness(superAdmin))
            {
                return false;
            }

            Contract.Destroy();
            return true;
        }
    }
}
