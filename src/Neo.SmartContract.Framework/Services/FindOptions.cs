// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Services
{
    [Flags]
    public enum FindOptions : byte
    {
        None = 0,

        KeysOnly = 1 << 0,
        RemovePrefix = 1 << 1,
        ValuesOnly = 1 << 2,
        DeserializeValues = 1 << 3,
        PickField0 = 1 << 4,
        PickField1 = 1 << 5
    }
}
