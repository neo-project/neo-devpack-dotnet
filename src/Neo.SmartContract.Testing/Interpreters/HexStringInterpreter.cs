// Copyright (C) 2015-2024 The Neo Project.
//
// HexStringInterpreter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Testing.Interpreters
{
    public class HexStringInterpreter : IStringInterpreter
    {
        public static readonly Regex HexRegex = new(@"^[a-zA-Z0-9_]+$");

        /// <summary>
        /// Get string from bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Value</returns>
        public virtual string GetString(ReadOnlySpan<byte> bytes)
        {
            return bytes.ToHexString();
        }
    }
}
