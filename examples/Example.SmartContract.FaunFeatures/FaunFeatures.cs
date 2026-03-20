// Copyright (C) 2015-2026 The Neo Project.
//
// FaunFeatures.cs file belongs to the neo project and is free
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
using System.ComponentModel;
using System.Numerics;

namespace FaunFeatures;

[DisplayName("SampleFaunFeatures")]
[ContractDescription("Demonstrates Neo v3.9 HF_Faun native additions")]
[ContractVersion("0.0.1")]
[ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
[ContractPermission(Permission.Any, Method.Any)]
public class SampleFaunFeatures : SmartContract
{
    [Safe]
    public static string HexEncode(byte[] data)
    {
        return StdLib.HexEncode((ByteString)data);
    }

    [Safe]
    public static byte[] HexDecode(string hex)
    {
        return (byte[])StdLib.HexDecode(hex);
    }

    [Safe]
    public static uint ExecFeeFactor()
    {
        return Policy.GetExecFeeFactor();
    }

    [Safe]
    public static bool IsCommitteeSigned()
    {
        return Treasury.Verify();
    }
}
