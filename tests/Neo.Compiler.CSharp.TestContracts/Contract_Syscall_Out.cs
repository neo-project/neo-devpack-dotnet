// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Syscall_Out.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_Syscall_Out : SmartContract.Framework.SmartContract
{
    public static (bool, byte) TestByteTryParse(string s)
    {
        bool success = byte.TryParse(s, out byte result);
        return (success, result);
    }

    public static (bool, sbyte) TestSByteTryParse(string s)
    {
        bool success = sbyte.TryParse(s, out sbyte result);
        return (success, result);
    }

    public static (bool, short) TestShortTryParse(string s)
    {
        bool success = short.TryParse(s, out short result);
        return (success, result);
    }

    public static (bool, ushort) TestUShortTryParse(string s)
    {
        bool success = ushort.TryParse(s, out ushort result);
        return (success, result);
    }

    public static (bool, int) TestIntTryParse(string s)
    {
        bool success = int.TryParse(s, out int result);
        return (success, result);
    }

    public static (bool, uint) TestUIntTryParse(string s)
    {
        bool success = uint.TryParse(s, out uint result);
        return (success, result);
    }

    public static (bool, long) TestLongTryParse(string s)
    {
        bool success = long.TryParse(s, out long result);
        return (success, result);
    }

    public static (bool, ulong) TestULongTryParse(string s)
    {
        var success = ulong.TryParse(s, out ulong result);
        return (success, result);
    }

    public static (bool, bool) TestBoolTryParse(string s)
    {
        var success = bool.TryParse(s, out bool result);
        return (success, result);
    }
}
