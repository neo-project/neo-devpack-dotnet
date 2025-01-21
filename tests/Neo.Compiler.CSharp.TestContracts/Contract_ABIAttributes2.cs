// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_ABIAttributes2.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts;

[ContractPermission(Permission.Any, "c")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a", "b")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", Permission.Any)]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "*")]
[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a")]
[ContractPermission("*", "a")]
[ContractPermission(Permission.Any, Method.Any)]
[ContractPermission("*", "b")]
[ContractTrust("0x0a0b00ff00ff00ff00ff00ff00ff00ff00ff00a4")]
public class Contract_ABIAttributes2 : SmartContract.Framework.SmartContract
{
    public static int test() => 0;
}
