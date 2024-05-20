// Copyright (C) 2015-2024 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Native
{
    /// <summary>
    /// Represents the named curve (RFC 4492) used in ECDSA. This enum is obsolete
    /// and will be removed in future versions. Please, use an extended compatible
    /// <see cref="NamedCurveHash"/> instead.
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc4492#section-5.1.1
    /// </remarks>
    [Obsolete("NamedCurve enum is obsolete and will be removed in future versions. Please, use an extended compatible NamedCurveHash instead.")]
    public enum NamedCurve : byte
    {
        [Obsolete("secp256k1 value is obsolete and will be removed in future versions. Please, use NamedCurveHash.secp256k1SHA256 for compatible behaviour.")]
        secp256k1 = 22,
        [Obsolete("secp256r1 value is obsolete and will be removed in future versions. Please, use NamedCurveHash.secp256r1SHA256 for compatible behaviour.")]
        secp256r1 = 23
    }
}
