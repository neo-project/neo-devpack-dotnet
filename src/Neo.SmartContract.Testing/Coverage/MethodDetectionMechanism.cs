// Copyright (C) 2015-2025 The Neo Project.
//
// MethodDetectionMechanism.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Testing.Coverage
{
    public enum MethodDetectionMechanism
    {
        /// <summary>
        /// Find RET
        /// </summary>
        FindRET,

        /// <summary>
        /// Next method defined in Abi
        /// If there are any private method, it probably will return undesired results
        /// </summary>
        NextMethodInAbi,

        /// <summary>
        /// It will compute the private methods
        /// </summary>
        NextMethod
    }
}
