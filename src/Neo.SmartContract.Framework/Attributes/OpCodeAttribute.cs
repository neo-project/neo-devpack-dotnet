// Copyright (C) 2015-2024 The Neo Project.
//
// OpCodeAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public class OpCodeAttribute : Attribute
    {
        public OpCodeAttribute(OpCode opCode, string opData = "")
        {
        }
    }
}
