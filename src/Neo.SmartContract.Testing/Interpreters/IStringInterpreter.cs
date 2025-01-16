// Copyright (C) 2015-2024 The Neo Project.
//
// IStringInterpreter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Testing.Interpreters
{
    public interface IStringInterpreter
    {
        /// <summary>
        /// Get string from bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Value</returns>
        public string GetString(ReadOnlySpan<byte> bytes);
    }
}
