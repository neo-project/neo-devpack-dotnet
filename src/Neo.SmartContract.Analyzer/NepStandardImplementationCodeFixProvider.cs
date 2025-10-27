// Copyright (C) 2015-2025 The Neo Project.
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

        foreach (var member in GenerateMembers(missingMembers, standard))
        {
            editor.AddMember(classDeclaration, member);
        }

        return editor.GetChangedDocument();
    }

    private static async Task<Document> AddInterfaceAsync(Document document, ClassDeclarationSyntax classDeclaration, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        if (!diagnostic.Properties.TryGetValue("Interface", out var interfaceName) ||
            string.IsNullOrWhiteSpace(interfaceName))
        {
            return document;
        }

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        bool AlreadyImplements(ClassDeclarationSyntax decl) =>
            decl.BaseList?.Types.Any(type => string.Equals(type.Type.ToString(), interfaceName, StringComparison.Ordinal)) == true;

        if (AlreadyImplements(classDeclaration))
            return document;

        var interfaceTypeSyntax = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(interfaceName));

        var updatedClass = classDeclaration.AddBaseListTypes(interfaceTypeSyntax)
            .WithAdditionalAnnotations(Formatter.Annotation);

        if (updatedClass.BaseList is { } baseList)
        {
            var colonToken = baseList.ColonToken
                .WithLeadingTrivia(SyntaxFactory.Space)
                .WithTrailingTrivia(SyntaxFactory.Space);
            updatedClass = updatedClass.WithBaseList(baseList.WithColonToken(colonToken));
        }

        var identifier = updatedClass.Identifier;
        if (identifier.TrailingTrivia.Any(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia)))
        {
            updatedClass = updatedClass.WithIdentifier(identifier.WithTrailingTrivia(SyntaxFactory.Space));
        }

        editor.ReplaceNode(classDeclaration, updatedClass);
        return editor.GetChangedDocument();
    }

    private static ImmutableArray<string> ParseMissingMembers(Diagnostic diagnostic)
    {
        if (!diagnostic.Properties.TryGetValue("MissingMembers", out var membersValue) ||
            string.IsNullOrWhiteSpace(membersValue))
            return ImmutableArray<string>.Empty;

        var members = membersValue!
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

    private static IEnumerable<MemberDeclarationSyntax> GenerateMembers(
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

    private enum NepStandardKind
    {
        Unknown,
        Nep11,
        Nep17
    }
}
