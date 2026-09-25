// Copyright (C) 2015-2026 The Neo Project.
//
// AssemblyAttributes.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;

// Test classes are parallelized (methods within a class still run sequentially on the
// same worker), which is safe because:
//  - Per-contract compilation results are cached in TestCleanup.CachedContracts and the
//    first-time compilation path is now guarded by a lock (see TestCleanup.TestInitialize).
//  - TestBase<T>.Coverage is a static field scoped per closed generic type T, so distinct
//    test classes (distinct T) never share the same Coverage instance.
//  - Methods of the same test class execute on the same worker thread, so per-class static
//    state (e.g. Coverage.Join in OnCleanup) is never accessed concurrently.
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]
