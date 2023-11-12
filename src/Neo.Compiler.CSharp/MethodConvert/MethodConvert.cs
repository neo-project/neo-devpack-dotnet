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
using Neo.Cryptography;
using Neo.IO;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private readonly CompilationContext context;
        private CallingConvention _callingConvention = CallingConvention.Cdecl;
        private bool _inline;
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

        public IMethodSymbol Symbol { get; }
        public SyntaxNode? SyntaxNode { get; private set; }
        public IReadOnlyList<Instruction> Instructions => _instructions;
        public IReadOnlyList<(ILocalSymbol Symbol, byte SlotIndex)> Variables => _variableSymbols;
        public bool IsEmpty => _instructions.Count == 0
            || (_instructions.Count == 1 && _instructions[^1].OpCode == OpCode.RET)
            || (_instructions.Count == 2 && _instructions[^1].OpCode == OpCode.RET && _instructions[0].OpCode == OpCode.INITSLOT);

        public MethodConvert(CompilationContext context, IMethodSymbol symbol)
        {
            this.Symbol = symbol;
            this.context = context;
            this._checkedStack.Push(context.Options.Checked);
        }

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
            IMethodSymbol? constructor = type.InstanceConstructors.FirstOrDefault(p => p.Parameters.Length == 0);
            if (constructor is null)
                throw new CompilationException(type, DiagnosticId.NoParameterlessConstructor, "The contract class requires a parameterless constructor.");
            Call(model, constructor, true, Array.Empty<ArgumentSyntax>());
            _returnTarget.Instruction = Jump(OpCode.JMP_L, target._startTarget);
            _startTarget.Instruction = _instructions[0];
        }

        private void ConvertExtern()
        {
            _inline = true;
            AttributeData? contractAttribute = Symbol.ContainingType.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(ContractAttribute));
            if (contractAttribute is null)
            {
                bool emitted = false;
                foreach (AttributeData attribute in Symbol.GetAttributes())
                {
                    switch (attribute.AttributeClass!.Name)
                    {
                        case nameof(OpCodeAttribute):
                            if (!emitted)
                            {
                                emitted = true;
                                _callingConvention = CallingConvention.StdCall;
                            }
                            AddInstruction(new Instruction
                            {
                                OpCode = (OpCode)attribute.ConstructorArguments[0].Value!,
                                Operand = ((string)attribute.ConstructorArguments[1].Value!).HexToBytes(true)
                            });
                            break;
                        case nameof(SyscallAttribute):
                            if (!emitted)
                            {
                                emitted = true;
                                _callingConvention = CallingConvention.Cdecl;
                            }
                            AddInstruction(new Instruction
                            {
                                OpCode = OpCode.SYSCALL,
                                Operand = Encoding.ASCII.GetBytes((string)attribute.ConstructorArguments[0].Value!).Sha256()[..4]
                            });
                            break;
                        case nameof(CallingConventionAttribute):
                            emitted = true;
                            _callingConvention = (CallingConvention)attribute.ConstructorArguments[0].Value!;
                            break;
                    }
                }
                if (!emitted) throw new CompilationException(Symbol, DiagnosticId.ExternMethod, $"Unknown method: {Symbol}");
            }
            else
            {
                UInt160 hash = UInt160.Parse((string)contractAttribute.ConstructorArguments[0].Value!);
                if (Symbol.MethodKind == MethodKind.PropertyGet)
                {
                    AttributeData? attribute = Symbol.AssociatedSymbol!.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(ContractHashAttribute));
                    if (attribute is not null)
                    {
                        Push(hash.ToArray());
                        return;
                    }
                }
                string method = Symbol.GetDisplayName(true);
                ushort parametersCount = (ushort)Symbol.Parameters.Length;
                bool hasReturnValue = !Symbol.ReturnsVoid || Symbol.MethodKind == MethodKind.Constructor;
                Call(hash, method, parametersCount, hasReturnValue);
            }
        }

        private void ConvertNoBody(AccessorDeclarationSyntax syntax)
        {
            _inline = true;
            _callingConvention = CallingConvention.Cdecl;
            IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
            INamedTypeSymbol type = property.ContainingType;
            IFieldSymbol[] fields = type.GetAllMembers().OfType<IFieldSymbol>().ToArray();
            using (InsertSequencePoint(syntax))
            {
                if (Symbol.IsStatic)
                {
                    IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
                    byte backingFieldIndex = context.AddStaticField(backingField);
                    switch (Symbol.MethodKind)
                    {
                        case MethodKind.PropertyGet:
                            AccessSlot(OpCode.LDSFLD, backingFieldIndex);
                            break;
                        case MethodKind.PropertySet:
                            AccessSlot(OpCode.STSFLD, backingFieldIndex);
                            break;
                        default:
                            throw new CompilationException(syntax, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {syntax}");
                    }
                }
                else
                {
                    fields = fields.Where(p => !p.IsStatic).ToArray();
                    int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                    switch (Symbol.MethodKind)
                    {
                        case MethodKind.PropertyGet:
                            Push(backingFieldIndex);
                            AddInstruction(OpCode.PICKITEM);
                            break;
                        case MethodKind.PropertySet:
                            Push(backingFieldIndex);
                            AddInstruction(OpCode.ROT);
                            AddInstruction(OpCode.SETITEM);
                            break;
                        default:
                            throw new CompilationException(syntax, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {syntax}");
                    }
                }
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

        private void ConvertSource(SemanticModel model)
        {
            if (SyntaxNode is null) return;
            for (byte i = 0; i < Symbol.Parameters.Length; i++)
            {
                IParameterSymbol parameter = Symbol.Parameters[i].OriginalDefinition;
                byte index = i;
                if (!Symbol.IsStatic) index++;
                _parameters.Add(parameter, index);
            }
            switch (SyntaxNode)
            {
                case AccessorDeclarationSyntax syntax:
                    if (syntax.Body is not null)
                        ConvertStatement(model, syntax.Body);
                    else if (syntax.ExpressionBody is not null)
                        using (InsertSequencePoint(syntax.ExpressionBody.Expression))
                            ConvertExpression(model, syntax.ExpressionBody.Expression);
                    else
                        ConvertNoBody(syntax);
                    break;
                case ArrowExpressionClauseSyntax syntax:
                    using (InsertSequencePoint(syntax))
                        ConvertExpression(model, syntax.Expression);
                    break;
                case BaseMethodDeclarationSyntax syntax:
                    if (syntax.Body is null)
                        using (InsertSequencePoint(syntax.ExpressionBody!.Expression))
                            ConvertExpression(model, syntax.ExpressionBody.Expression);
                    else
                        ConvertStatement(model, syntax.Body);
                    break;
                default:
                    throw new CompilationException(SyntaxNode, DiagnosticId.SyntaxNotSupported, $"Unsupported method body:{SyntaxNode}");
            }
            _initslot = !_inline;
        }
        private void ConvertStatement(SemanticModel model, StatementSyntax statement)
        {
            switch (statement)
            {
                case BlockSyntax syntax:
                    ConvertBlockStatement(model, syntax);
                    break;
                case BreakStatementSyntax syntax:
                    ConvertBreakStatement(syntax);
                    break;
                case CheckedStatementSyntax syntax:
                    ConvertCheckedStatement(model, syntax);
                    break;
                case ContinueStatementSyntax syntax:
                    ConvertContinueStatement(syntax);
                    break;
                case DoStatementSyntax syntax:
                    ConvertDoStatement(model, syntax);
                    break;
                case EmptyStatementSyntax syntax:
                    ConvertEmptyStatement(syntax);
                    break;
                case ExpressionStatementSyntax syntax:
                    ConvertExpressionStatement(model, syntax);
                    break;
                case ForEachStatementSyntax syntax:
                    ConvertForEachStatement(model, syntax);
                    break;
                case ForEachVariableStatementSyntax syntax:
                    ConvertForEachVariableStatement(model, syntax);
                    break;
                case ForStatementSyntax syntax:
                    ConvertForStatement(model, syntax);
                    break;
                case GotoStatementSyntax syntax:
                    ConvertGotoStatement(model, syntax);
                    break;
                case IfStatementSyntax syntax:
                    ConvertIfStatement(model, syntax);
                    break;
                case LabeledStatementSyntax syntax:
                    ConvertLabeledStatement(model, syntax);
                    break;
                case LocalDeclarationStatementSyntax syntax:
                    ConvertLocalDeclarationStatement(model, syntax);
                    break;
                case LocalFunctionStatementSyntax:
                    break;
                case ReturnStatementSyntax syntax:
                    ConvertReturnStatement(model, syntax);
                    break;
                case SwitchStatementSyntax syntax:
                    ConvertSwitchStatement(model, syntax);
                    break;
                case ThrowStatementSyntax syntax:
                    ConvertThrowStatement(model, syntax);
                    break;
                case TryStatementSyntax syntax:
                    ConvertTryStatement(model, syntax);
                    break;
                case WhileStatementSyntax syntax:
                    ConvertWhileStatement(model, syntax);
                    break;
                default:
                    throw new CompilationException(statement, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {statement}");
            }
        }

        private void ConvertExpression(SemanticModel model, ExpressionSyntax syntax)
        {
            Optional<object?> constant = model.GetConstantValue(syntax);
            if (constant.HasValue)
            {
                Push(constant.Value);
                return;
            }
            switch (syntax)
            {
                case AnonymousObjectCreationExpressionSyntax expression:
                    ConvertAnonymousObjectCreationExpression(model, expression);
                    break;
                case ArrayCreationExpressionSyntax expression:
                    ConvertArrayCreationExpression(model, expression);
                    break;
                case AssignmentExpressionSyntax expression:
                    ConvertAssignmentExpression(model, expression);
                    break;
                case BaseObjectCreationExpressionSyntax expression:
                    ConvertObjectCreationExpression(model, expression);
                    break;
                case BinaryExpressionSyntax expression:
                    ConvertBinaryExpression(model, expression);
                    break;
                case CastExpressionSyntax expression:
                    ConvertCastExpression(model, expression);
                    break;
                case CheckedExpressionSyntax expression:
                    ConvertCheckedExpression(model, expression);
                    break;
                case ConditionalAccessExpressionSyntax expression:
                    ConvertConditionalAccessExpression(model, expression);
                    break;
                case ConditionalExpressionSyntax expression:
                    ConvertConditionalExpression(model, expression);
                    break;
                case ElementAccessExpressionSyntax expression:
                    ConvertElementAccessExpression(model, expression);
                    break;
                case ElementBindingExpressionSyntax expression:
                    ConvertElementBindingExpression(model, expression);
                    break;
                case IdentifierNameSyntax expression:
                    ConvertIdentifierNameExpression(model, expression);
                    break;
                case ImplicitArrayCreationExpressionSyntax expression:
                    ConvertImplicitArrayCreationExpression(model, expression);
                    break;
                case InitializerExpressionSyntax expression:
                    ConvertInitializerExpression(model, expression);
                    break;
                case InterpolatedStringExpressionSyntax expression:
                    ConvertInterpolatedStringExpression(model, expression);
                    break;
                case InvocationExpressionSyntax expression:
                    ConvertInvocationExpression(model, expression);
                    break;
                case IsPatternExpressionSyntax expression:
                    ConvertIsPatternExpression(model, expression);
                    break;
                case MemberAccessExpressionSyntax expression:
                    ConvertMemberAccessExpression(model, expression);
                    break;
                case MemberBindingExpressionSyntax expression:
                    ConvertMemberBindingExpression(model, expression);
                    break;
                case ParenthesizedExpressionSyntax expression:
                    ConvertExpression(model, expression.Expression);
                    break;
                case PostfixUnaryExpressionSyntax expression:
                    ConvertPostfixUnaryExpression(model, expression);
                    break;
                case PrefixUnaryExpressionSyntax expression:
                    ConvertPrefixUnaryExpression(model, expression);
                    break;
                case SwitchExpressionSyntax expression:
                    ConvertSwitchExpression(model, expression);
                    break;
                case BaseExpressionSyntax:
                case ThisExpressionSyntax:
                    AddInstruction(OpCode.LDARG0);
                    break;
                case ThrowExpressionSyntax expression:
                    Throw(model, expression.Expression);
                    break;
                case TupleExpressionSyntax expression:
                    ConvertTupleExpression(model, expression);
                    break;
                default:
                    throw new CompilationException(syntax, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {syntax}");
            }
        }


        private void EnsureIntegerInRange(ITypeSymbol type)
        {
            if (type.Name == "BigInteger") return;
            var (minValue, maxValue, mask) = type.Name switch
            {
                "SByte" => ((BigInteger)sbyte.MinValue, (BigInteger)sbyte.MaxValue, (BigInteger)0xff),
                "Int16" => (short.MinValue, short.MaxValue, 0xffff),
                "Int32" => (int.MinValue, int.MaxValue, 0xffffffff),
                "Int64" => (long.MinValue, long.MaxValue, 0xffffffffffffffff),
                "Byte" => (byte.MinValue, byte.MaxValue, 0xff),
                "UInt16" => (ushort.MinValue, ushort.MaxValue, 0xffff),
                "UInt32" => (uint.MinValue, uint.MaxValue, 0xffffffff),
                "UInt64" => (ulong.MinValue, ulong.MaxValue, 0xffffffffffffffff),
                _ => throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Unsupported type: {type}")
            };
            JumpTarget checkUpperBoundTarget = new(), adjustTarget = new(), endTarget = new();
            AddInstruction(OpCode.DUP);
            Push(minValue);
            Jump(OpCode.JMPGE_L, checkUpperBoundTarget);
            if (_checkedStack.Peek())
                AddInstruction(OpCode.THROW);
            else
                Jump(OpCode.JMP_L, adjustTarget);
            checkUpperBoundTarget.Instruction = AddInstruction(OpCode.DUP);
            Push(maxValue);
            Jump(OpCode.JMPLE_L, endTarget);
            if (_checkedStack.Peek())
            {
                AddInstruction(OpCode.THROW);
            }
            else
            {
                adjustTarget.Instruction = Push(mask);
                AddInstruction(OpCode.AND);
                if (minValue < 0)
                {
                    AddInstruction(OpCode.DUP);
                    Push(maxValue);
                    Jump(OpCode.JMPLE_L, endTarget);
                    Push(mask + 1);
                    AddInstruction(OpCode.SUB);
                }
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }


        private void ConvertObjectToString(SemanticModel model, ExpressionSyntax expression)
        {
            ITypeSymbol? type = model.GetTypeInfo(expression).Type;
            switch (type?.ToString())
            {
                case "sbyte":
                case "byte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                case "System.Numerics.BigInteger":
                    ConvertExpression(model, expression);
                    Call(NativeContract.StdLib.Hash, "itoa", 1, true);
                    break;
                case "string":
                case "Neo.Cryptography.ECC.ECPoint":
                case "Neo.SmartContract.Framework.ByteString":
                case "Neo.SmartContract.Framework.UInt160":
                case "Neo.SmartContract.Framework.UInt256":
                    ConvertExpression(model, expression);
                    break;
                default:
                    throw new CompilationException(expression, DiagnosticId.InvalidToStringType, $"Unsupported interpolation: {expression}");
            }
        }


        private void ConvertPattern(SemanticModel model, PatternSyntax pattern, byte localIndex)
        {
            switch (pattern)
            {
                case BinaryPatternSyntax binaryPattern:
                    ConvertBinaryPattern(model, binaryPattern, localIndex);
                    break;
                case ConstantPatternSyntax constantPattern:
                    ConvertConstantPattern(model, constantPattern, localIndex);
                    break;
                case DeclarationPatternSyntax declarationPattern:
                    ConvertDeclarationPattern(model, declarationPattern, localIndex);
                    break;
                case DiscardPatternSyntax:
                    Push(true);
                    break;
                case RelationalPatternSyntax relationalPattern:
                    ConvertRelationalPattern(model, relationalPattern, localIndex);
                    break;
                case TypePatternSyntax typePattern:
                    ConvertTypePattern(model, typePattern, localIndex);
                    break;
                case UnaryPatternSyntax unaryPattern when unaryPattern.OperatorToken.ValueText == "not":
                    ConvertNotPattern(model, unaryPattern, localIndex);
                    break;
                default:
                    throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
            }
        }


        private void Push(bool value)
        {
            AddInstruction(value ? OpCode.PUSH1 : OpCode.PUSH0);
            ChangeType(VM.Types.StackItemType.Boolean);
        }

        private Instruction Push(BigInteger number)
        {
            if (number == BigInteger.MinusOne) return AddInstruction(OpCode.PUSHM1);
            if (number >= BigInteger.Zero && number <= 16) return AddInstruction(OpCode.PUSH0 + (byte)number);
            byte n = number.GetByteCount() switch
            {
                <= 1 => 0,
                <= 2 => 1,
                <= 4 => 2,
                <= 8 => 3,
                <= 16 => 4,
                <= 32 => 5,
                _ => throw new ArgumentOutOfRangeException(nameof(number))
            };
            byte[] buffer = new byte[1 << n];
            number.TryWriteBytes(buffer, out _);
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHINT8 + n,
                Operand = buffer
            });
        }

        private Instruction Push(string s)
        {
            return Push(Utility.StrictUTF8.GetBytes(s));
        }

        private Instruction Push(byte[] data)
        {
            OpCode opcode;
            byte[] buffer;
            switch (data.Length)
            {
                case <= byte.MaxValue:
                    opcode = OpCode.PUSHDATA1;
                    buffer = new byte[sizeof(byte) + data.Length];
                    buffer[0] = (byte)data.Length;
                    Buffer.BlockCopy(data, 0, buffer, sizeof(byte), data.Length);
                    break;
                case <= ushort.MaxValue:
                    opcode = OpCode.PUSHDATA2;
                    buffer = new byte[sizeof(ushort) + data.Length];
                    BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)data.Length);
                    Buffer.BlockCopy(data, 0, buffer, sizeof(ushort), data.Length);
                    break;
                default:
                    opcode = OpCode.PUSHDATA4;
                    buffer = new byte[sizeof(uint) + data.Length];
                    BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)data.Length);
                    Buffer.BlockCopy(data, 0, buffer, sizeof(uint), data.Length);
                    break;
            }
            return AddInstruction(new Instruction
            {
                OpCode = opcode,
                Operand = buffer
            });
        }

        private void Push(object? obj)
        {
            switch (obj)
            {
                case bool data:
                    Push(data);
                    break;
                case byte[] data:
                    Push(data);
                    break;
                case string data:
                    Push(data);
                    break;
                case BigInteger data:
                    Push(data);
                    break;
                case char data:
                    Push(data);
                    break;
                case sbyte data:
                    Push(data);
                    break;
                case byte data:
                    Push(data);
                    break;
                case short data:
                    Push(data);
                    break;
                case ushort data:
                    Push(data);
                    break;
                case int data:
                    Push(data);
                    break;
                case uint data:
                    Push(data);
                    break;
                case long data:
                    Push(data);
                    break;
                case ulong data:
                    Push(data);
                    break;
                case Enum data:
                    Push(BigInteger.Parse(data.ToString("d")));
                    break;
                case null:
                    AddInstruction(OpCode.PUSHNULL);
                    break;
                case float or double or decimal:
                    throw new CompilationException(DiagnosticId.FloatingPointNumber, "Floating-point numbers are not supported.");
                default:
                    throw new NotSupportedException($"Unsupported constant value: {obj}");
            }
        }

        private Instruction PushDefault(ITypeSymbol type)
        {
            return AddInstruction(type.GetStackItemType() switch
            {
                VM.Types.StackItemType.Boolean or VM.Types.StackItemType.Integer => OpCode.PUSH0,
                _ => OpCode.PUSHNULL,
            });
        }

        private JumpTarget AddLabel(ILabelSymbol symbol, bool checkTryStack)
        {
            if (!_labels.TryGetValue(symbol, out JumpTarget? target))
            {
                target = new JumpTarget();
                _labels.Add(symbol, target);
            }
            if (checkTryStack && _tryStack.TryPeek(out ExceptionHandling? result) && result.State != ExceptionHandlingState.Finally)
            {
                result.Labels.Add(symbol);
            }
            return target;
        }

        private void PushSwitchLabels((SwitchLabelSyntax, JumpTarget)[] labels)
        {
            _switchStack.Push(labels);
            if (_tryStack.TryPeek(out ExceptionHandling? result))
                result.SwitchCount++;
        }

        private void PopSwitchLabels()
        {
            _switchStack.Pop();
            if (_tryStack.TryPeek(out ExceptionHandling? result))
                result.SwitchCount--;
        }

        private void PushContinueTarget(JumpTarget target)
        {
            _continueTargets.Push(target);
            if (_tryStack.TryPeek(out ExceptionHandling? result))
                result.ContinueTargetCount++;
        }

        private void PopContinueTarget()
        {
            _continueTargets.Pop();
            if (_tryStack.TryPeek(out ExceptionHandling? result))
                result.ContinueTargetCount--;
        }

        private void PushBreakTarget(JumpTarget target)
        {
            _breakTargets.Push(target);
            if (_tryStack.TryPeek(out ExceptionHandling? result))
                result.BreakTargetCount++;
        }

        private void PopBreakTarget()
        {
            _breakTargets.Pop();
            if (_tryStack.TryPeek(out ExceptionHandling? result))
                result.BreakTargetCount--;
        }

        private Instruction AccessSlot(OpCode opcode, byte index)
        {
            return index >= 7
                ? AddInstruction(new Instruction { OpCode = opcode, Operand = new[] { index } })
                : AddInstruction(opcode - 7 + index);
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

        private Instruction Call(InteropDescriptor descriptor)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.SYSCALL,
                Operand = BitConverter.GetBytes(descriptor)
            });
        }

        private Instruction Call(UInt160 hash, string method, ushort parametersCount, bool hasReturnValue, CallFlags callFlags = CallFlags.All)
        {
            ushort token = context.AddMethodToken(hash, method, parametersCount, hasReturnValue, callFlags);
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.CALLT,
                Operand = BitConverter.GetBytes(token)
            });
        }

        private void Call(SemanticModel model, IMethodSymbol symbol, bool instanceOnStack, IReadOnlyList<ArgumentSyntax> arguments)
        {
            if (TryProcessSystemMethods(model, symbol, null, arguments))
                return;
            MethodConvert? convert;
            CallingConvention methodCallingConvention;
            if (symbol.IsVirtualMethod())
            {
                convert = null;
                methodCallingConvention = CallingConvention.Cdecl;
            }
            else
            {
                convert = context.ConvertMethod(model, symbol);
                methodCallingConvention = convert._callingConvention;
            }
            bool isConstructor = symbol.MethodKind == MethodKind.Constructor;
            if (instanceOnStack && methodCallingConvention != CallingConvention.Cdecl && isConstructor)
                AddInstruction(OpCode.DUP);
            PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);
            if (instanceOnStack && methodCallingConvention == CallingConvention.Cdecl)
            {
                switch (symbol.Parameters.Length)
                {
                    case 0:
                        if (isConstructor) AddInstruction(OpCode.DUP);
                        break;
                    case 1:
                        AddInstruction(isConstructor ? OpCode.OVER : OpCode.SWAP);
                        break;
                    default:
                        Push(symbol.Parameters.Length);
                        AddInstruction(isConstructor ? OpCode.PICK : OpCode.ROLL);
                        break;
                }
            }
            if (convert is null)
                CallVirtual(symbol);
            else
                EmitCall(convert);
        }

        private void Call(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, params SyntaxNode[] arguments)
        {
            if (TryProcessSystemMethods(model, symbol, instanceExpression, arguments))
                return;
            MethodConvert? convert;
            CallingConvention methodCallingConvention;
            if (symbol.IsVirtualMethod() && instanceExpression is not BaseExpressionSyntax)
            {
                convert = null;
                methodCallingConvention = CallingConvention.Cdecl;
            }
            else
            {
                convert = symbol.ReducedFrom is null
                    ? context.ConvertMethod(model, symbol)
                    : context.ConvertMethod(model, symbol.ReducedFrom);
                methodCallingConvention = convert._callingConvention;
            }
            if (!symbol.IsStatic && methodCallingConvention != CallingConvention.Cdecl)
            {
                if (instanceExpression is null)
                    AddInstruction(OpCode.LDARG0);
                else
                    ConvertExpression(model, instanceExpression);
            }
            PrepareArgumentsForMethod(model, symbol, arguments, methodCallingConvention);
            if (!symbol.IsStatic && methodCallingConvention == CallingConvention.Cdecl)
            {
                if (instanceExpression is null)
                    AddInstruction(OpCode.LDARG0);
                else
                    ConvertExpression(model, instanceExpression);
            }
            if (convert is null)
                CallVirtual(symbol);
            else
                EmitCall(convert);
        }

        private void Call(SemanticModel model, IMethodSymbol symbol, CallingConvention callingConvention = CallingConvention.Cdecl)
        {
            if (TryProcessSystemMethods(model, symbol, null, null))
                return;
            MethodConvert? convert;
            CallingConvention methodCallingConvention;
            if (symbol.IsVirtualMethod())
            {
                convert = null;
                methodCallingConvention = CallingConvention.Cdecl;
            }
            else
            {
                convert = context.ConvertMethod(model, symbol);
                methodCallingConvention = convert._callingConvention;
            }
            int pc = symbol.Parameters.Length;
            if (!symbol.IsStatic) pc++;
            if (pc > 1 && methodCallingConvention != callingConvention)
            {
                switch (pc)
                {
                    case 2:
                        AddInstruction(OpCode.SWAP);
                        break;
                    case 3:
                        AddInstruction(OpCode.REVERSE3);
                        break;
                    case 4:
                        AddInstruction(OpCode.REVERSE4);
                        break;
                    default:
                        Push(pc);
                        AddInstruction(OpCode.REVERSEN);
                        break;
                }
            }
            if (convert is null)
                CallVirtual(symbol);
            else
                EmitCall(convert);
        }

        private void EmitCall(MethodConvert target)
        {
            if (target._inline && !context.Options.NoInline)
                for (int i = 0; i < target._instructions.Count - 1; i++)
                    AddInstruction(target._instructions[i].Clone());
            else
                Jump(OpCode.CALL_L, target._startTarget);
        }

        private void CallVirtual(IMethodSymbol symbol)
        {
            if (symbol.ContainingType.TypeKind == TypeKind.Interface)
                throw new CompilationException(symbol.ContainingType, DiagnosticId.InterfaceCall, "Interfaces are not supported.");
            ISymbol[] members = symbol.ContainingType.GetAllMembers().Where(p => !p.IsStatic).ToArray();
            IFieldSymbol[] fields = members.OfType<IFieldSymbol>().ToArray();
            IMethodSymbol[] virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
            int index = Array.IndexOf(virtualMethods, symbol);
            AddInstruction(OpCode.DUP);
            Push(fields.Length);
            AddInstruction(OpCode.PICKITEM);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.CALLA);
        }
    }

    class MethodConvertCollection : KeyedCollection<IMethodSymbol, MethodConvert>
    {
        protected override IMethodSymbol GetKeyForItem(MethodConvert item) => item.Symbol;
    }
}
