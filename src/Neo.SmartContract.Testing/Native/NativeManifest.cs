// Copyright (C) 2015-2026 The Neo Project.
//
// NativeManifest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Persistence;
using Neo.Persistence.Providers;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;

namespace Neo.SmartContract.Testing.Native;

/// <summary>
/// Resolves the manifest of the native contracts mocked by the testing engine.
/// </summary>
internal static class NativeManifest
{
    /// <summary>
    /// Get the manifest of the native contract.
    /// </summary>
    /// <param name="contract">Native contract</param>
    /// <returns>
    /// The manifest with the methods active at <see cref="uint.MaxValue"/> height. The mocks are
    /// initialized without a Policy storage, so the activation heights are taken from
    /// <see cref="ProtocolSettings.Default"/> and Policy-managed hardforks are considered disabled.
    /// </returns>
    internal static ContractManifest Get(NativeContract contract)
    {
        using var snapshot = new StoreCache(new MemoryStore(), readOnly: true);
        return contract.GetContractState(IsConfigHardforkEnabled, ProtocolSettings.Default, snapshot, uint.MaxValue).Manifest;
    }

    private static bool IsConfigHardforkEnabled(ProtocolSettings settings, IReadOnlyStore snapshot, Hardfork hardfork, uint blockHeight) =>
        settings.IsHardforkEnabled(hardfork, blockHeight);
}
