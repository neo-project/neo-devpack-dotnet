// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_UIntTypes.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_UIntTypes : SmartContract.Framework.SmartContract
    {
        [Hash160("NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB")]
        static readonly UInt160 Owner = default!;

        public static bool checkOwner(UInt160 owner) { return owner == Owner; }
        public static bool checkZeroStatic(UInt160 owner) { return owner == UInt160.Zero; }
        public static UInt160 constructUInt160(byte[] bytes) { return (UInt160)bytes; }
        public static bool validateAddress(UInt160 address) => address.IsValid && !address.IsZero;
    }
}
