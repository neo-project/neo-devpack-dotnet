// Copyright (C) 2015-2026 The Neo Project.
//
// DiagnosticDescriptorFactory.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;

namespace Neo.SmartContract.Analyzer;

internal static class DiagnosticDescriptorFactory
{
    private const string CatalogBase =
        "https://github.com/neo-project/neo-devpack-dotnet/blob/master-n3/docs/diagnostics/catalog.md#";

    internal static DiagnosticDescriptor Create(
        string id,
        LocalizableString title,
        LocalizableString messageFormat,
        string category,
        DiagnosticSeverity defaultSeverity,
        bool isEnabledByDefault,
        LocalizableString? description = null,
        string? helpLinkUri = null,
        params string[] customTags)
    {
        helpLinkUri ??= CatalogBase + id.ToLowerInvariant();
        return new DiagnosticDescriptor(
            id,
            title,
            messageFormat,
            category,
            defaultSeverity,
            isEnabledByDefault,
            description,
            helpLinkUri,
            customTags);
    }
}
