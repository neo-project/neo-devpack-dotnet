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
    [DisplayName("ExampleToken")]
    [ManifestExtra("Author", "Neo Deployment Example")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Example NEP-17 Token Implementation")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public class TokenContract : SmartContract
    {
        // Token configuration
        private static readonly string TokenName = "Example Token";
        private static readonly string TokenSymbol = "EXT";
        private static readonly byte TokenDecimals = 8;
        private static readonly BigInteger InitialSupply = 100_000_000_00000000; // 100 million tokens
        private static readonly BigInteger MaxSupply = 1_000_000_000_00000000; // 1 billion tokens

        // Contract owner
        private static readonly UInt160 Owner = "NiHURp5SrPnxKHVQNvpDcPVHZnCUXn3w7G".ToScriptHash();

        // Storage prefixes
        private const byte Prefix_TotalSupply = 0x00;
        private const byte Prefix_Balance = 0x01;
        private const byte Prefix_Allowance = 0x02;
        private const byte Prefix_Paused = 0x03;
        private const byte Prefix_Blacklist = 0x04;

        // Helper methods
        private static void RequireOwner()
        {
            if (!Runtime.CheckWitness(Owner))
                throw new Exception("Only owner can perform this action");
        }

        private static void RequireNotPaused()
        {
            if (Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Paused }).Length > 0)
                throw new Exception("Contract is paused");
        }

        private static bool IsBlacklisted(UInt160 account)
        {
            var key = new byte[] { Prefix_Blacklist }.Concat(account);
            return Storage.Get(Storage.CurrentContext, key).Length > 0;
        }

        // NEP-17 Token Standard
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => TokenSymbol;

        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => TokenDecimals;

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

        // Deploy
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            // Set initial supply
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, InitialSupply);

            // Allocate initial supply to owner
            SetBalance(Owner, InitialSupply);

            OnTransfer(null, Owner, InitialSupply);
        }

        // Transfer
        [DisplayName("transfer")]
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data = null)
        {
            RequireNotPaused();

            if (!from.IsValid || !to.IsValid)
                throw new Exception("Invalid address");
            if (amount < 0)
                throw new Exception("Amount must be non-negative");
            if (!Runtime.CheckWitness(from))
                return false;
            if (IsBlacklisted(from) || IsBlacklisted(to))
                throw new Exception("Address is blacklisted");

            var fromBalance = GetBalance(from);
            if (fromBalance < amount)
                return false;

            if (from != to)
            {
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

        // Extended functionality

        // Approve spending
        [DisplayName("approve")]
        public static bool Approve(UInt160 owner, UInt160 spender, BigInteger amount)
        {
            RequireNotPaused();

            if (!owner.IsValid || !spender.IsValid)
                throw new Exception("Invalid address");
            if (amount < 0)
                throw new Exception("Amount must be non-negative");
            if (!Runtime.CheckWitness(owner))
                return false;

            SetAllowance(owner, spender, amount);
            OnApproval(owner, spender, amount);

            return true;
        }

        // Get allowance
        [DisplayName("allowance")]
        [Safe]
        public static BigInteger Allowance(UInt160 owner, UInt160 spender)
        {
            if (!owner.IsValid || !spender.IsValid)
                throw new Exception("Invalid address");

            return GetAllowance(owner, spender);
        }

        // Transfer from approved amount
        [DisplayName("transferFrom")]
        public static bool TransferFrom(UInt160 spender, UInt160 from, UInt160 to, BigInteger amount)
        {
            RequireNotPaused();

            if (!spender.IsValid || !from.IsValid || !to.IsValid)
                throw new Exception("Invalid address");
            if (amount < 0)
                throw new Exception("Amount must be non-negative");
            if (!Runtime.CheckWitness(spender))
                return false;
            if (IsBlacklisted(from) || IsBlacklisted(to))
                throw new Exception("Address is blacklisted");

            var allowed = GetAllowance(from, spender);
            if (allowed < amount)
                return false;

            var fromBalance = GetBalance(from);
            if (fromBalance < amount)
                return false;

            // Update balances
            SetBalance(from, fromBalance - amount);
            var toBalance = GetBalance(to);
            SetBalance(to, toBalance + amount);

            // Update allowance
            if (allowed != amount)
                SetAllowance(from, spender, allowed - amount);

            OnTransfer(from, to, amount);

            return true;
        }

        // Mint new tokens
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

            if (newSupply > MaxSupply)
                throw new Exception("Exceeds maximum supply");

            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_TotalSupply }, newSupply);

            var balance = GetBalance(to);
            SetBalance(to, balance + amount);

            OnTransfer(null, to, amount);
        }

        // Burn tokens
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

        // Pause/Unpause contract
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

        [DisplayName("isPaused")]
        [Safe]
        public static bool IsPaused()
        {
            return Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Paused }).Length > 0;
        }

        // Blacklist management
        [DisplayName("blacklist")]
        public static void Blacklist(UInt160 account)
        {
            RequireOwner();

            if (!account.IsValid || account.IsZero)
                throw new Exception("Invalid account");

            var key = new byte[] { Prefix_Blacklist }.Concat(account);
            Storage.Put(Storage.CurrentContext, key, 1);

            OnBlacklisted(account);
        }

        [DisplayName("removeFromBlacklist")]
        public static void RemoveFromBlacklist(UInt160 account)
        {
            RequireOwner();

            if (!account.IsValid || account.IsZero)
                throw new Exception("Invalid account");

            var key = new byte[] { Prefix_Blacklist }.Concat(account);
            Storage.Delete(Storage.CurrentContext, key);

            OnRemovedFromBlacklist(account);
        }

        [DisplayName("isBlacklisted")]
        [Safe]
        public static bool IsBlacklistedMethod(UInt160 account)
        {
            if (!account.IsValid || account.IsZero)
                return false;

            return IsBlacklisted(account);
        }

        // Get token info
        [DisplayName("getInfo")]
        [Safe]
        public static Map<string, object> GetInfo()
        {
            var info = new Map<string, object>();
            info["name"] = TokenName;
            info["symbol"] = TokenSymbol;
            info["decimals"] = TokenDecimals;
            info["totalSupply"] = TotalSupply();
            info["maxSupply"] = MaxSupply;
            info["owner"] = Owner;
            info["paused"] = IsPaused();

            return info;
        }

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

        private static byte[] CreateAllowanceKey(UInt160 owner, UInt160 spender)
        {
            return new byte[] { Prefix_Allowance }.Concat(owner).Concat(spender);
        }

        private static BigInteger GetAllowance(UInt160 owner, UInt160 spender)
        {
            var key = CreateAllowanceKey(owner, spender);
            return (BigInteger)Storage.Get(Storage.CurrentContext, key);
        }

        private static void SetAllowance(UInt160 owner, UInt160 spender, BigInteger amount)
        {
            var key = CreateAllowanceKey(owner, spender);
            if (amount <= 0)
                Storage.Delete(Storage.CurrentContext, key);
            else
                Storage.Put(Storage.CurrentContext, key, amount);
        }

        // Events
        [DisplayName("Transfer")]
        public static event Action<UInt160?, UInt160?, BigInteger> OnTransfer;

        [DisplayName("Approval")]
        public static event Action<UInt160, UInt160, BigInteger> OnApproval;

        [DisplayName("Paused")]
        public static event Action OnPaused;

        [DisplayName("Unpaused")]
        public static event Action OnUnpaused;

        [DisplayName("Blacklisted")]
        public static event Action<UInt160> OnBlacklisted;

        [DisplayName("RemovedFromBlacklist")]
        public static event Action<UInt160> OnRemovedFromBlacklist;

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