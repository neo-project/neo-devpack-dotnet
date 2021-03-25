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
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Neo.Compiler
{
    class MethodConvert
    {
        private CallingConvention _callingConvention = CallingConvention.Cdecl;
        private bool _inline;
        private readonly Dictionary<IParameterSymbol, byte> _parameters = new();
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

        public IReadOnlyList<Instruction> Instructions => _instructions;

        private byte AddLocalVariable(ILocalSymbol symbol)
        {
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

        public void Convert(CompilationContext context, SemanticModel model, IMethodSymbol symbol)
        {
            if (symbol.MethodKind == MethodKind.StaticConstructor)
                ProcessFields(context, model, symbol);
            if (symbol.DeclaringSyntaxReferences.Length > 0 && !symbol.IsExtern)
                ConvertSource(context, model, symbol);
            else if (symbol.MethodKind != MethodKind.StaticConstructor)
                ConvertExtern(context, symbol);
            if (symbol.MethodKind == MethodKind.StaticConstructor && context.StaticFieldsCount > 0)
            {
                _instructions.Insert(0, new Instruction
                {
                    OpCode = OpCode.INITSSLOT,
                    Operand = new[] { (byte)context.StaticFieldsCount }
                });
            }
            _returnTarget.Instruction = AddInstruction(OpCode.RET);
            _startTarget.Instruction = _instructions[0];
        }

        private void ProcessFields(CompilationContext context, SemanticModel model, IMethodSymbol symbol)
        {
            foreach (IFieldSymbol field in symbol.ContainingType.GetMembers().OfType<IFieldSymbol>())
            {
                if (field.IsConst || !field.IsStatic) continue;
                AttributeData? initialValue = field.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx.Neo.SmartContract.Framework.InitialValueAttribute));
                if (initialValue is null)
                {
                    VariableDeclaratorSyntax syntax = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences[0].GetSyntax();
                    if (syntax.Initializer is null) continue;
                    model = model.Compilation.GetSemanticModel(syntax.SyntaxTree);
                    ConvertExpression(context, model, syntax.Initializer.Value);
                }
                else
                {
                    string value = (string)initialValue.ConstructorArguments[0].Value!;
                    ContractParameterType type = (ContractParameterType)initialValue.ConstructorArguments[1].Value!;
                    switch (type)
                    {
                        case ContractParameterType.ByteArray:
                            Push(value.HexToBytes(true));
                            break;
                        case ContractParameterType.Hash160:
                            //TODO: Read AddressVersion from settings file.
                            Push(value.ToScriptHash(ProtocolSettings.Default.AddressVersion).ToArray());
                            break;
                        case ContractParameterType.PublicKey:
                            Push(ECPoint.Parse(value, ECCurve.Secp256r1).EncodePoint(true));
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported initial value type: {type}");
                    }
                }
                byte index = context.AddStaticField(field);
                AccessSlot(OpCode.STSFLD, index);
            }
        }

        private void ConvertExtern(CompilationContext context, IMethodSymbol symbol)
        {
            _inline = true;
            AttributeData? contractAttribute = symbol.ContainingType.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx.Neo.SmartContract.Framework.ContractAttribute));
            if (contractAttribute is null)
            {
                bool emitted = false;
                foreach (AttributeData attribute in symbol.GetAttributes())
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
                    }
                }
                if (!emitted) throw new NotSupportedException($"Unknown method: {symbol}");
            }
            else
            {
                UInt160 hash = UInt160.Parse((string)contractAttribute.ConstructorArguments[0].Value!);
                AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx.Neo.SmartContract.Framework.ContractHashAttribute));
                if (attribute is null)
                {
                    string method = symbol.GetDisplayName(true);
                    ushort parametersCount = (ushort)symbol.Parameters.Length;
                    bool hasReturnValue = !symbol.ReturnsVoid || symbol.MethodKind == MethodKind.Constructor;
                    Call(context, hash, method, parametersCount, hasReturnValue);
                }
                else
                {
                    Push(hash.ToArray());
                }
            }
        }

        private void ConvertNoBody(CompilationContext context, IMethodSymbol symbol)
        {
            _inline = true;
            _callingConvention = CallingConvention.Cdecl;
            IPropertySymbol property = (IPropertySymbol)symbol.AssociatedSymbol!;
            INamedTypeSymbol type = property.ContainingType;
            IFieldSymbol[] fields = type.GetMembers().OfType<IFieldSymbol>().ToArray();
            int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
            IFieldSymbol backingField = fields[backingFieldIndex];
            switch (symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    if (symbol.IsStatic)
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
                    if (symbol.IsStatic)
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
                    throw new NotSupportedException($"Unsupported accessor: {symbol}");
            }
        }

        private void ConvertSource(CompilationContext context, SemanticModel model, IMethodSymbol symbol)
        {
            var body = symbol.DeclaringSyntaxReferences[0].GetSyntax();
            for (byte i = 0; i < symbol.Parameters.Length; i++)
            {
                IParameterSymbol parameter = symbol.Parameters[i];
                byte index = i;
                if (!symbol.IsStatic) index++;
                _parameters.Add(parameter, index);
            }
            switch (body)
            {
                case AccessorDeclarationSyntax syntax:
                    if (syntax.Body is not null)
                        ConvertStatement(context, model, syntax.Body);
                    else if (syntax.ExpressionBody is not null)
                        ConvertExpression(context, model, syntax.ExpressionBody.Expression);
                    else
                        ConvertNoBody(context, symbol);
                    break;
                case BaseMethodDeclarationSyntax syntax:
                    if (syntax.Body is null)
                        ConvertExpression(context, model, syntax.ExpressionBody!.Expression);
                    else
                        ConvertStatement(context, model, syntax.Body);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported method body:{body}");
            }
            if (!_inline)
            {
                byte pc = (byte)_parameters.Count;
                byte lc = (byte)_localsCount;
                if (!symbol.IsStatic) pc++;
                if (pc == 0 && lc == 0) return;
                _instructions.Insert(0, new Instruction
                {
                    OpCode = OpCode.INITSLOT,
                    Operand = new[] { pc, lc }
                });
            }
        }

        private void ConvertStatement(CompilationContext context, SemanticModel model, StatementSyntax statement)
        {
            switch (statement)
            {
                case BlockSyntax syntax:
                    ConvertBlockStatement(context, model, syntax);
                    break;
                case BreakStatementSyntax:
                    ConvertBreakStatement();
                    break;
                case ContinueStatementSyntax:
                    ConvertContinueStatement();
                    break;
                case DoStatementSyntax syntax:
                    ConvertDoStatement(context, model, syntax);
                    break;
                case EmptyStatementSyntax:
                    break;
                case ExpressionStatementSyntax syntax:
                    ConvertExpressionStatement(context, model, syntax);
                    break;
                case ForEachStatementSyntax syntax:
                    ConvertForEachStatement(context, model, syntax);
                    break;
                case ForEachVariableStatementSyntax syntax:
                    ConvertForEachVariableStatement(context, model, syntax);
                    break;
                case ForStatementSyntax syntax:
                    ConvertForStatement(context, model, syntax);
                    break;
                case IfStatementSyntax syntax:
                    ConvertIfStatement(context, model, syntax);
                    break;
                case LocalDeclarationStatementSyntax syntax:
                    ConvertLocalDeclarationStatement(context, model, syntax);
                    break;
                case LocalFunctionStatementSyntax:
                    break;
                case ReturnStatementSyntax syntax:
                    ConvertReturnStatement(context, model, syntax);
                    break;
                case SwitchStatementSyntax syntax:
                    ConvertSwitchStatement(context, model, syntax);
                    break;
                case ThrowStatementSyntax syntax:
                    Throw(context, model, syntax.Expression);
                    break;
                case TryStatementSyntax syntax:
                    ConvertTryStatement(context, model, syntax);
                    break;
                case WhileStatementSyntax syntax:
                    ConvertWhileStatement(context, model, syntax);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported syntax: {statement}");
            }
        }

        private void ConvertBlockStatement(CompilationContext context, SemanticModel model, BlockSyntax syntax)
        {
            _blockSymbols.Push(new List<ILocalSymbol>());
            foreach (StatementSyntax child in syntax.Statements)
                ConvertStatement(context, model, child);
            foreach (ILocalSymbol symbol in _blockSymbols.Pop())
                _localVariables.Remove(symbol);
        }

        private void ConvertBreakStatement()
        {
            if (_tryStack.Count > 0)
                Jump(OpCode.ENDTRY_L, _breakTargets.Peek());
            else
                Jump(OpCode.JMP_L, _breakTargets.Peek());
        }

        private void ConvertContinueStatement()
        {
            if (_tryStack.Count > 0)
                Jump(OpCode.ENDTRY_L, _continueTargets.Peek());
            else
                Jump(OpCode.JMP_L, _continueTargets.Peek());
        }

        private void ConvertDoStatement(CompilationContext context, SemanticModel model, DoStatementSyntax syntax)
        {
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(context, model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(context, model, syntax.Condition);
            Jump(OpCode.JMPIF_L, startTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertExpressionStatement(CompilationContext context, SemanticModel model, ExpressionStatementSyntax syntax)
        {
            ITypeSymbol type = model.GetTypeInfo(syntax.Expression).Type!;
            ConvertExpression(context, model, syntax.Expression);
            if (type.SpecialType != SpecialType.System_Void)
                AddInstruction(OpCode.DROP);
        }

        private void ConvertForEachStatement(CompilationContext context, SemanticModel model, ForEachStatementSyntax syntax)
        {
            ILocalSymbol elementSymbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            byte elementIndex = AddLocalVariable(elementSymbol);
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            ConvertExpression(context, model, syntax.Expression);
            AccessSlot(OpCode.STLOC, iteratorIndex);
            Jump(OpCode.JMP_L, continueTarget);
            startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
            Call(ApplicationEngine.System_Iterator_Value);
            AccessSlot(OpCode.STLOC, elementIndex);
            ConvertStatement(context, model, syntax.Statement);
            continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
            Call(ApplicationEngine.System_Iterator_Next);
            Jump(OpCode.JMPIF_L, startTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _anonymousVariables.Remove(iteratorIndex);
            _localVariables.Remove(elementSymbol);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertForEachVariableStatement(CompilationContext context, SemanticModel model, ForEachVariableStatementSyntax syntax)
        {
            ILocalSymbol[] symbols = ((ParenthesizedVariableDesignationSyntax)((DeclarationExpressionSyntax)syntax.Variable).Designation).Variables.Select(p => (ILocalSymbol)model.GetDeclaredSymbol(p)!).ToArray();
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            ConvertExpression(context, model, syntax.Expression);
            AccessSlot(OpCode.STLOC, iteratorIndex);
            Jump(OpCode.JMP_L, continueTarget);
            startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
            Call(ApplicationEngine.System_Iterator_Value);
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i] is null) continue;
                byte variableIndex = AddLocalVariable(symbols[i]);
                AddInstruction(OpCode.DUP);
                Push(i);
                AddInstruction(OpCode.PICKITEM);
                AccessSlot(OpCode.STLOC, variableIndex);
            }
            AddInstruction(OpCode.DROP);
            ConvertStatement(context, model, syntax.Statement);
            continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
            Call(ApplicationEngine.System_Iterator_Next);
            Jump(OpCode.JMPIF_L, startTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _anonymousVariables.Remove(iteratorIndex);
            foreach (ILocalSymbol symbol in symbols)
                if (symbol is not null)
                    _localVariables.Remove(symbol);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertForStatement(CompilationContext context, SemanticModel model, ForStatementSyntax syntax)
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
                {
                    ConvertExpression(context, model, variable.Initializer.Value);
                    AccessSlot(OpCode.STLOC, variableIndex);
                }
            }
            Jump(OpCode.JMP_L, conditionTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(context, model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (ExpressionSyntax expression in syntax.Incrementors)
            {
                ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                ConvertExpression(context, model, expression);
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
                ConvertExpression(context, model, syntax.Condition);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (var (_, symbol) in variables)
                _localVariables.Remove(symbol);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertIfStatement(CompilationContext context, SemanticModel model, IfStatementSyntax syntax)
        {
            JumpTarget elseTarget = new();
            ConvertExpression(context, model, syntax.Condition);
            Jump(OpCode.JMPIFNOT_L, elseTarget);
            ConvertStatement(context, model, syntax.Statement);
            if (syntax.Else is null)
            {
                elseTarget.Instruction = AddInstruction(OpCode.NOP);
            }
            else
            {
                JumpTarget endTarget = new();
                Jump(OpCode.JMP_L, endTarget);
                elseTarget.Instruction = AddInstruction(OpCode.NOP);
                ConvertStatement(context, model, syntax.Else.Statement);
                endTarget.Instruction = AddInstruction(OpCode.NOP);
            }
        }

        private void ConvertLocalDeclarationStatement(CompilationContext context, SemanticModel model, LocalDeclarationStatementSyntax syntax)
        {
            if (syntax.IsConst) return;
            foreach (VariableDeclaratorSyntax variable in syntax.Declaration.Variables)
            {
                ILocalSymbol symbol = (ILocalSymbol)model.GetDeclaredSymbol(variable)!;
                byte variableIndex = AddLocalVariable(symbol);
                if (variable.Initializer is not null)
                {
                    ConvertExpression(context, model, variable.Initializer.Value);
                    AccessSlot(OpCode.STLOC, variableIndex);
                }
            }
        }

        private void ConvertReturnStatement(CompilationContext context, SemanticModel model, ReturnStatementSyntax syntax)
        {
            if (syntax.Expression is not null)
                ConvertExpression(context, model, syntax.Expression);
            if (_tryStack.Count > 0)
                Jump(OpCode.ENDTRY_L, _returnTarget);
            else
                Jump(OpCode.JMP_L, _returnTarget);
        }

        private void ConvertSwitchStatement(CompilationContext context, SemanticModel model, SwitchStatementSyntax syntax)
        {
            var sections = syntax.Sections.Select(p => (p.Labels, p.Statements, Target: new JumpTarget())).ToArray();
            var labels = sections.SelectMany(p => p.Labels, (p, l) => (l, p.Target)).ToArray();
            JumpTarget breakTarget = new();
            _breakTargets.Push(breakTarget);
            ConvertExpression(context, model, syntax.Expression);
            foreach (var (label, target) in labels)
            {
                switch (label)
                {
                    case CaseSwitchLabelSyntax caseSwitchLabel:
                        AddInstruction(OpCode.DUP);
                        ConvertExpression(context, model, caseSwitchLabel.Value);
                        Jump(OpCode.JMPEQ_L, target);
                        break;
                    case DefaultSwitchLabelSyntax:
                        Jump(OpCode.JMP_L, target);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported syntax: {label}");
                }
            }
            Jump(OpCode.JMP_L, breakTarget);
            foreach (var (_, statements, target) in sections)
            {
                target.Instruction = AddInstruction(OpCode.NOP);
                foreach (StatementSyntax statement in statements)
                    ConvertStatement(context, model, statement);
            }
            breakTarget.Instruction = AddInstruction(OpCode.DROP);
            _breakTargets.Pop();
        }

        private void ConvertTryStatement(CompilationContext context, SemanticModel model, TryStatementSyntax syntax)
        {
            JumpTarget catchTarget = new();
            JumpTarget finallyTarget = new();
            JumpTarget endTarget = new();
            AddInstruction(new Instruction { OpCode = OpCode.TRY_L, Target = catchTarget, Target2 = finallyTarget });
            _tryStack.Push(new ExceptionHandling { State = ExceptionHandlingState.Try });
            ConvertStatement(context, model, syntax.Block);
            Jump(OpCode.ENDTRY_L, endTarget);
            if (syntax.Catches.Count > 1)
                throw new NotSupportedException("Only support one single catch.");
            if (syntax.Catches.Count > 0)
            {
                CatchClauseSyntax catchClause = syntax.Catches[0];
                if (catchClause.Filter is not null)
                    throw new NotSupportedException($"Unsupported syntax: {catchClause.Filter}");
                ILocalSymbol? exceptionSymbol = null;
                _tryStack.Peek().State = ExceptionHandlingState.Catch;
                byte exceptionIndex;
                if (catchClause.Declaration is null)
                {
                    exceptionIndex = AddAnonymousVariable();
                }
                else
                {
                    exceptionSymbol = model.GetDeclaredSymbol(catchClause.Declaration)!;
                    exceptionIndex = AddLocalVariable(exceptionSymbol);
                }
                catchTarget.Instruction = AccessSlot(OpCode.STLOC, exceptionIndex);
                _exceptionStack.Push(exceptionIndex);
                ConvertStatement(context, model, catchClause.Block);
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
                ConvertStatement(context, model, syntax.Finally.Block);
                AddInstruction(OpCode.ENDFINALLY);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
            _tryStack.Pop();
        }

        private void ConvertWhileStatement(CompilationContext context, SemanticModel model, WhileStatementSyntax syntax)
        {
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            _continueTargets.Push(continueTarget);
            _breakTargets.Push(breakTarget);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(context, model, syntax.Condition);
            Jump(OpCode.JMPIFNOT_L, breakTarget);
            ConvertStatement(context, model, syntax.Statement);
            Jump(OpCode.JMP_L, continueTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            _continueTargets.Pop();
            _breakTargets.Pop();
        }

        private void ConvertExpression(CompilationContext context, SemanticModel model, ExpressionSyntax syntax)
        {
            switch (syntax)
            {
                case AnonymousObjectCreationExpressionSyntax expression:
                    ConvertAnonymousObjectCreationExpression(context, model, expression);
                    break;
                case ArrayCreationExpressionSyntax expression:
                    ConvertArrayCreationExpression(context, model, expression);
                    break;
                case AssignmentExpressionSyntax expression:
                    ConvertAssignmentExpression(context, model, expression);
                    break;
                case BaseObjectCreationExpressionSyntax expression:
                    ConvertObjectCreationExpression(context, model, expression);
                    break;
                case BinaryExpressionSyntax expression:
                    ConvertBinaryExpression(context, model, expression);
                    break;
                case CastExpressionSyntax expression:
                    ConvertCastExpression(context, model, expression);
                    break;
                case ConditionalAccessExpressionSyntax expression:
                    ConvertConditionalAccessExpression(context, model, expression);
                    break;
                case ConditionalExpressionSyntax expression:
                    ConvertConditionalExpression(context, model, expression);
                    break;
                case DefaultExpressionSyntax expression:
                    ConvertDefaultExpression(model, expression);
                    break;
                case ElementAccessExpressionSyntax expression:
                    ConvertElementAccessExpression(context, model, expression);
                    break;
                case ElementBindingExpressionSyntax expression:
                    ConvertElementBindingExpression(context, model, expression);
                    break;
                case IdentifierNameSyntax expression:
                    ConvertIdentifierNameExpression(context, model, expression);
                    break;
                case ImplicitArrayCreationExpressionSyntax expression:
                    ConvertImplicitArrayCreationExpression(context, model, expression);
                    break;
                case InterpolatedStringExpressionSyntax expression:
                    ConvertInterpolatedStringExpression(context, model, expression);
                    break;
                case InvocationExpressionSyntax expression:
                    ConvertInvocationExpression(context, model, expression);
                    break;
                case IsPatternExpressionSyntax expression:
                    ConvertIsPatternExpression(context, model, expression);
                    break;
                case LiteralExpressionSyntax expression:
                    ConvertLiteralExpression(model, expression);
                    break;
                case MemberAccessExpressionSyntax expression:
                    ConvertMemberAccessExpression(context, model, expression);
                    break;
                case MemberBindingExpressionSyntax expression:
                    ConvertMemberBindingExpression(context, model, expression);
                    break;
                case OmittedArraySizeExpressionSyntax:
                    break;
                case ParenthesizedExpressionSyntax expression:
                    ConvertExpression(context, model, expression.Expression);
                    break;
                case PostfixUnaryExpressionSyntax expression:
                    ConvertPostfixUnaryExpression(context, model, expression);
                    break;
                case PrefixUnaryExpressionSyntax expression:
                    ConvertPrefixUnaryExpression(context, model, expression);
                    break;
                case SizeOfExpressionSyntax expression:
                    ConvertSizeOfExpression(model, expression);
                    break;
                case SwitchExpressionSyntax expression:
                    ConvertSwitchExpression(context, model, expression);
                    break;
                case ThisExpressionSyntax:
                    AddInstruction(OpCode.LDARG0);
                    break;
                case ThrowExpressionSyntax expression:
                    Throw(context, model, expression.Expression);
                    break;
                case TupleExpressionSyntax expression:
                    ConvertTupleExpression(context, model, expression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported syntax: {syntax}");
            }
        }

        private void ConvertAnonymousObjectCreationExpression(CompilationContext context, SemanticModel model, AnonymousObjectCreationExpressionSyntax expression)
        {
            AddInstruction(OpCode.NEWARRAY0);
            foreach (AnonymousObjectMemberDeclaratorSyntax initializer in expression.Initializers)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(context, model, initializer.Expression);
                AddInstruction(OpCode.APPEND);
            }
        }

        private void ConvertArrayCreationExpression(CompilationContext context, SemanticModel model, ArrayCreationExpressionSyntax expression)
        {
            if (expression.Type.RankSpecifiers.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {expression.Type.RankSpecifiers}");
            ArrayRankSpecifierSyntax specifier = expression.Type.RankSpecifiers[0];
            if (specifier.Rank != 1)
                throw new NotSupportedException($"Unsupported array rank: {specifier}");
            ConvertExpression(context, model, specifier.Sizes[0]);
            if (expression.Initializer is null)
            {
                ITypeSymbol type = model.GetTypeInfo(expression.Type.ElementType).Type!;
                AddInstruction(new Instruction { OpCode = OpCode.NEWARRAY_T, Operand = new[] { (byte)type.GetStackItemType() } });
            }
            else
            {
                AddInstruction(OpCode.NEWARRAY0);
                foreach (ExpressionSyntax ex in expression.Initializer.Expressions)
                {
                    AddInstruction(OpCode.DUP);
                    ConvertExpression(context, model, ex);
                    AddInstruction(OpCode.APPEND);
                }
            }
        }

        private void ConvertAssignmentExpression(CompilationContext context, SemanticModel model, AssignmentExpressionSyntax expression)
        {
            ConvertExpression(context, model, expression.Right);
            AddInstruction(OpCode.DUP);
            switch (expression.Left)
            {
                case ElementAccessExpressionSyntax left:
                    ConvertElementAccessAssignment(context, model, left);
                    break;
                case IdentifierNameSyntax left:
                    ConvertIdentifierNameAssignment(context, model, left);
                    break;
                case MemberAccessExpressionSyntax left:
                    ConvertMemberAccessAssignment(context, model, left);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported assignment: {expression.Left}");
            }
        }

        private void ConvertElementAccessAssignment(CompilationContext context, SemanticModel model, ElementAccessExpressionSyntax left)
        {
            if (left.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {left.ArgumentList.Arguments}");
            ConvertExpression(context, model, left.Expression);
            ConvertExpression(context, model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.ROT);
            AddInstruction(OpCode.SETITEM);
        }

        private void ConvertIdentifierNameAssignment(CompilationContext context, SemanticModel model, IdentifierNameSyntax left)
        {
            ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
            switch (symbol)
            {
                case IDiscardSymbol:
                    _instructions.RemoveAt(_instructions.Count - 1);
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
                    Call(context, model, property.SetMethod!, CallingConvention.Cdecl);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertMemberAccessAssignment(CompilationContext context, SemanticModel model, MemberAccessExpressionSyntax left)
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
                        ConvertExpression(context, model, left.Expression);
                        Push(index);
                        AddInstruction(OpCode.ROT);
                        AddInstruction(OpCode.SETITEM);
                    }
                    break;
                case IPropertySymbol property:
                    if (!property.IsStatic) ConvertExpression(context, model, left.Expression);
                    Call(context, model, property.SetMethod!, CallingConvention.Cdecl);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertObjectCreationExpression(CompilationContext context, SemanticModel model, BaseObjectCreationExpressionSyntax expression)
        {
            bool needCallConstructor, needCreateObject;
            IMethodSymbol constructor = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
            if (constructor.IsExtern)
            {
                needCallConstructor = true;
                needCreateObject = false;
            }
            else if (constructor.DeclaringSyntaxReferences.Length > 0)
            {
                needCallConstructor = true;
                needCreateObject = true;
            }
            else
            {
                needCallConstructor = false;
                needCreateObject = true;
            }
            if (needCreateObject)
            {
                ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                IFieldSymbol[] fields = type.GetFields();
                AddInstruction(OpCode.NEWARRAY0);
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
                        ConvertExpression(context, model, right);
                    AddInstruction(OpCode.APPEND);
                }
            }
            if (needCallConstructor)
            {
                IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList?.Arguments ?? (IReadOnlyList<ArgumentSyntax>)Array.Empty<ArgumentSyntax>();
                Call(context, model, constructor, needCreateObject, arguments);
            }
        }

        private void ConvertBinaryExpression(CompilationContext context, SemanticModel model, BinaryExpressionSyntax expression)
        {
            if (expression.OperatorToken.ValueText == "??")
            {
                ConvertCoalesceExpression(context, model, expression.Left, expression.Right);
            }
            else
            {
                IMethodSymbol? symbol = (IMethodSymbol?)model.GetSymbolInfo(expression).Symbol;
                if (symbol is not null && TryProcessSystemOperators(context, model, symbol, expression.Left, expression.Right))
                    return;
                ConvertExpression(context, model, expression.Left);
                ConvertExpression(context, model, expression.Right);
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
        }

        private void ConvertCoalesceExpression(CompilationContext context, SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
        {
            JumpTarget endTarget = new();
            ConvertExpression(context, model, left);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            ConvertExpression(context, model, right);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertCastExpression(CompilationContext context, SemanticModel model, CastExpressionSyntax expression)
        {
            IMethodSymbol? symbol = (IMethodSymbol?)model.GetSymbolInfo(expression).Symbol;
            if (symbol is null)
                ConvertExpression(context, model, expression.Expression);
            else
                Call(context, model, symbol, null, expression.Expression);
        }

        private void ConvertConditionalAccessExpression(CompilationContext context, SemanticModel model, ConditionalAccessExpressionSyntax expression)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            JumpTarget nullTarget = new();
            ConvertExpression(context, model, expression.Expression);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, nullTarget);
            ConvertExpression(context, model, expression.WhenNotNull);
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

        private void ConvertConditionalExpression(CompilationContext context, SemanticModel model, ConditionalExpressionSyntax expression)
        {
            JumpTarget falseTarget = new();
            JumpTarget endTarget = new();
            ConvertExpression(context, model, expression.Condition);
            Jump(OpCode.JMPIFNOT_L, falseTarget);
            ConvertExpression(context, model, expression.WhenTrue);
            Jump(OpCode.JMP_L, endTarget);
            falseTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(context, model, expression.WhenFalse);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertDefaultExpression(SemanticModel model, DefaultExpressionSyntax expression)
        {
            PushDefault(model.GetTypeInfo(expression.Type).Type!);
        }

        private void ConvertElementAccessExpression(CompilationContext context, SemanticModel model, ElementAccessExpressionSyntax expression)
        {
            if (expression.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {expression.ArgumentList.Arguments}");
            ConvertExpression(context, model, expression.Expression);
            ConvertExpression(context, model, expression.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.PICKITEM);
        }

        private void ConvertElementBindingExpression(CompilationContext context, SemanticModel model, ElementBindingExpressionSyntax expression)
        {
            if (expression.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {expression.ArgumentList.Arguments}");
            ConvertExpression(context, model, expression.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.PICKITEM);
        }

        private void ConvertIdentifierNameExpression(CompilationContext context, SemanticModel model, IdentifierNameSyntax expression)
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
                case IParameterSymbol parameter:
                    AccessSlot(OpCode.LDARG, _parameters[parameter]);
                    break;
                case IPropertySymbol property:
                    if (property.IsStatic)
                    {
                        Call(context, model, property.GetMethod!);
                    }
                    else
                    {
                        AddInstruction(OpCode.LDARG0);
                        Call(context, model, property.GetMethod!);
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertImplicitArrayCreationExpression(CompilationContext context, SemanticModel model, ImplicitArrayCreationExpressionSyntax expression)
        {
            AddInstruction(OpCode.NEWARRAY0);
            foreach (ExpressionSyntax ex in expression.Initializer.Expressions)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(context, model, ex);
                AddInstruction(OpCode.APPEND);
            }
        }

        private void ConvertInterpolatedStringExpression(CompilationContext context, SemanticModel model, InterpolatedStringExpressionSyntax expression)
        {
            if (expression.Contents.Count == 0)
            {
                Push(string.Empty);
                return;
            }
            ConvertInterpolatedStringContent(context, model, expression.Contents[0]);
            for (int i = 1; i < expression.Contents.Count; i++)
            {
                ConvertInterpolatedStringContent(context, model, expression.Contents[i]);
                AddInstruction(OpCode.CAT);
            }
            if (expression.Contents.Count >= 2)
                ChangeType(VM.Types.StackItemType.ByteString);
        }

        private void ConvertInterpolatedStringContent(CompilationContext context, SemanticModel model, InterpolatedStringContentSyntax content)
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
                    ConvertObjectToString(context, model, syntax.Expression);
                    break;
            }
        }

        private void ConvertObjectToString(CompilationContext context, SemanticModel model, ExpressionSyntax expression)
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
                    Push(10);
                    ConvertExpression(context, model, expression);
                    Call(context, NativeContract.StdLib.Hash, "itoa", 2, true);
                    break;
                case SpecialType.System_String:
                    ConvertExpression(context, model, expression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported interpolation: {expression}");
            }
        }

        private void ConvertInvocationExpression(CompilationContext context, SemanticModel model, InvocationExpressionSyntax expression)
        {
            ArgumentSyntax[] arguments = expression.ArgumentList.Arguments.ToArray();
            ISymbol symbol = model.GetSymbolInfo(expression.Expression).Symbol!;
            switch (symbol)
            {
                case IEventSymbol @event:
                    ConvertEventInvocationExpression(context, model, @event, arguments);
                    break;
                case IMethodSymbol method:
                    ConvertMethodInvocationExpression(context, model, method, expression.Expression, arguments);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertEventInvocationExpression(CompilationContext context, SemanticModel model, IEventSymbol symbol, ArgumentSyntax[] arguments)
        {
            AddInstruction(OpCode.NEWARRAY0);
            foreach (ArgumentSyntax argument in arguments)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(context, model, argument.Expression);
                AddInstruction(OpCode.APPEND);
            }
            Push(symbol.GetDisplayName());
            Call(ApplicationEngine.System_Runtime_Notify);
        }

        private void ConvertMethodInvocationExpression(CompilationContext context, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax expression, ArgumentSyntax[] arguments)
        {
            switch (expression)
            {
                case IdentifierNameSyntax:
                    Call(context, model, symbol, null, arguments);
                    break;
                case MemberAccessExpressionSyntax syntax:
                    if (symbol.IsStatic)
                        Call(context, model, symbol, null, arguments);
                    else
                        Call(context, model, symbol, syntax.Expression, arguments);
                    break;
                case MemberBindingExpressionSyntax:
                    Call(context, model, symbol, true, arguments);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported expression: {expression}");
            }
        }

        private void ConvertIsPatternExpression(CompilationContext context, SemanticModel model, IsPatternExpressionSyntax expression)
        {
            ConvertExpression(context, model, expression.Expression);
            ConvertPattern(context, model, expression.Pattern);
            AddInstruction(OpCode.NIP);
        }

        private void ConvertLiteralExpression(SemanticModel model, LiteralExpressionSyntax expression)
        {
            Push(model.GetConstantValue(expression).Value);
        }

        private void ConvertMemberAccessExpression(CompilationContext context, SemanticModel model, MemberAccessExpressionSyntax expression)
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
                        ConvertExpression(context, model, expression.Expression);
                        Push(index);
                        AddInstruction(OpCode.PICKITEM);
                    }
                    break;
                case IPropertySymbol property:
                    ExpressionSyntax? instanceExpression = property.IsStatic ? null : expression.Expression;
                    Call(context, model, property.GetMethod!, instanceExpression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertMemberBindingExpression(CompilationContext context, SemanticModel model, MemberBindingExpressionSyntax expression)
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
                    Call(context, model, property.GetMethod!);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertPostfixUnaryExpression(CompilationContext context, SemanticModel model, PostfixUnaryExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "++":
                case "--":
                    ConvertPostIncrementOrDecrementExpression(context, model, expression);
                    break;
                case "!":
                    ConvertExpression(context, model, expression.Operand);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported operator: {expression.OperatorToken}");
            }
        }

        private void ConvertPostIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, PostfixUnaryExpressionSyntax expression)
        {
            switch (expression.Operand)
            {
                case ElementAccessExpressionSyntax operand:
                    ConvertElementAccessPostIncrementOrDecrementExpression(context, model, expression.OperatorToken, operand);
                    break;
                case IdentifierNameSyntax operand:
                    ConvertIdentifierNamePostIncrementOrDecrementExpression(context, model, expression.OperatorToken, operand);
                    break;
                case MemberAccessExpressionSyntax operand:
                    ConvertMemberAccessPostIncrementOrDecrementExpression(context, model, expression.OperatorToken, operand);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported postfix unary expression: {expression}");
            }
        }

        private void ConvertElementAccessPostIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
        {
            if (operand.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {operand.ArgumentList.Arguments}");
            ConvertExpression(context, model, operand.Expression);
            ConvertExpression(context, model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.SETITEM);
        }

        private void ConvertIdentifierNamePostIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(context, operatorToken, field);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(operatorToken, local);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(operatorToken, parameter);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(context, model, operatorToken, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(CompilationContext context, SyntaxToken operatorToken, IFieldSymbol symbol)
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

        private void ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(context, model, symbol.GetMethod!);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                Call(context, model, symbol.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Call(context, model, symbol.GetMethod!);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Call(context, model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertMemberAccessPostIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessPostIncrementOrDecrementExpression(context, model, operatorToken, operand, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(context, model, operatorToken, operand, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessPostIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
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
                ConvertExpression(context, model, operand.Expression);
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

        private void ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(context, model, symbol.GetMethod!);
                AddInstruction(OpCode.DUP);
                EmitIncrementOrDecrement(operatorToken);
                Call(context, model, symbol.SetMethod!);
            }
            else
            {
                ConvertExpression(context, model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Call(context, model, symbol.GetMethod!);
                AddInstruction(OpCode.TUCK);
                EmitIncrementOrDecrement(operatorToken);
                Call(context, model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertPrefixUnaryExpression(CompilationContext context, SemanticModel model, PrefixUnaryExpressionSyntax expression)
        {
            switch (expression.OperatorToken.ValueText)
            {
                case "+":
                    ConvertExpression(context, model, expression.Operand);
                    break;
                case "-":
                    ConvertExpression(context, model, expression.Operand);
                    AddInstruction(OpCode.NEGATE);
                    break;
                case "~":
                    ConvertExpression(context, model, expression.Operand);
                    AddInstruction(OpCode.INVERT);
                    break;
                case "!":
                    ConvertExpression(context, model, expression.Operand);
                    AddInstruction(OpCode.NOT);
                    break;
                case "++":
                case "--":
                    ConvertPreIncrementOrDecrementExpression(context, model, expression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported operator: {expression.OperatorToken}");
            }
        }

        private void ConvertPreIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, PrefixUnaryExpressionSyntax expression)
        {
            switch (expression.Operand)
            {
                case ElementAccessExpressionSyntax operand:
                    ConvertElementAccessPreIncrementOrDecrementExpression(context, model, expression.OperatorToken, operand);
                    break;
                case IdentifierNameSyntax operand:
                    ConvertIdentifierNamePreIncrementOrDecrementExpression(context, model, expression.OperatorToken, operand);
                    break;
                case MemberAccessExpressionSyntax operand:
                    ConvertMemberAccessPreIncrementOrDecrementExpression(context, model, expression.OperatorToken, operand);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported postfix unary expression: {expression}");
            }
        }

        private void ConvertElementAccessPreIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
        {
            if (operand.ArgumentList.Arguments.Count != 1)
                throw new NotSupportedException($"Unsupported array rank: {operand.ArgumentList.Arguments}");
            ConvertExpression(context, model, operand.Expression);
            ConvertExpression(context, model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            EmitIncrementOrDecrement(operatorToken);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }

        private void ConvertIdentifierNamePreIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(context, operatorToken, field);
                    break;
                case ILocalSymbol local:
                    ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(operatorToken, local);
                    break;
                case IParameterSymbol parameter:
                    ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(operatorToken, parameter);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(context, model, operatorToken, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(CompilationContext context, SyntaxToken operatorToken, IFieldSymbol symbol)
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

        private void ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(context, model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                Call(context, model, symbol.SetMethod!);
            }
            else
            {
                AddInstruction(OpCode.LDARG0);
                AddInstruction(OpCode.DUP);
                Call(context, model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(context, model, symbol.SetMethod!, CallingConvention.StdCall);
            }
        }

        private void ConvertMemberAccessPreIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
        {
            ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    ConvertFieldMemberAccessPreIncrementOrDecrementExpression(context, model, operatorToken, operand, field);
                    break;
                case IPropertySymbol property:
                    ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(context, model, operatorToken, operand, property);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported symbol: {symbol}");
            }
        }

        private void ConvertFieldMemberAccessPreIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
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
                ConvertExpression(context, model, operand.Expression);
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

        private void ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(CompilationContext context, SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
        {
            if (symbol.IsStatic)
            {
                Call(context, model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.DUP);
                Call(context, model, symbol.SetMethod!);
            }
            else
            {
                ConvertExpression(context, model, operand.Expression);
                AddInstruction(OpCode.DUP);
                Call(context, model, symbol.GetMethod!);
                EmitIncrementOrDecrement(operatorToken);
                AddInstruction(OpCode.TUCK);
                Call(context, model, symbol.SetMethod!, CallingConvention.StdCall);
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

        private void ConvertSwitchExpression(CompilationContext context, SemanticModel model, SwitchExpressionSyntax expression)
        {
            var arms = expression.Arms.Select(p => (p, new JumpTarget())).ToArray();
            JumpTarget breakTarget = new();
            ConvertExpression(context, model, expression.GoverningExpression);
            foreach (var (arm, nextTarget) in arms)
            {
                ConvertPattern(context, model, arm.Pattern);
                Jump(OpCode.JMPIFNOT_L, nextTarget);
                if (arm.WhenClause is not null)
                {
                    ConvertExpression(context, model, arm.WhenClause.Condition);
                    Jump(OpCode.JMPIFNOT_L, nextTarget);
                }
                ConvertExpression(context, model, arm.Expression);
                Jump(OpCode.JMP_L, breakTarget);
                nextTarget.Instruction = AddInstruction(OpCode.NOP);
            }
            AddInstruction(OpCode.THROW);
            breakTarget.Instruction = AddInstruction(OpCode.NIP);
        }

        private void ConvertPattern(CompilationContext context, SemanticModel model, PatternSyntax pattern)
        {
            switch (pattern)
            {
                case BinaryPatternSyntax binaryPattern:
                    ConvertBinaryPattern(context, model, binaryPattern);
                    break;
                case ConstantPatternSyntax constantPattern:
                    ConvertConstantPattern(context, model, constantPattern);
                    break;
                case DiscardPatternSyntax:
                    Push(true);
                    break;
                case RelationalPatternSyntax relationalPattern:
                    ConvertRelationalPattern(context, model, relationalPattern);
                    break;
                case UnaryPatternSyntax unaryPattern when unaryPattern.OperatorToken.ValueText == "not":
                    ConvertNotPattern(context, model, unaryPattern);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported pattern: {pattern}");
            }
        }

        private void ConvertBinaryPattern(CompilationContext context, SemanticModel model, BinaryPatternSyntax pattern)
        {
            switch (pattern.OperatorToken.ValueText)
            {
                case "and":
                    ConvertAndPattern(context, model, pattern.Left, pattern.Right);
                    break;
                case "or":
                    ConvertOrPattern(context, model, pattern.Left, pattern.Right);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported pattern: {pattern}");
            }
        }

        private void ConvertAndPattern(CompilationContext context, SemanticModel model, PatternSyntax left, PatternSyntax right)
        {
            JumpTarget rightTarget = new();
            JumpTarget endTarget = new();
            ConvertPattern(context, model, left);
            Jump(OpCode.JMPIF_L, rightTarget);
            Push(false);
            Jump(OpCode.JMP_L, endTarget);
            rightTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertPattern(context, model, right);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertOrPattern(CompilationContext context, SemanticModel model, PatternSyntax left, PatternSyntax right)
        {
            JumpTarget rightTarget = new();
            JumpTarget endTarget = new();
            ConvertPattern(context, model, left);
            Jump(OpCode.JMPIFNOT_L, rightTarget);
            Push(true);
            Jump(OpCode.JMP_L, endTarget);
            rightTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertPattern(context, model, right);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        private void ConvertConstantPattern(CompilationContext context, SemanticModel model, ConstantPatternSyntax pattern)
        {
            AddInstruction(OpCode.DUP);
            ConvertExpression(context, model, pattern.Expression);
            AddInstruction(OpCode.EQUAL);
        }

        private void ConvertRelationalPattern(CompilationContext context, SemanticModel model, RelationalPatternSyntax pattern)
        {
            AddInstruction(OpCode.DUP);
            ConvertExpression(context, model, pattern.Expression);
            AddInstruction(pattern.OperatorToken.ValueText switch
            {
                "<" => OpCode.LT,
                "<=" => OpCode.LE,
                ">" => OpCode.GT,
                ">=" => OpCode.GE,
                _ => throw new NotSupportedException($"Unsupported pattern: {pattern}")
            });
        }

        private void ConvertNotPattern(CompilationContext context, SemanticModel model, UnaryPatternSyntax pattern)
        {
            ConvertPattern(context, model, pattern.Pattern);
            AddInstruction(OpCode.NOT);
        }

        private void ConvertTupleExpression(CompilationContext context, SemanticModel model, TupleExpressionSyntax expression)
        {
            AddInstruction(OpCode.NEWSTRUCT0);
            foreach (ArgumentSyntax argument in expression.Arguments)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(context, model, argument.Expression);
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
            return Push(Encoding.UTF8.GetBytes(s));
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
            return AddInstruction(new Instruction
            {
                OpCode = opcode,
                Operand = new[] { index }
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

        private void Throw(CompilationContext context, SemanticModel model, ExpressionSyntax? exception)
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
                            ConvertExpression(context, model, expression.ArgumentList.Arguments[0].Expression);
                            break;
                        default:
                            throw new NotSupportedException("Only a single parameter is supported for exceptions.");
                    }
                    break;
                case null:
                    AccessSlot(OpCode.LDLOC, _exceptionStack.Peek());
                    break;
                default:
                    ConvertExpression(context, model, exception);
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

        private Instruction Call(CompilationContext context, UInt160 hash, string method, ushort parametersCount, bool hasReturnValue, CallFlags callFlags = CallFlags.All)
        {
            ushort token = context.AddMethodToken(hash, method, parametersCount, hasReturnValue, callFlags);
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.CALLT,
                Operand = BitConverter.GetBytes(token)
            });
        }

        private void Call(CompilationContext context, SemanticModel model, IMethodSymbol symbol, bool instanceOnStack, IReadOnlyList<ArgumentSyntax> arguments)
        {
            if (TryProcessSystemMethods(context, model, symbol, null, arguments))
                return;
            MethodConvert convert = context.ConvertMethod(model, symbol);
            bool isConstructor = symbol.MethodKind == MethodKind.Constructor;
            if (instanceOnStack && convert._callingConvention != CallingConvention.Cdecl && isConstructor)
                AddInstruction(OpCode.DUP);
            PrepareArgumentsForMethod(context, model, symbol, arguments, convert._callingConvention);
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

        private void Call(CompilationContext context, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, params SyntaxNode[] arguments)
        {
            if (TryProcessSystemMethods(context, model, symbol, instanceExpression, arguments))
                return;
            MethodConvert convert = context.ConvertMethod(model, symbol);
            if (!symbol.IsStatic && convert._callingConvention != CallingConvention.Cdecl)
            {
                if (instanceExpression is null)
                    AddInstruction(OpCode.LDARG0);
                else
                    ConvertExpression(context, model, instanceExpression);
            }
            PrepareArgumentsForMethod(context, model, symbol, arguments, convert._callingConvention);
            if (!symbol.IsStatic && convert._callingConvention == CallingConvention.Cdecl)
            {
                if (instanceExpression is null)
                    AddInstruction(OpCode.LDARG0);
                else
                    ConvertExpression(context, model, instanceExpression);
            }
            EmitCall(convert);
        }

        private void Call(CompilationContext context, SemanticModel model, IMethodSymbol symbol, CallingConvention callingConvention = CallingConvention.Cdecl)
        {
            if (TryProcessSystemMethods(context, model, symbol, null, null))
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

        private void PrepareArgumentsForMethod(CompilationContext context, SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode> arguments, CallingConvention callingConvention = CallingConvention.Cdecl)
        {
            var namedArguments = arguments.OfType<ArgumentSyntax>().Where(p => p.NameColon is not null).Select(p => (Symbol: (IParameterSymbol)model.GetSymbolInfo(p.NameColon!.Name).Symbol!, p.Expression)).ToDictionary(p => p.Symbol, p => p.Expression);
            var parameters = symbol.Parameters.Select((p, i) => (Symbol: p, Index: i));
            if (callingConvention == CallingConvention.Cdecl)
                parameters = parameters.Reverse();
            foreach (var (parameter, index) in parameters)
            {
                if (namedArguments.TryGetValue(parameter, out ExpressionSyntax? expression))
                {
                    ConvertExpression(context, model, expression);
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
                                    ConvertExpression(context, model, argument.Expression);
                                    continue;
                                }
                                break;
                            case ExpressionSyntax ex:
                                ConvertExpression(context, model, ex);
                                continue;
                            default:
                                throw new Exception("Unknown exception.");
                        }
                    }
                    Push(parameter.ExplicitDefaultValue);
                }
            }
        }

        private bool TryProcessSystemMethods(CompilationContext context, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
                        ConvertExpression(context, model, instanceExpression);
                    ChangeType(VM.Types.StackItemType.Boolean);
                    return true;
                case "System.Numerics.BigInteger.IsOne.get":
                    if (instanceExpression is not null)
                        ConvertExpression(context, model, instanceExpression);
                    Push(1);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.Sign.get":
                    if (instanceExpression is not null)
                        ConvertExpression(context, model, instanceExpression);
                    AddInstruction(OpCode.SIGN);
                    return true;
                case "System.Array.Length.get":
                    if (instanceExpression is not null)
                        ConvertExpression(context, model, instanceExpression);
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
                    Push(10);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(context, model, symbol, arguments);
                    Call(context, NativeContract.StdLib.Hash, "atoi", 2, true);
                    return true;
                case "System.Numerics.BigInteger.Equals(long)":
                case "System.Numerics.BigInteger.Equals(ulong)":
                case "System.Numerics.BigInteger.Equals(System.Numerics.BigInteger)":
                    if (instanceExpression is not null)
                        ConvertExpression(context, model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(context, model, symbol, arguments);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "object.Equals(object?)":
                    if (instanceExpression is not null)
                        ConvertExpression(context, model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(context, model, symbol, arguments);
                    AddInstruction(OpCode.EQUAL);
                    return true;
                default:
                    return false;
            }
        }

        private bool TryProcessSystemOperators(CompilationContext context, SemanticModel model, IMethodSymbol symbol, params ExpressionSyntax[] arguments)
        {
            switch (symbol.ToString())
            {
                case "object.operator ==(object, object)":
                    ConvertExpression(context, model, arguments[0]);
                    ConvertExpression(context, model, arguments[1]);
                    AddInstruction(OpCode.EQUAL);
                    return true;
                case "object.operator !=(object, object)":
                    ConvertExpression(context, model, arguments[0]);
                    ConvertExpression(context, model, arguments[1]);
                    AddInstruction(OpCode.NOTEQUAL);
                    return true;
                case "string.operator +(string, object)":
                    ConvertExpression(context, model, arguments[0]);
                    ConvertObjectToString(context, model, arguments[1]);
                    AddInstruction(OpCode.CAT);
                    ChangeType(VM.Types.StackItemType.ByteString);
                    return true;
                case "string.operator +(object, string)":
                    ConvertObjectToString(context, model, arguments[0]);
                    ConvertExpression(context, model, arguments[1]);
                    AddInstruction(OpCode.CAT);
                    ChangeType(VM.Types.StackItemType.ByteString);
                    return true;
                default:
                    return false;
            }
        }

        private void EmitCall(MethodConvert target)
        {
            if (target._inline)
                for (int i = 0; i < target._instructions.Count - 1; i++)
                    AddInstruction(target._instructions[i].Clone());
            else
                Jump(OpCode.CALL_L, target._startTarget);
        }
    }
}
