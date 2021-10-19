// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography.ECC;
using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    public class NeoAccountState
    {
        public readonly BigInteger Balance;
        public readonly BigInteger Height;
        public readonly ECPoint VoteTo;
    }
}
