// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using scfx::Neo.SmartContract.Framework.Attributes;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler;

extern alias scfx;

internal partial class MethodConvert
{
    private bool TryProcessInlineMethods(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments)
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

        _internalInline = true;

        using (InsertSequencePoint(syntax))
        {
            if (arguments is not null) PrepareArgumentsForMethod(model, symbol, arguments);
            if (syntax.Body != null) ConvertStatement(model, syntax.Body);
        }
        return true;
    }

    // Helper methods
    private void InsertStaticFieldInitialization()
    {
        if (Context.StaticFieldCount > 0)
        {
            _instructionsBuilder.Insert(0, new Instruction
            {
                OpCode = OpCode.INITSSLOT,
                Operand = [(byte)Context.StaticFieldCount]
            });
        }
    }

    private void InitializeFieldsBasedOnMethodKind(SemanticModel model)
    {
        switch (Symbol.MethodKind)
        {
            case MethodKind.Constructor:
                ProcessFields(model);
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
        if (Symbol.MethodKind == MethodKind.StaticConstructor && Context.StaticFieldCount > 0)
        {
            InsertStaticFieldInitialization();
        }

        // Check if we need to add an INITSLOT instruction
        if (!_initSlot) return;
        byte pc = (byte)_parameters.Count;
        byte lc = (byte)_localsCount;
        if (IsInstanceMethod(Symbol)) pc++;
        // Only add INITSLOT if we have local variables or parameters
        if (pc > 0 || lc > 0)
        {
            // Insert INITSLOT at the beginning of the method
            // lc: number of local variables
            // pc: number of parameters (including 'this' for instance methods)
            _instructionsBuilder.Insert(0, new Instruction
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
            if (Instructions.Count > 0 && Instructions[^1].OpCode == OpCode.NOP && Instructions[^1].SourceLocation is not null)
            {
                Instructions[^1].OpCode = OpCode.RET;
                _returnTarget.Instruction = Instructions[^1];
            }
            else
            {
                _returnTarget.Instruction = _instructionsBuilder.Ret();
            }
        }
        else
        {
            // it comes from modifier clean up
            _instructionsBuilder.Ret();
        }
    }

    private IEnumerable<(byte fieldIndex, AttributeData attribute)> ConvertModifier(SemanticModel model)
    {
        foreach (var attribute in Symbol.GetAttributesWithInherited())
        {
            if (attribute.AttributeClass?.IsSubclassOf(nameof(ModifierAttribute)) != true)
                continue;

            JumpTarget notNullTarget = new();
            byte fieldIndex = Context.AddAnonymousStaticField();
            _instructionsBuilder.LdSFld(fieldIndex);
            _instructionsBuilder.IsNull();
            _instructionsBuilder.JmpIfNotL(notNullTarget);

            MethodConvert constructor = Context.ConvertMethod(model, attribute.AttributeConstructor!);
            CreateObject(model, attribute.AttributeClass, null);
            foreach (var arg in attribute.ConstructorArguments.Reverse())
                _instructionsBuilder.Push(arg.Value);
            _instructionsBuilder.Push(attribute.ConstructorArguments.Length);
            _instructionsBuilder.Pick();
            EmitCall(constructor);
            _instructionsBuilder.StSFld(fieldIndex);

            notNullTarget.Instruction = _instructionsBuilder.LdSFld(fieldIndex);
            var enterSymbol = attribute.AttributeClass.GetAllMembers()
                .OfType<IMethodSymbol>()
                .First(p => p.Name == nameof(ModifierAttribute.Enter) && p.Parameters.Length == 0);
            MethodConvert enterMethod = Context.ConvertMethod(model, enterSymbol);
            EmitCall(enterMethod);
            yield return (fieldIndex, attribute);
        }
    }

    private Instruction? ExitModifier(SemanticModel model, byte fieldIndex, AttributeData attribute)
    {
        var exitSymbol = attribute.AttributeClass!.GetAllMembers()
            .OfType<IMethodSymbol>()
            .First(p => p is { Name: nameof(ModifierAttribute.Exit), Parameters.Length: 0 });
        MethodConvert exitMethod = Context.ConvertMethod(model, exitSymbol);
        if (exitMethod.IsEmpty) return null;
        var instruction = _instructionsBuilder.LdSFld(fieldIndex);
        EmitCall(exitMethod);
        return instruction;
    }

    private void InitializeFieldForObject(SemanticModel model, IFieldSymbol field, InitializerExpressionSyntax? initializer)
    {
        ExpressionSyntax? expression = null;
        if (initializer is not null)
        {
            foreach (var e in initializer.Expressions)
            {
                if (e is not AssignmentExpressionSyntax ae)
                    throw new CompilationException(initializer, DiagnosticId.SyntaxNotSupported, $"Unsupported initializer: {initializer}");
                if (SymbolEqualityComparer.Default.Equals(field, model.GetSymbolInfo(ae.Left).Symbol))
                {
                    expression = ae.Right;
                    break;
                }
            }
        }
        if (expression is null)
            _instructionsBuilder.PushDefault(field.Type);
        else
            ConvertExpression(model, expression);
    }

    private void CreateObject(SemanticModel model, ITypeSymbol type, InitializerExpressionSyntax? initializer)
    {
        var members = type.GetAllMembers().Where(p => !p.IsStatic).ToArray();
        var fields = members.OfType<IFieldSymbol>().ToArray();
        if (fields.Length == 0 || type.IsValueType || type.IsRecord)
        {
            _instructionsBuilder.AddInstruction(type.IsValueType || type.IsRecord ? OpCode.NEWSTRUCT0 : OpCode.NEWARRAY0);
            foreach (var field in fields)
            {
                _instructionsBuilder.Dup();
                InitializeFieldForObject(model, field, initializer);
                _instructionsBuilder.Append();
            }
        }
        else
        {
            for (int i = fields.Length - 1; i >= 0; i--)
                InitializeFieldForObject(model, fields[i], initializer);
            _instructionsBuilder.Pack(fields.Length);
        }
        var virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
        if (type.IsRecord || virtualMethods.Length <= 0) return;
        var index = Context.AddVTable(type);
        _instructionsBuilder.Dup();
        _instructionsBuilder.LdSFld(index);
        _instructionsBuilder.Append();
    }
}
