// Copyright (C) 2015-2026 The Neo Project.
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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using scfx::Neo.SmartContract.Framework.Attributes;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler;

extern alias scfx;

internal partial class MethodConvert
{
    private static byte RequireByteSizedSlotCount(ISymbol symbol, int count, string description)
    {
        if ((uint)count > byte.MaxValue)
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, $"Methods with more than {byte.MaxValue} {description} are not supported.");
        return (byte)count;
    }

    private bool TryProcessInlineMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (_context.Options.NoInline)
            return false;

        IMethodSymbol inlineSymbol = symbol.ReducedFrom ?? symbol;
        SyntaxNode? syntaxNode = null;
        if (!inlineSymbol.DeclaringSyntaxReferences.IsEmpty)
            syntaxNode = inlineSymbol.DeclaringSyntaxReferences[0].GetSyntax();

        if (syntaxNode is not BaseMethodDeclarationSyntax syntax) return false;
        if (!inlineSymbol.IsAggressiveInlineMethod())
            return false;

        if (NeedInstanceConstructor(inlineSymbol) || inlineSymbol.Parameters.Any(parameter => IsByRef(parameter.RefKind)))
            return false;

        IReadOnlyList<SyntaxNode>? inlineArguments = GetInlineArguments(symbol, inlineSymbol, instanceExpression, arguments);
        if (inlineArguments is null && inlineSymbol.Parameters.Length > 0)
            return false;

        Dictionary<IParameterSymbol, byte>? parameterSlots = null;
        List<byte> anonymousSlots = [];
        if (inlineArguments is not null && inlineSymbol.Parameters.Length > 0)
        {
            PrepareArgumentsForMethod(model, inlineSymbol, inlineArguments);
            parameterSlots = new Dictionary<IParameterSymbol, byte>(SymbolEqualityComparer.Default);

            foreach (IParameterSymbol parameter in inlineSymbol.Parameters)
            {
                byte slot = AddAnonymousVariable();
                AccessSlot(OpCode.STLOC, slot);
                anonymousSlots.Add(slot);
                parameterSlots[parameter] = slot;

                IParameterSymbol original = parameter.OriginalDefinition;
                if (!SymbolEqualityComparer.Default.Equals(parameter, original))
                    parameterSlots[original] = slot;
            }
        }

        JumpTarget inlineReturnTarget = new();
        if (parameterSlots is not null)
            _inlineParameterScopes.Push(parameterSlots);
        _inlineReturnTargets.Push(inlineReturnTarget);

        try
        {
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

            // If the method has no return value,
            // but the expression body has a return value, example: a+=1;
            // drop the return value
            // Problem:
            //   [MethodImpl(MethodImplOptions.AggressiveInlining)]
            //   public void Test() => a+=1; // this will push an int value to the stack
            //   [MethodImpl(MethodImplOptions.AggressiveInlining)]
            //   public void Test() { a+=1; } // this will not push value to the stack
            if (syntax is MethodDeclarationSyntax methodSyntax
                && methodSyntax.ReturnType.ToString() == "void"
                && IsExpressionReturningValue(model, methodSyntax))
                AddInstruction(OpCode.DROP);
        }
        finally
        {
            _inlineReturnTargets.Pop();
            if (parameterSlots is not null)
                _inlineParameterScopes.Pop();

            foreach (byte slot in anonymousSlots)
                RemoveAnonymousVariable(slot);
        }

        inlineReturnTarget.Instruction = AddInstruction(OpCode.NOP);

        return true;
    }

    private static IReadOnlyList<SyntaxNode>? GetInlineArguments(
        IMethodSymbol symbol,
        IMethodSymbol inlineSymbol,
        ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (SymbolEqualityComparer.Default.Equals(symbol, inlineSymbol))
            return arguments;

        if (instanceExpression is null || arguments is null)
            return null;

        List<SyntaxNode> inlineArguments = new(inlineSymbol.Parameters.Length) { instanceExpression };
        inlineArguments.AddRange(arguments);
        return inlineArguments;
    }

    // Helper methods
    private void InsertStaticFieldInitialization()
    {
        if (_context.StaticFieldCount == 0)
            return;

        if (_instructions.Count > 0 && _instructions[0].OpCode == OpCode.INITSSLOT)
            return;

        _instructions.Insert(0, new Instruction
        {
            OpCode = OpCode.INITSSLOT,
            Operand = [(byte)_context.StaticFieldCount]
        });
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

        if (_initSlot)
        {
            int parameterSlotCount = Symbol.Parameters.Length + (NeedInstanceConstructor(Symbol) ? 1 : 0);
            byte pc = RequireByteSizedSlotCount(Symbol, parameterSlotCount, "parameters");
            byte lc = RequireByteSizedSlotCount(Symbol, _localsCount, "local slots");
            if (pc > 0 || lc > 0)
            {
                _instructions.Insert(0, new Instruction
                {
                    OpCode = OpCode.INITSLOT,
                    Operand = [lc, pc]
                });
            }
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
