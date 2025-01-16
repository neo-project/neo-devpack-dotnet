// Copyright (C) 2015-2024 The Neo Project.
//
// InvalidUInt160.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidUInt160
    {
        /// <summary>
        /// Zero
        /// </summary>
        public static readonly UInt160 Zero = UInt160.Zero;

        /// <summary>
        /// Null UInt160
        /// </summary>
        public static readonly UInt160? Null = null;

        /// <summary>
        /// This will be an invalid UInt160 (ByteString)
        /// </summary>
        public static readonly UInt160 InvalidLength = new();

        /// <summary>
        /// This will be an invalid UInt160 (Integer)
        /// </summary>
        public static readonly UInt160 InvalidType = new();
    }
}
