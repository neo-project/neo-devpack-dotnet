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
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Neo.Compiler
{
    partial class MethodConvert
    {

        #region Fields

        private readonly CompilationContext _context;
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
            this._context = context;
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
            if (!_context.Options.NoOptimize)
                _anonymousVariables.Remove(index);
        }

        private void RemoveLocalVariable(ILocalSymbol symbol)
        {
            if (!_context.Options.NoOptimize)
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
                    if (_context.StaticFieldCount > 0)
                    {
                        _instructions.Insert(0, new Instruction
                        {
                            OpCode = OpCode.INITSSLOT,
                            Operand = new[] { (byte)_context.StaticFieldCount }
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
                if (Symbol.MethodKind == MethodKind.StaticConstructor && _context.StaticFieldCount > 0)
                {
                    _instructions.Insert(0, new Instruction
                    {
                        OpCode = OpCode.INITSSLOT,
                        Operand = new[] { (byte)_context.StaticFieldCount }
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
            if (!_context.Options.NoOptimize)
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

        private void ProcessStaticFields(SemanticModel model)
        {
            foreach (INamedTypeSymbol @class in _context.StaticFieldSymbols.Select(p => p.ContainingType).Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).ToArray())
            {
                foreach (IFieldSymbol field in @class.GetAllMembers().OfType<IFieldSymbol>())
                {
                    if (field.IsConst || !field.IsStatic) continue;
                    ProcessFieldInitializer(model, field, null, () =>
                    {
                        byte index = _context.AddStaticField(field);
                        AccessSlot(OpCode.STSFLD, index);
                    });
                }
            }
            foreach (var (fieldIndex, type) in _context.VTables)
            {
                IMethodSymbol[] virtualMethods = type.GetAllMembers().OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
                for (int i = virtualMethods.Length - 1; i >= 0; i--)
                {
                    IMethodSymbol method = virtualMethods[i];
                    if (method.IsAbstract)
                    {
                        Push((object?)null);
                    }
                    else
                    {
                        MethodConvert convert = _context.ConvertMethod(model, method);
                        Jump(OpCode.PUSHA, convert._startTarget);
                    }
                }
                Push(virtualMethods.Length);
                AddInstruction(OpCode.PACK);
                AccessSlot(OpCode.STSFLD, fieldIndex);
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
                            Push((UInt160.TryParse(value, out var hash) ? hash : value.ToScriptHash(_context.Options.AddressVersion)).ToArray());
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

        private void ProcessConstructorInitializer(SemanticModel model)
        {
            INamedTypeSymbol type = Symbol.ContainingType;
            if (type.IsValueType) return;
            INamedTypeSymbol baseType = type.BaseType!;
            if (baseType.SpecialType == SpecialType.System_Object) return;
            ConstructorInitializerSyntax? initializer = ((ConstructorDeclarationSyntax?)SyntaxNode)?.Initializer;
            if (initializer is null)
            {
                IMethodSymbol baseConstructor = baseType.InstanceConstructors.First(p => p.Parameters.Length == 0);
                if (baseType.DeclaringSyntaxReferences.IsEmpty && baseConstructor.GetAttributes().All(p => p.AttributeClass!.ContainingAssembly.Name != "Neo.SmartContract.Framework"))
                    return;
                Call(model, baseConstructor, null);
            }
            else
            {
                IMethodSymbol baseConstructor = (IMethodSymbol)model.GetSymbolInfo(initializer).Symbol!;
                using (InsertSequencePoint(initializer))
                    Call(model, baseConstructor, null, initializer.ArgumentList.Arguments.ToArray());
            }
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
                if (Symbol.ToString()?.StartsWith("System.Array.Empty") == true)
                {
                    emitted = true;
                    AddInstruction(OpCode.NEWARRAY0);
                }
                else if (Symbol.ToString()?.Equals("Neo.SmartContract.Framework.Services.Runtime.Debug(string)") == true)
                {
                    _context.AddEvent(new AbiEvent(Symbol, "Debug", new SmartContract.Manifest.ContractParameterDefinition() { Name = "message", Type = ContractParameterType.String }), false);
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
            _callingConvention = CallingConvention.Cdecl;
            IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
            AttributeData? attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(StoredAttribute));
            using (InsertSequencePoint(syntax))
            {
                _inline = attribute is null;
                ConvertFieldBackedProperty(property);
                if (attribute is not null)
                    ConvertStorageBackedProperty(property, attribute);
            }
        }

        private void ConvertFieldBackedProperty(IPropertySymbol property)
        {
            IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
            if (Symbol.IsStatic)
            {
                IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
                byte backingFieldIndex = _context.AddStaticField(backingField);
                switch (Symbol.MethodKind)
                {
                    case MethodKind.PropertyGet:
                        AccessSlot(OpCode.LDSFLD, backingFieldIndex);
                        break;
                    case MethodKind.PropertySet:
                        if (!_inline) AccessSlot(OpCode.LDARG, 0);
                        AccessSlot(OpCode.STSFLD, backingFieldIndex);
                        break;
                    default:
                        throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
                }
            }
            else
            {
                fields = fields.Where(p => !p.IsStatic).ToArray();
                int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                switch (Symbol.MethodKind)
                {
                    case MethodKind.PropertyGet:
                        if (!_inline) AccessSlot(OpCode.LDARG, 0);
                        Push(backingFieldIndex);
                        AddInstruction(OpCode.PICKITEM);
                        break;
                    case MethodKind.PropertySet:
                        if (_inline)
                        {
                            Push(backingFieldIndex);
                            AddInstruction(OpCode.ROT);
                        }
                        else
                        {
                            AccessSlot(OpCode.LDARG, 0);
                            Push(backingFieldIndex);
                            AccessSlot(OpCode.LDARG, 1);
                        }
                        AddInstruction(OpCode.SETITEM);
                        break;
                    default:
                        throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
                }
            }
        }

        private byte[] GetStorageBackedKey(IPropertySymbol property, AttributeData attribute)
        {
            byte[] key;

            if (attribute.ConstructorArguments.Length == 0)
            {
                key = Utility.StrictUTF8.GetBytes(property.Name);
            }
            else
            {
                if (attribute.ConstructorArguments[0].Value is byte b)
                {
                    key = new byte[] { b };
                }
                else if (attribute.ConstructorArguments[0].Value is string s)
                {
                    key = Utility.StrictUTF8.GetBytes(s);
                }
                else
                {
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unknown StorageBacked constructor: {Symbol}");
                }
            }
            return key;
        }

        private void ConvertStorageBackedProperty(IPropertySymbol property, AttributeData attribute)
        {
            IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
            byte[] key = GetStorageBackedKey(property, attribute);
            if (Symbol.MethodKind == MethodKind.PropertyGet)
            {
                JumpTarget endTarget = new();
                if (Symbol.IsStatic)
                {
                    // AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    // Ensure that no object was sent
                    Jump(OpCode.JMPIFNOT_L, endTarget);
                }
                else
                {
                    // Check class
                    Jump(OpCode.JMPIF_L, endTarget);
                }
                Push(key);
                Call(ApplicationEngine.System_Storage_GetReadOnlyContext);
                Call(ApplicationEngine.System_Storage_Get);
                switch (property.Type.Name)
                {
                    case "byte":
                    case "sbyte":
                    case "Byte":
                    case "SByte":

                    case "short":
                    case "ushort":
                    case "Int16":
                    case "UInt16":

                    case "int":
                    case "uint":
                    case "Int32":
                    case "UInt32":

                    case "long":
                    case "ulong":
                    case "Int64":
                    case "UInt64":
                    case "BigInteger":
                        // Replace NULL with 0
                        AddInstruction(OpCode.DUP);
                        AddInstruction(OpCode.ISNULL);
                        JumpTarget ifFalse = new();
                        Jump(OpCode.JMPIFNOT_L, ifFalse);
                        {
                            AddInstruction(OpCode.DROP);
                            AddInstruction(OpCode.PUSH0);
                        }
                        ifFalse.Instruction = AddInstruction(OpCode.NOP);
                        break;
                    case "String":
                    case "ByteString":
                    case "UInt160":
                    case "UInt256":
                    case "ECPoint":
                        break;
                    default:
                        Call(NativeContract.StdLib.Hash, "deserialize", 1, true);
                        break;
                }
                AddInstruction(OpCode.DUP);
                if (Symbol.IsStatic)
                {
                    IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
                    byte backingFieldIndex = _context.AddStaticField(backingField);
                    AccessSlot(OpCode.STSFLD, backingFieldIndex);
                }
                else
                {
                    fields = fields.Where(p => !p.IsStatic).ToArray();
                    int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                    AccessSlot(OpCode.LDARG, 0);
                    Push(backingFieldIndex);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SETITEM);
                }
                endTarget.Instruction = AddInstruction(OpCode.NOP);
            }
            else
            {
                if (Symbol.IsStatic)
                    AccessSlot(OpCode.LDARG, 0);
                else
                    AccessSlot(OpCode.LDARG, 1);
                switch (property.Type.Name)
                {
                    case "byte":
                    case "sbyte":
                    case "Byte":
                    case "SByte":

                    case "short":
                    case "ushort":
                    case "Int16":
                    case "UInt16":

                    case "int":
                    case "uint":
                    case "Int32":
                    case "UInt32":

                    case "long":
                    case "ulong":
                    case "Int64":
                    case "UInt64":
                    case "BigInteger":
                    case "String":
                    case "ByteString":
                    case "UInt160":
                    case "UInt256":
                    case "ECPoint":
                        break;
                    default:
                        Call(NativeContract.StdLib.Hash, "serialize", 1, true);
                        break;
                }
                Push(key);
                Call(ApplicationEngine.System_Storage_GetContext);
                Call(ApplicationEngine.System_Storage_Put);
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
                .First(p => p.Name == nameof(ModifierAttribute.Exit) && p.Parameters.Length == 0);
            MethodConvert exitMethod = _context.ConvertMethod(model, exitSymbol);
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
        #endregion

        #region ConvertStatement

        private void ConvertStatement(SemanticModel model, StatementSyntax statement)
        {
            switch (statement)
            {
                // Converts a block statement, which is a series of statements enclosed in braces.
                // Example: { int x = 1; Console.WriteLine(x); }
                case BlockSyntax syntax:
                    ConvertBlockStatement(model, syntax);
                    break;
                // Converts a break statement, typically used within loops or switch cases.
                // Example: break;
                case BreakStatementSyntax syntax:
                    ConvertBreakStatement(syntax);
                    break;
                // Converts a checked statement, used for arithmetic operations with overflow checking.
                // Example: checked { int x = int.MaxValue; }
                case CheckedStatementSyntax syntax:
                    ConvertCheckedStatement(model, syntax);
                    break;
                // Converts a continue statement, used to skip the current iteration of a loop.
                // Example: continue;
                case ContinueStatementSyntax syntax:
                    ConvertContinueStatement(syntax);
                    break;
                // Converts a do-while loop statement.
                // Example: do { /* actions */ } while (condition);
                case DoStatementSyntax syntax:
                    ConvertDoStatement(model, syntax);
                    break;
                // Converts an empty statement, which is typically just a standalone semicolon.
                // Example: ;
                case EmptyStatementSyntax syntax:
                    ConvertEmptyStatement(syntax);
                    break;
                // Converts an expression statement, which is a statement consisting of a single expression.
                // Example: Console.WriteLine("Hello");
                case ExpressionStatementSyntax syntax:
                    ConvertExpressionStatement(model, syntax);
                    break;
                // Converts a foreach loop statement.
                // Example: foreach (var item in collection) { /* actions */ }
                case ForEachStatementSyntax syntax:
                    ConvertForEachStatement(model, syntax);
                    break;
                // Converts a foreach loop statement with variable declarations.
                // Example: foreach (var (key, value) in dictionary) { /* actions */ }
                case ForEachVariableStatementSyntax syntax:
                    ConvertForEachVariableStatement(model, syntax);
                    break;
                // Converts a for loop statement.
                // Example: for (int i = 0; i < 10; i++) { /* actions */ }
                case ForStatementSyntax syntax:
                    ConvertForStatement(model, syntax);
                    break;
                // Converts a goto statement, used for jumping to a labeled statement.
                // Example: goto myLabel;
                case GotoStatementSyntax syntax:
                    ConvertGotoStatement(model, syntax);
                    break;
                // Converts an if statement, including any else or else if branches.
                // Example: if (condition) { /* actions */ } else { /* actions */ }
                case IfStatementSyntax syntax:
                    ConvertIfStatement(model, syntax);
                    break;
                // Converts a labeled statement, used as a target for goto statements.
                // Example: myLabel: /* actions */
                case LabeledStatementSyntax syntax:
                    ConvertLabeledStatement(model, syntax);
                    break;
                // Converts a local variable declaration statement.
                // Example: int x = 5;
                case LocalDeclarationStatementSyntax syntax:
                    ConvertLocalDeclarationStatement(model, syntax);
                    break;
                // Currently, local function statements are not supported in this context.
                case LocalFunctionStatementSyntax:
                    break;
                // Converts a return statement, used to exit a method and optionally return a value.
                // Example: return x + y;
                case ReturnStatementSyntax syntax:
                    ConvertReturnStatement(model, syntax);
                    break;
                // Converts a switch statement, including its cases and default case.
                // Example: switch (variable) { case 1: /* actions */ break; default: /* actions */
                case SwitchStatementSyntax syntax:
                    ConvertSwitchStatement(model, syntax);
                    break;
                // Converts a throw statement, used for exception handling.
                // Example: throw new Exception("Error");
                case ThrowStatementSyntax syntax:
                    ConvertThrowStatement(model, syntax);
                    break;
                // Converts a try-catch-finally statement, used for exception handling.
                // Example: try { /* actions */ } catch (Exception e) { /* actions */ } finally { /* actions */ }
                case TryStatementSyntax syntax:
                    ConvertTryStatement(model, syntax);
                    break;
                // Converts a while loop statement.
                // Example: while (condition) { /* actions */ }
                case WhileStatementSyntax syntax:
                    ConvertWhileStatement(model, syntax);
                    break;
                default:
                    throw new CompilationException(statement, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {statement}");
            }
        }

        #endregion

        #region ConvertExpression

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

        #endregion

        #region ConvertPattern

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

        private void ConvertBinaryPattern(SemanticModel model, BinaryPatternSyntax pattern, byte localIndex)
        {
            switch (pattern.OperatorToken.ValueText)
            {
                case "and":
                    ConvertAndPattern(model, pattern.Left, pattern.Right, localIndex);
                    break;
                case "or":
                    ConvertOrPattern(model, pattern.Left, pattern.Right, localIndex);
                    break;
                default:
                    throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
            }
        }

        private void ConvertAndPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
        {
            JumpTarget rightTarget = new();
            JumpTarget endTarget = new();
            ConvertPattern(model, left, localIndex);
            Jump(OpCode.JMPIF_L, rightTarget);
            Push(false);
            Jump(OpCode.JMP_L, endTarget);
            rightTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertPattern(model, right, localIndex);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertOrPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
        {
            JumpTarget rightTarget = new();
            JumpTarget endTarget = new();
            ConvertPattern(model, left, localIndex);
            Jump(OpCode.JMPIFNOT_L, rightTarget);
            Push(true);
            Jump(OpCode.JMP_L, endTarget);
            rightTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertPattern(model, right, localIndex);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertConstantPattern(SemanticModel model, ConstantPatternSyntax pattern, byte localIndex)
        {
            AccessSlot(OpCode.LDLOC, localIndex);
            ConvertExpression(model, pattern.Expression);
            AddInstruction(OpCode.EQUAL);
        }

        private void ConvertDeclarationPattern(SemanticModel model, DeclarationPatternSyntax pattern, byte localIndex)
        {
            ITypeSymbol type = model.GetTypeInfo(pattern.Type).Type!;
            AccessSlot(OpCode.LDLOC, localIndex);
            IsType(type.GetPatternType());
            switch (pattern.Designation)
            {
                case DiscardDesignationSyntax:
                    break;
                case SingleVariableDesignationSyntax variable:
                    ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(variable)!;
                    byte index = AddLocalVariable(local);
                    AccessSlot(OpCode.LDLOC, localIndex);
                    AccessSlot(OpCode.STLOC, index);
                    break;
                default:
                    throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
            }
        }

        private void ConvertRelationalPattern(SemanticModel model, RelationalPatternSyntax pattern, byte localIndex)
        {
            AccessSlot(OpCode.LDLOC, localIndex);
            ConvertExpression(model, pattern.Expression);
            AddInstruction(pattern.OperatorToken.ValueText switch
            {
                "<" => OpCode.LT,
                "<=" => OpCode.LE,
                ">" => OpCode.GT,
                ">=" => OpCode.GE,
                _ => throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}")
            });
        }

        private void ConvertTypePattern(SemanticModel model, TypePatternSyntax pattern, byte localIndex)
        {
            ITypeSymbol type = model.GetTypeInfo(pattern.Type).Type!;
            AccessSlot(OpCode.LDLOC, localIndex);
            IsType(type.GetPatternType());
        }

        private void ConvertNotPattern(SemanticModel model, UnaryPatternSyntax pattern, byte localIndex)
        {
            ConvertPattern(model, pattern.Pattern, localIndex);
            AddInstruction(OpCode.NOT);
        }

        #endregion

        #region StackHelpers

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

        #region LabelsAndTargets

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

        #endregion

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
        #endregion

        #region SlotHelpers

        private Instruction AccessSlot(OpCode opcode, byte index)
        {
            return index >= 7
                ? AddInstruction(new Instruction { OpCode = opcode, Operand = new[] { index } })
                : AddInstruction(opcode - 7 + index);
        }
        #endregion

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
                byte index = _context.AddVTable(type);
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

        #region CallHelpers

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
            ushort token = _context.AddMethodToken(hash, method, parametersCount, hasReturnValue, callFlags);
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
            if (TryProcessInlineMethods(model, symbol, arguments))
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
                convert = _context.ConvertMethod(model, symbol);
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
            if (TryProcessInlineMethods(model, symbol, arguments))
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
                    ? _context.ConvertMethod(model, symbol)
                    : _context.ConvertMethod(model, symbol.ReducedFrom);
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
            if (TryProcessInlineMethods(model, symbol, null))
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
                convert = _context.ConvertMethod(model, symbol);
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

        private bool TryProcessSystemConstructors(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
        {
            switch (symbol.ToString())
            {
                case "System.Numerics.BigInteger.BigInteger(byte[])":
                    PrepareArgumentsForMethod(model, symbol, arguments);
                    ChangeType(VM.Types.StackItemType.Integer);
                    return true;
                default:
                    return false;
            }
        }

        private bool TryProcessSystemMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        {
            if (symbol.ContainingType.TypeKind == TypeKind.Delegate && symbol.Name == "Invoke")
            {
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.Cdecl);
                ConvertExpression(model, instanceExpression!);
                AddInstruction(OpCode.CALLA);
                return true;
            }
            switch (symbol.ToString())
            {
                case "System.Numerics.BigInteger.One.get":
                    Push(1);
                    return true;
                case "System.Numerics.BigInteger.MinusOne.get":
                    Push(-1);
                    return true;
                case "System.Numerics.BigInteger.Zero.get":
                    Push(0);
                    return true;
                case "System.Numerics.BigInteger.IsZero.get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.IsOne.get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Push(1);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.IsEven.get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Push(1);
                    AddInstruction(OpCode.AND);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.Sign.get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.SIGN);
                    return true;
                case "System.Numerics.BigInteger.Pow(System.Numerics.BigInteger, int)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.POW);
                    return true;
                case "System.Numerics.BigInteger.ModPow(System.Numerics.BigInteger, System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MODPOW);
                    return true;
                case "System.Numerics.BigInteger.Add(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.ADD);
                    return true;
                case "System.Numerics.BigInteger.Subtract(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.SUB);
                    return true;
                case "System.Numerics.BigInteger.Negate(System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.NEGATE);
                    return true;
                case "System.Numerics.BigInteger.Multiply(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MUL);
                    return true;
                case "System.Numerics.BigInteger.Divide(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.DIV);
                    return true;
                case "System.Numerics.BigInteger.Remainder(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MOD);
                    return true;
                case "System.Numerics.BigInteger.Compare(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    // if left < right return -1;
                    // if left = right return 0;
                    // if left > right return 1;
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.SIGN);
                    return true;
                case "System.Numerics.BigInteger.GreatestCommonDivisor(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    JumpTarget gcdTarget = new()
                    {
                        Instruction = AddInstruction(OpCode.DUP)
                    };
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.MOD);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.PUSH0);
                    AddInstruction(OpCode.NUMEQUAL);
                    Jump(OpCode.JMPIFNOT, gcdTarget);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.ABS);
                    return true;
                case "System.Numerics.BigInteger.ToByteArray()":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    ChangeType(VM.Types.StackItemType.Buffer);
                    return true;
                case "System.Numerics.BigInteger.explicit operator sbyte(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(sbyte.MinValue);
                        Push(sbyte.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.explicit operator byte(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(byte.MinValue);
                        Push(byte.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.explicit operator short(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(short.MinValue);
                        Push(short.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.explicit operator ushort(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(ushort.MinValue);
                        Push(ushort.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.explicit operator int(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(int.MinValue);
                        Push(new BigInteger(int.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.explicit operator uint(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(uint.MinValue);
                        Push(new BigInteger(uint.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.explicit operator long(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(long.MinValue);
                        Push(new BigInteger(long.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.explicit operator ulong(System.Numerics.BigInteger)":
                    {
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        JumpTarget endTarget = new();
                        AddInstruction(OpCode.DUP);
                        Push(ulong.MinValue);
                        Push(new BigInteger(ulong.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(char)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(sbyte)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(byte)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(short)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(ushort)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(int)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(uint)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(long)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(ulong)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    return true;
                case "System.Array.Length.get":
                case "string.Length.get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.SIZE);
                    return true;
                case "sbyte.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(sbyte.MinValue);
                        Push(sbyte.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "byte.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(byte.MinValue);
                        Push(byte.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "short.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(short.MinValue);
                        Push(short.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "ushort.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(ushort.MinValue);
                        Push(ushort.MaxValue + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "int.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(int.MinValue);
                        Push(new BigInteger(int.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "uint.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(uint.MinValue);
                        Push(new BigInteger(uint.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "long.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(long.MinValue);
                        Push(new BigInteger(long.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "ulong.Parse(string)":
                    {
                        JumpTarget endTarget = new();
                        if (arguments is not null)
                            PrepareArgumentsForMethod(model, symbol, arguments);
                        Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                        AddInstruction(OpCode.DUP);
                        Push(ulong.MinValue);
                        Push(new BigInteger(ulong.MaxValue) + 1);
                        AddInstruction(OpCode.WITHIN);
                        Jump(OpCode.JMPIF, endTarget);
                        AddInstruction(OpCode.THROW);
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "System.Numerics.BigInteger.Parse(string)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    return true;
                case "System.Math.Abs(sbyte)":
                case "System.Math.Abs(short)":
                case "System.Math.Abs(int)":
                case "System.Math.Abs(long)":
                case "System.Numerics.BigInteger.Abs(System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.ABS);
                    return true;
                case "System.Math.Sign(sbyte)":
                case "System.Math.Sign(short)":
                case "System.Math.Sign(int)":
                case "System.Math.Sign(long)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.SIGN);
                    return true;
                case "System.Math.Max(byte, byte)":
                case "System.Math.Max(sbyte, sbyte)":
                case "System.Math.Max(short, short)":
                case "System.Math.Max(ushort, ushort)":
                case "System.Math.Max(int, int)":
                case "System.Math.Max(uint, uint)":
                case "System.Math.Max(long, long)":
                case "System.Math.Max(ulong, ulong)":
                case "System.Numerics.BigInteger.Max(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.MAX);
                    return true;
                case "System.Math.Min(byte, byte)":
                case "System.Math.Min(sbyte, sbyte)":
                case "System.Math.Min(short, short)":
                case "System.Math.Min(ushort, ushort)":
                case "System.Math.Min(int, int)":
                case "System.Math.Min(uint, uint)":
                case "System.Math.Min(long, long)":
                case "System.Math.Min(ulong, ulong)":
                case "System.Numerics.BigInteger.Min(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.MIN);
                    return true;
                case "bool.ToString()":
                    {
                        JumpTarget trueTarget = new(), endTarget = new();
                        if (instanceExpression is not null)
                            ConvertExpression(model, instanceExpression);
                        Jump(OpCode.JMPIF_L, trueTarget);
                        Push("False");
                        Jump(OpCode.JMP_L, endTarget);
                        trueTarget.Instruction = Push("True");
                        endTarget.Instruction = AddInstruction(OpCode.NOP);
                    }
                    return true;
                case "sbyte.ToString()":
                case "byte.ToString()":
                case "short.ToString()":
                case "ushort.ToString()":
                case "int.ToString()":
                case "uint.ToString()":
                case "long.ToString()":
                case "ulong.ToString()":
                case "System.Numerics.BigInteger.ToString()":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Call(NativeContract.StdLib.Hash, "itoa", 1, true);
                    return true;
                case "System.Numerics.BigInteger.Equals(long)":
                case "System.Numerics.BigInteger.Equals(ulong)":
                case "System.Numerics.BigInteger.Equals(System.Numerics.BigInteger)":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "object.Equals(object?)":
                case "string.Equals(string?)":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.EQUAL);
                    return true;
                case "string.this[int].get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.PICKITEM);
                    return true;
                case "string.Substring(int)":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.OVER);
                    AddInstruction(OpCode.SIZE);
                    AddInstruction(OpCode.OVER);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.SUBSTR);
                    return true;
                case "string.Substring(int, int)":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.SUBSTR);
                    return true;
                default:
                    return false;
            }
        }

        private bool TryProcessSystemOperators(SemanticModel model, IMethodSymbol symbol, params ExpressionSyntax[] arguments)
        {
            switch (symbol.ToString())
            {
                case "object.operator ==(object, object)":
                case "string.operator ==(string, string)":
                    ConvertExpression(model, arguments[0]);
                    ConvertExpression(model, arguments[1]);
                    AddInstruction(OpCode.EQUAL);
                    return true;
                case "object.operator !=(object, object)":
                    ConvertExpression(model, arguments[0]);
                    ConvertExpression(model, arguments[1]);
                    AddInstruction(OpCode.NOTEQUAL);
                    return true;
                case "string.operator +(string, string)":
                    ConvertExpression(model, arguments[0]);
                    ConvertExpression(model, arguments[1]);
                    AddInstruction(OpCode.CAT);
                    ChangeType(VM.Types.StackItemType.ByteString);
                    return true;
                case "string.operator +(string, object)":
                    ConvertExpression(model, arguments[0]);
                    ConvertObjectToString(model, arguments[1]);
                    AddInstruction(OpCode.CAT);
                    ChangeType(VM.Types.StackItemType.ByteString);
                    return true;
                case "string.operator +(object, string)":
                    ConvertObjectToString(model, arguments[0]);
                    ConvertExpression(model, arguments[1]);
                    AddInstruction(OpCode.CAT);
                    ChangeType(VM.Types.StackItemType.ByteString);
                    return true;
                default:
                    return false;
            }
        }

        private void EmitCall(MethodConvert target)
        {
            if (target._inline && !_context.Options.NoInline)
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
        #endregion
    }

    class MethodConvertCollection : KeyedCollection<IMethodSymbol, MethodConvert>
    {
        protected override IMethodSymbol GetKeyForItem(MethodConvert item) => item.Symbol;
    }
}
