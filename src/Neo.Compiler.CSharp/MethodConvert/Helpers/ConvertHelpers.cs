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
using Microsoft.CodeAnalysis;
using scfx::Neo.SmartContract.Framework.Attributes;
using Neo.VM;

namespace Neo.Compiler;

extern alias scfx;

partial class MethodConvert
{

    #region Instructions
    private Instruction AddInstruction(Instruction instruction)
    {
        _instructions.Add(instruction);
        return instruction;
    }

    private Instruction AddInstruction(OpCode opcode)
    {
        return AddInstruction(new Instruction
        {
            OpCode = opcode
        });
    }

    private SequencePointInserter InsertSequencePoint(SyntaxNodeOrToken? syntax)
    {
        return new SequencePointInserter(_instructions, syntax);
    }

    private SequencePointInserter InsertSequencePoint(SyntaxReference? syntax)
    {
        return new SequencePointInserter(_instructions, syntax);
    }

    private SequencePointInserter InsertSequencePoint(Location? location)
    {
        return new SequencePointInserter(_instructions, location);
    }

    #endregion

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
        if (Symbol.MethodKind == MethodKind.StaticConstructor && _context.StaticFieldCount > 0)
        {
            InsertStaticFieldInitialization();
        }

        // Check if we need to add an INITSLOT instruction
        if (_initSlot)
        {
            byte pc = (byte)_parameters.Count;
            byte lc = (byte)_localsCount;
            if (IsInstanceMethod(Symbol)) pc++;
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
            if (_instructions.Count > 0 && _instructions[^1].OpCode == OpCode.NOP && _instructions[^1].SourceLocation is not null)
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
            CreateObject(model, attribute.AttributeClass, null);
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
}
