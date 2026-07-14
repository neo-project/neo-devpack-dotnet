// Copyright (C) 2015-2026 The Neo Project.
//
// NeoAnalyzerSuite.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Neo.SmartContract.Analyzer;

public static class NeoAnalyzerSuite
{
    /// <summary>
    /// Creates the complete set of Neo smart contract analyzers.
    /// </summary>
    public static ImmutableArray<DiagnosticAnalyzer> Create() =>
    [
        new BanCastMethodAnalyzer(),
        new BigIntegerCreationAnalyzer(),
        new BigIntegerUsageAnalyzer(),
        new BigIntegerUsingUsageAnalyzer(),
        new BitOperationsUsageAnalyzer(),
        new CatchOnlySystemExceptionAnalyzer(),
        new CharMethodsUsageAnalyzer(),
        new CollectionTypesUsageAnalyzer(),
        new DecimalUsageAnalyzer(),
        new DoubleUsageAnalyzer(),
        new EnumMethodsUsageAnalyzer(),
        new FloatUsageAnalyzer(),
        new InitialValueAnalyzer(),
        new KeywordUsageAnalyzer(),
        new LinqUsageAnalyzer(),
        new MultipleCatchBlockAnalyzer(),
        new NepStandardImplementationAnalyzer(),
        new NotifyEventNameAnalyzer(),
        new RefKeywordUsageAnalyzer(),
        new SmartContractMethodNamingAnalyzer(),
        new SmartContractMethodNamingAnalyzerUnderline(),
        new StaticFieldInitializationAnalyzer(),
        new StorageKeyCollisionAnalyzer(),
        new StringBuilderUsageAnalyzer(),
        new StringMethodUsageAnalyzer(),
        new SupportedStandardsAnalyzer(),
        new SystemDiagnosticsUsageAnalyzer(),
        new SystemMathUsageAnalyzer(),
        new TaskLikeTypeUsageAnalyzer(),
        new UnsupportedPlatformApiAnalyzer(),
        new UnsupportedSyntaxAnalyzer(),
        new VolatileKeywordUsageAnalyzer()
    ];
}
