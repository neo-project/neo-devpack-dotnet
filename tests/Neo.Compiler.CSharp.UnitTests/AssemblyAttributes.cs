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
//    first-time compilation path is guarded by a lock (see TestCleanup.TestInitialize).
//  - The shared static compilation engine and coverage bookkeeping in TestCleanup are only
//    mutated under that lock or during the sequential AssemblyInitialize/AssemblyCleanup hooks.
//
// A small set of classes are excluded via [DoNotParallelize] because they mutate process-wide
// state that cannot be safely shared across concurrent workers:
//  - Classes that swap Console.Out/Console.Error (a static, process-global resource):
//    UnitTest_PrintAbi, UnitTest_ArtifactDiff, UnitTest_AnalyzerExecution, UnitTest_NewCommand,
//    SecurityAnalyzer.SecurityAnalyzerTests, SecurityAnalyzer.WriteInTryAnalyzeTryCatchTests,
//    SecurityAnalyzer.UpdateAnalyzerTests, SecurityAnalyzer.TokenCallbackAuthorizationAnalyzerTests.
//  - Classes that invoke Program.Main against shared/relative output paths or the current
//    working directory: Peripheral.UnitTest_Parameters, Peripheral.UnitTest_OutputNameSecurity.
// Workers is capped (rather than 0/auto) because this project's tests are heavier than
// Neo.SmartContract.Framework.UnitTests: each test constructs a full TestEngine (which
// initializes native contracts) and/or spawns dotnet/MSBuild subprocesses (UnitTest_NewCommand,
// CompilationEngine restore/evaluate). Running with Workers = 0 (one worker per core) oversubscribes
// CPU/memory and produced intermittent, non-deterministic failures across unrelated classes under load.
[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.ClassLevel)]
