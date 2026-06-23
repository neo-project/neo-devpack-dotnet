// Copyright (C) 2015-2026 The Neo Project.
//
// DebugLauncher.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.Text;

namespace Neo.SmartContract.Debugging
{
    /// <summary>
    /// Helpers for launching a compiled contract under a <see cref="DebugSession"/>: deploying it
    /// into a <see cref="TestEngine"/> and building the script that invokes one of its methods.
    /// </summary>
    public static class DebugLauncher
    {
        /// <summary>
        /// Deploys a contract from its NEF and manifest into <paramref name="engine"/> and returns
        /// its contract hash. Deployment runs without a debug session attached, so the contract's
        /// <c>_deploy</c> method is not debugged.
        /// </summary>
        public static UInt160 Deploy(TestEngine engine, NefFile nef, ContractManifest manifest, object? data = null)
        {
            if (engine is null) throw new ArgumentNullException(nameof(engine));
            if (nef is null) throw new ArgumentNullException(nameof(nef));
            if (manifest is null) throw new ArgumentNullException(nameof(manifest));

            var state = engine.Native.ContractManagement.Deploy(
                nef.ToArray(),
                Encoding.UTF8.GetBytes(manifest.ToJson().ToString(false)),
                data) ?? throw new InvalidOperationException("Contract deployment failed.");

            return state.Hash;
        }

        /// <summary>
        /// Builds an invocation script that calls <paramref name="method"/> on the contract at
        /// <paramref name="contract"/> with <paramref name="args"/>. Pass it to
        /// <see cref="DebugSession.RunAsync"/> to debug the call.
        /// </summary>
        public static Script BuildInvocation(UInt160 contract, string method, params object[] args)
        {
            if (contract is null) throw new ArgumentNullException(nameof(contract));
            if (method is null) throw new ArgumentNullException(nameof(method));

            using ScriptBuilder builder = new();
            builder.EmitDynamicCall(contract, method, args);
            return builder.ToArray();
        }
    }
}
