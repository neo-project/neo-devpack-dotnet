// Copyright (C) 2015-2026 The Neo Project.
//
// IVerificable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.ComponentModel;

namespace Neo.SmartContract.Testing.TestingStandards;

public interface IVerificable
{
    /// <summary>
    /// Safe property
    /// </summary>
    public bool? Verify { [DisplayName("verify")] get; }
}
