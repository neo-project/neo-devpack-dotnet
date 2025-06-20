// Copyright (C) 2015-2025 The Neo Project.
//
// ErrorMessageBuilder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler
{
    /// <summary>
    /// Provides enhanced error message building with context, location information, and suggestions.
    /// </summary>
    internal static class ErrorMessageBuilder
    {
        /// <summary>
        /// Creates an enhanced error message for unsupported syntax with location and context.
        /// </summary>
        /// <param name="syntax">The syntax node that caused the error</param>
        /// <param name="baseMessage">The base error message</param>
        /// <returns>Enhanced error message with location and context</returns>
        public static string BuildUnsupportedSyntaxMessage(SyntaxNodeOrToken syntax, string? baseMessage = null)
        {
            var location = syntax.GetLocation();
            var syntaxType = syntax.IsNode ? syntax.AsNode()!.GetType().Name : syntax.AsToken().ValueText;

            var message = baseMessage ?? $"Unsupported {syntaxType} syntax";

            if (location != null && location.SourceTree != null)
            {
                var lineSpan = location.GetLineSpan();
                var lineNumber = lineSpan.StartLinePosition.Line + 1;
                var column = lineSpan.StartLinePosition.Character + 1;
                var fileName = System.IO.Path.GetFileName(location.SourceTree.FilePath);

                message += $" at {fileName}:{lineNumber}:{column}";

                // Add code snippet if available
                var sourceText = location.SourceTree.GetText();
                if (sourceText != null && lineSpan.StartLinePosition.Line < sourceText.Lines.Count)
                {
                    var line = sourceText.Lines[lineSpan.StartLinePosition.Line].ToString().Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        message += $". Code: '{line}'";
                    }
                }
            }

            // Add suggestions based on syntax type
            var suggestions = GetSyntaxSuggestions(syntax);
            if (!string.IsNullOrEmpty(suggestions))
            {
                message += $". {suggestions}";
            }

            return message;
        }

        /// <summary>
        /// Creates an enhanced error message for invalid types with type information.
        /// </summary>
        /// <param name="syntax">The syntax node</param>
        /// <param name="type">The invalid type</param>
        /// <param name="context">Additional context about where the type was used</param>
        /// <returns>Enhanced error message with type details</returns>
        public static string BuildInvalidTypeMessage(SyntaxNodeOrToken syntax, ITypeSymbol? type, string? context = null)
        {
            var typeName = type?.ToDisplayString() ?? "unknown";
            var message = $"Invalid type '{typeName}'";

            if (!string.IsNullOrEmpty(context))
            {
                message += $" in {context}";
            }

            var location = syntax.GetLocation();
            if (location != null)
            {
                var lineSpan = location.GetLineSpan();
                var fileName = System.IO.Path.GetFileName(location.SourceTree?.FilePath);
                message += $" at {fileName}:{lineSpan.StartLinePosition.Line + 1}:{lineSpan.StartLinePosition.Character + 1}";
            }

            // Add suggestions for supported types
            var supportedTypes = GetSupportedTypeSuggestions(type);
            if (!string.IsNullOrEmpty(supportedTypes))
            {
                message += $". Consider using: {supportedTypes}";
            }

            return message;
        }

        /// <summary>
        /// Creates an enhanced error message for file operations with full context.
        /// </summary>
        /// <param name="operation">The file operation that failed</param>
        /// <param name="filePath">The file path involved</param>
        /// <param name="innerException">The underlying exception if any</param>
        /// <returns>Enhanced error message with file operation context</returns>
        public static string BuildFileOperationMessage(string operation, string filePath, Exception? innerException = null)
        {
            var message = $"Failed to {operation} file '{filePath}'";

            if (innerException != null)
            {
                message += $": {innerException.Message}";
            }

            // Add helpful suggestions based on the type of operation
            var suggestions = GetFileOperationSuggestions(operation, filePath, innerException);
            if (!string.IsNullOrEmpty(suggestions))
            {
                message += $". {suggestions}";
            }

            return message;
        }

        /// <summary>
        /// Creates an enhanced error message for method-related errors with signature details.
        /// </summary>
        /// <param name="syntax">The syntax node</param>
        /// <param name="methodName">The method name</param>
        /// <param name="issue">The specific issue with the method</param>
        /// <returns>Enhanced error message with method context</returns>
        public static string BuildMethodErrorMessage(SyntaxNodeOrToken syntax, string methodName, string issue)
        {
            var message = $"Method '{methodName}': {issue}";

            var location = syntax.GetLocation();
            if (location != null)
            {
                var lineSpan = location.GetLineSpan();
                var fileName = System.IO.Path.GetFileName(location.SourceTree?.FilePath);
                message += $" at {fileName}:{lineSpan.StartLinePosition.Line + 1}";
            }

            return message;
        }

        /// <summary>
        /// Gets syntax-specific suggestions for unsupported features.
        /// </summary>
        private static string GetSyntaxSuggestions(SyntaxNodeOrToken syntax)
        {
            if (syntax.IsNode)
            {
                return syntax.AsNode() switch
                {
                    PredefinedTypeSyntax predefined when predefined.Keyword.ValueText == "float" || predefined.Keyword.ValueText == "double" =>
                        "Consider using decimal or BigInteger for precise calculations",
                    InterfaceDeclarationSyntax => "Use abstract classes instead of interfaces",
                    UnsafeStatementSyntax => "Remove unsafe code blocks - NEO VM doesn't support unsafe operations",
                    PointerTypeSyntax => "Use arrays or references instead of pointers",
                    FixedStatementSyntax => "Use regular variable declarations instead of fixed statements",
                    LockStatementSyntax => "Remove lock statements - NEO contracts are single-threaded",
                    UsingStatementSyntax when syntax.AsNode() is UsingStatementSyntax u && u.Declaration != null =>
                        "Consider manual resource management or try-finally blocks",
                    _ => ""
                };
            }

            return "";
        }

        /// <summary>
        /// Gets suggestions for supported types when an unsupported type is used.
        /// </summary>
        private static string GetSupportedTypeSuggestions(ITypeSymbol? type)
        {
            if (type == null) return "";

            var typeName = type.Name.ToLowerInvariant();

            return typeName switch
            {
                "float" or "double" => "int, BigInteger, or decimal",
                "datetime" => "long (timestamp) or string",
                "guid" => "string or byte array",
                "tuple" => "custom struct or class",
                "nullable" => "direct type usage with null checks",
                _ when type.TypeKind == TypeKind.Interface => "abstract class or concrete implementation",
                _ when type.IsReferenceType && type.TypeKind == TypeKind.Class =>
                    "struct, primitive types, or Neo framework types",
                _ => ""
            };
        }

        /// <summary>
        /// Gets suggestions for file operation failures.
        /// </summary>
        private static string GetFileOperationSuggestions(string operation, string filePath, Exception? innerException)
        {
            return operation.ToLowerInvariant() switch
            {
                "read" or "load" => "Ensure the file exists and you have read permissions",
                "write" or "save" => "Ensure the directory exists and you have write permissions",
                "delete" => "Ensure the file exists and is not in use by another process",
                "compile" => "Check that the project file is valid and all dependencies are available",
                _ => "Verify the file path is correct and accessible"
            };
        }
    }
}
