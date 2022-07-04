// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Adapted from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/CSharpHelpers.cs

using System.Collections.Immutable;
using System.Globalization;

namespace Neo.BuildTasks
{
    static class CSharpHelpers
    {
        // generated from Roslyn C# 4.2 SyntaxFacts.GetKeywordKinds().Select(k => SyntaxFacts.GetText(k)).OrderBy(k => k)
        static readonly ImmutableHashSet<string> CSHARP_KEYWORDS = ImmutableHashSet.Create("__arglist", "__makeref",
            "__reftype", "__refvalue", "abstract", "add", "alias", "and", "as", "ascending", "assembly", "async",
            "await", "base", "bool", "break", "by", "byte", "case", "catch", "char", "checked", "class", "const",
            "continue", "decimal", "default", "delegate", "descending", "do", "double", "else", "enum", "equals",
            "event", "explicit", "extern", "false", "field", "finally", "fixed", "float", "for", "foreach", "from",
            "get", "global", "goto", "group", "if", "implicit", "in", "init", "int", "interface", "internal", "into",
            "is", "join", "let", "lock", "long", "managed", "method", "module", "nameof", "namespace", "new", "not",
            "null", "object", "on", "operator", "or", "orderby", "out", "override", "param", "params", "partial",
            "private", "property", "protected", "public", "readonly", "record", "ref", "remove", "return", "sbyte",
            "sealed", "select", "set", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this",
            "throw", "true", "try", "type", "typeof", "typevar", "uint", "ulong", "unchecked", "unmanaged", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "when", "where", "while", "with", "yield");

        public static string CreateEscapedIdentifier(string name)
        {
            // Any identifier started with two consecutive underscores are
            // reserved by CSharp.
            if (IsKeyword(name) || IsPrefixTwoUnderscore(name))
            {
                return "@" + name;
            }
            return name;

            static bool IsKeyword(string value)
            {
                return CSHARP_KEYWORDS.Contains(value);
            }

            static bool IsPrefixTwoUnderscore(string value)
            {
                if (value.Length < 3)
                {
                    return false;
                }
                else
                {
                    return ((value[0] == '_') && (value[1] == '_') && (value[2] != '_'));
                }
            }
        }

        public static bool IsValidTypeName(string value) => IsValidTypeNameOrIdentifier(value, true);
        public static bool IsValidIdentifier(string value) => IsValidTypeNameOrIdentifier(value, false);

        static bool IsValidTypeNameOrIdentifier(string value, bool isTypeName)
        {
            bool nextMustBeStartChar = true;

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            // each char must be Lu, Ll, Lt, Lm, Lo, Nd, Mn, Mc, Pc
            //
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                switch (uc)
                {
                    case UnicodeCategory.UppercaseLetter:        // Lu
                    case UnicodeCategory.LowercaseLetter:        // Ll
                    case UnicodeCategory.TitlecaseLetter:        // Lt
                    case UnicodeCategory.ModifierLetter:         // Lm
                    case UnicodeCategory.LetterNumber:           // Lm
                    case UnicodeCategory.OtherLetter:            // Lo
                        nextMustBeStartChar = false;
                        break;

                    case UnicodeCategory.NonSpacingMark:         // Mn
                    case UnicodeCategory.SpacingCombiningMark:   // Mc
                    case UnicodeCategory.ConnectorPunctuation:   // Pc
                    case UnicodeCategory.DecimalDigitNumber:     // Nd
                        // Underscore is a valid starting character, even though it is a ConnectorPunctuation.
                        if (nextMustBeStartChar && ch != '_')
                            return false;

                        nextMustBeStartChar = false;
                        break;
                    default:
                        // We only check the special Type chars for type names.
                        if (isTypeName && IsSpecialTypeChar(ch, ref nextMustBeStartChar))
                        {
                            break;
                        }

                        return false;
                }
            }

            return true;
        }

        // This can be a special character like a separator that shows up in a type name
        // This is an odd set of characters.  Some come from characters that are allowed by C++, like < and >.
        // Others are characters that are specified in the type and assembly name grammar.
        static bool IsSpecialTypeChar(char ch, ref bool nextMustBeStartChar)
        {
            switch (ch)
            {
                case ':':
                case '.':
                case '$':
                case '+':
                case '<':
                case '>':
                case '-':
                case '[':
                case ']':
                case ',':
                case '&':
                case '*':
                    nextMustBeStartChar = true;
                    return true;

                case '`':
                    return true;
            }
            return false;
        }
    }
}