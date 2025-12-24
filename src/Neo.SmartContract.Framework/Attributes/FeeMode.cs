// Copyright (C) 2015-2025 The Neo Project.
//
// FeeMode.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Specifies the mode for calculating custom contract fees.
    /// </summary>
    public enum FeeMode
    {
        /// <summary>
        /// A fixed fee amount specified in datoshi (1e-8 GAS).
        /// The fee is constant regardless of method parameters.
        /// </summary>
        Fixed,

        /// <summary>
        /// A dynamic fee computed at runtime by a fee calculator contract.
        /// The calculator contract must implement <see cref="Interfaces.IFeeCalculator"/>.
        /// </summary>
        Dynamic
    }
}
