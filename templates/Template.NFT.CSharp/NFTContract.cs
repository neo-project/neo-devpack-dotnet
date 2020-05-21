using Neo;
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
        [DisplayName("MintedToken")]
        public static event Action<byte[], byte[], byte[]> MintedToken;

        [DisplayName("Transferred")]
        public static event Action<byte[], byte[], BigInteger, byte[]> Transferred;

        private static readonly byte[] superAdmin = Helper.ToScriptHash("Nj9Epc1x2sDmd6yH5qJPYwXRqSRf5X6KHE");
        private static string TotalSupplyKey() => "totalSupply";

        private static StorageContext Context() => Storage.CurrentContext;
        private static byte[] StorageProfixTokenOwner(byte[] tokenId) => new byte[] { 0x10 }.Concat(tokenId);
        private static byte[] StorageProfixTotalBalance(byte[] owner) => new byte[] { 0x11 }.Concat(owner);
        private static byte[] StorageProfixTokenBalance(byte[] owner) => new byte[] { 0x12 }.Concat(owner);
        private static byte[] StorageProfixProperties(byte[] tokenId) => new byte[] { 0x13 }.Concat(tokenId);
        private static byte[] StorageProfixTokensOf(byte[] owner) => new byte[] { 0x14 }.Concat(owner);

        private const int TOKEN_DECIMALS = 8;
        private const int MAX_AMOUNT = 100_000_000;

        public static string Name()
        {
            return "MyNFT";
        }

        public static string Symbol()
        {
            return "MNFT";
        }

        public static string SupportedStandards()
        {
            return "NEP-10, NEP-11";
        }

        public static BigInteger TotalSupply()
        {
            return Storage.Get(Context(), TotalSupplyKey()).ToBigInteger();
        }

        public static int Decimals()
        {
            return TOKEN_DECIMALS;
        }

        public static Enumerator<byte[]> OwnerOf(byte[] tokenid)
        {
            return Storage.Find(Context(), new byte[] { 0x10 }.Concat(tokenid)).Values;
        }

        public static Enumerator<byte[]> TokensOf(byte[] owner)
        {
            if (owner.Length != 20) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            return Storage.Find(Context(), new byte[] { 0x14 }.Concat(owner)).Values;
        }

        public static byte[] Properties(byte[] tokenid)
        {
            return Storage.Get(Context(), StorageProfixProperties(tokenid));
        }

        public static bool MintNFT(byte[] tokenId, byte[] owner, byte[] properties)
        {
            if (!Runtime.CheckWitness(superAdmin)) return false;

            if (owner.Length != 20) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            if (properties.Length > 2048) throw new FormatException("The length of 'properties' should be less than 2048.");

            StorageMap tokenOwnerMap = Storage.CurrentContext.CreateMap(StorageProfixTokenOwner(tokenId));
            if (tokenOwnerMap.Get(owner) != null) return false;

            StorageMap tokenOfMap = Storage.CurrentContext.CreateMap(StorageProfixTokensOf(owner));
            Storage.Put(Context(), StorageProfixProperties(tokenId), properties);
            tokenOwnerMap.Put(owner, owner);
            tokenOfMap.Put(tokenId, tokenId);

            var totalSupplyKey = TotalSupplyKey();
            var totalSupply = Storage.Get(Context(), totalSupplyKey);
            if (totalSupply is null)
                Storage.Put(Context(), totalSupplyKey, 1);
            else
                Storage.Put(Context(), totalSupplyKey, totalSupply.ToBigInteger() + 1);

            StorageMap tokenBalanceMap = Storage.CurrentContext.CreateMap(StorageProfixTokenBalance(owner));
            tokenBalanceMap.Put(tokenId, MAX_AMOUNT);

            var totalBalanceKey = StorageProfixTotalBalance(owner);
            var totalBalance = Storage.Get(Context(), totalBalanceKey);
            if (totalBalance is null)
                Storage.Put(Context(), totalBalanceKey, MAX_AMOUNT);
            else
                Storage.Put(Context(), totalBalanceKey, totalBalance.ToBigInteger() + MAX_AMOUNT);

            //notify
            MintedToken(owner, tokenId, properties);
            return true;
        }

        public static BigInteger BalanceOf(byte[] owner, byte[] tokenid)
        {
            if (owner.Length != 20) throw new FormatException("The parameter 'owner' should be 20-byte address.");
            if (tokenid is null)
                return Storage.Get(Context(), StorageProfixTotalBalance(owner)).ToBigInteger();
            else
                return Storage.CurrentContext.CreateMap(StorageProfixTokenBalance(owner)).Get(tokenid).ToBigInteger();
        }

        public static bool Transfer(byte[] from, byte[] to, BigInteger amount, byte[] tokenId)
        {
            if (from.Length != 20 || to.Length != 20) throw new FormatException("The parameters 'from' and 'to' should be 20-byte addresses.");
            if (amount < 0 || amount > MAX_AMOUNT) throw new FormatException("The parameters 'amount' is out of range.");
            if (!Runtime.CheckWitness(from)) return false;

            if (from.Equals(to))
            {
                Transferred(from, to, amount, tokenId);
                return true;
            }

            StorageMap fromTokenBalanceMap = Storage.CurrentContext.CreateMap(StorageProfixTokenBalance(from));
            StorageMap toTokenBalanceMap = Storage.CurrentContext.CreateMap(StorageProfixTokenBalance(to));
            StorageMap tokenOwnerMap = Storage.CurrentContext.CreateMap(StorageProfixTokenOwner(tokenId));
            StorageMap fromTokensOfMap = Storage.CurrentContext.CreateMap(StorageProfixTokensOf(from));
            StorageMap toTokensOfMap = Storage.CurrentContext.CreateMap(StorageProfixTokensOf(to));

            var fromTokenBalance = fromTokenBalanceMap.Get(tokenId);
            var fromTotalBalance = Storage.Get(Context(), StorageProfixTotalBalance(from));
            if (fromTokenBalance == null || fromTokenBalance.ToBigInteger() < amount) return false;
            var fromNewBalance = fromTokenBalance.ToBigInteger() - amount;
            if (fromNewBalance == 0)
            {
                tokenOwnerMap.Delete(from);
                fromTokensOfMap.Delete(tokenId);
            }
            fromTokenBalanceMap.Put(tokenId, fromNewBalance);
            Storage.Put(Context(), StorageProfixTotalBalance(from), fromTotalBalance.ToBigInteger() - amount);

            var toTokenBalance = toTokenBalanceMap.Get(tokenId);
            var toTotalBalance = Storage.Get(Context(), StorageProfixTotalBalance(to));
            if (toTokenBalance is null && amount > 0)
            {
                tokenOwnerMap.Put(to, to);
                toTokenBalanceMap.Put(tokenId, amount);
                toTokensOfMap.Put(tokenId, tokenId);
            }
            else
            {
                toTokenBalanceMap.Put(tokenId, toTokenBalance.ToBigInteger() + amount);
                Storage.Put(Context(), StorageProfixTotalBalance(to), toTotalBalance.ToBigInteger() + amount);
            }

            //notify
            Transferred(from, to, amount, tokenId);
            return true;
        }
    }
}
