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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler.Optimizer;
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
using System.Numerics;
using System.Runtime.InteropServices;
using Neo.VM.Types;
using Array = System.Array;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        #region Fields

        private readonly CompilationContext _context;
        private CallingConvention _callingConvention = CallingConvention.Cdecl;
        private bool _inline;
        private bool _internalInline;
        // _initSlot is a boolean flag that determines whether an INITSLOT instruction
        // should be added at the beginning of the method's bytecode.
        // It's used to allocate space for local variables and parameters in the Neo VM.
        // The _initSlot flag allows the compiler to avoid adding unnecessary INITSLOT
        // instructions for methods that don't need local variables or parameters.
        // It's typically set to false for inline methods or external method calls.
        // By using this flag, the compiler can efficiently manage stack space allocation
        // for method execution in the Neo VM, only allocating space when necessary.
        private bool _initSlot;
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
            || _instructions is [{ OpCode: OpCode.RET }]
            || _instructions is [{ OpCode: OpCode.INITSLOT }, { OpCode: OpCode.RET }];

        /// <summary>
        /// captured local variable/parameter symbols when converting current method
        /// </summary>
        public HashSet<ISymbol> CapturedLocalSymbols { get; } = new(SymbolEqualityComparer.Default);

        #endregion

        #region Constructors

        public MethodConvert(CompilationContext context, IMethodSymbol symbol)
        {
            this.Symbol = symbol;
            this._context = context;
            this._checkedStack.Push(context.Options.Checked);
        }
        #endregion

        #region Convert

        /// <summary>
        /// This method is responsible for converting a method into a Neo VM bytecode.
        /// </summary>
        /// <param name="model">The semantic model of the method</param>
        public void Convert(SemanticModel model)
        {
            // Step 1: Determine if the method is extern or empty
            // This checks if:
            // a) The method is marked as extern, e.g.:
            //    public static extern int ExternMethod(int arg);
            // b) The containing type (class) of the method has no declaring syntax references
            //    This can happen with partial classes or classes defined in other assemblies
            if (Symbol.IsExtern || Symbol.ContainingType.DeclaringSyntaxReferences.IsEmpty)
            {
                // Step 2a: Handle extern or empty methods
                // This path is taken for:
                // - Methods explicitly marked as extern (see example above)
                // - Methods in classes defined in other assemblies or partial classes, e.g.:
                //   public partial class MyClass
                //   {
                //       // This method might be defined in another file or assembly
                //       public void EmptyMethod();
                //   }
                if (Symbol.Name == "_initialize")
                {
                    // Special handling for _initialize method
                    // This method is used for contract initialization
                    ProcessStaticFields(model);
                    InsertStaticFieldInitialization();
                }
                else
                {
                    // Convert extern method
                    // This usually involves creating a stub or placeholder for the external implementation
                    ConvertExtern();
                }
            }
            else
            {
                // Step 2b: Handle regular methods
                // This path is taken for normal methods with implementations in the current compilation unit
                // These methods require full processing and conversion
                // Example of a regular method:
                // public class MyClass
                // {
                //     public int RegularMethod(int arg)
                //     {
                //         return arg * 2;
                //     }
                // }
                // Set syntax node if available
                if (!Symbol.DeclaringSyntaxReferences.IsEmpty)
                    SyntaxNode = Symbol.DeclaringSyntaxReferences[0].GetSyntax();

                // Step 3: Process method based on its kind
                // This handles special cases for constructors and static constructors
                // Examples:
                // public MyClass() { } // Constructor
                // static MyClass() { } // Static constructor
                InitializeFieldsBasedOnMethodKind(model);

                // Step 4: Validate method name
                // Ensures that method names starting with '_' are valid
                // Example of an invalid method name:
                // public void _invalidMethod() { } // This would throw an exception
                ValidateMethodName();

                // Step 5: Convert modifiers
                // process ModifierAttribute
                var modifiers = ConvertModifier(model).ToArray();

                // Step 6: Convert the main method body
                // This is where the actual method implementation is converted
                ConvertSource(model);

                // Step 7: Insert initialization instructions if needed
                // This includes initializing static fields and local variables
                // This might insert INITSLOT instruction to the beginning of the method
                // Example:
                // static int staticField = 10;
                // public void Method()
                // {
                //     int localVar = 5;
                //     // ... rest of the method
                // }
                InsertInitializationInstructions();

                // Step 8: Process modifiers (exit)
                // Handle any cleanup or additional instructions required by modifiers
                ProcessModifiersExit(model, modifiers);
            }

            // Step 9: Finalize the method
            // Add RET instruction to the end of the method
            // This ensures proper method termination and return instruction
            FinalizeMethod();

            // Step 10: Optimize the instructions if needed
            // Basic optimization to remove unnecessary NOP instructions
            if (_context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic))
                BasicOptimizer.RemoveNops(_instructions);

            // Step 11: Set the start target
            // Mark the first instruction as the entry point of the method
            _startTarget.Instruction = _instructions[0];
        }

        public void ConvertForward(SemanticModel model, MethodConvert target)
        {
            INamedTypeSymbol type = Symbol.ContainingType;
            CreateObject(model, type);
            IMethodSymbol? constructor = type.InstanceConstructors.FirstOrDefault(p => p.Parameters.Length == 0)
                ?? throw new CompilationException(type, DiagnosticId.NoParameterlessConstructor, "The contract class requires a parameterless constructor.");
            CallInstanceMethod(model, constructor, true, Array.Empty<ArgumentSyntax>());
            _returnTarget.Instruction = Jump(OpCode.JMP_L, target._startTarget);
            _startTarget.Instruction = _instructions[0];
        }

        private void ProcessFieldInitializer(SemanticModel model, IFieldSymbol field, Action? preInitialize, Action? postInitialize)
        {
            AttributeData? initialValue = field.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(InitialValueAttribute) || p.AttributeClass!.IsSubclassOf(nameof(InitialValueAttribute)));
            if (initialValue is null)
            {
                EqualsValueClauseSyntax? initializer = null;
                SyntaxNode syntaxNode;
                if (field.DeclaringSyntaxReferences.IsEmpty)
                {
                    // Special handling for string.Empty is required as it lacks an AssociatedSymbol.
                    // Without this check, the method would return prematurely, bypassing necessary processing.
                    if (field.ContainingType.ToString() == "string" && field.Name == "Empty")
                    {
                        preInitialize?.Invoke();
                        Push(string.Empty);
                        postInitialize?.Invoke();
                        return;
                    }

                    if (field.AssociatedSymbol is not IPropertySymbol property) return;
                    syntaxNode = property.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntaxNode is PropertyDeclarationSyntax syntax)
                    {
                        initializer = syntax.Initializer;
                    }
                }
                else
                {
                    VariableDeclaratorSyntax syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences[0].GetSyntax();
                    syntaxNode = syntax;
                    initializer = syntax.Initializer;
                }

                using (InsertSequencePoint(syntaxNode))
                {
                    preInitialize?.Invoke();
                    if (initializer is null)
                        PushDefault(field.Type);
                    else
                    {
                        model = model.Compilation.GetSemanticModel(syntaxNode.SyntaxTree);
                        ConvertExpression(model, initializer.Value, syntaxNode);
                    }
                    postInitialize?.Invoke();
                }
            }
            else
            {
                preInitialize?.Invoke();
                string value = (string)initialValue.ConstructorArguments[0].Value!;
                var attributeName = initialValue.AttributeClass!.Name;
                ContractParameterType parameterType = attributeName switch
                {
                    nameof(InitialValueAttribute) => (ContractParameterType)initialValue.ConstructorArguments[1].Value!,
                    nameof(IntegerAttribute) => ContractParameterType.Integer,
                    nameof(Hash160Attribute) => ContractParameterType.Hash160,
                    nameof(PublicKeyAttribute) => ContractParameterType.PublicKey,
                    nameof(ByteArrayAttribute) => ContractParameterType.ByteArray,
                    nameof(StringAttribute) => ContractParameterType.String,
                    _ => throw new CompilationException(field, DiagnosticId.InvalidInitialValueType, $"Unsupported initial value type: {attributeName}"),
                };

                try
                {
                    switch (parameterType)
                    {
                        case ContractParameterType.String:
                            Push(value);
                            break;
                        case ContractParameterType.Integer:
                            Push(BigInteger.Parse(value));
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
                            throw new CompilationException(field, DiagnosticId.InvalidInitialValueType, $"Unsupported initial value type: {parameterType}");
                    }
                }
                catch (Exception ex) when (ex is not CompilationException)
                {
                    throw new CompilationException(field, DiagnosticId.InvalidInitialValue, $"Invalid initial value: {value} of type: {parameterType}");
                }
                postInitialize?.Invoke();
            }
        }


        #endregion
    }

    class MethodConvertCollection : KeyedCollection<IMethodSymbol, MethodConvert>
    {
        protected override IMethodSymbol GetKeyForItem(MethodConvert item) => item.Symbol;
    }
}
