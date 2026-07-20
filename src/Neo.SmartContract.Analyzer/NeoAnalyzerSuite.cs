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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Neo.SmartContract.Analyzer;

public static class NeoAnalyzerSuite
{
    /// <summary>
    /// Creates the complete set of Neo smart contract analyzers.
    /// </summary>
    public static ImmutableArray<DiagnosticAnalyzer> Create() =>
        GetLoadableTypes()
            .Where(type => !type.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(type))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .Select(type => (DiagnosticAnalyzer)Activator.CreateInstance(type)!)
            .ToImmutableArray();

    private static IEnumerable<Type> GetLoadableTypes()
    {
        try
        {
            return typeof(NeoAnalyzerSuite).Assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}
