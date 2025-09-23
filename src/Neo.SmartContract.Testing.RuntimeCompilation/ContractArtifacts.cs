// Copyright (C) 2015-2025 The Neo Project.
//
// ContractArtifacts.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;

namespace Neo.SmartContract.Testing.RuntimeCompilation;

/// <summary>
/// Represents the compiled artifacts for a single smart contract, including
/// manifest, NEF payload and the generated testing proxy type.
/// </summary>
public sealed record ContractArtifacts(
    string ContractName,
    NefFile Nef,
    ContractManifest Manifest,
    NeoDebugInfo? DebugInfo,
    Type ProxyType)
{
    /// <summary>
    /// Gets a value indicating whether NEP-19 debugging information is available.
    /// </summary>
    public bool HasDebugInfo => DebugInfo is not null;
}
