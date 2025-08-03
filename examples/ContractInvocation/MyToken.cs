// Example NEP-17 Token Contract demonstrating basic contract structure
// This shows what a typical contract looks like that would be invoked by others

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Examples.ContractInvocation
{
    [DisplayName("MyToken")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "Example NEP-17 Token")]
    [ContractPermission("*", "onNEP17Payment")]
    public class MyToken : Nep17Token
    {
        #region Token Settings

        [InitialValue("NbnjKGMBJzJ6j5PHeYhjJDaQ5Vy5UYu4Fv", ContractParameterType.Hash160)]
        static readonly UInt160 Owner = UInt160.Zero;

        static readonly ulong InitialSupply = 10_000_000_00000000; // 10M tokens with 8 decimals

        #endregion

        #region NEP-17 Standard

        public override string Symbol => "NMY";
        public override byte Decimals => 8;

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Mint(Owner, InitialSupply);
            }
        }

        #endregion

        #region Custom Methods

        [DisplayName("mint")]
        public static new bool Mint(UInt160 to, BigInteger amount)
        {
            if (!Runtime.CheckWitness(Owner))
                throw new InvalidOperationException("No authorization");

            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");

            Mint(to, amount);
            return true;
        }

        [DisplayName("burn")]
        public static new bool Burn(UInt160 from, BigInteger amount)
        {
            if (!Runtime.CheckWitness(from) && !Runtime.CheckWitness(Owner))
                throw new InvalidOperationException("No authorization");

            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");

            if (BalanceOf(from) < amount)
                throw new InvalidOperationException("Insufficient balance");

            Burn(from, amount);
            return true;
        }

        [DisplayName("transferFrom")]
        public static bool TransferFrom(UInt160 from, UInt160 to, BigInteger amount, object? data = null)
        {
            if (!Runtime.CheckWitness(from))
                throw new InvalidOperationException("No authorization");

            return Transfer(from, to, amount, data!);
        }

        [DisplayName("approve")]
        public static bool Approve(UInt160 owner, UInt160 spender, BigInteger amount)
        {
            if (!Runtime.CheckWitness(owner))
                throw new InvalidOperationException("No authorization");

            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            StorageMap allowanceMap = new(Storage.CurrentContext, "allowance");
            allowanceMap.Put(owner + spender, amount);

            OnApproval?.Invoke(owner, spender, amount);
            return true;
        }

        [DisplayName("allowance")]
        public static BigInteger Allowance(UInt160 owner, UInt160 spender)
        {
            StorageMap allowanceMap = new(Storage.CurrentReadOnlyContext, "allowance");
            var result = allowanceMap.Get(owner + spender);
            return result != null ? (BigInteger)result : 0;
        }

        #endregion

        #region Events

        [DisplayName("Approval")]
        public static event System.Action<UInt160, UInt160, BigInteger>? OnApproval;

        #endregion
    }
}