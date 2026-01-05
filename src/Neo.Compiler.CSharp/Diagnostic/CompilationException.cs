// Copyright (C) 2015-2026 The Neo Project.
//
// CompilationException.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using System;

namespace Neo.Compiler
{
    class CompilationException : Exception
    {
        public Diagnostic Diagnostic { get; }

        private CompilationException(Location? location, string id, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            Diagnostic = Diagnostic.Create(id, DiagnosticCategory.Default, message, DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, location: location);
        }

        public CompilationException(string id, string message, Exception? innerException = null)
            : this((Location?)null, id, message, innerException)
        {
        }

        public CompilationException(SyntaxNodeOrToken syntax, string id, string message, Exception? innerException = null)
            : this(syntax.GetLocation(), id, message, innerException)
        {
        }

        public CompilationException(ISymbol symbol, string id, string message, Exception? innerException = null)
            : this(symbol.Locations.IsEmpty ? null : symbol.Locations[0], id, message, innerException)
        {
        }

        /// <summary>
        /// Creates a CompilationException for unsupported syntax with enhanced error message.
        /// </summary>
        /// <param name="syntax">The unsupported syntax node</param>
        /// <param name="baseMessage">Optional custom base message</param>
        /// <returns>CompilationException with enhanced error message</returns>
        public static CompilationException UnsupportedSyntax(SyntaxNodeOrToken syntax, string? baseMessage = null)
        {
            var enhancedMessage = ErrorMessageBuilder.BuildUnsupportedSyntaxMessage(syntax, baseMessage);
            return new CompilationException(syntax, DiagnosticId.SyntaxNotSupported, enhancedMessage);
        }

        /// <summary>
        /// Creates a CompilationException for invalid types with enhanced error message.
        /// </summary>
        /// <param name="syntax">The syntax node where the invalid type was used</param>
        /// <param name="type">The invalid type</param>
        /// <param name="context">Additional context about where the type was used</param>
        /// <returns>CompilationException with enhanced error message</returns>
        public static CompilationException InvalidType(SyntaxNodeOrToken syntax, ITypeSymbol? type, string? context = null)
        {
            var enhancedMessage = ErrorMessageBuilder.BuildInvalidTypeMessage(syntax, type, context);
            return new CompilationException(syntax, DiagnosticId.InvalidType, enhancedMessage);
        }

        /// <summary>
        /// Creates a CompilationException for method-related errors with enhanced error message.
        /// </summary>
        /// <param name="syntax">The syntax node</param>
        /// <param name="methodName">The method name</param>
        /// <param name="issue">The specific issue with the method</param>
        /// <param name="diagnosticId">The diagnostic ID to use</param>
        /// <returns>CompilationException with enhanced error message</returns>
        public static CompilationException MethodError(SyntaxNodeOrToken syntax, string methodName, string issue, string diagnosticId)
        {
            var enhancedMessage = ErrorMessageBuilder.BuildMethodErrorMessage(syntax, methodName, issue);
            return new CompilationException(syntax, diagnosticId, enhancedMessage);
        }

        /// <summary>
        /// Creates a CompilationException for file operation errors with enhanced error message.
        /// </summary>
        /// <param name="operation">The file operation that failed</param>
        /// <param name="filePath">The file path involved</param>
        /// <param name="diagnosticId">The diagnostic ID to use</param>
        /// <param name="innerException">The underlying exception if any</param>
        /// <returns>CompilationException with enhanced error message</returns>
        public static CompilationException FileOperation(string operation, string filePath, string diagnosticId = DiagnosticId.FileOperationFailed, Exception? innerException = null)
        {
            var enhancedMessage = ErrorMessageBuilder.BuildFileOperationMessage(operation, filePath, innerException);
            return new CompilationException((Location?)null, diagnosticId, enhancedMessage, innerException);
        }

        /// <summary>
        /// Creates a CompilationException for unexpected compiler errors with detailed context.
        /// </summary>
        /// <param name="context">Description of the operation that failed</param>
        /// <param name="exception">The unexpected exception</param>
        /// <returns>CompilationException with enhanced unexpected-error message</returns>
        public static CompilationException Unexpected(string context, Exception exception)
        {
            if (exception is null) throw new ArgumentNullException(nameof(exception));

            var enhancedMessage = ErrorMessageBuilder.BuildUnexpectedErrorMessage(context, exception);
            return new CompilationException((Location?)null, DiagnosticId.UnexpectedCompilerError, enhancedMessage, exception);
        }
    }
}
