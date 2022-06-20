// Copyright (C) 2015-2022 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Attributes;

/// <summary>
/// This Attribute is intended to prevent re-entry attack
/// Once a method uses this Attribute, it can only be called
/// once during the execution of a transaction
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NoReentryAttribute : Attribute
{
}
