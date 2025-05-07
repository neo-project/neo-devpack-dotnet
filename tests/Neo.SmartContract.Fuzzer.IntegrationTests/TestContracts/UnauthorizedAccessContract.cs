using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.IntegrationTests.TestContracts
{
    [DisplayName("UnauthorizedAccessContract")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "A contract with unauthorized access vulnerabilities for testing")]
    public class UnauthorizedAccessContract : Framework.SmartContract
    {
        // Storage keys
        private static readonly StorageMap BalanceMap = new StorageMap(Storage.CurrentContext, "balance");
        private static readonly StorageMap AdminMap = new StorageMap(Storage.CurrentContext, "admin");

        // Events
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

        // Vulnerable to unauthorized access - no witness check
        public static void SetBalance(UInt160 account, BigInteger amount)
        {
            // No witness check
            BalanceMap.Put(account, amount);
            OnTransfer(null, account, amount);
        }

        // Vulnerable to unauthorized access - no witness check
        public static void TransferBalance(UInt160 from, UInt160 to, BigInteger amount)
        {
            // No witness check
            BigInteger fromBalance = GetBalance(from);
            if (fromBalance < amount)
                throw new Exception("Insufficient balance");
                
            BalanceMap.Put(from, fromBalance - amount);
            
            BigInteger toBalance = GetBalance(to);
            BalanceMap.Put(to, toBalance + amount);
            
            OnTransfer(from, to, amount);
        }

        // Safe version with witness check
        public static void SafeSetBalance(UInt160 account, BigInteger amount)
        {
            if (!Runtime.CheckWitness(account))
                throw new Exception("Unauthorized");
                
            BalanceMap.Put(account, amount);
            OnTransfer(null, account, amount);
        }

        // Safe version with witness check
        public static void SafeTransferBalance(UInt160 from, UInt160 to, BigInteger amount)
        {
            if (!Runtime.CheckWitness(from))
                throw new Exception("Unauthorized");
                
            BigInteger fromBalance = GetBalance(from);
            if (fromBalance < amount)
                throw new Exception("Insufficient balance");
                
            BalanceMap.Put(from, fromBalance - amount);
            
            BigInteger toBalance = GetBalance(to);
            BalanceMap.Put(to, toBalance + amount);
            
            OnTransfer(from, to, amount);
        }

        // Helper method
        public static BigInteger GetBalance(UInt160 account)
        {
            return (BigInteger)BalanceMap.Get(account);
        }
    }
}
