// Copyright (C) 2015-2024 The Neo Project.
//
// HelloWorld.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

using System.ComponentModel;

namespace HelloWorld;

[DisplayName("SampleHelloWorld")]
[ContractDescription("A simple `hello world` contract")]
[ContractEmail("core@neo.org")]
[ContractVersion("0.0.1")]
[ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
[ContractPermission(Permission.WildCard, Method.WildCard)]
public class HelloWorldorld : SmartContract
{
    [Safe]
    public static string SayHello()
    {
        return "Hello, World!";
    }
}
