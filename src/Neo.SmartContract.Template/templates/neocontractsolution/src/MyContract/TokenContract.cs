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
    [ManifestExtra("Description", "NEP-17 Token Implementation")]
    [ContractSourceCode("https://github.com/mycompany/mycontract")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public class TokenContract : SmartContract
    {
        // Token settings
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => "MYT";

        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => 8;

        private const byte Prefix_TotalSupply = 0x00;
        private const byte Prefix_Balance = 0x01;

        #if (enableSecurityFeatures)
        // Security: Contract owner for administrative functions
        private static readonly UInt160 Owner = "NYourOwnerAddressHere".ToScriptHash();
        
        // Security: Paused state
        private const byte Prefix_Paused = 0x02;

        private static void RequireOwner()
        {
            if (!Runtime.CheckWitness(Owner))
                throw new Exception("Only owner can perform this action");
        }

        [DisplayName("pause")]
        public static void Pause()
        {
            RequireOwner();
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Paused }, 1);
            OnPaused();
        }

        [DisplayName("unpause")]
        public static void Unpause()
        {
            RequireOwner();
            Storage.Delete(Storage.CurrentContext, new byte[] { Prefix_Paused });
            OnUnpaused();
        }

        private static void RequireNotPaused()
        {
            if (Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Paused }).Length > 0)
                throw new Exception("Contract is paused");
        }
        #endif

        // Deploy
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            // Initial supply: 10,000,000 tokens
            var initialSupply = 10_000_000_00000000;
            var deployerKey = CreateBalanceKey(Runtime.ExecutingScriptHash);
            
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, initialSupply);
            Storage.Put(Storage.CurrentContext, deployerKey, initialSupply);

            OnTransfer(null, Runtime.ExecutingScriptHash, initialSupply);
        }

        // NEP-17 Methods
        [DisplayName("totalSupply")]
        [Safe]
        public static BigInteger TotalSupply()
        {
            return (BigInteger)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_TotalSupply });
        }

        [DisplayName("balanceOf")]
        [Safe]
        public static BigInteger BalanceOf(UInt160 account)
        {
            if (!account.IsValid || account.IsZero)
                throw new Exception("Invalid account");

            return GetBalance(account);
        }

        [DisplayName("transfer")]
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null)
        {
            #if (enableSecurityFeatures)
            RequireNotPaused();
            #endif

            if (!from.IsValid || !to.IsValid)
                throw new Exception("Invalid address");
            if (amount < 0)
                throw new Exception("Amount must be non-negative");
            if (!Runtime.CheckWitness(from))
                return false;

            var fromBalance = GetBalance(from);
            if (fromBalance < amount)
                return false;

            if (from != to)
            {
                // Update balances
                SetBalance(from, fromBalance - amount);
                var toBalance = GetBalance(to);
                SetBalance(to, toBalance + amount);
            }

            OnTransfer(from, to, amount);

            // Call onNEP17Payment if receiver is a contract
            if (ContractManagement.GetContract(to) != null)
                Contract.Call(to, "onNEP17Payment", CallFlags.All, from, amount, data);

            return true;
        }

        #if (enableSecurityFeatures)
        // Additional token features
        [DisplayName("mint")]
        public static void Mint(UInt160 to, BigInteger amount)
        {
            RequireOwner();
            RequireNotPaused();
            
            if (!to.IsValid || to.IsZero)
                throw new Exception("Invalid recipient");
            if (amount <= 0)
                throw new Exception("Amount must be positive");

            var currentSupply = TotalSupply();
            var newSupply = currentSupply + amount;
            
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, newSupply);
            
            var balance = GetBalance(to);
            SetBalance(to, balance + amount);
            
            OnTransfer(null, to, amount);
        }

        [DisplayName("burn")]
        public static void Burn(BigInteger amount)
        {
            RequireNotPaused();
            
            var from = (UInt160)Runtime.CallingScriptHash;
            if (amount <= 0)
                throw new Exception("Amount must be positive");

            var balance = GetBalance(from);
            if (balance < amount)
                throw new Exception("Insufficient balance");

            SetBalance(from, balance - amount);
            
            var currentSupply = TotalSupply();
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, currentSupply - amount);
            
            OnTransfer(from, null, amount);
        }
        #endif

        // Helper methods
        private static byte[] CreateBalanceKey(UInt160 account)
        {
            return new byte[] { Prefix_Balance }.Concat(account);
        }

        private static BigInteger GetBalance(UInt160 account)
        {
            var key = CreateBalanceKey(account);
            return (BigInteger)Storage.Get(Storage.CurrentContext, key);
        }

        private static void SetBalance(UInt160 account, BigInteger balance)
        {
            var key = CreateBalanceKey(account);
            if (balance <= 0)
                Storage.Delete(Storage.CurrentContext, key);
            else
                Storage.Put(Storage.CurrentContext, key, balance);
        }

        // Events
        [DisplayName("Transfer")]
        public static event Action<UInt160?, UInt160?, BigInteger> OnTransfer;

        #if (enableSecurityFeatures)
        [DisplayName("Paused")]
        public static event Action OnPaused;

        [DisplayName("Unpaused")]
        public static event Action OnUnpaused;
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