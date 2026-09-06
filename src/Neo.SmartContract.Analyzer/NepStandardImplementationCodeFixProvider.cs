// Copyright (C) 2015-2026 The Neo Project.
//
// NepStandardImplementationCodeFixProvider.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace Neo.SmartContract.Analyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NepStandardImplementationCodeFixProvider))]
public sealed class NepStandardImplementationCodeFixProvider : CodeFixProvider
{
    private const string Title = "Add missing NEP standard members";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        NepStandardImplementationAnalyzer.DiagnosticId,
        NepStandardImplementationAnalyzer.InterfaceDiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var node = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();

        if (node is null)
            return;

        Func<CancellationToken, Task<Document>> action = diagnostic.Id switch
        {
            NepStandardImplementationAnalyzer.InterfaceDiagnosticId =>
                ct => AddInterfaceAsync(context.Document, node, diagnostic, ct),
            _ => ct => AddMembersAsync(context.Document, node, diagnostic, ct)
        };

        context.RegisterCodeFix(
            CodeAction.Create(
                Title,
                action,
                Title),
            diagnostic);
    }

    private static async Task<Document> AddMembersAsync(Document document, ClassDeclarationSyntax classDeclaration, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var missingMembers = ParseMissingMembers(diagnostic);
        if (missingMembers.Length == 0)
            return document;

        var standard = ParseStandard(diagnostic);
        var formatAnnotation = new SyntaxAnnotation("NepGeneratedMember");
        var usedNames = new HashSet<string>(StringComparer.Ordinal);
        for (var type = editor.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);
             type is not null; type = type.BaseType)
        {
            usedNames.Add(type.Name);
            usedNames.UnionWith(type.TypeParameters.Select(parameter => parameter.Name));
            usedNames.UnionWith(type.GetMembers().Select(member => member.Name));
        }

        foreach (var generatedMember in GenerateMembers(missingMembers, standard))
        {
            var member = generatedMember;
            var originalName = member.Identifier.ValueText;
            if (!usedNames.Add(originalName))
            {
                var suffix = 1;
                string uniqueName;
                do
                {
                    uniqueName = originalName + suffix++;
                }
                while (!usedNames.Add(uniqueName));

                var abiName = char.ToLowerInvariant(originalName[0]) + originalName.Substring(1);
                var displayName = SyntaxFactory.Attribute(SyntaxFactory.ParseName("global::System.ComponentModel.DisplayName"))
                    .WithArgumentList(SyntaxFactory.AttributeArgumentList(SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(abiName))))));
                member = member.WithIdentifier(SyntaxFactory.Identifier(uniqueName))
                    .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(displayName)));
            }

            if (originalName is "Symbol" or "Decimals" or "TotalSupply" or "BalanceOf" or
                "OwnerOf" or "Properties" or "Tokens" or "TokensOf")
            {
                member = member.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.ParseName("global::Neo.SmartContract.Framework.Attributes.Safe")))));
            }

            editor.AddMember(classDeclaration, member.WithAdditionalAnnotations(formatAnnotation));
        }

        return await FormatAnnotatedNodesAsync(editor.GetChangedDocument(), document, formatAnnotation, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> AddInterfaceAsync(Document document, ClassDeclarationSyntax classDeclaration, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        if (!diagnostic.Properties.TryGetValue("Interface", out var interfaceName) ||
            interfaceName is not { Length: > 0 })
        {
            return document;
        }

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        bool AlreadyImplements(ClassDeclarationSyntax decl) =>
            decl.BaseList?.Types.Any(type => string.Equals(type.Type.ToString(), interfaceName, StringComparison.Ordinal)) == true;

        if (AlreadyImplements(classDeclaration))
            return document;

        var formatAnnotation = new SyntaxAnnotation("NepGeneratedInterface");
        var interfaceTypeSyntax = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(interfaceName));

        var updatedClass = classDeclaration.AddBaseListTypes(interfaceTypeSyntax);

        if (updatedClass.BaseList is { } baseList)
        {
            var colonToken = baseList.ColonToken.WithLeadingTrivia(SyntaxFactory.Space).WithTrailingTrivia(SyntaxFactory.Space);
            updatedClass = updatedClass.WithBaseList(baseList.WithColonToken(colonToken).WithAdditionalAnnotations(formatAnnotation));
        }

        var identifier = updatedClass.Identifier;
        if (identifier.TrailingTrivia.Any(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia)))
        {
            var hasComment = identifier.TrailingTrivia.Any(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) || t.IsKind(SyntaxKind.MultiLineCommentTrivia));

            if (!hasComment)
                updatedClass = updatedClass.WithIdentifier(
                    identifier.WithTrailingTrivia(SyntaxFactory.Space));
        }

        editor.ReplaceNode(classDeclaration, updatedClass);
        return await FormatAnnotatedNodesAsync(editor.GetChangedDocument(), document, formatAnnotation, cancellationToken).ConfigureAwait(false);
    }

    private static ImmutableArray<string> ParseMissingMembers(Diagnostic diagnostic)
    {
        if (!diagnostic.Properties.TryGetValue("MissingMembers", out var membersValue) ||
            membersValue is not { Length: > 0 })
            return ImmutableArray<string>.Empty;

        var members = membersValue
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(static member => member.Trim())
            .Where(static member => member.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .ToImmutableArray();

        return members;
    }

    private static NepStandardKind ParseStandard(Diagnostic diagnostic)
    {
        if (diagnostic.Properties.TryGetValue("Standard", out var value))
        {
            if (string.Equals(value, "NEP-17", StringComparison.OrdinalIgnoreCase))
                return NepStandardKind.Nep17;
            if (string.Equals(value, "NEP-11", StringComparison.OrdinalIgnoreCase))
                return NepStandardKind.Nep11;
        }

        return NepStandardKind.Unknown;
    }

    private static IEnumerable<MethodDeclarationSyntax> GenerateMembers(
        ImmutableArray<string> missingMembers,
        NepStandardKind standard)
    {
        foreach (var memberName in missingMembers)
        {
            yield return memberName switch
            {
                "Symbol" => CreateMethod("string", "Symbol"),
                "Decimals" => CreateMethod("byte", "Decimals"),
                "TotalSupply" => CreateMethod("System.Numerics.BigInteger", "TotalSupply"),
                "BalanceOf" => CreateMethod("System.Numerics.BigInteger", "BalanceOf",
                    ("Neo.SmartContract.Framework.UInt160", "owner")),
                "Transfer" => standard == NepStandardKind.Nep11
                    ? CreateMethod("bool", "Transfer",
                        ("Neo.SmartContract.Framework.UInt160", "to"),
                        ("Neo.SmartContract.Framework.ByteString", "tokenId"),
                        ("object", "data"))
                    : CreateMethod("bool", "Transfer",
                        ("Neo.SmartContract.Framework.UInt160", "from"),
                        ("Neo.SmartContract.Framework.UInt160", "to"),
                        ("System.Numerics.BigInteger", "amount"),
                        ("object", "data")),
                "OwnerOf" => CreateMethod("Neo.SmartContract.Framework.UInt160", "OwnerOf",
                    ("Neo.SmartContract.Framework.ByteString", "tokenId")),
                "Properties" => CreateMethod("Neo.SmartContract.Framework.Map<string, object>", "Properties",
                    ("Neo.SmartContract.Framework.ByteString", "tokenId")),
                "Tokens" => CreateMethod("Neo.SmartContract.Framework.Services.Iterator", "Tokens"),
                "TokensOf" => CreateMethod("Neo.SmartContract.Framework.Services.Iterator", "TokensOf",
                    ("Neo.SmartContract.Framework.UInt160", "owner")),
                _ => CreateMethod("object", memberName)
            };
        }
    }

    private static MethodDeclarationSyntax CreateMethod(
        string returnTypeName,
        string methodName,
        params (string TypeName, string ParameterName)[] parameters)
    {
        var returnType = SyntaxFactory.ParseTypeName(returnTypeName);
        var parameterList = parameters
            .Select(parameter =>
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.ParameterName))
                    .WithType(SyntaxFactory.ParseTypeName(parameter.TypeName)))
            .ToArray();

        var throwStatement = SyntaxFactory.ThrowStatement(
            SyntaxFactory.ObjectCreationExpression(
                    SyntaxFactory.ParseTypeName("System.NotImplementedException"))
                .WithArgumentList(SyntaxFactory.ArgumentList()));

        return SyntaxFactory.MethodDeclaration(returnType, methodName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList)))
            .WithBody(SyntaxFactory.Block(throwStatement));
    }

    private static async Task<Document> FormatAnnotatedNodesAsync(
        Document changedDocument,
        Document originalDocument,
        SyntaxAnnotation annotation,
        CancellationToken cancellationToken)
    {
        var originalText = await originalDocument.GetTextAsync(cancellationToken).ConfigureAwait(false);
        var eol = DetectLineEnding(originalText);
        var options = changedDocument.Project.Solution.Workspace.Options
            .WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, eol);
        return await Formatter.FormatAsync(changedDocument, annotation, options, cancellationToken).ConfigureAwait(false);
    }

    private static string DetectLineEnding(SourceText sourceText)
    {
        var crlfCount = 0;
        var lfCount = 0;

        for (var i = 0; i < sourceText.Length; i++)
        {
            if (sourceText[i] == '\r' && i + 1 < sourceText.Length && sourceText[i + 1] == '\n')
            {
                crlfCount++;
                i++; // skip the \n already counted as part of \r\n
            }
            else if (sourceText[i] == '\n')
            {
                lfCount++;
            }
        }

        return crlfCount > 0 && crlfCount >= lfCount ? "\r\n" : "\n";
    }

    private enum NepStandardKind
    {
        Unknown,
        Nep11,
        Nep17
    }
}
