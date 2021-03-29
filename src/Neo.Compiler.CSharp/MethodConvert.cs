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
    class MethodConvert
    {
        private readonly CompilationContext context;
        private CallingConvention _callingConvention = CallingConvention.Cdecl;
        private bool _inline;
        private bool _initslot;
        private readonly Dictionary<IParameterSymbol, byte> _parameters = new();
        private readonly List<ILocalSymbol> _variableSymbols = new();
        private readonly Dictionary<ILocalSymbol, byte> _localVariables = new();
        private readonly List<byte> _anonymousVariables = new();
        private int _localsCount;
        private readonly Stack<List<ILocalSymbol>> _blockSymbols = new();
        private readonly List<Instruction> _instructions = new();
        private readonly JumpTarget _startTarget = new();
        private readonly Stack<JumpTarget> _continueTargets = new();
        private readonly Stack<JumpTarget> _breakTargets = new();
        private readonly JumpTarget _returnTarget = new();
        private readonly Stack<ExceptionHandling> _tryStack = new();
        private readonly Stack<byte> _exceptionStack = new();

        public IMethodSymbol Symbol { get; }
        public SyntaxNode? SyntaxNode { get; private set; }
        public IReadOnlyList<Instruction> Instructions => _instructions;
        public IReadOnlyList<ILocalSymbol> Variables => _variableSymbols;

        public MethodConvert(CompilationContext context, IMethodSymbol symbol)
        {
            this.Symbol = symbol;
            this.context = context;
        }

        private byte AddLocalVariable(ILocalSymbol symbol)
        {
            _variableSymbols.Add(symbol);
            byte index = (byte)(_localVariables.Count + _anonymousVariables.Count);
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
                ConvertExtern();
            }
            else
            {
                switch (Symbol.MethodKind)
                {
                    case MethodKind.Constructor:
                        ProcessFields(model);
                        break;
                    case MethodKind.StaticConstructor:
                        ProcessStaticFields(model);
                        break;
                }
                if (!Symbol.DeclaringSyntaxReferences.IsEmpty)
                    ConvertSource(model);
                if (Symbol.MethodKind == MethodKind.StaticConstructor && context.StaticFields.Count > 0)
                {
                    _instructions.Insert(0, new Instruction
                    {
                        OpCode = OpCode.INITSSLOT,
                        Operand = new[] { (byte)context.StaticFields.Count }
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
            }
            _returnTarget.Instruction = AddInstruction(OpCode.RET);
            if (!context.Options.NoOptimize)
                Optimizer.RemoveNops(_instructions);
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
            foreach (INamedTypeSymbol @class in context.StaticFields.Select(p => p.ContainingType).Distinct().ToArray())
            {
                foreach (IFieldSymbol field in @class.GetMembers().OfType<IFieldSymbol>())
                {
                    if (field.IsConst || !field.IsStatic) continue;
                    ProcessFieldInitializer(model, field, null, () =>
                    {
                        byte index = context.AddStaticField(field);
                        AccessSlot(OpCode.STSFLD, index);
                    });
                }
            }
        }

        private void ProcessFieldInitializer(SemanticModel model, IFieldSymbol field, Action? preInitialize, Action? postInitialize)
        {
            AttributeData? initialValue = field.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx.Neo.SmartContract.Framework.InitialValueAttribute));
            if (initialValue is null)
            {
                if (field.DeclaringSyntaxReferences.IsEmpty) return;
                VariableDeclaratorSyntax syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences[0].GetSyntax();
                if (syntax.Initializer is null) return;
                model = model.Compilation.GetSemanticModel(syntax.SyntaxTree);
                using (InsertSequencePoint(syntax))
                {
                    preInitialize?.Invoke();
                    ConvertExpression(model, syntax.Initializer.Value);
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
                    case ContractParameterType.ByteArray:
                        Push(value.HexToBytes(true));
                        break;
                    case ContractParameterType.Hash160:
                        Push(value.ToScriptHash(context.Options.AddressVersion).ToArray());
                        break;
                    case ContractParameterType.PublicKey:
                        Push(ECPoint.Parse(value, ECCurve.Secp256r1).EncodePoint(true));
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported initial value type: {type}");
                }
                postInitialize?.Invoke();
            }
        }

        private void ConvertExtern()
        {
            _inline = true;
            AttributeData? contractAttribute = Symbol.ContainingType.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx.Neo.SmartContract.Framework.ContractAttribute));
            if (contractAttribute is null)
            {
                bool emitted = false;
                foreach (AttributeData attribute in Symbol.GetAttributes())
                {
                    switch (attribute.AttributeClass!.Name)
                    {
                        case nameof(scfx.Neo.SmartContract.Framework.OpCodeAttribute):
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
                        case nameof(scfx.Neo.SmartContract.Framework.SyscallAttribute):
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
                        case nameof(scfx.Neo.SmartContract.Framework.CallingConventionAttribute):
                            emitted = true;
                            _callingConvention = (CallingConvention)attribute.ConstructorArguments[0].Value!;
                            break;
                    }
                }
                if (!emitted) throw new NotSupportedException($"Unknown method: {Symbol}");
            }
            else
            {
                UInt160 hash = UInt160.Parse((string)contractAttribute.ConstructorArguments[0].Value!);
                AttributeData? attribute = Symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx.Neo.SmartContract.Framework.ContractHashAttribute));
                if (attribute is null)
                {
                    string method = Symbol.GetDisplayName(true);
                    ushort parametersCount = (ushort)Symbol.Parameters.Length;
                    bool hasReturnValue = !Symbol.ReturnsVoid || Symbol.MethodKind == MethodKind.Constructor;
                    Call(hash, method, parametersCount, hasReturnValue);
                }
                else
                {
                    Push(hash.ToArray());
                }
            }
        }

        private void ConvertNoBody(AccessorDeclarationSyntax syntax)
        {
            _inline = true;
            _callingConvention = CallingConvention.Cdecl;
            IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
            INamedTypeSymbol type = property.ContainingType;
            IFieldSymbol[] fields = type.GetMembers().OfType<IFieldSymbol>().ToArray();
            int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
            IFieldSymbol backingField = fields[backingFieldIndex];
            using (InsertSequencePoint(syntax))
            {
                switch (Symbol.MethodKind)
                {
                    case MethodKind.PropertyGet:
                        if (Symbol.IsStatic)
                        {
                            byte index = context.AddStaticField(backingField);
                            AccessSlot(OpCode.LDSFLD, index);
                        }
                        else
                        {
                            Push(backingFieldIndex);
                            AddInstruction(OpCode.PICKITEM);
                        }
                        break;
                    case MethodKind.PropertySet:
                        if (Symbol.IsStatic)
                        {
                            byte index = context.AddStaticField(backingField);
                            AccessSlot(OpCode.STSFLD, index);
                        }
                        else
                        {
                            Push(backingFieldIndex);
                            AddInstruction(OpCode.ROT);
                            AddInstruction(OpCode.SETITEM);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported accessor: {Symbol}");
                }
            }
        }

        private void ConvertSource(SemanticModel model)
        {
            SyntaxNode = Symbol.DeclaringSyntaxReferences[0].GetSyntax();
            for (byte i = 0; i < Symbol.Parameters.Length; i++)
            {
                IParameterSymbol parameter = Symbol.Parameters[i];
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
                    throw new NotSupportedException($"Unsupported method body:{SyntaxNode}");
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
                case IfStatementSyntax syntax:
                    ConvertIfStatement(model, syntax);
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
                    throw new NotSupportedException($"Unsupported syntax: {statement}");
            }
        }

        private void ConvertBlockStatement(SemanticModel model, BlockSyntax syntax)
        {
            _blockSymbols.Push(new List<ILocalSymbol>());
            using (InsertSequencePoint(syntax.OpenBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (StatementSyntax child in syntax.Statements)
                ConvertStatement(model, child);
            using (InsertSequencePoint(syntax.CloseBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (ILocalSymbol symbol in _blockSymbols.Pop())
                _localVariables.Remove(symbol);
        }

        private void ConvertBreakStatement(BreakStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                if (_tryStack.Count > 0)
                    Jump(OpCode.ENDTRY_L, _breakTargets.Peek());
                else
                    Jump(OpCode.JMP_L, _breakTargets.Peek());
        }

        private void ConvertContinueStatement(ContinueStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                if (_tryStack.Count > 0)
                    Jump(OpCode.ENDTRY_L, _continueTargets.Peek());
                else
                    Jump(OpCode.JMP_L, _continueTargets.Peek());
        }

        private void ConvertDoStatement(SemanticModel model, DoStatementSyntax syntax)
        {
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            using (InsertSequencePoint(syntax.Condition))
                ConvertExpression(model, syntax.Condition);
            Jump(OpCode.JMPIF_L, startTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertEmptyStatement(EmptyStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                AddInstruction(OpCode.NOP);
        }

        private void ConvertExpressionStatement(SemanticModel model, ExpressionStatementSyntax syntax)
        {
            ITypeSymbol type = model.GetTypeInfo(syntax.Expression).Type!;
            using (InsertSequencePoint(syntax))
            {
                ConvertExpression(model, syntax.Expression);
                if (type.SpecialType != SpecialType.System_Void)
                    AddInstruction(OpCode.DROP);
            }
        }

        private void ConvertForEachStatement(SemanticModel model, ForEachStatementSyntax syntax)
        {
            ILocalSymbol elementSymbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            byte elementIndex = AddLocalVariable(elementSymbol);
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Identifier))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Value);
                AccessSlot(OpCode.STLOC, elementIndex);
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Next);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _anonymousVariables.Remove(iteratorIndex);
            _localVariables.Remove(elementSymbol);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertForEachVariableStatement(SemanticModel model, ForEachVariableStatementSyntax syntax)
        {
            ILocalSymbol[] symbols = ((ParenthesizedVariableDesignationSyntax)((DeclarationExpressionSyntax)syntax.Variable).Designation).Variables.Select(p => (ILocalSymbol)model.GetDeclaredSymbol(p)!).ToArray();
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Variable))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Value);
                AddInstruction(OpCode.UNPACK);
                AddInstruction(OpCode.DROP);
                for (int i = 0; i < symbols.Length; i++)
                {
                    if (symbols[i] is null)
                    {
                        AddInstruction(OpCode.DROP);
                    }
                    else
                    {
                        byte variableIndex = AddLocalVariable(symbols[i]);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
                }
                AddInstruction(OpCode.DROP);
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Next);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _anonymousVariables.Remove(iteratorIndex);
            foreach (ILocalSymbol symbol in symbols)
                if (symbol is not null)
                    _localVariables.Remove(symbol);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertForStatement(SemanticModel model, ForStatementSyntax syntax)
        {
            var variables = (syntax.Declaration?.Variables ?? Enumerable.Empty<VariableDeclaratorSyntax>())
                .Select(p => (p, (ILocalSymbol)model.GetDeclaredSymbol(p)!))
                .ToArray();
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget breakTarget = new();
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            foreach (var (variable, symbol) in variables)
            {
                byte variableIndex = AddLocalVariable(symbol);
                if (variable.Initializer is not null)
                    using (InsertSequencePoint(variable))
                    {
                        ConvertExpression(model, variable.Initializer.Value);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
            }
            Jump(OpCode.JMP_L, conditionTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (ExpressionSyntax expression in syntax.Incrementors)
                using (InsertSequencePoint(expression))
                {
                    ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                    ConvertExpression(model, expression);
                    if (type.SpecialType != SpecialType.System_Void)
                        AddInstruction(OpCode.DROP);
                }
            conditionTarget.Instruction = AddInstruction(OpCode.NOP);
            if (syntax.Condition is null)
            {
                Jump(OpCode.JMP_L, startTarget);
            }
            else
            {
                using (InsertSequencePoint(syntax.Condition))
                    ConvertExpression(model, syntax.Condition);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (var (_, symbol) in variables)
                _localVariables.Remove(symbol);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertIfStatement(SemanticModel model, IfStatementSyntax syntax)
        {
            JumpTarget elseTarget = new();
            using (InsertSequencePoint(syntax.Condition))
                ConvertExpression(model, syntax.Condition);
            Jump(OpCode.JMPIFNOT_L, elseTarget);
            ConvertStatement(model, syntax.Statement);
            if (syntax.Else is null)
            {
                elseTarget.Instruction = AddInstruction(OpCode.NOP);
            }
            else
            {
                JumpTarget endTarget = new();
                Jump(OpCode.JMP_L, endTarget);
                elseTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertStatement(model, syntax.Else.Statement);
                endTarget.Instruction = AddInstruction(OpCode.NOP);
            }
        }

        private void ConvertLocalDeclarationStatement(SemanticModel model, LocalDeclarationStatementSyntax syntax)
        {
            if (syntax.IsConst) return;
            foreach (VariableDeclaratorSyntax variable in syntax.Declaration.Variables)
            {
                ILocalSymbol symbol = (ILocalSymbol)model.GetDeclaredSymbol(variable)!;
                byte variableIndex = AddLocalVariable(symbol);
                if (variable.Initializer is not null)
                    using (InsertSequencePoint(variable))
                    {
                        ConvertExpression(model, variable.Initializer.Value);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
            }
        }

        private void ConvertReturnStatement(SemanticModel model, ReturnStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
            {
                if (syntax.Expression is not null)
                    ConvertExpression(model, syntax.Expression);
                if (_tryStack.Count > 0)
                    Jump(OpCode.ENDTRY_L, _returnTarget);
                else
                    Jump(OpCode.JMP_L, _returnTarget);
            }
        }

        private void ConvertSwitchStatement(SemanticModel model, SwitchStatementSyntax syntax)
        {
            var sections = syntax.Sections.Select(p => (p.Labels, p.Statements, Target: new JumpTarget())).ToArray();
            var labels = sections.SelectMany(p => p.Labels, (p, l) => (l, p.Target)).ToArray();
            JumpTarget breakTarget = new();
            byte anonymousIndex = AddAnonymousVariable();
            _breakTargets.Push(breakTarget);
            using (InsertSequencePoint(syntax.Expression))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, anonymousIndex);
            }
            foreach (var (label, target) in labels)
            {
                switch (label)
                {
                    case CasePatternSwitchLabelSyntax casePatternSwitchLabel:
                        using (InsertSequencePoint(casePatternSwitchLabel))
                        {
                            JumpTarget endTarget = new();
                            ConvertPattern(model, casePatternSwitchLabel.Pattern, anonymousIndex);
                            Jump(OpCode.JMPIFNOT_L, endTarget);
                            if (casePatternSwitchLabel.WhenClause is not null)
                            {
                                ConvertExpression(model, casePatternSwitchLabel.WhenClause.Condition);
                                Jump(OpCode.JMPIFNOT_L, endTarget);
                            }
                            Jump(OpCode.JMP_L, target);
                            endTarget.Instruction = AddInstruction(OpCode.NOP);
                        }
                        break;
                    case CaseSwitchLabelSyntax caseSwitchLabel:
                        using (InsertSequencePoint(caseSwitchLabel))
                        {
                            AccessSlot(OpCode.LDLOC, anonymousIndex);
                            ConvertExpression(model, caseSwitchLabel.Value);
                            AddInstruction(OpCode.EQUAL);
                            Jump(OpCode.JMPIF_L, target);
                        }
                        break;
                    case DefaultSwitchLabelSyntax defaultSwitchLabel:
                        using (InsertSequencePoint(defaultSwitchLabel))
                        {
                            Jump(OpCode.JMP_L, target);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported syntax: {label}");
                }
            }
            _anonymousVariables.Remove(anonymousIndex);
            Jump(OpCode.JMP_L, breakTarget);
            foreach (var (_, statements, target) in sections)
            {
                target.Instruction = AddInstruction(OpCode.NOP);
                foreach (StatementSyntax statement in statements)
                    ConvertStatement(model, statement);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _breakTargets.Pop();
        }

        private void ConvertThrowStatement(SemanticModel model, ThrowStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                Throw(model, syntax.Expression);
        }

        private void ConvertTryStatement(SemanticModel model, TryStatementSyntax syntax)
        {
            JumpTarget catchTarget = new();
            JumpTarget finallyTarget = new();
            JumpTarget endTarget = new();
            AddInstruction(new Instruction { OpCode = OpCode.TRY_L, Target = catchTarget, Target2 = finallyTarget });
            _tryStack.Push(new ExceptionHandling { State = ExceptionHandlingState.Try });
            ConvertStatement(model, syntax.Block);
            Jump(OpCode.ENDTRY_L, endTarget);
            if (syntax.Catches.Count > 1)
                throw new NotSupportedException("Only support one single catch.");
            if (syntax.Catches.Count > 0)
            {
                CatchClauseSyntax catchClause = syntax.Catches[0];
                if (catchClause.Filter is not null)
                    throw new NotSupportedException($"Unsupported syntax: {catchClause.Filter}");
                _tryStack.Peek().State = ExceptionHandlingState.Catch;
                ILocalSymbol? exceptionSymbol = null;
                byte exceptionIndex;
                if (catchClause.Declaration is null)
                {
                    exceptionIndex = AddAnonymousVariable();
                }
                else
                {
                    exceptionSymbol = model.GetDeclaredSymbol(catchClause.Declaration);
                    exceptionIndex = exceptionSymbol is null
                        ? AddAnonymousVariable()
                        : AddLocalVariable(exceptionSymbol);
                }
                using (InsertSequencePoint(catchClause.CatchKeyword))
                    catchTarget.Instruction = AccessSlot(OpCode.STLOC, exceptionIndex);
                _exceptionStack.Push(exceptionIndex);
                ConvertStatement(model, catchClause.Block);
                Jump(OpCode.ENDTRY_L, endTarget);
                if (exceptionSymbol is null)
                    _anonymousVariables.Remove(exceptionIndex);
                else
                    _localVariables.Remove(exceptionSymbol);
                _exceptionStack.Pop();
            }
            if (syntax.Finally is not null)
            {
                _tryStack.Peek().State = ExceptionHandlingState.Finally;
                finallyTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertStatement(model, syntax.Finally.Block);
                AddInstruction(OpCode.ENDFINALLY);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
            _tryStack.Pop();
        }

        private void ConvertWhileStatement(SemanticModel model, WhileStatementSyntax syntax)
        {
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            using (InsertSequencePoint(syntax.Condition))
            {
                ConvertExpression(model, syntax.Condition);
                Jump(OpCode.JMPIFNOT_L, breakTarget);
            }
            ConvertStatement(model, syntax.Statement);
            Jump(OpCode.JMP_L, continueTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertExpression(SemanticModel model, ExpressionSyntax syntax)
        {
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
                case ConditionalAccessExpressionSyntax expression:
                    ConvertConditionalAccessExpression(model, expression);
                    break;
                case ConditionalExpressionSyntax expression:
                    ConvertConditionalExpression(model, expression);
                    break;
                case DefaultExpressionSyntax expression:
                    ConvertDefaultExpression(model, expression);
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
                case InterpolatedStringExpressionSyntax expression:
                    ConvertInterpolatedStringExpression(model, expression);
                    break;
                case InvocationExpressionSyntax expression:
                    ConvertInvocationExpression(model, expression);
                    break;
                case IsPatternExpressionSyntax expression:
                    ConvertIsPatternExpression(model, expression);
                    break;
                case LiteralExpressionSyntax expression:
                    ConvertLiteralExpression(model, expression);
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
                case SizeOfExpressionSyntax expression:
                    ConvertSizeOfExpression(model, expression);
                    break;
                case SwitchExpressionSyntax expression:
                    ConvertSwitchExpression(model, expression);
                    break;
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
                    throw new NotSupportedException($"Unsupported syntax: {syntax}");
            }
        }

        private void ConvertAnonymousObjectCreationExpression(SemanticModel model, AnonymousObjectCreationExpressionSyntax expression)
        {
            AddInstruction(OpCode.NEWARRAY0);
            foreach (AnonymousObjectMemberDeclaratorSyntax initializer in expression.Initializers)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(model, initializer.Expression);
                AddInstruction(OpCode.APPEND);
            }
        }

        private void ConvertArrayCreationExpression(SemanticModel model, ArrayCreationExpressionSyntax expression)
        {
            if (expression.Type.RankSpecifiers.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {expression.Type.RankSpecifiers}");
            ArrayRankSpecifierSyntax specifier = expression.Type.RankSpecifiers[0];
            if (specifier.Rank != 1)
                throw new NotSupportedException($"Unsupported array rank: {specifier}");
            ITypeSymbol type = model.GetTypeInfo(expression.Type.ElementType).Type!;
            if (type.SpecialType == SpecialType.System_Byte)
            {
                if (expression.Initializer is null)
                {
                    ConvertExpression(model, specifier.Sizes[0]);
                    AddInstruction(OpCode.NEWBUFFER);
                }
                else
                {
                    Optional<object?>[] values = expression.Initializer.Expressions.Select(p => model.GetConstantValue(p)).ToArray();
                    if (values.Any(p => !p.HasValue))
                        throw new NotSupportedException($"Unsupported array initializer: {expression.Initializer}");
                    byte[] data = values.Select(p => (byte)System.Convert.ChangeType(p.Value, typeof(byte))!).ToArray();
                    Push(data);
                    ChangeType(VM.Types.StackItemType.Buffer);
                }
            }
            else
            {
                if (expression.Initializer is null)
                {
                    ConvertExpression(model, specifier.Sizes[0]);
                    AddInstruction(new Instruction { OpCode = OpCode.NEWARRAY_T, Operand = new[] { (byte)type.GetStackItemType() } });
                }
                else
                {
                    AddInstruction(OpCode.NEWARRAY0);
                    foreach (ExpressionSyntax ex in expression.Initializer.Expressions)
                    {
                        AddInstruction(OpCode.DUP);
                        ConvertExpression(model, ex);
                        AddInstruction(OpCode.APPEND);
                    }
                }
            }
        }

        private void ConvertAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "=":
                    ConvertSimpleAssignmentExpression(model, expression);
                    break;
                case "??=":
                    ConvertCoalesceAssignmentExpression(model, expression);
                    break;
                default:
                    ConvertComplexAssignmentExpression(model, expression);
                    break;
            }
        }

        private void ConvertSimpleAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
        {
            ConvertExpression(model, expression.Right);
            AddInstruction(OpCode.DUP);
            switch (expression.Left)
            {
                case DeclarationExpressionSyntax left:
                    ConvertDeclarationAssignment(model, left);
                    break;
                case ElementAccessExpressionSyntax left:
                    ConvertElementAccessAssignment(model, left);
                    break;
                case IdentifierNameSyntax left:
                    ConvertIdentifierNameAssignment(model, left);
                    break;
                case MemberAccessExpressionSyntax left:
                    ConvertMemberAccessAssignment(model, left);
                    break;
                case TupleExpressionSyntax left:
                    ConvertTupleAssignment(model, left);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported assignment: {expression.Left}");
            }
        }

        private void ConvertDeclarationAssignment(SemanticModel model, DeclarationExpressionSyntax left)
        {
            ITypeSymbol type = model.GetTypeInfo(left).Type!;
            if (!type.IsValueType)
                throw new NotSupportedException($"Unsupported assignment type: {type}");
            AddInstruction(OpCode.UNPACK);
            AddInstruction(OpCode.DROP);
            foreach (VariableDesignationSyntax variable in ((ParenthesizedVariableDesignationSyntax)left.Designation).Variables)
            {
                switch (variable)
                {
                    case SingleVariableDesignationSyntax singleVariableDesignation:
                        ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(singleVariableDesignation)!;
                        byte index = AddLocalVariable(local);
                        AccessSlot(OpCode.STLOC, index);
                        break;
                    case DiscardDesignationSyntax:
                        AddInstruction(OpCode.DROP);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported designation: {variable}");
                }
            }
        }

        private void ConvertElementAccessAssignment(SemanticModel model, ElementAccessExpressionSyntax left)
        {
            if (left.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {left.ArgumentList.Arguments}");
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.ROT);
            AddInstruction(OpCode.SETITEM);
        }

        private void ConvertIdentifierNameAssignment(SemanticModel model, IdentifierNameSyntax left)
        {
            ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
            switch (symbol)
            {
                case IDiscardSymbol:
                    AddInstruction(OpCode.DROP);
                    break;
                case IFieldSymbol field:
                    if (field.IsStatic)
                    {
                        byte index = context.AddStaticField(field);
                        AccessSlot(OpCode.STSFLD, index);
                    }
                    else
                    {
                        int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                        AddInstruction(OpCode.LDARG0);
                        Push(index);
                        AddInstruction(OpCode.ROT);
                        AddInstruction(OpCode.SETITEM);
                    }
                    break;
                case ILocalSymbol local:
                    AccessSlot(OpCode.STLOC, _localVariables[local]);
                    break;
                case IParameterSymbol parameter:
                    AccessSlot(OpCode.STARG, _parameters[parameter]);
                    break;
                case IPropertySymbol property:
                    if (!property.IsStatic) AddInstruction(OpCode.LDARG0);
                    Call(model, property.SetMethod!, CallingConvention.Cdecl);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertMemberAccessAssignment(SemanticModel model, MemberAccessExpressionSyntax left)
        {
            ISymbol symbol = model.GetSymbolInfo(left.Name).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    if (field.IsStatic)
                    {
                        byte index = context.AddStaticField(field);
                        AccessSlot(OpCode.STSFLD, index);
                    }
                    else
                    {
                        int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                        ConvertExpression(model, left.Expression);
                        Push(index);
                        AddInstruction(OpCode.ROT);
                        AddInstruction(OpCode.SETITEM);
                    }
                    break;
                case IPropertySymbol property:
                    if (!property.IsStatic) ConvertExpression(model, left.Expression);
                    Call(model, property.SetMethod!, CallingConvention.Cdecl);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertTupleAssignment(SemanticModel model, TupleExpressionSyntax left)
        {
            AddInstruction(OpCode.UNPACK);
            AddInstruction(OpCode.DROP);
            foreach (ArgumentSyntax argument in left.Arguments)
            {
                switch (argument.Expression)
                {
                    case DeclarationExpressionSyntax declaration:
                        switch (declaration.Designation)
                        {
                            case SingleVariableDesignationSyntax singleVariableDesignation:
                                ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(singleVariableDesignation)!;
                                byte index = AddLocalVariable(local);
                                AccessSlot(OpCode.STLOC, index);
                                break;
                            case DiscardDesignationSyntax:
                                AddInstruction(OpCode.DROP);
                                break;
                            default:
                                throw new NotSupportedException($"Unsupported designation: {argument}");
                        }
                        break;
                    case IdentifierNameSyntax identifier:
                        ConvertIdentifierNameAssignment(model, identifier);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported assignment: {left}");
                }
            }
        }

        private void ConvertCoalesceAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
        {
            switch (expression.Left)
            {
                case ElementAccessExpressionSyntax left:
                    ConvertElementAccessCoalesceAssignment(model, left, expression.Right);
                    break;
                case IdentifierNameSyntax left:
                    ConvertIdentifierNameCoalesceAssignment(model, left, expression.Right);
                    break;
                case MemberAccessExpressionSyntax left:
                    ConvertMemberAccessCoalesceAssignment(model, left, expression.Right);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported coalesce assignment: {expression}");
            }
        }

        private void ConvertElementAccessCoalesceAssignment(SemanticModel model, ElementAccessExpressionSyntax left, ExpressionSyntax right)
        {
            if (left.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {left.ArgumentList.Arguments}");
            JumpTarget assignmentTarget = new();
            JumpTarget endTarget = new();
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.PICKITEM);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertIdentifierNameCoalesceAssignment(SemanticModel model, IdentifierNameSyntax left, ExpressionSyntax right)
        {
            ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNameCoalesceAssignment(model, field, right);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNameCoalesceAssignment(model, local, right);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNameCoalesceAssignment(model, parameter, right);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNameCoalesceAssignment(model, property, right);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNameCoalesceAssignment(SemanticModel model, IFieldSymbol left, ExpressionSyntax right)
        {
            JumpTarget assignmentTarget = new();
            JumpTarget endTarget = new();
            if (left.IsStatic)
            {
                byte index = context.AddStaticField(left);
                AccessSlot(OpCode.LDSFLD, index);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIF_L, assignmentTarget);
                AccessSlot(OpCode.LDSFLD, index);
                Jump(OpCode.JMP_L, endTarget);
                assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertExpression(model, right);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(left.ContainingType.GetFields(), left);
                AddInstruction(OpCode.LDARG0);
                Push(index);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIF_L, assignmentTarget);
                AddInstruction(OpCode.PICKITEM);
                Jump(OpCode.JMP_L, endTarget);
                assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertExpression(model, right);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.REVERSE4);
                AddInstruction(OpCode.REVERSE3);
                AddInstruction(OpCode.SETITEM);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertLocalIdentifierNameCoalesceAssignment(SemanticModel model, ILocalSymbol left, ExpressionSyntax right)
        {
            JumpTarget assignmentTarget = new();
            JumpTarget endTarget = new();
            byte index = _localVariables[left];
            AccessSlot(OpCode.LDLOC, index);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AccessSlot(OpCode.LDLOC, index);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STLOC, index);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertParameterIdentifierNameCoalesceAssignment(SemanticModel model, IParameterSymbol left, ExpressionSyntax right)
        {
            JumpTarget assignmentTarget = new();
            JumpTarget endTarget = new();
            byte index = _parameters[left];
            AccessSlot(OpCode.LDARG, index);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AccessSlot(OpCode.LDARG, index);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STARG, index);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertPropertyIdentifierNameCoalesceAssignment(SemanticModel model, IPropertySymbol left, ExpressionSyntax right)
        {
            JumpTarget endTarget = new();
            if (left.IsStatic)
            {
                Call(model, left.GetMethod!);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIFNOT_L, endTarget);
                AddInstruction(OpCode.DROP);
                ConvertExpression(model, right);
                AddInstruction(OpCode.DUP);
                Call(model, left.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                Call(model, left.GetMethod!);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIFNOT_L, endTarget);
                AddInstruction(OpCode.DROP);
                AddInstruction(OpCode.LDARG0);
                ConvertExpression(model, right);
                AddInstruction(OpCode.TUCK);
                Call(model, left.SetMethod!, CallingConvention.StdCall);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right)
        {
            ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessCoalesceAssignment(model, left, right, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessCoalesceAssignment(model, left, right, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right, IFieldSymbol field)
        {
            JumpTarget assignmentTarget = new();
            JumpTarget endTarget = new();
            if (field.IsStatic)
            {
                byte index = context.AddStaticField(field);
                AccessSlot(OpCode.LDSFLD, index);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIF_L, assignmentTarget);
                AccessSlot(OpCode.LDSFLD, index);
                Jump(OpCode.JMP_L, endTarget);
                assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertExpression(model, right);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                ConvertExpression(model, left.Expression);
                Push(index);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIF_L, assignmentTarget);
                AddInstruction(OpCode.PICKITEM);
                Jump(OpCode.JMP_L, endTarget);
                assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertExpression(model, right);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.REVERSE4);
                AddInstruction(OpCode.REVERSE3);
                AddInstruction(OpCode.SETITEM);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertPropertyMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right, IPropertySymbol property)
        {
            JumpTarget endTarget = new();
            if (property.IsStatic)
            {
                Call(model, property.GetMethod!);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIFNOT_L, endTarget);
                AddInstruction(OpCode.DROP);
                ConvertExpression(model, right);
                AddInstruction(OpCode.DUP);
                Call(model, property.SetMethod!);
            }
            else
            {
                JumpTarget assignmentTarget = new();
                ConvertExpression(model, left.Expression);
                AddInstruction(OpCode.DUP);
                Call(model, property.GetMethod!);
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.ISNULL);
                Jump(OpCode.JMPIF_L, assignmentTarget);
                AddInstruction(OpCode.NIP);
                Jump(OpCode.JMP_L, endTarget);
                assignmentTarget.Instruction = AddInstruction(OpCode.DROP);
                ConvertExpression(model, right);
                AddInstruction(OpCode.TUCK);
                Call(model, property.SetMethod!, CallingConvention.StdCall);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertComplexAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            switch (expression.Left)
            {
                case ElementAccessExpressionSyntax left:
                    ConvertElementAccessComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                    break;
                case IdentifierNameSyntax left:
                    ConvertIdentifierNameComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                    break;
                case MemberAccessExpressionSyntax left:
                    ConvertMemberAccessComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported assignment expression: {expression}");
            }
        }

        private void ConvertElementAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, ElementAccessExpressionSyntax left, ExpressionSyntax right)
        {
            if (left.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {left.ArgumentList.Arguments}");
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }

        private void ConvertIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IdentifierNameSyntax left, ExpressionSyntax right)
        {
            ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNameComplexAssignment(model, type, operatorToken, field, right);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNameComplexAssignment(model, type, operatorToken, local, right);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNameComplexAssignment(model, type, operatorToken, parameter, right);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNameComplexAssignment(model, type, operatorToken, property, right);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IFieldSymbol left, ExpressionSyntax right)
        {
            if (left.IsStatic)
            {
                byte index = context.AddStaticField(left);
                AccessSlot(OpCode.LDSFLD, index);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(left.ContainingType.GetFields(), left);
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.TUCK);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertLocalIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, ILocalSymbol left, ExpressionSyntax right)
        {
            byte index = _localVariables[left];
            AccessSlot(OpCode.LDLOC, index);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STLOC, index);
        }

        private void ConvertParameterIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IParameterSymbol left, ExpressionSyntax right)
        {
            byte index = _parameters[left];
            AccessSlot(OpCode.LDARG, index);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STARG, index);
        }

        private void ConvertPropertyIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IPropertySymbol left, ExpressionSyntax right)
        {
            if (left.IsStatic)
            {
                Call(model, left.GetMethod!);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.DUP);
                Call(model, left.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Call(model, left.GetMethod!);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(model, left.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right)
        {
            ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessComplexAssignment(model, type, operatorToken, left, right, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessComplexAssignment(model, type, operatorToken, left, right, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right, IFieldSymbol field)
        {
            if (field.IsStatic)
            {
                byte index = context.AddStaticField(field);
                AccessSlot(OpCode.LDSFLD, index);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                ConvertExpression(model, left.Expression);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.TUCK);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertPropertyMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right, IPropertySymbol property)
        {
            if (property.IsStatic)
            {
                Call(model, property.GetMethod!);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.DUP);
                Call(model, property.SetMethod!);
            }
            else
            {
                ConvertExpression(model, left.Expression);
                AddInstruction(OpCode.DUP);
                Call(model, property.GetMethod!);
                ConvertExpression(model, right);
                EmitComplexAssignmentOperator(type, operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(model, property.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void EmitComplexAssignmentOperator(ITypeSymbol type, SyntaxToken operatorToken)
        {
            bool isBoolean = type.SpecialType == SpecialType.System_Boolean;
            AddInstruction(operatorToken.ValueText switch
            {
                "+=" => OpCode.ADD,
                "-=" => OpCode.SUB,
                "*=" => OpCode.MUL,
                "/=" => OpCode.DIV,
                "%=" => OpCode.MOD,
                "&=" => isBoolean ? OpCode.BOOLAND : OpCode.AND,
                "^=" when !isBoolean => OpCode.XOR,
                "|=" => isBoolean ? OpCode.BOOLOR : OpCode.OR,
                "<<=" => OpCode.SHL,
                ">>=" => OpCode.SHR,
                _ => throw new NotSupportedException($"Unsupported operator: {operatorToken}")
            });
        }

        private void ConvertObjectCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            IMethodSymbol constructor = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
            IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList?.Arguments ?? (IReadOnlyList<ArgumentSyntax>)Array.Empty<ArgumentSyntax>();
            if (TryProcessSystemConstructors(model, constructor, arguments))
                return;
            bool needCreateObject = !type.DeclaringSyntaxReferences.IsEmpty && !constructor.IsExtern;
            if (needCreateObject)
            {
                IFieldSymbol[] fields = type.GetFields();
                AddInstruction(type.IsValueType ? OpCode.NEWSTRUCT0 : OpCode.NEWARRAY0);
                foreach (IFieldSymbol field in fields)
                {
                    ExpressionSyntax? right = null;
                    if (expression.Initializer is not null)
                    {
                        foreach (ExpressionSyntax e in expression.Initializer.Expressions)
                        {
                            if (e is not AssignmentExpressionSyntax ae)
                                throw new NotSupportedException($"Unsupported initializer: {expression.Initializer}");
                            if (SymbolEqualityComparer.Default.Equals(field, model.GetSymbolInfo(ae.Left).Symbol))
                            {
                                right = ae.Right;
                                break;
                            }
                        }
                    }
                    AddInstruction(OpCode.DUP);
                    if (right is null)
                        PushDefault(field.Type);
                    else
                        ConvertExpression(model, right);
                    AddInstruction(OpCode.APPEND);
                }
            }
            Call(model, constructor, needCreateObject, arguments);
        }

        private void ConvertBinaryExpression(SemanticModel model, BinaryExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "is":
                    ConvertIsExpression(model, expression.Left, expression.Right);
                    return;
                case "as":
                    ConvertAsExpression(model, expression.Left, expression.Right);
                    return;
                case "??":
                    ConvertCoalesceExpression(model, expression.Left, expression.Right);
                    return;
            }
            IMethodSymbol? symbol = (IMethodSymbol?)model.GetSymbolInfo(expression).Symbol;
            if (symbol is not null && TryProcessSystemOperators(model, symbol, expression.Left, expression.Right))
                return;
            ConvertExpression(model, expression.Left);
            ConvertExpression(model, expression.Right);
            AddInstruction(expression.OperatorToken.ValueText switch
            {
                "+" => OpCode.ADD,
                "-" => OpCode.SUB,
                "*" => OpCode.MUL,
                "/" => OpCode.DIV,
                "%" => OpCode.MOD,
                "<<" => OpCode.SHL,
                ">>" => OpCode.SHR,
                "||" => OpCode.BOOLOR,
                "&&" => OpCode.BOOLAND,
                "|" => OpCode.OR,
                "&" => OpCode.AND,
                "^" => OpCode.XOR,
                "==" => OpCode.NUMEQUAL,
                "!=" => OpCode.NUMNOTEQUAL,
                "<" => OpCode.LT,
                "<=" => OpCode.LE,
                ">" => OpCode.GT,
                ">=" => OpCode.GE,
                _ => throw new NotSupportedException($"Unsupported operator: {expression.OperatorToken}")
            });
        }

        private void ConvertIsExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
        {
            ITypeSymbol type = model.GetTypeInfo(right).Type!;
            ConvertExpression(model, left);
            IsType(type.GetPatternType());
        }

        private void ConvertAsExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
        {
            JumpTarget endTarget = new();
            ITypeSymbol type = model.GetTypeInfo(right).Type!;
            ConvertExpression(model, left);
            AddInstruction(OpCode.DUP);
            IsType(type.GetPatternType());
            Jump(OpCode.JMPIF_L, endTarget);
            AddInstruction(OpCode.DROP);
            Push((object?)null);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertCoalesceExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
        {
            JumpTarget endTarget = new();
            ConvertExpression(model, left);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertCastExpression(SemanticModel model, CastExpressionSyntax expression)
        {
            IMethodSymbol? symbol = (IMethodSymbol?)model.GetSymbolInfo(expression).Symbol;
            if (symbol is null)
                ConvertExpression(model, expression.Expression);
            else
                Call(model, symbol, null, expression.Expression);
        }

        private void ConvertConditionalAccessExpression(SemanticModel model, ConditionalAccessExpressionSyntax expression)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            JumpTarget nullTarget = new();
            ConvertExpression(model, expression.Expression);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, nullTarget);
            ConvertExpression(model, expression.WhenNotNull);
            if (type.SpecialType == SpecialType.System_Void)
            {
                JumpTarget endTarget = new();
                Jump(OpCode.JMP_L, endTarget);
                nullTarget.Instruction = AddInstruction(OpCode.DROP);
                endTarget.Instruction = AddInstruction(OpCode.NOP);
            }
            else
            {
                nullTarget.Instruction = AddInstruction(OpCode.NOP);
            }
        }

        private void ConvertConditionalExpression(SemanticModel model, ConditionalExpressionSyntax expression)
        {
            JumpTarget falseTarget = new();
            JumpTarget endTarget = new();
            ConvertExpression(model, expression.Condition);
            Jump(OpCode.JMPIFNOT_L, falseTarget);
            ConvertExpression(model, expression.WhenTrue);
            Jump(OpCode.JMP_L, endTarget);
            falseTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, expression.WhenFalse);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertDefaultExpression(SemanticModel model, DefaultExpressionSyntax expression)
        {
            Push(model.GetConstantValue(expression).Value);
        }

        private void ConvertElementAccessExpression(SemanticModel model, ElementAccessExpressionSyntax expression)
        {
            if (expression.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {expression.ArgumentList.Arguments}");
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            ConvertExpression(model, expression.Expression);
            ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
        }

        private void ConvertElementBindingExpression(SemanticModel model, ElementBindingExpressionSyntax expression)
        {
            if (expression.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {expression.ArgumentList.Arguments}");
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
        }

        private void ConvertIndexOrRange(SemanticModel model, ITypeSymbol type, ExpressionSyntax indexOrRange)
        {
            if (indexOrRange is RangeExpressionSyntax range)
            {
                if (range.RightOperand is null)
                {
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                }
                else
                {
                    ConvertExpression(model, range.RightOperand);
                }
                AddInstruction(OpCode.SWAP);
                if (range.LeftOperand is null)
                {
                    Push(0);
                }
                else
                {
                    ConvertExpression(model, range.LeftOperand);
                }
                AddInstruction(OpCode.ROT);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.SUB);
                switch (type.ToString())
                {
                    case "byte[]":
                        AddInstruction(OpCode.SUBSTR);
                        break;
                    case "string":
                        AddInstruction(OpCode.SUBSTR);
                        ChangeType(VM.Types.StackItemType.ByteString);
                        break;
                    default:
                        throw new NotSupportedException($"The type {type} does not support range access.");
                }
            }
            else
            {
                ConvertExpression(model, indexOrRange);
                AddInstruction(OpCode.PICKITEM);
            }
        }

        private void ConvertIdentifierNameExpression(SemanticModel model, IdentifierNameSyntax expression)
        {
            ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    if (field.IsConst)
                    {
                        Push(field.ConstantValue);
                    }
                    else if (field.IsStatic)
                    {
                        byte index = context.AddStaticField(field);
                        AccessSlot(OpCode.LDSFLD, index);
                    }
                    else
                    {
                        int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                        AddInstruction(OpCode.LDARG0);
                        Push(index);
                        AddInstruction(OpCode.PICKITEM);
                    }
                    break;
                case ILocalSymbol local:
                    if (local.IsConst)
                        Push(local.ConstantValue);
                    else
                        AccessSlot(OpCode.LDLOC, _localVariables[local]);
                    break;
                case IMethodSymbol method:
                    if (!method.IsStatic)
                        throw new NotSupportedException($"Unsupported delegate: {method}");
                    MethodConvert convert = context.ConvertMethod(model, method);
                    Jump(OpCode.PUSHA, convert._startTarget);
                    break;
                case IParameterSymbol parameter:
                    AccessSlot(OpCode.LDARG, _parameters[parameter]);
                    break;
                case IPropertySymbol property:
                    if (property.IsStatic)
                    {
                        Call(model, property.GetMethod!);
                    }
                    else
                    {
                        AddInstruction(OpCode.LDARG0);
                        Call(model, property.GetMethod!);
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertImplicitArrayCreationExpression(SemanticModel model, ImplicitArrayCreationExpressionSyntax expression)
        {
            AddInstruction(OpCode.NEWARRAY0);
            foreach (ExpressionSyntax ex in expression.Initializer.Expressions)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(model, ex);
                AddInstruction(OpCode.APPEND);
            }
        }

        private void ConvertInterpolatedStringExpression(SemanticModel model, InterpolatedStringExpressionSyntax expression)
        {
            if (expression.Contents.Count == 0)
            {
                Push(string.Empty);
                return;
            }
            ConvertInterpolatedStringContent(model, expression.Contents[0]);
            for (int i = 1; i < expression.Contents.Count; i++)
            {
                ConvertInterpolatedStringContent(model, expression.Contents[i]);
                AddInstruction(OpCode.CAT);
            }
            if (expression.Contents.Count >= 2)
                ChangeType(VM.Types.StackItemType.ByteString);
        }

        private void ConvertInterpolatedStringContent(SemanticModel model, InterpolatedStringContentSyntax content)
        {
            switch (content)
            {
                case InterpolatedStringTextSyntax syntax:
                    Push(syntax.TextToken.ValueText);
                    break;
                case InterpolationSyntax syntax:
                    if (syntax.AlignmentClause is not null)
                        throw new NotSupportedException($"Unsupported alignment clause: {syntax.AlignmentClause}");
                    if (syntax.FormatClause is not null)
                        throw new NotSupportedException($"Unsupported format clause: {syntax.FormatClause}");
                    ConvertObjectToString(model, syntax.Expression);
                    break;
            }
        }

        private void ConvertObjectToString(SemanticModel model, ExpressionSyntax expression)
        {
            ITypeSymbol? type = model.GetTypeInfo(expression).Type;
            switch (type?.SpecialType)
            {
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.None when type.Name == nameof(BigInteger):
                    ConvertExpression(model, expression);
                    Call(NativeContract.StdLib.Hash, "itoa", 1, true);
                    break;
                case SpecialType.System_String:
                    ConvertExpression(model, expression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported interpolation: {expression}");
            }
        }

        private void ConvertInvocationExpression(SemanticModel model, InvocationExpressionSyntax expression)
        {
            ArgumentSyntax[] arguments = expression.ArgumentList.Arguments.ToArray();
            ISymbol symbol = model.GetSymbolInfo(expression.Expression).Symbol!;
            switch (symbol)
            {
                case IEventSymbol @event:
                    ConvertEventInvocationExpression(model, @event, arguments);
                    break;
                case IMethodSymbol method:
                    ConvertMethodInvocationExpression(model, method, expression.Expression, arguments);
                    break;
                default:
                    ConvertDelegateInvocationExpression(model, expression.Expression, arguments);
                    break;
            }
        }

        private void ConvertEventInvocationExpression(SemanticModel model, IEventSymbol symbol, ArgumentSyntax[] arguments)
        {
            AddInstruction(OpCode.NEWARRAY0);
            foreach (ArgumentSyntax argument in arguments)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(model, argument.Expression);
                AddInstruction(OpCode.APPEND);
            }
            Push(symbol.GetDisplayName());
            Call(ApplicationEngine.System_Runtime_Notify);
        }

        private void ConvertMethodInvocationExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax expression, ArgumentSyntax[] arguments)
        {
            switch (expression)
            {
                case IdentifierNameSyntax:
                    Call(model, symbol, null, arguments);
                    break;
                case MemberAccessExpressionSyntax syntax:
                    if (symbol.IsStatic)
                        Call(model, symbol, null, arguments);
                    else
                        Call(model, symbol, syntax.Expression, arguments);
                    break;
                case MemberBindingExpressionSyntax:
                    Call(model, symbol, true, arguments);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression: {expression}");
            }
        }

        private void ConvertDelegateInvocationExpression(SemanticModel model, ExpressionSyntax expression, ArgumentSyntax[] arguments)
        {
            INamedTypeSymbol type = (INamedTypeSymbol)model.GetTypeInfo(expression).Type!;
            PrepareArgumentsForMethod(model, type.DelegateInvokeMethod!, arguments);
            ConvertExpression(model, expression);
            AddInstruction(OpCode.CALLA);
        }

        private void ConvertIsPatternExpression(SemanticModel model, IsPatternExpressionSyntax expression)
        {
            byte anonymousIndex = AddAnonymousVariable();
            ConvertExpression(model, expression.Expression);
            AccessSlot(OpCode.STLOC, anonymousIndex);
            ConvertPattern(model, expression.Pattern, anonymousIndex);
            _anonymousVariables.Remove(anonymousIndex);
        }

        private void ConvertLiteralExpression(SemanticModel model, LiteralExpressionSyntax expression)
        {
            Push(model.GetConstantValue(expression).Value);
        }

        private void ConvertMemberAccessExpression(SemanticModel model, MemberAccessExpressionSyntax expression)
        {
            ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    if (field.IsConst)
                    {
                        Push(field.ConstantValue);
                    }
                    else if (field.IsStatic)
                    {
                        byte index = context.AddStaticField(field);
                        AccessSlot(OpCode.LDSFLD, index);
                    }
                    else
                    {
                        int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                        ConvertExpression(model, expression.Expression);
                        Push(index);
                        AddInstruction(OpCode.PICKITEM);
                    }
                    break;
                case IMethodSymbol method:
                    if (!method.IsStatic)
                        throw new NotSupportedException($"Unsupported delegate: {method}");
                    MethodConvert convert = context.ConvertMethod(model, method);
                    Jump(OpCode.PUSHA, convert._startTarget);
                    break;
                case IPropertySymbol property:
                    ExpressionSyntax? instanceExpression = property.IsStatic ? null : expression.Expression;
                    Call(model, property.GetMethod!, instanceExpression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertMemberBindingExpression(SemanticModel model, MemberBindingExpressionSyntax expression)
        {
            ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    Push(index);
                    AddInstruction(OpCode.PICKITEM);
                    break;
                case IPropertySymbol property:
                    Call(model, property.GetMethod!);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertPostfixUnaryExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "++":
                case "--":
                    ConvertPostIncrementOrDecrementExpression(model, expression);
                    break;
                case "!":
                    ConvertExpression(model, expression.Operand);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported operator: {expression.OperatorToken}");
            }
        }

        private void ConvertPostIncrementOrDecrementExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
        {
            switch (expression.Operand)
            {
                case ElementAccessExpressionSyntax operand:
                    ConvertElementAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case IdentifierNameSyntax operand:
                    ConvertIdentifierNamePostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case MemberAccessExpressionSyntax operand:
                    ConvertMemberAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported postfix unary expression: {expression}");
            }
        }

        private void ConvertElementAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
        {
            if (operand.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {operand.ArgumentList.Arguments}");
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.SETITEM);
        }

        private void ConvertIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(operatorToken, field);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(operatorToken, local);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(operatorToken, parameter);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(model, operatorToken, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
        {
            byte index = _localVariables[symbol];
            AccessSlot(OpCode.LDLOC, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken);
            AccessSlot(OpCode.STLOC, index);
        }

        private void ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
        {
            byte index = _parameters[symbol];
            AccessSlot(OpCode.LDARG, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken);
            AccessSlot(OpCode.STARG, index);
        }

        private void ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertPrefixUnaryExpression(SemanticModel model, PrefixUnaryExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "+":
                    ConvertExpression(model, expression.Operand);
                    break;
                case "-":
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.NEGATE);
                    break;
                case "~":
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.INVERT);
                    break;
                case "!":
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.NOT);
                    break;
                case "++":
                case "--":
                    ConvertPreIncrementOrDecrementExpression(model, expression);
                    break;
                case "^":
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    ConvertExpression(model, expression.Operand);
                    AddInstruction(OpCode.SUB);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported operator: {expression.OperatorToken}");
            }
        }

        private void ConvertPreIncrementOrDecrementExpression(SemanticModel model, PrefixUnaryExpressionSyntax expression)
        {
            switch (expression.Operand)
            {
                case ElementAccessExpressionSyntax operand:
                    ConvertElementAccessPreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case IdentifierNameSyntax operand:
                    ConvertIdentifierNamePreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                case MemberAccessExpressionSyntax operand:
                    ConvertMemberAccessPreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported postfix unary expression: {expression}");
            }
        }

        private void ConvertElementAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
        {
            if (operand.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {operand.ArgumentList.Arguments}");
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }

        private void ConvertIdentifierNamePreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(operatorToken, field);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(operatorToken, local);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(operatorToken, parameter);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(model, operatorToken, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
        {
            byte index = _localVariables[symbol];
            AccessSlot(OpCode.LDLOC, index);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STLOC, index);
        }

        private void ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
        {
            byte index = _parameters[symbol];
            AccessSlot(OpCode.LDARG, index);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STARG, index);
        }

        private void ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessPreIncrementOrDecrementExpression(model, operatorToken, operand, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(model, operatorToken, operand, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
        {
            if (symbol.IsStatic)
            {
                byte index = context.AddStaticField(symbol);
                AccessSlot(OpCode.LDSFLD, index);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STSFLD, index);
            }
            else
            {
                int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Push(index);
                AddInstruction(OpCode.SWAP);
                AddInstruction(OpCode.SETITEM);
            }
        }

        private void ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.SetMethod!);
            }
            else
            {
                ConvertExpression(model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Call(model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void EmitIncrementOrDecrement(SyntaxToken operatorToken)
        {
            AddInstruction(operatorToken.ValueText switch
            {
                "++" => OpCode.INC,
                "--" => OpCode.DEC,
                _ => throw new NotSupportedException($"Unsupported operator: {operatorToken}")
            });
        }

        private void ConvertSizeOfExpression(SemanticModel model, SizeOfExpressionSyntax expression)
        {
            Push((int)model.GetConstantValue(expression).Value!);
        }

        private void ConvertSwitchExpression(SemanticModel model, SwitchExpressionSyntax expression)
        {
            var arms = expression.Arms.Select(p => (p, new JumpTarget())).ToArray();
            JumpTarget breakTarget = new();
            byte anonymousIndex = AddAnonymousVariable();
            ConvertExpression(model, expression.GoverningExpression);
            AccessSlot(OpCode.STLOC, anonymousIndex);
            foreach (var (arm, nextTarget) in arms)
            {
                ConvertPattern(model, arm.Pattern, anonymousIndex);
                Jump(OpCode.JMPIFNOT_L, nextTarget);
                if (arm.WhenClause is not null)
                {
                    ConvertExpression(model, arm.WhenClause.Condition);
                    Jump(OpCode.JMPIFNOT_L, nextTarget);
                }
                ConvertExpression(model, arm.Expression);
                Jump(OpCode.JMP_L, breakTarget);
                nextTarget.Instruction = AddInstruction(OpCode.NOP);
            }
            AccessSlot(OpCode.LDLOC, anonymousIndex);
            AddInstruction(OpCode.THROW);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _anonymousVariables.Remove(anonymousIndex);
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
                    throw new NotSupportedException($"Unsupported pattern: {pattern}");
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
                    throw new NotSupportedException($"Unsupported pattern: {pattern}");
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
                    throw new NotSupportedException($"Unsupported pattern: {pattern}");
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
                _ => throw new NotSupportedException($"Unsupported pattern: {pattern}")
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

        private void ConvertTupleExpression(SemanticModel model, TupleExpressionSyntax expression)
        {
            AddInstruction(OpCode.NEWSTRUCT0);
            foreach (ArgumentSyntax argument in expression.Arguments)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(model, argument.Expression);
                AddInstruction(OpCode.APPEND);
            }
        }

        private Instruction Push(bool value)
        {
            return AddInstruction(value ? OpCode.PUSH1 : OpCode.PUSH0);
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

        private Instruction Push(object? obj)
        {
            return obj switch
            {
                bool data => Push(data),
                byte[] data => Push(data),
                string data => Push(data),
                BigInteger data => Push(data),
                sbyte data => Push(data),
                byte data => Push(data),
                short data => Push(data),
                ushort data => Push(data),
                int data => Push(data),
                uint data => Push(data),
                long data => Push(data),
                ulong data => Push(data),
                Enum data => Push(BigInteger.Parse(data.ToString("d"))),
                null => AddInstruction(OpCode.PUSHNULL),
                _ => throw new NotSupportedException($"Unsupported constant value: {obj}"),
            };
        }

        private Instruction PushDefault(ITypeSymbol type)
        {
            return AddInstruction(type.GetStackItemType() switch
            {
                VM.Types.StackItemType.Boolean or VM.Types.StackItemType.Integer => OpCode.PUSH0,
                _ => OpCode.PUSHNULL,
            });
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
                            throw new NotSupportedException("Only a single parameter is supported for exceptions.");
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
            MethodConvert convert = context.ConvertMethod(model, symbol);
            bool isConstructor = symbol.MethodKind == MethodKind.Constructor;
            if (instanceOnStack && convert._callingConvention != CallingConvention.Cdecl && isConstructor)
                AddInstruction(OpCode.DUP);
            PrepareArgumentsForMethod(model, symbol, arguments, convert._callingConvention);
            if (instanceOnStack && convert._callingConvention == CallingConvention.Cdecl)
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
            EmitCall(convert);
        }

        private void Call(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, params SyntaxNode[] arguments)
        {
            if (TryProcessSystemMethods(model, symbol, instanceExpression, arguments))
                return;
            MethodConvert convert = context.ConvertMethod(model, symbol);
            if (!symbol.IsStatic && convert._callingConvention != CallingConvention.Cdecl)
            {
                if (instanceExpression is null)
                    AddInstruction(OpCode.LDARG0);
                else
                    ConvertExpression(model, instanceExpression);
            }
            PrepareArgumentsForMethod(model, symbol, arguments, convert._callingConvention);
            if (!symbol.IsStatic && convert._callingConvention == CallingConvention.Cdecl)
            {
                if (instanceExpression is null)
                    AddInstruction(OpCode.LDARG0);
                else
                    ConvertExpression(model, instanceExpression);
            }
            EmitCall(convert);
        }

        private void Call(SemanticModel model, IMethodSymbol symbol, CallingConvention callingConvention = CallingConvention.Cdecl)
        {
            if (TryProcessSystemMethods(model, symbol, null, null))
                return;
            MethodConvert convert = context.ConvertMethod(model, symbol);
            int pc = symbol.Parameters.Length;
            if (!symbol.IsStatic) pc++;
            if (pc > 1 && convert._callingConvention != callingConvention)
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
            EmitCall(convert);
        }

        private void PrepareArgumentsForMethod(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention = CallingConvention.Cdecl)
        {
            var namedArguments = arguments.OfType<ArgumentSyntax>().Where(p => p.NameColon is not null).Select(p => (Symbol: (IParameterSymbol)model.GetSymbolInfo(p.NameColon!.Name).Symbol!, p.Expression)).ToDictionary(p => p.Symbol, p => p.Expression);
            var parameters = symbol.Parameters.Select((p, i) => (Symbol: p, Index: i));
            if (callingConvention == CallingConvention.Cdecl)
                parameters = parameters.Reverse();
            foreach (var (parameter, index) in parameters)
            {
                if (namedArguments.TryGetValue(parameter, out ExpressionSyntax? expression))
                {
                    ConvertExpression(model, expression);
                }
                else
                {
                    if (arguments.Count > index)
                    {
                        switch (arguments[index])
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
                                throw new Exception("Unknown exception.");
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
                    ChangeType(VM.Types.StackItemType.Boolean);
                    return true;
                case "System.Numerics.BigInteger.IsOne.get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Push(1);
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
                case "System.Numerics.BigInteger.ToByteArray()":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    ChangeType(VM.Types.StackItemType.Buffer);
                    return true;
                case "System.Numerics.BigInteger.explicit operator sbyte(System.Numerics.BigInteger)":
                case "System.Numerics.BigInteger.explicit operator byte(System.Numerics.BigInteger)":
                case "System.Numerics.BigInteger.explicit operator short(System.Numerics.BigInteger)":
                case "System.Numerics.BigInteger.explicit operator ushort(System.Numerics.BigInteger)":
                case "System.Numerics.BigInteger.explicit operator int(System.Numerics.BigInteger)":
                case "System.Numerics.BigInteger.explicit operator uint(System.Numerics.BigInteger)":
                case "System.Numerics.BigInteger.explicit operator long(System.Numerics.BigInteger)":
                case "System.Numerics.BigInteger.explicit operator ulong(System.Numerics.BigInteger)":
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
                case "byte.Parse(string)":
                case "short.Parse(string)":
                case "ushort.Parse(string)":
                case "int.Parse(string)":
                case "uint.Parse(string)":
                case "long.Parse(string)":
                case "ulong.Parse(string)":
                case "System.Numerics.BigInteger.Parse(string)":
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
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
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.EQUAL);
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
            if (target._inline && !context.Options.NoInline)
                for (int i = 0; i < target._instructions.Count - 1; i++)
                    AddInstruction(target._instructions[i].Clone());
            else
                Jump(OpCode.CALL_L, target._startTarget);
        }
    }

    class MethodConvertCollection : KeyedCollection<IMethodSymbol, MethodConvert>
    {
        protected override IMethodSymbol GetKeyForItem(MethodConvert item) => item.Symbol;
    }
}
