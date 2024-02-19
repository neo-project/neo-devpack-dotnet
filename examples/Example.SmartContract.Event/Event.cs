// Copyright (C) 2015-2024 The Neo Project.
//
// Event.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Event;

[DisplayName("SampleEvent")]
[ContractAuthor("code-dev", "core@neo.org")]
[ContractDescription("A sample contract that demonstrates how to use Events")]
[ContractVersion("0.0.1")]
[ContractSourceCode("https://github.com/neo-project/samples")]
[ContractPermission(Permission.WildCard, Method.WildCard)]
public class SampleEvent : SmartContract
{
    [DisplayName("new_event_name")]
    public static event Action<byte[], string, BigInteger> event_name;

    public static event Action<byte[], BigInteger> event2;

    public static bool Main()
    {
        byte[] ba = new byte[] { 0x01, 0x02, 0x03 };
        event_name(ba, "oi", 10); // will Example.SmartContract.Runtime.Notify: 'new_event_name', '\x01\x02\x03', 'oi', 10

        event2(ba, 50); // will Example.SmartContract.Runtime.Notify: 'event2', '\x01\x02\x03', '\x32'

        return false;
    }
}
