// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;
using System;

#pragma warning disable IDE0060

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class CallFlagsAttribute : Attribute
    {
        public CallFlagsAttribute(CallFlags flags) { }
    }
}
