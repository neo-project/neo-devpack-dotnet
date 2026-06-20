// Copyright (C) 2015-2026 The Neo Project.
//
// CSharpSourceName.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace Neo.Compiler
{
    internal static class CSharpSourceName
    {
        public static string Identifier(string value, string fallback)
        {
            if (string.IsNullOrEmpty(value))
                return fallback;

            var builder = new StringBuilder(value.Length + 1);

            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];

                if (i == 0)
                {
                    if (SyntaxFacts.IsIdentifierStartCharacter(ch))
                    {
                        builder.Append(ch);
                    }
                    else
                    {
                        builder.Append('_');
                        if (SyntaxFacts.IsIdentifierPartCharacter(ch))
                            builder.Append(ch);
                    }

                    continue;
                }

                builder.Append(SyntaxFacts.IsIdentifierPartCharacter(ch) ? ch : '_');
            }

            var identifier = builder.Length == 0 ? fallback : builder.ToString();
            return SyntaxFacts.GetKeywordKind(identifier) == SyntaxKind.None ? identifier : "@" + identifier;
        }

        public static string IdentifierSuffix(string value, string fallback)
        {
            var identifier = Identifier(value, fallback);
            return identifier.StartsWith('@') ? identifier[1..] : identifier;
        }

        public static string StringLiteralValue(string value)
        {
            var builder = new StringBuilder(value.Length);

            foreach (var ch in value)
            {
                builder.Append(ch switch
                {
                    '\\' => @"\\",
                    '"' => "\\\"",
                    '\0' => @"\0",
                    '\a' => @"\a",
                    '\b' => @"\b",
                    '\f' => @"\f",
                    '\n' => @"\n",
                    '\r' => @"\r",
                    '\t' => @"\t",
                    '\v' => @"\v",
                    _ when char.IsControl(ch) => $"\\u{(int)ch:x4}",
                    _ => ch.ToString()
                });
            }

            return builder.ToString();
        }
    }
}
