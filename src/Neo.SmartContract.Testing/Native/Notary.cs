// Copyright (C) 2015-2025 The Neo Project.
//
// StdLib.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Native;

namespace Neo.SmartContract.Testing.Native;

public abstract class Notary : SmartContract
{
    #region Compiled data

    public static Manifest.ContractManifest Manifest { get; } =
        NativeContract.Notary.GetContractState(ProtocolSettings.Default, uint.MaxValue).Manifest;

    #endregion

    #region Constructor for internal use only

    protected Notary(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
