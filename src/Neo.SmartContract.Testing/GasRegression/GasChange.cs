// Copyright (C) 2015-2026 The Neo Project.
//
// GasChange.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using System;

namespace Neo.SmartContract.Testing.GasRegression;

/// <summary>
/// One compared measurement.
/// </summary>
public sealed record GasChange(string Name, long? Baseline, long? Current, GasChangeStatus Status)
{
    /// <summary>
    /// Current minus baseline, or null when either side is absent.
    /// </summary>
    public Int128? Difference => Baseline is null || Current is null ? null : (Int128)Current.Value - Baseline.Value;

    /// <summary>
    /// True when this change must fail CI.
    /// </summary>
    public bool IsFailure => Status is GasChangeStatus.Regression or GasChangeStatus.Missing or GasChangeStatus.Unexpected;
}
