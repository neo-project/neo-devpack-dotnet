// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Linq;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ProcessFields(SemanticModel model)
    {
        _initslot = true;
        IFieldSymbol[] fields = Symbol.ContainingType.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            ProcessFieldInitializer(model, fields[i], () =>
            {
                AddInstruction(OpCode.LDARG0);
                Push(i);
            }, () =>
            {
                AddInstruction(OpCode.SETITEM);
            });
        }
    }
    private void ProcessFieldInitializer(SemanticModel model, IFieldSymbol field, Action? preInitialize, Action? postInitialize)
    {
        AttributeData? initialValue = field.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(InitialValueAttribute));
        if (initialValue is null)
        {
            EqualsValueClauseSyntax? initializer;
            SyntaxNode syntaxNode;
            if (field.DeclaringSyntaxReferences.IsEmpty)
            {
                if (field.AssociatedSymbol is not IPropertySymbol property) return;
                PropertyDeclarationSyntax syntax = (PropertyDeclarationSyntax)property.DeclaringSyntaxReferences[0].GetSyntax();
                syntaxNode = syntax;
                initializer = syntax.Initializer;
            }
            else
            {
                VariableDeclaratorSyntax syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences[0].GetSyntax();
                syntaxNode = syntax;
                initializer = syntax.Initializer;
            }
            if (initializer is null) return;
            model = model.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);
            using (InsertSequencePoint(syntaxNode))
            {
                preInitialize?.Invoke();
                ConvertExpression(model, initializer.Value);
                postInitialize?.Invoke();
            }
        }
        else
        {
            preInitialize?.Invoke();
            string value = (string)initialValue.ConstructorArguments[0].Value!;
            ContractParameterType type = (ContractParameterType)initialValue.ConstructorArguments[1].Value!;
            switch (type)
            {
                case ContractParameterType.String:
                    Push(value);
                    break;
                case ContractParameterType.ByteArray:
                    Push(value.HexToBytes(true));
                    break;
                case ContractParameterType.Hash160:
                    Push((UInt160.TryParse(value, out var hash) ? hash : value.ToScriptHash(context.Options.AddressVersion)).ToArray());
                    break;
                case ContractParameterType.PublicKey:
                    Push(ECPoint.Parse(value, ECCurve.Secp256r1).EncodePoint(true));
                    break;
                default:
                    throw new CompilationException(field, DiagnosticId.InvalidInitialValueType, $"Unsupported initial value type: {type}");
            }
            postInitialize?.Invoke();
        }
    }


}
