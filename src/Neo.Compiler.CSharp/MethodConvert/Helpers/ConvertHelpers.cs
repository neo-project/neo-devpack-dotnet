// Copyright (C) 2015-2025 The Neo Project.
//
// ConvertHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using scfx::Neo.SmartContract.Framework.Attributes;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler;

extern alias scfx;

internal partial class MethodConvert
{
    private bool TryProcessInlineMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, bool instanceOnStack = false)
    {
        SyntaxNode? syntaxNode = null;
        if (!symbol.DeclaringSyntaxReferences.IsEmpty)
            syntaxNode = symbol.DeclaringSyntaxReferences[0].GetSyntax();

        if (syntaxNode is not BaseMethodDeclarationSyntax syntax) return false;
        if (!symbol.GetAttributesWithInherited().Any(attribute => attribute.ConstructorArguments.Length > 0
                                                                  && attribute.AttributeClass?.Name == nameof(MethodImplAttribute)
                                                                  && attribute.ConstructorArguments[0].Value is not null
                                                                  && (MethodImplOptions)attribute.ConstructorArguments[0].Value! == MethodImplOptions.AggressiveInlining))
            return false;

        var walker = new InlineUsageWalker(model, symbol);
        if (syntax.Body is not null)
            walker.Visit(syntax.Body);
        else if (syntax.ExpressionBody is not null)
            walker.Visit(syntax.ExpressionBody.Expression);

        Dictionary<IParameterSymbol, InlineParameterInfo> parameterInfos = new(SymbolEqualityComparer.Default);
        foreach (var parameter in symbol.Parameters)
        {
            int totalUses = walker.ParameterUsage.TryGetValue(parameter, out var count) ? count : 0;
            if (totalUses > 1 || parameter.RefKind != RefKind.None)
                parameterInfos[parameter] = new InlineParameterInfo(totalUses);
        }

        InlineThisInfo? thisInfo = null;
        if (!symbol.IsStatic && walker.ThisUsageCount > 1)
            thisInfo = new InlineThisInfo(walker.ThisUsageCount);

        bool previousInternalInline = _internalInline;
        InlineContext? inlineContext = null;
        if (parameterInfos.Count > 0 || thisInfo is not null)
        {
            inlineContext = new InlineContext(parameterInfos, thisInfo);
            _inlineContexts.Push(inlineContext);
        }

        _internalInline = true;

        try
        {
            if (!symbol.IsStatic && !instanceOnStack)
            {
                _internalInline = previousInternalInline;
                ConvertInstanceExpression(model, instanceExpression);
            }

            if (arguments is not null)
            {
                _internalInline = previousInternalInline;
                PrepareArgumentsForMethod(model, symbol, arguments);
            }

            _internalInline = true;

            using (InsertSequencePoint(syntax))
            {
                if (syntax.Body != null)
                {
                    ConvertStatement(model, syntax.Body);
                }
                else if (syntax.ExpressionBody != null)
                {
                    ConvertExpression(model, syntax.ExpressionBody.Expression);
                }
            }

            if (syntax is MethodDeclarationSyntax methodSyntax
                && methodSyntax.ReturnType.ToString() == "void"
                && IsExpressionReturningValue(model, methodSyntax))
                AddInstruction(OpCode.DROP);
        }
        finally
        {
            if (inlineContext is not null)
                _inlineContexts.Pop();
            _internalInline = previousInternalInline;
        }

        return true;
    }

    // Helper methods
    private void InsertStaticFieldInitialization()
    {
        if (_context.StaticFieldCount > 0)
        {
            _instructions.Insert(0, new Instruction
            {
                OpCode = OpCode.INITSSLOT,
                Operand = [(byte)_context.StaticFieldCount]
            });
        }
    }

    private sealed class InlineUsageWalker : Microsoft.CodeAnalysis.CSharp.CSharpSyntaxWalker
    {
        private readonly SemanticModel _model;
        private readonly IMethodSymbol _methodSymbol;

        public InlineUsageWalker(SemanticModel model, IMethodSymbol methodSymbol)
        {
            _model = model;
            _methodSymbol = methodSymbol;
        }

        public Dictionary<IParameterSymbol, int> ParameterUsage { get; } = new(SymbolEqualityComparer.Default);
        public int ThisUsageCount { get; private set; }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            var symbol = _model.GetSymbolInfo(node).Symbol;
            if (symbol is IParameterSymbol parameter && SymbolEqualityComparer.Default.Equals(parameter.ContainingSymbol, _methodSymbol))
            {
                ParameterUsage.TryGetValue(parameter, out var count);
                ParameterUsage[parameter] = count + 1;
            }
            base.VisitIdentifierName(node);
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
            ThisUsageCount++;
            base.VisitThisExpression(node);
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
            ThisUsageCount++;
            base.VisitBaseExpression(node);
        }
    }

    private void InitializeFieldsBasedOnMethodKind(SemanticModel model)
    {
        switch (Symbol.MethodKind)
        {
            case MethodKind.Constructor:
                ProcessConstructorInitializer(model);
                break;
            case MethodKind.StaticConstructor:
                ProcessStaticFields(model);
                break;
        }
    }

    private void ValidateMethodName()
    {
        if (Symbol.Name.StartsWith("_") && !Symbol.IsInternalCoreMethod())
            throw new CompilationException(Symbol, DiagnosticId.InvalidMethodName, $"The method name {Symbol.Name} is not valid.");
    }

    private void InsertInitializationInstructions()
    {
        if (Symbol.MethodKind == MethodKind.StaticConstructor && _context.StaticFieldCount > 0)
        {
            InsertStaticFieldInitialization();
        }

        // Check if we need to add an INITSLOT instruction
        if (!_initSlot) return;
        byte pc = (byte)_parameters.Count;
        byte lc = (byte)_localsCount;
        if (NeedInstanceConstructor(Symbol)) pc++;
        // Only add INITSLOT if we have local variables or parameters
        if (pc > 0 || lc > 0)
        {
            // Insert INITSLOT at the beginning of the method
            // lc: number of local variables
            // pc: number of parameters (including 'this' for instance methods)
            _instructions.Insert(0, new Instruction
            {
                OpCode = OpCode.INITSLOT,
                Operand = [lc, pc]
            });
        }
    }

    private void ProcessModifiersExit(SemanticModel model, (byte fieldIndex, AttributeData attribute)[] modifiers)
    {
        foreach (var (fieldIndex, attribute) in modifiers)
        {
            var disposeInstruction = ExitModifier(model, fieldIndex, attribute);
            if (disposeInstruction is not null && _returnTarget.Instruction is null)
            {
                _returnTarget.Instruction = disposeInstruction;
            }
        }
    }

    private void FinalizeMethod()
    {
        if (_returnTarget.Instruction is null)
        {
            if (_instructions.Count > 0 && _instructions[^1].OpCode == OpCode.NOP && _instructions[^1].Location?.Source is not null)
            {
                _instructions[^1].OpCode = OpCode.RET;
                _returnTarget.Instruction = _instructions[^1];
            }
            else
            {
                _returnTarget.Instruction = AddInstruction(OpCode.RET);
            }
        }
        else
        {
            // it comes from modifier clean up
            AddInstruction(OpCode.RET);
        }
    }

    private IEnumerable<(byte fieldIndex, AttributeData attribute)> ConvertModifier(SemanticModel model)
    {
        foreach (var attribute in Symbol.GetAttributesWithInherited())
        {
            if (attribute.AttributeClass?.IsSubclassOf(nameof(ModifierAttribute)) != true)
                continue;

            JumpTarget notNullTarget = new();
            byte fieldIndex = _context.AddAnonymousStaticField();
            AccessSlot(OpCode.LDSFLD, fieldIndex);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, notNullTarget);

            MethodConvert constructor = _context.ConvertMethod(model, attribute.AttributeConstructor!);
            CreateObject(model, attribute.AttributeClass);
            foreach (var arg in attribute.ConstructorArguments.Reverse())
                Push(arg.Value);
            Push(attribute.ConstructorArguments.Length);
            AddInstruction(OpCode.PICK);
            EmitCall(constructor);
            AccessSlot(OpCode.STSFLD, fieldIndex);

            notNullTarget.Instruction = AccessSlot(OpCode.LDSFLD, fieldIndex);
            var enterSymbol = attribute.AttributeClass.GetAllMembers()
                .OfType<IMethodSymbol>()
                .First(p => p.Name == nameof(ModifierAttribute.Enter) && p.Parameters.Length == 0);
            MethodConvert enterMethod = _context.ConvertMethod(model, enterSymbol);
            EmitCall(enterMethod);
            yield return (fieldIndex, attribute);
        }
    }

    private Instruction? ExitModifier(SemanticModel model, byte fieldIndex, AttributeData attribute)
    {
        var exitSymbol = attribute.AttributeClass!.GetAllMembers()
            .OfType<IMethodSymbol>()
            .First(p => p is { Name: nameof(ModifierAttribute.Exit), Parameters.Length: 0 });
        MethodConvert exitMethod = _context.ConvertMethod(model, exitSymbol);
        if (exitMethod.IsEmpty) return null;
        var instruction = AccessSlot(OpCode.LDSFLD, fieldIndex);
        EmitCall(exitMethod);
        return instruction;
    }

    private void CreateObject(SemanticModel model, ITypeSymbol type)
    {
        var members = type.GetAllMembers().Where(p => !p.IsStatic).ToArray();
        var fields = members.OfType<IFieldSymbol>().ToArray();

        int needVirtualMethodTable = 0;
        var virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
        if (!type.IsRecord && virtualMethods.Length > 0)
        {
            needVirtualMethodTable += 1;
            byte vTableIndex = _context.AddVTable(type);
            AccessSlot(OpCode.LDSFLD, vTableIndex);
        }

        if (fields.Length == 0 && needVirtualMethodTable == 0)
        {
            AddInstruction(type.IsValueType || type.IsRecord ? OpCode.NEWSTRUCT0 : OpCode.NEWARRAY0);
            return;
        }

        foreach (var field in fields.Reverse())  // PACK and PACKSTRUCT works in a reversed way
            ProcessFieldInitializer(model, field, null, null);
        Push(fields.Length + needVirtualMethodTable);
        AddInstruction(type.IsValueType || type.IsRecord ? OpCode.PACKSTRUCT : OpCode.PACK);
    }
}
