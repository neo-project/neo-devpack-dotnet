// Copyright (C) 2015-2026 The Neo Project.
//
// GasChangeStatus.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

namespace Neo.SmartContract.Testing.GasRegression;

/// <summary>
/// Classification of a single gas measurement compared with its baseline.
/// </summary>
public enum GasChangeStatus
{
    Unchanged,
    WithinTolerance,
    Improvement,
    Regression,
    Missing,
    Unexpected
}
