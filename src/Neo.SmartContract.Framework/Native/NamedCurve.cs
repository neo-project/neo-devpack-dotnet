// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Native
{
    /// <summary>
    /// RFC 4492
    /// </summary>
    /// <remarks>
    /// https://tools.ietf.org/html/rfc4492#section-5.1.1
    /// </remarks>
    public enum NamedCurve : byte
    {
        secp256k1 = 22,
        secp256r1 = 23
    }
}
