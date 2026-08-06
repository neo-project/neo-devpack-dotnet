// Copyright (C) 2015-2026 The Neo Project.
//
// SyntaxProbeExpectedDiagnostics.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

internal static class SyntaxProbeExpectedDiagnostics
{
    private static readonly IReadOnlyDictionary<string, string[]> ExpectedByProbe =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["unsafe_code"] = ["NC4033"],
            ["numerics_bit_operations"] = ["NC4060"],
            ["datetime_methods"] = ["NC4058"],
            ["timespan_methods"] = ["NC4058"],
            ["convert_methods"] = ["NC4058"],
            ["anonymous_method"] = ["NC4034"],
            ["iterator_block"] = ["NC4035"],
            ["query_expression"] = ["NC4036"],
            ["dynamic_binding"] = ["NC4037"],
            ["async_method"] = ["NC4038"],
            ["await_expression"] = ["NC4039"],
            ["exception_filter"] = ["NC4040"],
            ["local_function"] = ["NC4042"],
            ["ref_return"] = ["NC4042"],
            ["ref_argument_array_element"] = ["NC4010"],
            ["ref_argument_span_element"] = ["NC4010"],
            ["range_on_general_arrays"] = ["NC2010"],
            ["using_declaration"] = ["NC4059"],
            ["async_streams"] = ["NC4045"],
            ["native_int"] = ["NC4046"],
            ["top_level_statements"] = ["NC4047"],
            ["function_pointer"] = ["NC4048"],
            ["global_using"] = ["NC4049"],
            ["extended_property_pattern"] = ["NC4061"],
            ["list_patterns"] = ["NC4050"],
            ["utf_8_string_literals"] = ["NC4051"],
            ["file_local_types"] = ["NC4053"],
            ["numeric_intptr_and_uintptr"] = ["NC4046"],
            ["collection_expression_spread_elements"] = ["NC4062"],
            ["ref_readonly_parameters"] = ["NC4054"],
            ["interceptors"] = ["CS0246"],
            ["new_lock_object"] = ["NC4015"],
            ["ref_and_unsafe_in_iterators_and_async_methods"] = ["NC4038"],
            ["extension_types"] = ["CS9283"],
            ["shape_constraints"] = ["CS0246"],
            ["discriminated_union_types"] = ["CS0246"],
            ["lambda_default_parameters"] = ["CS1746"],
            ["span_arraysegment_conversions"] = ["NC4013"],
            ["lambda_parameter_modifiers"] = ["CS8171"],
            ["partial_events_constructors"] = ["CS0079"],
            ["user_defined_compound_assignment"] = ["CS0019"]
        };

    internal static IEnumerable<string> ProbeIds => ExpectedByProbe.Keys;

    internal static IReadOnlyList<string> Get(string probeId) =>
        ExpectedByProbe.TryGetValue(probeId, out var diagnostics)
            ? diagnostics
            : throw new InvalidOperationException($"Unsupported syntax probe '{probeId}' has no expected diagnostic ID.");
}
