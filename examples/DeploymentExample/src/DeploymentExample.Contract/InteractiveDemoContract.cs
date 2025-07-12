// Copyright (C) 2015-2025 The Neo Project.
//
// InteractiveDemoContract.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace DeploymentExample.Contract
{
    /// <summary>
    /// Interactive demonstration contract showcasing Web GUI generation and plugin creation
    /// Features: Counter, Storage, Events, Access Control, Token Interaction
    /// </summary>
    [DisplayName("InteractiveDemoContract")]
    [ManifestExtra("Author", "Neo DevPack Team")]
    [ManifestExtra("Description", "Interactive demo contract for Web GUI and plugin generation")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Version", "1.0.0")]
    [SupportedStandards("NEP-26")]
    [ContractPermission("*", "*")]
    public class InteractiveDemoContract : SmartContract
    {
        #region Constants

        private static readonly UInt160 Owner = "NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX".ToScriptHash();
        private const string CounterKey = "counter";
        private const string PausedKey = "paused";
        private const string OwnerKey = "owner";

        #endregion

        #region Events

        /// <summary>
        /// Fired when the contract is deployed
        /// </summary>
        [DisplayName("ContractDeployed")]
        public static event Action<UInt160, BigInteger> OnContractDeployed;

        /// <summary>
        /// Fired when the counter is incremented
        /// </summary>
        [DisplayName("CounterIncremented")]
        public static event Action<UInt160, BigInteger, BigInteger> OnCounterIncremented;

        /// <summary>
        /// Fired when a value is stored
        /// </summary>
        [DisplayName("ValueStored")]
        public static event Action<UInt160, string, string> OnValueStored;

        /// <summary>
        /// Fired when the contract is paused or unpaused
        /// </summary>
        [DisplayName("PauseToggled")]
        public static event Action<UInt160, bool> OnPauseToggled;

        /// <summary>
        /// Fired when GAS is received
        /// </summary>
        [DisplayName("GasReceived")]
        public static event Action<UInt160, BigInteger> OnGasReceived;

        #endregion

        #region Deployment & Initialization

        /// <summary>
        /// Initialize the contract when deployed
        /// </summary>
        public static void _deploy(object data, bool update)
        {
            if (update) return;

            // Initialize storage
            Storage.Put(Storage.CurrentContext, OwnerKey, Owner);
            Storage.Put(Storage.CurrentContext, CounterKey, 0);
            Storage.Put(Storage.CurrentContext, PausedKey, false);

            // Fire deployment event
            OnContractDeployed(Runtime.ExecutingScriptHash, Runtime.Time);
        }

        /// <summary>
        /// Update the contract (only owner can update)
        /// </summary>
        [DisplayName("update")]
        public static bool Update(ByteString nefFile, string manifest, object data)
        {
            if (!IsOwner()) throw new Exception("Only owner can update contract");
            
            ContractManagement.Update(nefFile, manifest, data);
            return true;
        }

        /// <summary>
        /// Destroy the contract (only owner can destroy)
        /// </summary>
        [DisplayName("destroy")]
        public static void Destroy()
        {
            if (!IsOwner()) throw new Exception("Only owner can destroy contract");
            
            ContractManagement.Destroy();
        }

        #endregion

        #region Counter Functions

        /// <summary>
        /// Get the current counter value
        /// </summary>
        [DisplayName("getCounter")]
        [Safe]
        public static BigInteger GetCounter()
        {
            return (BigInteger)Storage.Get(Storage.CurrentContext, CounterKey);
        }

        /// <summary>
        /// Increment the counter by 1
        /// </summary>
        [DisplayName("increment")]
        public static BigInteger Increment()
        {
            if (IsPaused()) throw new Exception("Contract is paused");

            var currentValue = GetCounter();
            var newValue = currentValue + 1;
            
            Storage.Put(Storage.CurrentContext, CounterKey, newValue);
            
            OnCounterIncremented(Runtime.CallingScriptHash, currentValue, newValue);
            return newValue;
        }

        /// <summary>
        /// Increment the counter by a specific amount
        /// </summary>
        [DisplayName("incrementBy")]
        public static BigInteger IncrementBy(BigInteger amount)
        {
            if (IsPaused()) throw new Exception("Contract is paused");
            if (amount <= 0) throw new Exception("Amount must be positive");

            var currentValue = GetCounter();
            var newValue = currentValue + amount;
            
            Storage.Put(Storage.CurrentContext, CounterKey, newValue);
            
            OnCounterIncremented(Runtime.CallingScriptHash, currentValue, newValue);
            return newValue;
        }

        /// <summary>
        /// Reset the counter to zero (only owner)
        /// </summary>
        [DisplayName("resetCounter")]
        public static bool ResetCounter()
        {
            if (!IsOwner()) throw new Exception("Only owner can reset counter");

            var currentValue = GetCounter();
            Storage.Put(Storage.CurrentContext, CounterKey, 0);
            
            OnCounterIncremented(Runtime.CallingScriptHash, currentValue, 0);
            return true;
        }

        #endregion

        #region Storage Functions

        /// <summary>
        /// Store a key-value pair
        /// </summary>
        [DisplayName("storeValue")]
        public static bool StoreValue(string key, string value)
        {
            if (IsPaused()) throw new Exception("Contract is paused");
            if (string.IsNullOrEmpty(key)) throw new Exception("Key cannot be empty");

            Storage.Put(Storage.CurrentContext, key, value);
            OnValueStored(Runtime.CallingScriptHash, key, value);
            return true;
        }

        /// <summary>
        /// Get a stored value by key
        /// </summary>
        [DisplayName("getValue")]
        [Safe]
        public static string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            
            var value = Storage.Get(Storage.CurrentContext, key);
            return value?.ToString() ?? "";
        }

        /// <summary>
        /// Delete a stored value (only owner)
        /// </summary>
        [DisplayName("deleteValue")]
        public static bool DeleteValue(string key)
        {
            if (!IsOwner()) throw new Exception("Only owner can delete values");
            if (string.IsNullOrEmpty(key)) throw new Exception("Key cannot be empty");

            Storage.Delete(Storage.CurrentContext, key);
            OnValueStored(Runtime.CallingScriptHash, key, "");
            return true;
        }

        #endregion

        #region Admin Functions

        /// <summary>
        /// Get the contract owner
        /// </summary>
        [DisplayName("getOwner")]
        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
        }

        /// <summary>
        /// Transfer ownership (only current owner)
        /// </summary>
        [DisplayName("transferOwnership")]
        public static bool TransferOwnership(UInt160 newOwner)
        {
            if (!IsOwner()) throw new Exception("Only owner can transfer ownership");
            if (!newOwner.IsValid) throw new Exception("Invalid new owner address");

            Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
            return true;
        }

        /// <summary>
        /// Check if the contract is paused
        /// </summary>
        [DisplayName("isPaused")]
        [Safe]
        public static bool IsPaused()
        {
            return (bool)Storage.Get(Storage.CurrentContext, PausedKey);
        }

        /// <summary>
        /// Pause or unpause the contract (only owner)
        /// </summary>
        [DisplayName("setPaused")]
        public static bool SetPaused(bool paused)
        {
            if (!IsOwner()) throw new Exception("Only owner can pause contract");

            Storage.Put(Storage.CurrentContext, PausedKey, paused);
            OnPauseToggled(Runtime.CallingScriptHash, paused);
            return true;
        }

        #endregion

        #region Token Interaction

        /// <summary>
        /// Get the contract's GAS balance
        /// </summary>
        [DisplayName("getGasBalance")]
        [Safe]
        public static BigInteger GetGasBalance()
        {
            return GAS.BalanceOf(Runtime.ExecutingScriptHash);
        }

        /// <summary>
        /// Get the contract's NEO balance
        /// </summary>
        [DisplayName("getNeoBalance")]
        [Safe]
        public static BigInteger GetNeoBalance()
        {
            return NEO.BalanceOf(Runtime.ExecutingScriptHash);
        }

        /// <summary>
        /// Withdraw GAS from contract (only owner)
        /// </summary>
        [DisplayName("withdrawGas")]
        public static bool WithdrawGas(UInt160 to, BigInteger amount)
        {
            if (!IsOwner()) throw new Exception("Only owner can withdraw GAS");
            if (!to.IsValid) throw new Exception("Invalid recipient address");
            if (amount <= 0) throw new Exception("Amount must be positive");

            var balance = GetGasBalance();
            if (amount > balance) throw new Exception("Insufficient GAS balance");

            return GAS.Transfer(Runtime.ExecutingScriptHash, to, amount);
        }

        /// <summary>
        /// Withdraw NEO from contract (only owner)
        /// </summary>
        [DisplayName("withdrawNeo")]
        public static bool WithdrawNeo(UInt160 to, BigInteger amount)
        {
            if (!IsOwner()) throw new Exception("Only owner can withdraw NEO");
            if (!to.IsValid) throw new Exception("Invalid recipient address");
            if (amount <= 0) throw new Exception("Amount must be positive");

            var balance = GetNeoBalance();
            if (amount > balance) throw new Exception("Insufficient NEO balance");

            return NEO.Transfer(Runtime.ExecutingScriptHash, to, amount, null);
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Get contract information
        /// </summary>
        [DisplayName("getContractInfo")]
        [Safe]
        public static object[] GetContractInfo()
        {
            return new object[]
            {
                Runtime.ExecutingScriptHash,
                GetOwner(),
                GetCounter(),
                IsPaused(),
                GetGasBalance(),
                GetNeoBalance(),
                Runtime.Time
            };
        }

        /// <summary>
        /// Calculate a simple hash of input data
        /// </summary>
        [DisplayName("calculateHash")]
        [Safe]
        public static ByteString CalculateHash(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return CryptoLib.Sha256(input);
        }

        /// <summary>
        /// Get block information
        /// </summary>
        [DisplayName("getBlockInfo")]
        [Safe]
        public static object[] GetBlockInfo()
        {
            return new object[]
            {
                Ledger.CurrentIndex,
                Ledger.CurrentHash,
                Runtime.Time,
                Runtime.GasLeft
            };
        }

        #endregion

        #region Receive Functions

        /// <summary>
        /// Handle GAS payments to the contract
        /// </summary>
        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            // Only accept GAS payments
            if (Runtime.CallingScriptHash == GAS.Hash)
            {
                OnGasReceived(from, amount);
            }
            else
            {
                // Reject other token payments
                throw new Exception("Only GAS payments accepted");
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Check if the calling account is the owner
        /// </summary>
        private static bool IsOwner()
        {
            return Runtime.CheckWitness(GetOwner());
        }

        #endregion
    }
}