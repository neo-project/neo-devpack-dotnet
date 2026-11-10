// Copyright (C) 2015-2026 The Neo Project.
//
// AssemblyInfo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Runtime.CompilerServices;

// The differential fuzz harness needs the engine's per-contract coverage dictionary to
// implement coverage-guided input selection. The covered-contract type and its Hit
// method are public; only the dictionary that indexes them is internal.
//
// The name must match the fuzz project's AssemblyName, which is "neofuzz".
[assembly: InternalsVisibleTo("neofuzz")]
