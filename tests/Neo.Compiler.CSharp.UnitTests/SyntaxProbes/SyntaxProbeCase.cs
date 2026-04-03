// Copyright (C) 2015-2026 The Neo Project.
//
// SyntaxProbeCase.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

public enum SyntaxProbeScope
{
    Method,
    Class,
    File
}

public enum SyntaxSupportStatus
{
    Supported,
    Unsupported
}

public sealed record SyntaxProbeCase(
    string Id,
    string Title,
    string Version,
    SyntaxProbeScope Scope,
    SyntaxSupportStatus Status,
    string Snippet,
    string? Notes)
{
    public override string ToString() => $"{Version}:{Id} ({Status})";
}
