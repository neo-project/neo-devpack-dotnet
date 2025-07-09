using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace DeploymentExample.Contract
{
    [DisplayName("DeploymentExample.TokenContract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [ManifestExtra("Author", "Neo Community")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Fungible Token Contract")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractPermission("*", "*")]
    [SupportedStandards("NEP-17")]
    // Updated: 2025-07-08 16:50:11
    public class TokenContract : SmartContract
    {
        // Token metadata
        private const string TOKEN_NAME = "Example Token";
        private const string TOKEN_SYMBOL = "EXT";
        private const byte DECIMALS = 8;
        private static readonly BigInteger TOTAL_SUPPLY = 100_000_000_00000000; // 100 million with 8 decimals

        // Storage keys
        private const byte PREFIX_BALANCE = 0x10;
        private const byte PREFIX_OWNER = 0x11;
        private const byte PREFIX_GOVERNANCE = 0x12;
        private const byte PREFIX_PAUSED = 0x13;

        // Events
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

        [DisplayName("Minted")]
        public static event Action<UInt160, BigInteger> OnMinted;

        [DisplayName("Burned")]
        public static event Action<UInt160, BigInteger> OnBurned;

        /// <summary>
        /// Deploy/update the token contract
        /// </summary>
        [DisplayName("_deploy")]
        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // Check authorization for updates
                if (!Runtime.CheckWitness(GetOwner()))
                {
                    throw new Exception("Only owner can update contract");
                }
                // Perform any migration logic here if needed
                return;
            }
            
            // Initial deployment - just mark as deployed
            Storage.Put(Storage.CurrentContext, "deployed", 1);
        }
        
        /// <summary>
        /// Initialize the token contract after deployment
        /// </summary>
        [DisplayName("initialize")]
        public static bool Initialize(UInt160 owner)
        {
            // Check if already initialized
            var initialized = Storage.Get(Storage.CurrentContext, "initialized");
            if (initialized != null && initialized.Length > 0)
            {
                throw new Exception("Already initialized");
            }
            
            if (!owner.IsValid || owner.IsZero)
            {
                throw new Exception("Invalid owner address");
            }
            
            if (!Runtime.CheckWitness(owner))
            {
                throw new Exception("No authorization");
            }
            
            // Set contract owner
            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_OWNER }, owner);
            
            // Mint initial supply to owner
            var key = Helper.Concat(new byte[] { PREFIX_BALANCE }, owner);
            Storage.Put(Storage.CurrentContext, key, TOTAL_SUPPLY);
            
            // Mark as initialized
            Storage.Put(Storage.CurrentContext, "initialized", 1);
            
            OnTransfer(UInt160.Zero, owner, TOTAL_SUPPLY);
            return true;
        }

        /// <summary>
        /// Get token symbol
        /// </summary>
        [DisplayName("symbol")]
        [Safe]
        public static string Symbol() => TOKEN_SYMBOL;

        /// <summary>
        /// Get token decimals
        /// </summary>
        [DisplayName("decimals")]
        [Safe]
        public static byte Decimals() => DECIMALS;

        /// <summary>
        /// Get total supply
        /// </summary>
        [DisplayName("totalSupply")]
        [Safe]
        public static BigInteger TotalSupply() => TOTAL_SUPPLY;

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
        /// Transfer tokens
        /// </summary>
        [DisplayName("transfer")]
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            if (!from.IsValid || !to.IsValid)
            {
                throw new Exception("Invalid address");
            }

            if (amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            if (IsPaused())
            {
                throw new Exception("Contract is paused");
            }

            if (!Runtime.CheckWitness(from))
            {
                return false;
            }

            var fromBalance = BalanceOf(from);
            if (fromBalance < amount)
            {
                return false;
            }

            if (from != to)
            {
                // Update sender balance
                var newFromBalance = fromBalance - amount;
                var fromKey = Helper.Concat(new byte[] { PREFIX_BALANCE }, from);
                if (newFromBalance == 0)
                {
                    Storage.Delete(Storage.CurrentContext, fromKey);
                }
                else
                {
                    Storage.Put(Storage.CurrentContext, fromKey, newFromBalance);
                }

                // Update recipient balance
                var toBalance = BalanceOf(to);
                var newToBalance = toBalance + amount;
                var toKey = Helper.Concat(new byte[] { PREFIX_BALANCE }, to);
                Storage.Put(Storage.CurrentContext, toKey, newToBalance);
            }

            OnTransfer(from, to, amount);

            // Call onNEP17Payment if recipient is a contract
            if (ContractManagement.GetContract(to) != null)
            {
                Neo.SmartContract.Framework.Services.Contract.Call(to, "onNEP17Payment", CallFlags.All, from, amount, data);
            }

            return true;
        }

        /// <summary>
        /// Mint new tokens (only governance can mint)
        /// </summary>
        [DisplayName("mint")]
        public static bool Mint(UInt160 to, BigInteger amount)
        {
            if (!CheckGovernance())
            {
                throw new Exception("Only governance can mint");
            }

            if (!to.IsValid || to.IsZero)
            {
                throw new Exception("Invalid recipient");
            }

            if (amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            var balance = BalanceOf(to);
            var newBalance = balance + amount;
            var key = Helper.Concat(new byte[] { PREFIX_BALANCE }, to);
            Storage.Put(Storage.CurrentContext, key, newBalance);

            OnTransfer(UInt160.Zero, to, amount);
            OnMinted(to, amount);
            return true;
        }

        /// <summary>
        /// Burn tokens
        /// </summary>
        [DisplayName("burn")]
        public static bool Burn(BigInteger amount)
        {
            var account = Runtime.CallingScriptHash;
            if (account == null || !Runtime.CheckWitness(account))
            {
                throw new Exception("Unauthorized");
            }

            if (amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            var balance = BalanceOf(account);
            if (balance < amount)
            {
                throw new Exception("Insufficient balance");
            }

            var newBalance = balance - amount;
            var key = Helper.Concat(new byte[] { PREFIX_BALANCE }, account);
            if (newBalance == 0)
            {
                Storage.Delete(Storage.CurrentContext, key);
            }
            else
            {
                Storage.Put(Storage.CurrentContext, key, newBalance);
            }

            OnTransfer(account, UInt160.Zero, amount);
            OnBurned(account, amount);
            return true;
        }

        /// <summary>
        /// Pause/unpause transfers
        /// </summary>
        [DisplayName("setPaused")]
        public static bool SetPaused(bool paused)
        {
            if (!CheckGovernance())
            {
                throw new Exception("Only governance can pause");
            }

            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_PAUSED }, paused ? 1 : 0);
            return true;
        }

        /// <summary>
        /// Check if contract is paused
        /// </summary>
        [DisplayName("isPaused")]
        [Safe]
        public static bool IsPaused()
        {
            var paused = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_PAUSED });
            return paused?.Length > 0 && (BigInteger)paused == 1;
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
        /// Get governance contract
        /// </summary>
        [DisplayName("getGovernance")]
        [Safe]
        public static UInt160 GetGovernance()
        {
            var governance = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_GOVERNANCE });
            return governance?.Length == 20 ? (UInt160)governance : GetOwner();
        }

        /// <summary>
        /// Set governance contract
        /// </summary>
        [DisplayName("setGovernance")]
        public static bool SetGovernance(UInt160 newGovernance)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new Exception("Only owner can set governance");
            }

            if (!newGovernance.IsValid || newGovernance.IsZero)
            {
                throw new Exception("Invalid governance address");
            }

            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_GOVERNANCE }, newGovernance);
            return true;
        }

        private static bool CheckGovernance()
        {
            var governance = GetGovernance();
            return Runtime.CheckWitness(governance);
        }

    }
}