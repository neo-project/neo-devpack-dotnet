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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neo.Compiler
{
    partial class MethodConvert
    {

        #region Fields

        private readonly CompilationContext context;
        private CallingConvention _callingConvention = CallingConvention.Cdecl;
        private bool _inline;
        private bool _internalInline;
        private bool _initslot;
        private readonly Dictionary<IParameterSymbol, byte> _parameters = new(SymbolEqualityComparer.Default);
        private readonly List<(ILocalSymbol, byte)> _variableSymbols = new();
        private readonly Dictionary<ILocalSymbol, byte> _localVariables = new(SymbolEqualityComparer.Default);
        private readonly List<byte> _anonymousVariables = new();
        private int _localsCount;
        private readonly Stack<List<ILocalSymbol>> _blockSymbols = new();
        private readonly List<Instruction> _instructions = new();
        private readonly JumpTarget _startTarget = new();
        private readonly Dictionary<ILabelSymbol, JumpTarget> _labels = new(SymbolEqualityComparer.Default);
        private readonly Stack<JumpTarget> _continueTargets = new();
        private readonly Stack<JumpTarget> _breakTargets = new();
        private readonly JumpTarget _returnTarget = new();
        private readonly Stack<ExceptionHandling> _tryStack = new();
        private readonly Stack<byte> _exceptionStack = new();
        private readonly Stack<(SwitchLabelSyntax, JumpTarget)[]> _switchStack = new();
        private readonly Stack<bool> _checkedStack = new();

        #endregion

        #region Properties

        public IMethodSymbol Symbol { get; }
        public SyntaxNode? SyntaxNode { get; private set; }
        public IReadOnlyList<Instruction> Instructions => _instructions;
        public IReadOnlyList<(ILocalSymbol Symbol, byte SlotIndex)> Variables => _variableSymbols;
        public bool IsEmpty => _instructions.Count == 0
            || (_instructions.Count == 1 && _instructions[^1].OpCode == OpCode.RET)
            || (_instructions.Count == 2 && _instructions[^1].OpCode == OpCode.RET && _instructions[0].OpCode == OpCode.INITSLOT);

        #endregion

        #region Constructors

        public MethodConvert(CompilationContext context, IMethodSymbol symbol)
        {
            this.Symbol = symbol;
            this.context = context;
            this._checkedStack.Push(context.Options.Checked);
        }

        #endregion

        #region Variables

        private byte AddLocalVariable(ILocalSymbol symbol)
        {
            byte index = (byte)(_localVariables.Count + _anonymousVariables.Count);
            _variableSymbols.Add((symbol, index));
            _localVariables.Add(symbol, index);
            if (_localsCount < index + 1)
                _localsCount = index + 1;
            _blockSymbols.Peek().Add(symbol);
            return index;
        }

        private byte AddAnonymousVariable()
        {
            byte index = (byte)(_localVariables.Count + _anonymousVariables.Count);
            _anonymousVariables.Add(index);
            if (_localsCount < index + 1)
                _localsCount = index + 1;
            return index;
        }

        private void RemoveAnonymousVariable(byte index)
        {
            if (!context.Options.NoOptimize)
                _anonymousVariables.Remove(index);
        }

        private void RemoveLocalVariable(ILocalSymbol symbol)
        {
            if (!context.Options.NoOptimize)
                _localVariables.Remove(symbol);
        }

        #endregion

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

        private SequencePointInserter InsertSequencePoint(SyntaxNodeOrToken syntax)
        {
            return new SequencePointInserter(_instructions, syntax);
        }
        #endregion

        #region Convert
        public void Convert(SemanticModel model)
        {
            if (Symbol.IsExtern || Symbol.ContainingType.DeclaringSyntaxReferences.IsEmpty)
            {
                if (Symbol.Name == "_initialize")
                {
                    ProcessStaticFields(model);
                    if (context.StaticFieldCount > 0)
                    {
                        _instructions.Insert(0, new Instruction
                        {
                            OpCode = OpCode.INITSSLOT,
                            Operand = new[] { (byte)context.StaticFieldCount }
                        });
                    }
                }
                else
                {
                    ConvertExtern();
                }
            }
            else
            {
                if (!Symbol.DeclaringSyntaxReferences.IsEmpty)
                    SyntaxNode = Symbol.DeclaringSyntaxReferences[0].GetSyntax();
                switch (Symbol.MethodKind)
                {
                    case MethodKind.Constructor:
                        ProcessFields(model);
                        ProcessConstructorInitializer(model);
                        break;
                    case MethodKind.StaticConstructor:
                        ProcessStaticFields(model);
                        break;
                    default:
                        if (Symbol.Name.StartsWith("_") && !Helper.IsInternalCoreMethod(Symbol))
                            throw new CompilationException(Symbol, DiagnosticId.InvalidMethodName, $"The method name {Symbol.Name} is not valid.");
                        break;
                }
                var modifiers = ConvertModifier(model).ToArray();
                ConvertSource(model);
                if (Symbol.MethodKind == MethodKind.StaticConstructor && context.StaticFieldCount > 0)
                {
                    _instructions.Insert(0, new Instruction
                    {
                        OpCode = OpCode.INITSSLOT,
                        Operand = new[] { (byte)context.StaticFieldCount }
                    });
                }
                if (_initslot)
                {
                    byte pc = (byte)_parameters.Count;
                    byte lc = (byte)_localsCount;
                    if (!Symbol.IsStatic) pc++;
                    if (pc > 0 || lc > 0)
                    {
                        _instructions.Insert(0, new Instruction
                        {
                            OpCode = OpCode.INITSLOT,
                            Operand = new[] { lc, pc }
                        });
                    }
                }
                foreach (var (fieldIndex, attribute) in modifiers)
                {
                    var disposeInstruction = ExitModifier(model, fieldIndex, attribute);
                    if (disposeInstruction is not null && _returnTarget.Instruction is null)
                    {
                        _returnTarget.Instruction = disposeInstruction;
                    }
                }
            }
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
            if (!context.Options.NoOptimize)
                Optimizer.RemoveNops(_instructions);
            _startTarget.Instruction = _instructions[0];
        }

        public void ConvertForward(SemanticModel model, MethodConvert target)
        {
            INamedTypeSymbol type = Symbol.ContainingType;
            CreateObject(model, type, null);
            IMethodSymbol? constructor = type.InstanceConstructors.FirstOrDefault(p => p.Parameters.Length == 0)
                ?? throw new CompilationException(type, DiagnosticId.NoParameterlessConstructor, "The contract class requires a parameterless constructor.");
            Call(model, constructor, true, Array.Empty<ArgumentSyntax>());
            _returnTarget.Instruction = Jump(OpCode.JMP_L, target._startTarget);
            _startTarget.Instruction = _instructions[0];
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
                try
                {
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
                }
                catch (Exception ex) when (ex is not CompilationException)
                {
                    throw new CompilationException(field, DiagnosticId.InvalidInitialValue, $"Invalid initial value: {value} of type: {type}");
                }
                postInitialize?.Invoke();
            }
        }


        private IEnumerable<(byte fieldIndex, AttributeData attribute)> ConvertModifier(SemanticModel model)
        {
            foreach (var attribute in Symbol.GetAttributesWithInherited())
            {
                if (attribute.AttributeClass?.IsSubclassOf(nameof(ModifierAttribute)) != true)
                    continue;

                JumpTarget notNullTarget = new();
                byte fieldIndex = context.AddAnonymousStaticField();
                AccessSlot(OpCode.LDSFLD, fieldIndex);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIFNOT_L, notNullTarget);

                MethodConvert constructor = context.ConvertMethod(model, attribute.AttributeConstructor!);
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
                MethodConvert enterMethod = context.ConvertMethod(model, enterSymbol);
                EmitCall(enterMethod);
                yield return (fieldIndex, attribute);
            }
        }

        private Instruction? ExitModifier(SemanticModel model, byte fieldIndex, AttributeData attribute)
        {
            var exitSymbol = attribute.AttributeClass!.GetAllMembers()
                .OfType<IMethodSymbol>()
                .First(p => p.Name == nameof(ModifierAttribute.Exit) && p.Parameters.Length == 0);
            MethodConvert exitMethod = context.ConvertMethod(model, exitSymbol);
            if (exitMethod.IsEmpty) return null;
            var instruction = AccessSlot(OpCode.LDSFLD, fieldIndex);
            EmitCall(exitMethod);
            return instruction;
        }
        #endregion

        #region Helper
        private Instruction AccessSlot(OpCode opcode, byte index)
        {
            return index >= 7
                ? AddInstruction(new Instruction { OpCode = opcode, Operand = new[] { index } })
                : AddInstruction(opcode - 7 + index);
        }

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
                if (arguments is not null) PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.Cdecl);
                if (syntax.Body != null) ConvertStatement(model, syntax.Body);
            }
            return true;
        }

        private void PrepareArgumentsForMethod(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention = CallingConvention.Cdecl)
        {
            var namedArguments = arguments.OfType<ArgumentSyntax>().Where(p => p.NameColon is not null).Select(p => (Symbol: (IParameterSymbol)model.GetSymbolInfo(p.NameColon!.Name).Symbol!, p.Expression)).ToDictionary(p => p.Symbol, p => p.Expression, (IEqualityComparer<IParameterSymbol>)SymbolEqualityComparer.Default);
            IEnumerable<IParameterSymbol> parameters = symbol.Parameters;
            if (callingConvention == CallingConvention.Cdecl)
                parameters = parameters.Reverse();
            foreach (IParameterSymbol parameter in parameters)
            {
                if (namedArguments.TryGetValue(parameter, out ExpressionSyntax? expression))
                {
                    ConvertExpression(model, expression);
                }
                else if (parameter.IsParams)
                {
                    if (arguments.Count > parameter.Ordinal)
                    {
                        if (arguments.Count == parameter.Ordinal + 1)
                        {
                            expression = arguments[parameter.Ordinal] switch
                            {
                                ArgumentSyntax argument => argument.Expression,
                                ExpressionSyntax exp => exp,
                                _ => throw new CompilationException(arguments[parameter.Ordinal], DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {arguments[parameter.Ordinal]}"),
                            };
                            Conversion conversion = model.ClassifyConversion(expression, parameter.Type);
                            if (conversion.Exists)
                            {
                                ConvertExpression(model, expression);
                                continue;
                            }
                        }
                        for (int i = arguments.Count - 1; i >= parameter.Ordinal; i--)
                        {
                            expression = arguments[i] switch
                            {
                                ArgumentSyntax argument => argument.Expression,
                                ExpressionSyntax exp => exp,
                                _ => throw new CompilationException(arguments[i], DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {arguments[i]}"),
                            };
                            ConvertExpression(model, expression);
                        }
                        Push(arguments.Count - parameter.Ordinal);
                        AddInstruction(OpCode.PACK);
                    }
                    else
                    {
                        AddInstruction(OpCode.NEWARRAY0);
                    }
                }
                else
                {
                    if (arguments.Count > parameter.Ordinal)
                    {
                        switch (arguments[parameter.Ordinal])
                        {
                            case ArgumentSyntax argument:
                                if (argument.NameColon is null)
                                {
                                    ConvertExpression(model, argument.Expression);
                                    continue;
                                }
                                break;
                            case ExpressionSyntax ex:
                                ConvertExpression(model, ex);
                                continue;
                            default:
                                throw new CompilationException(arguments[parameter.Ordinal], DiagnosticId.SyntaxNotSupported, $"Unsupported argument: {arguments[parameter.Ordinal]}");
                        }
                    }
                    Push(parameter.ExplicitDefaultValue);
                }
            }
        }

        private Instruction IsType(VM.Types.StackItemType type)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.ISTYPE,
                Operand = new[] { (byte)type }
            });
        }

        private Instruction ChangeType(VM.Types.StackItemType type)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.CONVERT,
                Operand = new[] { (byte)type }
            });
        }

        private void InitializeFieldForObject(SemanticModel model, IFieldSymbol field, InitializerExpressionSyntax? initializer)
        {
            ExpressionSyntax? expression = null;
            if (initializer is not null)
            {
                foreach (ExpressionSyntax e in initializer.Expressions)
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
                PushDefault(field.Type);
            else
                ConvertExpression(model, expression);
        }

        private void CreateObject(SemanticModel model, ITypeSymbol type, InitializerExpressionSyntax? initializer)
        {
            ISymbol[] members = type.GetAllMembers().Where(p => !p.IsStatic).ToArray();
            IFieldSymbol[] fields = members.OfType<IFieldSymbol>().ToArray();
            if (fields.Length == 0 || type.IsValueType)
            {
                AddInstruction(type.IsValueType ? OpCode.NEWSTRUCT0 : OpCode.NEWARRAY0);
                foreach (IFieldSymbol field in fields)
                {
                    AddInstruction(OpCode.DUP);
                    InitializeFieldForObject(model, field, initializer);
                    AddInstruction(OpCode.APPEND);
                }
            }
            else
            {
                for (int i = fields.Length - 1; i >= 0; i--)
                    InitializeFieldForObject(model, fields[i], initializer);
                Push(fields.Length);
                AddInstruction(OpCode.PACK);
            }
            IMethodSymbol[] virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
            if (virtualMethods.Length > 0)
            {
                byte index = context.AddVTable(type);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.LDSFLD, index);
                AddInstruction(OpCode.APPEND);
            }
        }

        private Instruction Jump(OpCode opcode, JumpTarget target)
        {
            return AddInstruction(new Instruction
            {
                OpCode = opcode,
                Target = target
            });
        }

        private void Throw(SemanticModel model, ExpressionSyntax? exception)
        {
            if (exception is not null)
            {
                ITypeSymbol type = model.GetTypeInfo(exception).Type!;
                if (type.IsSubclassOf(nameof(scfx::Neo.SmartContract.Framework.UncatchableException), includeThisClass: true))
                {
                    AddInstruction(OpCode.ABORT);
                    return;
                }
            }
            switch (exception)
            {
                case ObjectCreationExpressionSyntax expression:
                    switch (expression.ArgumentList?.Arguments.Count)
                    {
                        case null:
                        case 0:
                            Push("exception");
                            break;
                        case 1:
                            ConvertExpression(model, expression.ArgumentList.Arguments[0].Expression);
                            break;
                        default:
                            throw new CompilationException(expression, DiagnosticId.MultiplyThrows, "Only a single parameter is supported for exceptions.");
                    }
                    break;
                case null:
                    AccessSlot(OpCode.LDLOC, _exceptionStack.Peek());
                    break;
                default:
                    ConvertExpression(model, exception);
                    break;
            }
            AddInstruction(OpCode.THROW);
        }
        #endregion
    }

    class MethodConvertCollection : KeyedCollection<IMethodSymbol, MethodConvert>
    {
        protected override IMethodSymbol GetKeyForItem(MethodConvert item) => item.Symbol;
    }
}
