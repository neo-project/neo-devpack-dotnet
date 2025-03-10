// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_ConcatByteStringAddAssign.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_ConcatByteStringAddAssign : SmartContract.Framework.SmartContract
    {
        public static ByteString ByteStringAddAssign(ByteString a, ByteString b, string c)
        {
            ByteString result = ByteString.Empty;
            result += a;
            result += b;
            result += c;
            return result;
        }
    }
}
