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
    public enum OracleResponseCode : byte
    {
        Success = 0x00,

        ProtocolNotSupported = 0x10,
        ConsensusUnreachable = 0x12,
        NotFound = 0x14,
        Timeout = 0x16,
        Forbidden = 0x18,
        ResponseTooLarge = 0x1a,
        InsufficientFunds = 0x1c,

        Error = 0xff
    }
}
