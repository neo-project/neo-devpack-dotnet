// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_ContractInvocation.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.ContractInvocation;
using Neo.SmartContract.Framework.ContractInvocation.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    /// <summary>
    /// Test contract demonstrating the contract invocation system.
    /// </summary>
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_ContractInvocation : SmartContract
    {
        // Reference to a development contract (another contract under development)
        [ContractReference("../SampleNep17Token",
            ReferenceType = ContractReferenceType.Development,
            ProjectPath = "../Example.SmartContract.NEP17/Example.SmartContract.NEP17.csproj")]
        private static readonly DevelopmentContractReference TokenContract;

        // Reference to a deployed NEP-17 contract with multi-network addresses
        [ContractReference("GAS",
            ReferenceType = ContractReferenceType.Deployed,
            PrivnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
            TestnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf",
            MainnetAddress = "0xd2a4cff31913016155e38e474a2c06d08be276cf")]
        private static readonly DeployedContractReference GasContract;

        // Auto-detected contract reference (will be treated as deployed since it has addresses)
        [ContractReference("NEO",
            PrivnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
            TestnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5",
            MainnetAddress = "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5")]
        private static readonly IContractReference NeoContract;

        /// <summary>
        /// Demonstrates calling a method on a development contract.
        /// </summary>
        /// <param name="account">The account to check balance for</param>
        /// <returns>The token balance</returns>
        public static BigInteger GetTokenBalance(UInt160 account)
        {
            // This will be compiled to Contract.Call(TokenContract.ResolvedHash, "balanceOf", CallFlags.ReadOnly, account)
            var proxy = new Nep17ContractProxy(TokenContract);
            return proxy.BalanceOf(account);
        }

        /// <summary>
        /// Demonstrates calling a method on a deployed contract (GAS).
        /// </summary>
        /// <param name="account">The account to check balance for</param>
        /// <returns>The GAS balance</returns>
        public static BigInteger GetGasBalance(UInt160 account)
        {
            // This will be compiled to Contract.Call(GasContract.ResolvedHash, "balanceOf", CallFlags.ReadOnly, account)
            var proxy = new Nep17ContractProxy(GasContract);
            return proxy.BalanceOf(account);
        }

        /// <summary>
        /// Demonstrates calling multiple contracts in sequence.
        /// </summary>
        /// <param name="account">The account to check</param>
        /// <returns>Array containing [NEO balance, GAS balance, Token balance]</returns>
        public static BigInteger[] GetAllBalances(UInt160 account)
        {
            var neoProxy = new Nep17ContractProxy(NeoContract);
            var gasProxy = new Nep17ContractProxy(GasContract);
            var tokenProxy = new Nep17ContractProxy(TokenContract);

            return new BigInteger[]
            {
                neoProxy.BalanceOf(account),
                gasProxy.BalanceOf(account),
                tokenProxy.BalanceOf(account)
            };
        }

        /// <summary>
        /// Demonstrates transferring tokens using contract proxies.
        /// </summary>
        /// <param name="from">The sender</param>
        /// <param name="to">The recipient</param>
        /// <param name="amount">The amount to transfer</param>
        /// <returns>True if successful</returns>
        public static bool TransferTokens(UInt160 from, UInt160 to, BigInteger amount)
        {
            var tokenProxy = new Nep17ContractProxy(TokenContract);
            return tokenProxy.Transfer(from, to, amount, null);
        }

        /// <summary>
        /// Gets the hash of a referenced contract.
        /// </summary>
        /// <param name="contractName">The contract name ("NEO", "GAS", or "TOKEN")</param>
        /// <returns>The contract hash</returns>
        public static UInt160 GetContractHash(string contractName)
        {
            return contractName switch
            {
                "NEO" => ((ContractProxyBase)new Nep17ContractProxy(NeoContract)).GetContractHash(),
                "GAS" => ((ContractProxyBase)new Nep17ContractProxy(GasContract)).GetContractHash(),
                "TOKEN" => ((ContractProxyBase)new Nep17ContractProxy(TokenContract)).GetContractHash(),
                _ => UInt160.Zero
            };
        }
    }

    /// <summary>
    /// Sample NEP-17 contract proxy for demonstration.
    /// In practice, this would be auto-generated from the contract manifest.
    /// </summary>
    public class Nep17ContractProxy : ContractProxyBase
    {
        public Nep17ContractProxy(IContractReference contractReference) : base(contractReference)
        {
        }

        [ContractMethod(ReadOnly = true)]
        public BigInteger BalanceOf(UInt160 account)
        {
            return (BigInteger)InvokeReadOnly("balanceOf", account);
        }

        [ContractMethod(ReadOnly = true)]
        public BigInteger TotalSupply()
        {
            return (BigInteger)InvokeReadOnly("totalSupply");
        }

        [ContractMethod(ReadOnly = true)]
        public string Symbol()
        {
            return (string)InvokeReadOnly("symbol");
        }

        [ContractMethod(ReadOnly = true)]
        public byte Decimals()
        {
            return (byte)InvokeReadOnly("decimals");
        }

        public bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object? data)
        {
            return (bool)InvokeWithStates("transfer", from, to, amount, data);
        }
    }
}
