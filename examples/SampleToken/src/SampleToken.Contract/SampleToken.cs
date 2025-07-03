using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace SampleToken.Contract
{
    [DisplayName("SampleToken")]
    [ManifestExtra("Author", "Neo Community")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Sample NEP-17 Token Contract")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public class SampleToken : SmartContract
    {
        #region Token Settings
        static readonly string Name = "Sample Token";
        static readonly string Symbol = "SAMPLE";
        static readonly byte Decimals = 8;
        static readonly BigInteger InitialSupply = 100_000_000_00000000; // 100 million tokens
        static readonly BigInteger MaxSupply = 1_000_000_000_00000000; // 1 billion tokens
        #endregion

        #region Storage Prefixes
        static readonly byte[] TotalSupplyPrefix = new byte[] { 0x00 };
        static readonly byte[] BalancePrefix = new byte[] { 0x01 };
        static readonly byte[] OwnerPrefix = new byte[] { 0x02 };
        static readonly byte[] MinterPrefix = new byte[] { 0x03 };
        #endregion

        #region Events
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

        [DisplayName("OwnershipTransferred")]
        public static event Action<UInt160, UInt160> OnOwnershipTransferred;

        [DisplayName("MinterChanged")]
        public static event Action<UInt160, bool> OnMinterChanged;
        #endregion

        #region Initialization
        [DisplayName("_deploy")]
        public static void OnDeploy(object data, bool update)
        {
            if (!update)
            {
                var owner = (UInt160)data;
                Storage.Put(Storage.CurrentContext, OwnerPrefix, owner);
                Storage.Put(Storage.CurrentContext, TotalSupplyPrefix, InitialSupply);
                Storage.Put(Storage.CurrentContext, BalancePrefix.Concat(owner), InitialSupply);
                
                OnTransfer(null, owner, InitialSupply);
            }
        }
        #endregion

        #region NEP-17 Methods
        [Safe]
        public static string GetName() => Name;

        [Safe]
        public static string GetSymbol() => Symbol;

        [Safe]
        public static byte GetDecimals() => Decimals;

        [Safe]
        public static BigInteger TotalSupply()
        {
            return Storage.Get(Storage.CurrentContext, TotalSupplyPrefix).ToBigInteger();
        }

        [Safe]
        public static BigInteger BalanceOf(UInt160 account)
        {
            if (!account.IsValid)
                throw new Exception("Invalid address");
            
            return Storage.Get(Storage.CurrentContext, BalancePrefix.Concat(account)).ToBigInteger();
        }

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
                Storage.Put(Storage.CurrentContext, BalancePrefix.Concat(from), fromBalance - amount);
                
                var toBalance = BalanceOf(to);
                Storage.Put(Storage.CurrentContext, BalancePrefix.Concat(to), toBalance + amount);
            }
            
            OnTransfer(from, to, amount);
            
            if (ContractManagement.GetContract(to) != null)
                Contract.Call(to, "onNEP17Payment", CallFlags.All, from, amount, data);
            
            return true;
        }
        #endregion

        #region Owner Methods
        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, OwnerPrefix);
        }

        public static bool TransferOwnership(UInt160 newOwner)
        {
            if (!newOwner.IsValid)
                throw new Exception("Invalid address");
            
            var currentOwner = GetOwner();
            if (!Runtime.CheckWitness(currentOwner))
                return false;
            
            Storage.Put(Storage.CurrentContext, OwnerPrefix, newOwner);
            OnOwnershipTransferred(currentOwner, newOwner);
            
            return true;
        }
        #endregion

        #region Minting Methods
        public static bool Mint(UInt160 to, BigInteger amount)
        {
            if (!to.IsValid)
                throw new Exception("Invalid address");
            
            if (amount <= 0)
                throw new Exception("Invalid amount");
            
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner) && !IsMinter(Runtime.CallingScriptHash))
                return false;
            
            var totalSupply = TotalSupply();
            var newSupply = totalSupply + amount;
            
            if (newSupply > MaxSupply)
                throw new Exception("Exceeds max supply");
            
            Storage.Put(Storage.CurrentContext, TotalSupplyPrefix, newSupply);
            
            var toBalance = BalanceOf(to);
            Storage.Put(Storage.CurrentContext, BalancePrefix.Concat(to), toBalance + amount);
            
            OnTransfer(null, to, amount);
            
            return true;
        }

        public static bool SetMinter(UInt160 account, bool isMinter)
        {
            if (!account.IsValid)
                throw new Exception("Invalid address");
            
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
                return false;
            
            if (isMinter)
                Storage.Put(Storage.CurrentContext, MinterPrefix.Concat(account), 1);
            else
                Storage.Delete(Storage.CurrentContext, MinterPrefix.Concat(account));
            
            OnMinterChanged(account, isMinter);
            
            return true;
        }

        [Safe]
        public static bool IsMinter(UInt160 account)
        {
            if (!account.IsValid)
                return false;
            
            return Storage.Get(Storage.CurrentContext, MinterPrefix.Concat(account)).Equals(1);
        }
        #endregion

        #region Utility Methods
        [Safe]
        public static BigInteger GetMaxSupply() => MaxSupply;

        [Safe]
        public static BigInteger GetRemainingSupply()
        {
            return MaxSupply - TotalSupply();
        }
        #endregion
    }
}