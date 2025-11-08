extern alias scfx;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler.HIR;
using Neo.Compiler;
using Neo.Extensions;
using Neo.SmartContract;

namespace Neo.Compiler.HIR.Import;

internal sealed partial class HirMethodImporter
{
    private static readonly SymbolDisplayFormat FullyQualifiedMethodFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        memberOptions: SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeParameters,
        parameterOptions: SymbolDisplayParameterOptions.IncludeType,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    private HirValue LowerExpression(SemanticModel model, ExpressionSyntax expression)
    {
        var constant = model.GetConstantValue(expression);
        if (constant.HasValue)
            return EmitConstant(constant.Value);

        switch (expression)
        {
            case ParenthesizedExpressionSyntax parenthesized:
                return LowerExpression(model, parenthesized.Expression);

            case LiteralExpressionSyntax literal:
                return EmitLiteralExpression(literal);

            case IdentifierNameSyntax identifier:
                return ResolveIdentifier(model, identifier);

            case ThisExpressionSyntax:
            case BaseExpressionSyntax:
                return GetThisValue();

            case MemberAccessExpressionSyntax memberAccess:
                return LowerMemberAccessExpression(model, memberAccess);

            case InvocationExpressionSyntax invocation:
                return EmitInvocationExpression(model, invocation);

            case AssignmentExpressionSyntax assignment:
                return EmitAssignmentExpression(model, assignment);

            case BinaryExpressionSyntax binary:
                return EmitBinaryExpression(model, binary);

            case PrefixUnaryExpressionSyntax prefix:
                return EmitPrefixUnaryExpression(model, prefix);

            case PostfixUnaryExpressionSyntax postfix:
                return EmitPostfixUnaryExpression(model, postfix);

            case ConditionalExpressionSyntax conditional:
                return EmitConditionalExpression(model, conditional);

            case ElementAccessExpressionSyntax elementAccess:
                return EmitElementAccessExpression(model, elementAccess);

            case ObjectCreationExpressionSyntax objectCreation:
                return EmitObjectCreationExpression(model, objectCreation);

            case ImplicitObjectCreationExpressionSyntax implicitCreation:
                return EmitObjectCreationExpression(model, implicitCreation);

            case TupleExpressionSyntax tupleExpression:
                return EmitTupleExpression(model, tupleExpression);

            case ArrayCreationExpressionSyntax arrayCreation:
                return EmitArrayCreationExpression(model, arrayCreation);

            case ImplicitArrayCreationExpressionSyntax implicitArrayCreation:
                return EmitArrayCreationExpression(model, implicitArrayCreation);

            case CollectionExpressionSyntax collection:
                return EmitCollectionExpression(model, collection);

            case InitializerExpressionSyntax arrayInitializer
                when arrayInitializer.Kind() == SyntaxKind.ArrayInitializerExpression:
                return EmitArrayInitializerExpression(model, arrayInitializer);

            case DefaultExpressionSyntax defaultExpression:
                return EmitHirDefault(MapType(model.GetTypeInfo(defaultExpression).ConvertedType));

            case CastExpressionSyntax castExpression:
                return EmitCastExpression(model, castExpression);

            case ConditionalAccessExpressionSyntax conditionalAccess:
                return EmitConditionalAccessExpression(model, conditionalAccess);

            default:
                throw new NotSupportedException($"HIR conversion does not yet support expression '{expression.Kind()}'.");
        }
    }

    private HirValue LowerMemberAccessExpression(SemanticModel model, MemberAccessExpressionSyntax memberAccess)
    {
        var symbol = model.GetSymbolInfo(memberAccess).Symbol;

        return symbol switch
        {
            IPropertySymbol property => EmitPropertyValue(property.IsStatic ? null : LowerExpression(model, memberAccess.Expression), property),
            IFieldSymbol field when field.HasConstantValue && field.IsStatic => EmitConstant(field.ConstantValue),
            IFieldSymbol field when field.IsStatic => EmitStaticFieldLoad(field),
            IFieldSymbol field => EmitFieldValue(LowerExpression(model, memberAccess.Expression), field),
            IMethodSymbol method when method.MethodKind == MethodKind.Ordinary && method.IsStatic
                => EmitInvocation(method, null, Array.Empty<HirValue>()),
            _ => throw new NotSupportedException($"Member access '{symbol?.Kind}' is not yet supported in HIR conversion.")
        };
    }

    private HirValue EmitLiteralExpression(LiteralExpressionSyntax literal)
        => EmitConstant(literal.Token.Value);

    private HirValue EmitConstant(object? value)
    {
        if (value is double or float or decimal)
            throw new NotSupportedException("Floating-point literals are not supported in Neo smart contracts.");

        HirInst literal;
        switch (value)
        {
            case null:
                literal = new HirConstNull();
                break;
            case bool b:
                literal = new HirConstBool(b);
                break;
            case char ch:
                literal = new HirConstInt(new BigInteger(ch), new HirIntType(16, false));
                break;
            case sbyte v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(8, true));
                break;
            case byte v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(8, false));
                break;
            case short v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(16, true));
                break;
            case ushort v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(16, false));
                break;
            case int v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(32, true));
                break;
            case uint v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(32, false));
                break;
            case long v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(64, true));
                break;
            case ulong v:
                literal = new HirConstInt(new BigInteger(v), new HirIntType(64, false));
                break;
            case BigInteger bigInt:
                literal = new HirConstInt(bigInt);
                break;
            case string s:
                literal = new HirConstByteString(Encoding.UTF8.GetBytes(s));
                break;
            case byte[] bytes:
                literal = new HirConstBuffer(bytes);
                break;
            default:
                throw new NotSupportedException($"Literal of type '{value.GetType()}' is not supported in Neo smart contracts.");
        }

        _hirBuilder?.Append(literal);
        return literal;
    }

    private HirValue ResolveIdentifier(SemanticModel model, IdentifierNameSyntax identifier)
    {
        if (identifier.Identifier.ValueText is "this" or "base")
            return GetThisValue();

        var symbol = model.GetSymbolInfo(identifier).Symbol;
        if (symbol is IParameterSymbol lambdaParameter && TryGetLambdaParameterValue(lambdaParameter, out var parameterValue))
            return parameterValue;

        return symbol switch
        {
            IParameterSymbol parameter when TryGetSymbolValue(parameter, out var argumentValue) => argumentValue,
            ILocalSymbol localSymbol when TryGetSymbolValue(localSymbol, out var localValue) => localValue,
            IFieldSymbol fieldSymbol when fieldSymbol.HasConstantValue && fieldSymbol.IsStatic => EmitConstant(fieldSymbol.ConstantValue),
            IFieldSymbol fieldSymbol when fieldSymbol.IsStatic => EmitStaticFieldLoad(fieldSymbol),
            IFieldSymbol fieldSymbol => EmitFieldValue(GetThisValue(), fieldSymbol),
            _ => throw new NotSupportedException($"Unable to resolve identifier '{identifier.Identifier.ValueText}' in HIR conversion.")
        };
    }

    private HirValue EmitBinaryExpression(SemanticModel model, BinaryExpressionSyntax binary)
    {
        if (binary.IsKind(SyntaxKind.LogicalAndExpression) || binary.IsKind(SyntaxKind.LogicalOrExpression))
            return EmitLogicalBinaryExpression(model, binary);

        var left = LowerExpression(model, binary.Left);
        var right = LowerExpression(model, binary.Right);

        HirInst instruction = binary.OperatorToken.ValueText switch
        {
            "+" => new HirAdd(left, right, left.Type),
            "-" => new HirSub(left, right, left.Type),
            "*" => new HirMul(left, right, left.Type),
            "/" => new HirDiv(left, right, left.Type),
            "%" => new HirMod(left, right, left.Type),
            "&" => new HirBitAnd(left, right, left.Type),
            "|" => new HirBitOr(left, right, left.Type),
            "^" => new HirBitXor(left, right, left.Type),
            "<<" => new HirShl(left, right, left.Type),
            ">>" => new HirShr(left, right, left.Type),
            "==" => new HirCompare(HirCmpKind.Eq, left, right),
            "!=" => new HirCompare(HirCmpKind.Ne, left, right),
            ">" => new HirCompare(HirCmpKind.Gt, left, right),
            ">=" => new HirCompare(HirCmpKind.Ge, left, right),
            "<" => new HirCompare(HirCmpKind.Lt, left, right),
            "<=" => new HirCompare(HirCmpKind.Le, left, right),
            _ => throw new NotSupportedException($"Binary operator '{binary.OperatorToken.ValueText}' not yet supported in HIR conversion.")
        };

        _hirBuilder!.Append(instruction);
        return instruction;
    }

    private HirValue EmitPostfixUnaryExpression(SemanticModel model, PostfixUnaryExpressionSyntax postfix)
    {
        var symbol = model.GetSymbolInfo(postfix.Operand).Symbol
            ?? throw new NotSupportedException($"Postfix operator target '{postfix.Operand}' is not supported in HIR conversion.");

        if (!TryGetSymbolValue(symbol, out var currentValue))
            throw new NotSupportedException($"Value for symbol '{symbol.Name}' is not available for postfix operation.");

        var one = EnsureType(EmitConstant(1), currentValue.Type);
        HirInst updated = postfix.Kind() switch
        {
            SyntaxKind.PostIncrementExpression => new HirAdd(currentValue, one, currentValue.Type),
            SyntaxKind.PostDecrementExpression => new HirSub(currentValue, one, currentValue.Type),
            _ => throw new NotSupportedException($"Postfix operator '{postfix.Kind()}' is not yet supported in HIR conversion.")
        };

        _hirBuilder!.Append(updated);

        switch (symbol)
        {
            case ILocalSymbol local:
                AssignLocalSymbol(local, updated);
                break;
            case IParameterSymbol parameter:
                AssignParameterSymbol(parameter, updated);
                break;
            default:
                throw new NotSupportedException($"Postfix operator target '{symbol.Kind}' is not yet supported in HIR conversion.");
        }

        return currentValue;
    }

    private HirValue EmitLogicalBinaryExpression(SemanticModel model, BinaryExpressionSyntax binary)
    {
        var builder = _hirBuilder!;
        var resultLocal = CreateSyntheticLocal("logic_tmp", HirType.BoolType);
        var rhsBlock = builder.CreateBlock(NewBlockLabel("logic_rhs"));
        var mergeBlock = builder.CreateBlock(NewBlockLabel("logic_merge"));

        var left = LowerExpression(model, binary.Left);
        _hirBuilder!.Append(new HirStoreLocal(resultLocal, left));
        if (binary.IsKind(SyntaxKind.LogicalAndExpression))
        {
            AppendConditional(left, rhsBlock, mergeBlock);
        }
        else
        {
            AppendConditional(left, mergeBlock, rhsBlock);
        }

        builder.SetCurrentBlock(rhsBlock);
        var right = LowerExpression(model, binary.Right);
        _hirBuilder.Append(new HirStoreLocal(resultLocal, right));
        AppendBranch(mergeBlock);

        builder.SetCurrentBlock(mergeBlock);
        return LoadLocal(resultLocal);
    }

    private HirValue EmitAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax assignment)
    {
        if (assignment.Right is LambdaExpressionSyntax lambdaExpression &&
            model.GetSymbolInfo(assignment.Left).Symbol is ILocalSymbol localTarget)
        {
            if (!TryRegisterLambda(localTarget, lambdaExpression))
                throw new NotSupportedException("Lambda expressions with captures or block bodies are not yet supported in HIR conversion.");

            var lambdaDefault = EnsureType(null, MapType(localTarget.Type));
            AssignLocalSymbol(localTarget, lambdaDefault);
            return lambdaDefault;
        }

        HirValue value;
        if (assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
        {
            value = LowerExpression(model, assignment.Right);
        }
        else
        {
            var targetSymbolForCompound = model.GetSymbolInfo(assignment.Left).Symbol;
            if (targetSymbolForCompound is null)
                throw new NotSupportedException($"Assignment target '{assignment.Left}' is not supported.");

            if (!TryGetSymbolValue(targetSymbolForCompound, out var currentValue))
                throw new NotSupportedException($"Compound assignment target '{assignment.Left}' has no current value.");

            var rhs = LowerExpression(model, assignment.Right);
            HirInst operation = assignment.Kind() switch
            {
                SyntaxKind.AddAssignmentExpression => new HirAdd(currentValue, rhs, currentValue.Type),
                SyntaxKind.SubtractAssignmentExpression => new HirSub(currentValue, rhs, currentValue.Type),
                SyntaxKind.MultiplyAssignmentExpression => new HirMul(currentValue, rhs, currentValue.Type),
                SyntaxKind.DivideAssignmentExpression => new HirDiv(currentValue, rhs, currentValue.Type),
                SyntaxKind.ModuloAssignmentExpression => new HirMod(currentValue, rhs, currentValue.Type),
                SyntaxKind.AndAssignmentExpression => new HirBitAnd(currentValue, rhs, currentValue.Type),
                SyntaxKind.OrAssignmentExpression => new HirBitOr(currentValue, rhs, currentValue.Type),
                SyntaxKind.ExclusiveOrAssignmentExpression => new HirBitXor(currentValue, rhs, currentValue.Type),
                SyntaxKind.LeftShiftAssignmentExpression => new HirShl(currentValue, rhs, currentValue.Type),
                SyntaxKind.RightShiftAssignmentExpression => new HirShr(currentValue, rhs, currentValue.Type),
                _ => throw new NotSupportedException($"Assignment operator '{assignment.Kind()}' is not yet supported in HIR conversion.")
            };

            _hirBuilder!.Append(operation);
            value = operation;
        }

        if (assignment.Left is ElementAccessExpressionSyntax elementAccess)
            return ResolveIndexerAssignment(model, elementAccess, value);

        var targetSymbol = model.GetSymbolInfo(assignment.Left).Symbol;

        switch (targetSymbol)
        {
            case ILocalSymbol localSymbol:
                _lambdaAssignments.Remove(localSymbol);
                AssignLocalSymbol(localSymbol, EnsureType(value, MapType(localSymbol.Type)));
                return value;

            case IParameterSymbol parameterSymbol when TryGetHirArgument(parameterSymbol) is HirArgument argument:
                AssignParameterSymbol(parameterSymbol, EnsureType(value, MapType(parameterSymbol.Type)));
                return value;

            case IPropertySymbol propertySymbol:
                return EmitPropertyAssignment(model, propertySymbol, assignment.Left, EnsureType(value, MapType(propertySymbol.Type)));

            case IFieldSymbol fieldSymbol:
                return EmitFieldAssignment(model, fieldSymbol, assignment.Left, EnsureType(value, MapType(fieldSymbol.Type)));

            default:
                throw new NotSupportedException("Only assignments to locals, parameters, properties, or fields are supported in HIR conversion.");
        }
    }

    private HirValue EmitInvocationExpression(SemanticModel model, InvocationExpressionSyntax invocation)
    {
        var expressionSymbol = model.GetSymbolInfo(invocation.Expression).Symbol;
        var argumentValues = invocation.ArgumentList.Arguments.Select(arg => LowerExpression(model, arg.Expression)).ToList();

        if (expressionSymbol is IEventSymbol eventSymbol)
            return EmitEventInvocation(eventSymbol, argumentValues, invocation);

        var methodSymbol = (IMethodSymbol)model.GetSymbolInfo(invocation).Symbol!;

        if (methodSymbol.MethodKind == MethodKind.DelegateInvoke)
        {
            var lambdaValue = TryInlineLambdaInvocation(invocation.Expression, argumentValues);
            if (lambdaValue is not null)
                return lambdaValue;

            return EmitDelegateInvocation(model, invocation.Expression, methodSymbol, argumentValues);
        }

        HirValue? receiver = null;

        if (methodSymbol.IsExtensionMethod)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var receiverSymbol = model.GetSymbolInfo(memberAccess.Expression).Symbol;
                if (receiverSymbol is ITypeSymbol)
                {
                    if (argumentValues.Count == 0)
                        throw new NotSupportedException("Extension method invocation requires a receiver.");
                    receiver = argumentValues[0];
                    argumentValues = argumentValues.Skip(1).ToList();
                }
                else
                {
                    receiver = LowerExpression(model, memberAccess.Expression);
                }
            }
            else if (argumentValues.Count > 0)
            {
                receiver = argumentValues[0];
                argumentValues = argumentValues.Skip(1).ToList();
            }
            else
            {
                throw new NotSupportedException("Extension method invocation requires a receiver.");
            }
        }
        else if (!methodSymbol.IsStatic)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                receiver = LowerExpression(model, memberAccess.Expression);
                if (receiver is not null)
                    EnsureNonNull(receiver);
            }
            else
            {
                receiver = GetThisValue();
            }
        }

        return EmitInvocation(methodSymbol, receiver, argumentValues);
    }

    private HirValue EmitDelegateInvocation(SemanticModel model, ExpressionSyntax targetExpression, IMethodSymbol invokeMethod, IReadOnlyList<HirValue> arguments)
    {
        var pointer = LowerExpression(model, targetExpression);
        var canonicalArguments = arguments is List<HirValue> list ? list.ToArray() : arguments.ToArray();

        var pointerCall = new HirPointerCall(pointer, canonicalArguments, MapType(invokeMethod.ReturnType), HirCallSemantics.Effectful);
        _hirBuilder!.Append(pointerCall);

        return pointerCall.Type is HirVoidType ? EmitHirDefault(HirType.VoidType) : pointerCall;
    }

    private HirValue? TryInlineLambdaInvocation(ExpressionSyntax targetExpression, IReadOnlyList<HirValue> arguments)
    {
        ISymbol? targetSymbol = targetExpression switch
        {
            IdentifierNameSyntax identifier => _model.GetSymbolInfo(identifier).Symbol,
            _ => null
        };

        if (targetSymbol is null)
            return null;

        if (!_lambdaAssignments.TryGetValue(targetSymbol, out var lambdaInfo))
            return null;

        return LowerLambdaInvocation(lambdaInfo, arguments);
    }

    private HirValue EmitInvocation(IMethodSymbol method, HirValue? receiver, IReadOnlyList<HirValue> arguments)
    {
        var callArgs = new List<HirValue>(arguments.Count + (receiver is null ? 0 : 1));
        if (receiver is not null)
            callArgs.Add(receiver);
        callArgs.AddRange(arguments);

        if (TryEmitExternContractInvocation(method, callArgs, out var externalCall))
            return externalCall;

        if (TryEmitIntrinsicCall(method, callArgs, out var intrinsic))
            return intrinsic;

        if (TryEmitFrameworkHelper(method, callArgs, out var frameworkValue))
            return frameworkValue;

        var calleeName = TrimGlobal(method.ToDisplayString(FullyQualifiedMethodFormat));
        var call = new HirCall(
            calleeName,
            callArgs,
            MapType(method.ReturnType),
            method.IsStatic,
            HirCallSemantics.Effectful);

        _hirBuilder!.Append(call);
        return call.Type is HirVoidType ? EmitHirDefault(HirType.VoidType) : call;
    }

    private bool TryEmitExternContractInvocation(IMethodSymbol method, IReadOnlyList<HirValue> arguments, out HirValue value)
    {
        value = default!;

        if (_hirBuilder is null)
            return false;

        if (method.DeclaringSyntaxReferences.Length > 0 && !method.IsExtern)
            return false;

        var contractAttribute = method.ContainingType.GetAttributes()
            .FirstOrDefault(attr => string.Equals(attr.AttributeClass?.Name, "ContractAttribute", StringComparison.Ordinal));
        if (contractAttribute is null || contractAttribute.ConstructorArguments.Length == 0)
            return false;

        var hashLiteral = contractAttribute.ConstructorArguments[0].Value as string
            ?? throw new NotSupportedException("Contract attribute must supply a script hash.");

        var scriptHash = UInt160.Parse(hashLiteral);

        if (method.MethodKind == MethodKind.PropertyGet &&
            method.AssociatedSymbol is IPropertySymbol property &&
            property.GetAttributes().Any(attr => string.Equals(attr.AttributeClass?.Name, "ContractHashAttribute", StringComparison.Ordinal)))
        {
            var hashBytes = scriptHash.ToArray();
            var constant = EmitConstant(hashBytes);
            value = EnsureType(constant, MapType(method.ReturnType));
            return true;
        }

        var token = _context.AddMethodToken(
            scriptHash,
            method.GetDisplayName(true),
            (ushort)method.Parameters.Length,
            !method.ReturnsVoid || method.MethodKind == MethodKind.Constructor,
            CallFlags.All);

        if (token > byte.MaxValue)
            throw new NotSupportedException("Call table index exceeds supported byte range.");

        var pointerCall = new HirPointerCall(
            pointer: null,
            arguments: arguments.ToArray(),
            MapType(method.ReturnType),
            HirCallSemantics.Effectful,
            isTailCall: true,
            callTableIndex: (byte)token);

        _hirBuilder.Append(pointerCall);
        value = pointerCall.Type is HirVoidType ? EmitHirDefault(HirType.VoidType) : pointerCall;
        return true;
    }

    private HirValue EmitPropertyAssignment(SemanticModel model, IPropertySymbol property, ExpressionSyntax left, HirValue value)
    {
        if (property.SetMethod is null)
            throw new NotSupportedException($"Property '{property.Name}' does not define a setter.");

        HirValue? receiver = null;
        if (!property.IsStatic)
        {
            receiver = left switch
            {
                MemberAccessExpressionSyntax memberAccess => LowerExpression(model, memberAccess.Expression),
                _ => GetThisValue()
            };
        }

        EmitInvocation(property.SetMethod, receiver, new[] { value });
        return value;
    }

    private HirValue EmitPropertyValue(HirValue? receiver, IPropertySymbol property)
    {
        if (property.GetMethod is null)
            throw new NotSupportedException($"Property '{property.Name}' does not define a getter.");

        if (receiver is not null)
            EnsureNonNull(receiver);

        return EmitInvocation(property.GetMethod, receiver, Array.Empty<HirValue>());
    }

    private bool TryEmitIntrinsicCall(IMethodSymbol method, IReadOnlyList<HirValue> arguments, out HirValue value)
    {
        value = default!;
        if (_hirBuilder is null)
            return false;

        var containingType = method.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (containingType == "global::Neo.SmartContract.Framework.ExecutionEngine" && method.Name == "Abort")
        {
            if (arguments.Count == 0)
            {
                _hirBuilder.AppendTerminator(new HirAbort());
                value = EmitHirDefault(HirType.VoidType);
                return true;
            }

            if (arguments.Count == 1)
            {
                var message = EnsureType(arguments[0], HirType.ByteStringType);
                _hirBuilder.AppendTerminator(new HirAbortMessage(message));
                value = EmitHirDefault(HirType.VoidType);
                return true;
            }
        }

        var calleeName = TrimGlobal(method.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        if (TryResolveIntrinsic(calleeName, arguments, out value))
            return true;

        foreach (var attribute in method.GetAttributes())
        {
            if (!IsSyscallAttribute(attribute) || attribute.ConstructorArguments.Length == 0)
                continue;

            if (attribute.ConstructorArguments[0].Value is not string syscallName)
                continue;

            if (TryResolveIntrinsicSyscall(syscallName, arguments, out value))
                return true;
        }

        return false;
    }

    private bool TryResolveIntrinsic(string fullyQualifiedMethodName, IReadOnlyList<HirValue> arguments, out HirValue value)
    {
        value = default!;
        if (!HirIntrinsicCatalog.TryResolve(fullyQualifiedMethodName, out var metadata))
            return false;

        var coercedArguments = CoerceIntrinsicArguments(metadata, arguments);
        var intrinsic = new HirIntrinsicCall(metadata.Category, metadata.Name, coercedArguments, metadata);
        _hirBuilder!.Append(intrinsic);
        value = metadata.ReturnType is HirVoidType ? EmitHirDefault(HirType.VoidType) : intrinsic;
        return true;
    }

    private bool TryResolveIntrinsicSyscall(string syscallName, IReadOnlyList<HirValue> arguments, out HirValue value)
    {
        value = default!;
        if (!HirIntrinsicCatalog.TryResolveSyscall(syscallName, out var metadata))
            return false;

        var coercedArguments = CoerceIntrinsicArguments(metadata, arguments);
        var intrinsic = new HirIntrinsicCall(metadata.Category, metadata.Name, coercedArguments, metadata);
        _hirBuilder!.Append(intrinsic);
        value = metadata.ReturnType is HirVoidType ? EmitHirDefault(HirType.VoidType) : intrinsic;
        return true;
    }

    private bool TryEmitFrameworkHelper(IMethodSymbol method, IReadOnlyList<HirValue> arguments, out HirValue value)
    {
        value = default!;

        if (_hirBuilder is null)
            return false;

        if (IsSystemBufferBlockCopy(method, arguments, out var copy))
        {
            _hirBuilder.Append(copy);
            value = copy;
            return true;
        }

        return false;
    }

    private bool IsSystemBufferBlockCopy(IMethodSymbol method, IReadOnlyList<HirValue> arguments, out HirBufferCopy copy)
    {
        copy = default!;

        if (!method.IsStatic || method.Name != "BlockCopy" || arguments.Count != 5)
            return false;

        if (method.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) != "global::System.Buffer")
            return false;

        var source = arguments[0].Type is HirBufferType ? arguments[0] : EnsureType(arguments[0], HirType.BufferType);
        var sourceOffset = EnsureType(arguments[1], HirType.IntType);
        var destination = arguments[2].Type is HirBufferType ? arguments[2] : EnsureType(arguments[2], HirType.BufferType);
        var destinationOffset = EnsureType(arguments[3], HirType.IntType);
        var length = EnsureType(arguments[4], HirType.IntType);

        copy = new HirBufferCopy(destination, source, destinationOffset, sourceOffset, length);
        return true;
    }

    private IReadOnlyList<HirValue> CoerceIntrinsicArguments(HirIntrinsicMetadata metadata, IReadOnlyList<HirValue> arguments)
    {
        if (metadata.ParameterTypes.Count != arguments.Count)
            throw new NotSupportedException($"Intrinsic '{metadata.Category}.{metadata.Name}' expects {metadata.ParameterTypes.Count} arguments but received {arguments.Count}.");

        var coerced = new HirValue[arguments.Count];
        for (int i = 0; i < arguments.Count; i++)
        {
            var targetType = metadata.ParameterTypes[i];
            var argument = arguments[i];
            coerced[i] = targetType.Kind == HirTypeKind.Unknown ? argument : EnsureType(argument, targetType);
        }

        return coerced;
    }

    private static bool IsSyscallAttribute(AttributeData attribute)
    {
        if (attribute.AttributeClass is null)
            return false;

        var name = TrimGlobal(attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        return string.Equals(name, "Neo.SmartContract.Framework.Attributes.SyscallAttribute", StringComparison.Ordinal);
    }

    private HirValue EmitFieldValue(HirValue? receiver, IFieldSymbol field)
    {
        if (field.IsStatic)
            return EmitStaticFieldLoad(field);

        receiver ??= GetThisValue();
        var structTypeInstance = MapType(field.ContainingType) as HirStructType
            ?? throw new NotSupportedException("Struct field access requires struct type mapping.");
        var get = new HirStructGet(receiver, GetFieldIndex(structTypeInstance, field.Name), MapType(field.Type));
        _hirBuilder!.Append(get);
        return get;
    }

    private HirValue EmitFieldAssignment(SemanticModel model, IFieldSymbol field, ExpressionSyntax left, HirValue value)
    {
        if (field.IsStatic)
            return EmitStaticFieldStore(field, value);

        HirValue? receiver = left is MemberAccessExpressionSyntax memberAccess
            ? LowerExpression(model, memberAccess.Expression)
            : GetThisValue();

        receiver ??= GetThisValue();
        var nonNullReceiver = receiver ?? throw new InvalidOperationException("Instance field assignment requires a receiver.");

        EnsureNonNull(nonNullReceiver);

        var structType = MapType(field.ContainingType) as HirStructType
            ?? throw new NotSupportedException("Struct field assignment requires struct type mapping.");

        var set = new HirStructSet(nonNullReceiver, GetFieldIndex(structType, field.Name), value, structType);
        _hirBuilder!.Append(set);
        return value;
    }

    private HirValue EmitPrefixUnaryExpression(SemanticModel model, PrefixUnaryExpressionSyntax prefix)
    {
        var operand = LowerExpression(model, prefix.Operand);
        HirInst instruction = prefix.Kind() switch
        {
            SyntaxKind.UnaryMinusExpression => new HirNeg(operand, operand.Type),
            SyntaxKind.LogicalNotExpression => new HirNot(operand),
            _ => throw new NotSupportedException($"Prefix operator '{prefix.Kind()}' not yet supported in HIR conversion.")
        };
        _hirBuilder!.Append(instruction);
        return instruction;
    }

    private HirValue EmitConditionalExpression(SemanticModel model, ConditionalExpressionSyntax conditional)
    {
        var builder = _hirBuilder!;
        var condition = LowerExpression(model, conditional.Condition);
        var trueBlock = builder.CreateBlock(NewBlockLabel("cond_true"));
        var falseBlock = builder.CreateBlock(NewBlockLabel("cond_false"));
        var mergeBlock = builder.CreateBlock(NewBlockLabel("cond_merge"));
        var tempLocal = CreateSyntheticLocal("cond_result", MapType(model.GetTypeInfo(conditional).ConvertedType));

        AppendConditional(condition, trueBlock, falseBlock);

        builder.SetCurrentBlock(trueBlock);
        var trueValue = LowerExpression(model, conditional.WhenTrue);
        builder.Append(new HirStoreLocal(tempLocal, trueValue));
        AppendBranch(mergeBlock);

        builder.SetCurrentBlock(falseBlock);
        var falseValue = LowerExpression(model, conditional.WhenFalse);
        builder.Append(new HirStoreLocal(tempLocal, falseValue));
        AppendBranch(mergeBlock);

        builder.SetCurrentBlock(mergeBlock);
        return LoadLocal(tempLocal);
    }

    private HirValue EmitObjectCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax creation)
    {
        var typeInfo = model.GetTypeInfo(creation);
        var creationType = typeInfo.Type ?? typeInfo.ConvertedType;
        var convertedType = typeInfo.ConvertedType ?? typeInfo.Type;

        if (creationType is null)
            throw new NotSupportedException("Object creation without type information is not supported.");

        var arguments = creation.ArgumentList?.Arguments.Select(a => LowerExpression(model, a.Expression)).ToArray() ?? Array.Empty<HirValue>();

        if (model.GetSymbolInfo(creation).Symbol is IMethodSymbol ctorSymbol)
        {
            if (TryEmitIntrinsicCall(ctorSymbol, arguments, out var intrinsicValue))
                return intrinsicValue;
        }

        if (creationType.IsValueType || creationType.TypeKind == TypeKind.Struct)
        {
            var structType = MapType(creationType) as HirStructType
                ?? throw new NotSupportedException("Struct constructor mapping failed.");

            if (creation.Initializer is not null)
            {
                if (arguments.Length > 0)
                    throw new NotSupportedException("Struct initializers with constructor arguments are not yet supported in HIR conversion.");

                return EmitStructInitializer(model, creation.Initializer, structType);
            }

            var newStruct = new HirNewStruct(arguments, structType);
            _hirBuilder!.Append(newStruct);
            return newStruct;
        }

        var structMapping = MapType(convertedType) as HirStructType ?? new HirStructType(Array.Empty<HirField>());
        var newObj = new HirNewObject(creationType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), arguments, structMapping);
        _hirBuilder!.Append(newObj);

        return newObj;
    }

    private HirValue EmitStructInitializer(SemanticModel model, InitializerExpressionSyntax initializer, HirStructType structType)
    {
        if (_hirBuilder is null)
            throw new InvalidOperationException("HIR builder is not initialized.");

        var fieldValues = structType.Fields
            .Select(field => EnsureType(null, field.Type))
            .ToArray();

        foreach (var expression in initializer.Expressions)
        {
            if (expression is not AssignmentExpressionSyntax assignment)
                throw new NotSupportedException("Only assignment expressions are supported inside struct initializers.");

            var fieldName = assignment.Left switch
            {
                IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
                _ => throw new NotSupportedException("Unsupported struct initializer target.")
            };

            var fieldIndex = GetFieldIndex(structType, fieldName);
            var value = LowerExpression(model, assignment.Right);
            fieldValues[fieldIndex] = EnsureType(value, structType.Fields[fieldIndex].Type);
        }

        var rewritten = new HirNewStruct(fieldValues, structType);
        _hirBuilder.Append(rewritten);
        return rewritten;
    }

    private HirValue EmitArrayCreationExpression(SemanticModel model, ArrayCreationExpressionSyntax arrayCreation)
    {
        var arrayType = (IArrayTypeSymbol)model.GetTypeInfo(arrayCreation).ConvertedType!;
        if (arrayType.Rank != 1)
            throw new NotSupportedException("Multi-dimensional arrays are not supported.");

        var elementType = MapType(arrayType.ElementType);
        var rankSpecifier = arrayCreation.Type.RankSpecifiers.FirstOrDefault();
        if (rankSpecifier is null || rankSpecifier.Sizes.Count == 0)
            throw new NotSupportedException("Array creation requires an explicit length or initializer.");

        HirValue length;
        var sizeSyntax = rankSpecifier.Sizes[0];
        if (sizeSyntax is OmittedArraySizeExpressionSyntax)
        {
            if (arrayCreation.Initializer is null)
                throw new NotSupportedException("Array creation without explicit length or initializer is not supported.");
            length = EmitConstant(arrayCreation.Initializer.Expressions.Count);
        }
        else
        {
            length = LowerExpression(model, sizeSyntax);
        }

        var arrayNew = new HirArrayNew(length, elementType);
        _hirBuilder!.Append(arrayNew);

        if (arrayCreation.Initializer is not null)
            PopulateArrayInitializer(model, arrayNew, elementType, arrayCreation.Initializer);

        return arrayNew;
    }

    private HirValue EmitTupleExpression(SemanticModel model, TupleExpressionSyntax tuple)
    {
        var typeInfo = model.GetTypeInfo(tuple).ConvertedType as INamedTypeSymbol
            ?? throw new NotSupportedException("Tuple expression without type information is not supported.");

        var structType = MapType(typeInfo) as HirStructType
            ?? throw new NotSupportedException("Tuple mapping to struct type failed.");

        var values = new HirValue[tuple.Arguments.Count];
        for (int i = 0; i < tuple.Arguments.Count; i++)
        {
            var element = LowerExpression(model, tuple.Arguments[i].Expression);
            values[i] = EnsureType(element, structType.Fields[i].Type);
        }

        var newStruct = new HirNewStruct(values, structType);
        _hirBuilder!.Append(newStruct);
        return newStruct;
    }

    private HirValue EmitArrayCreationExpression(SemanticModel model, ImplicitArrayCreationExpressionSyntax arrayCreation)
    {
        if (arrayCreation.Initializer is null)
            throw new NotSupportedException("Implicit array creation requires an initializer.");

        var arrayType = model.GetTypeInfo(arrayCreation).ConvertedType as IArrayTypeSymbol
            ?? throw new NotSupportedException("Implicit array creation must resolve to an array type.");

        var elementType = MapType(arrayType.ElementType);
        var length = EmitConstant(arrayCreation.Initializer.Expressions.Count);
        var arrayNew = new HirArrayNew(length, elementType);
        _hirBuilder!.Append(arrayNew);

        PopulateArrayInitializer(model, arrayNew, elementType, arrayCreation.Initializer);
        return arrayNew;
    }

    private HirValue EmitCollectionExpression(SemanticModel model, CollectionExpressionSyntax collection)
    {
        var info = model.GetTypeInfo(collection);
        var targetType = info.ConvertedType ?? info.Type
            ?? throw new NotSupportedException("Collection expression without type information is not supported.");

        targetType = UnwrapNullable(targetType);

        return targetType switch
        {
            IArrayTypeSymbol arrayType => EmitCollectionArrayExpression(model, collection, arrayType),
            _ => throw new NotSupportedException($"Collection expression for target type '{targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}' is not supported.")
        };
    }

    private HirValue EmitCollectionArrayExpression(
        SemanticModel model,
        CollectionExpressionSyntax collection,
        IArrayTypeSymbol arrayType)
    {
        if (_hirBuilder is null)
            throw new InvalidOperationException("HIR builder is not initialized.");

        if (arrayType.Rank != 1)
            throw new NotSupportedException("Multi-dimensional arrays are not supported in collection expressions.");

        if (TryEmitConstantByteCollection(model, collection, arrayType, out var constant))
            return constant;

        var elementType = MapType(arrayType.ElementType);
        var length = EmitConstant(collection.Elements.Count);
        var arrayNew = new HirArrayNew(length, elementType);
        _hirBuilder.Append(arrayNew);

        for (int index = 0; index < collection.Elements.Count; index++)
        {
            var element = LowerCollectionElement(model, collection.Elements[index]);
            element = EnsureType(element, elementType);
            var indexValue = EmitConstant(index);
            _hirBuilder.Append(new HirArraySet(arrayNew, indexValue, element));
        }

        return arrayNew;
    }

    private HirValue LowerCollectionElement(SemanticModel model, CollectionElementSyntax element)
        => element switch
        {
            ExpressionElementSyntax exprElement => LowerExpression(model, exprElement.Expression),
            _ => throw new NotSupportedException($"Collection element '{element.Kind()}' is not supported.")
        };

    private bool TryEmitConstantByteCollection(
        SemanticModel model,
        CollectionExpressionSyntax collection,
        IArrayTypeSymbol arrayType,
        out HirValue constant)
    {
        constant = null!;

        if (arrayType.ElementType.SpecialType != SpecialType.System_Byte)
            return false;

        var data = new byte[collection.Elements.Count];
        for (int i = 0; i < collection.Elements.Count; i++)
        {
            if (collection.Elements[i] is not ExpressionElementSyntax expressionElement)
                return false;

            var constantValue = model.GetConstantValue(expressionElement.Expression);
            if (!constantValue.HasValue)
                return false;

            data[i] = Convert.ToByte(constantValue.Value);
        }

        constant = EmitConstant(data);
        return true;
    }

    private HirValue EmitArrayInitializerExpression(
        SemanticModel model,
        InitializerExpressionSyntax initializer)
    {
        if (_hirBuilder is null)
            throw new InvalidOperationException("HIR builder is not initialized.");

        var typeInfo = model.GetTypeInfo(initializer).ConvertedType as IArrayTypeSymbol
            ?? throw new NotSupportedException("Array initializer must resolve to an array type.");

        var elementType = MapType(typeInfo.ElementType);
        var length = EmitConstant(initializer.Expressions.Count);
        var arrayNew = new HirArrayNew(length, elementType);
        _hirBuilder.Append(arrayNew);

        PopulateArrayInitializer(model, arrayNew, elementType, initializer);
        return arrayNew;
    }

    private void PopulateArrayInitializer(
        SemanticModel model,
        HirValue array,
        HirType elementType,
        InitializerExpressionSyntax initializer)
    {
        for (int index = 0; index < initializer.Expressions.Count; index++)
        {
            var elementExpression = initializer.Expressions[index];
            var value = LowerExpression(model, elementExpression);
            value = EnsureType(value, elementType);
            var indexValue = EmitConstant(index);

            var set = new HirArraySet(array, indexValue, value);
            _hirBuilder!.Append(set);
        }
    }

    private HirValue EmitElementAccessExpression(SemanticModel model, ElementAccessExpressionSyntax elementAccess)
    {
        var target = LowerExpression(model, elementAccess.Expression);
        var indices = elementAccess.ArgumentList.Arguments.Select(a => LowerExpression(model, a.Expression)).ToArray();
        var targetType = model.GetTypeInfo(elementAccess.Expression).ConvertedType;

        if (targetType is IArrayTypeSymbol arrayType)
        {
            if (indices.Length != 1)
                throw new NotSupportedException("Only single-dimension arrays are supported.");

            var get = new HirArrayGet(target, indices[0], MapType(arrayType.ElementType));
            _hirBuilder!.Append(get);
            return get;
        }

        if (model.GetSymbolInfo(elementAccess).Symbol is IPropertySymbol indexer && indexer.GetMethod is not null)
            return EmitInvocation(indexer.GetMethod, target, indices);

        if (model.GetSymbolInfo(elementAccess).Symbol is IPropertySymbol missingGetter)
            throw new NotSupportedException($"Indexer '{missingGetter.Name}' does not define a getter.");

        throw new NotSupportedException("Element access is only supported for arrays and indexers.");
    }

    private HirValue EmitCastExpression(SemanticModel model, CastExpressionSyntax cast)
    {
        var value = LowerExpression(model, cast.Expression);
        var targetType = MapType(model.GetTypeInfo(cast.Type).ConvertedType);
        if (Equals(value.Type, targetType))
            return value;

        var conv = new HirConvert(HirConvKind.Narrow, value, targetType);
        _hirBuilder!.Append(conv);
        return conv;
    }

    private HirValue EmitConditionalAccessExpression(SemanticModel model, ConditionalAccessExpressionSyntax conditionalAccess)
    {
        if (conditionalAccess.Expression is IdentifierNameSyntax eventIdentifier &&
            model.GetSymbolInfo(eventIdentifier).Symbol is IEventSymbol eventSymbol &&
            conditionalAccess.WhenNotNull is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberBindingExpressionSyntax memberBinding &&
            string.Equals(memberBinding.Name.Identifier.ValueText, "Invoke", StringComparison.Ordinal))
        {
            var eventArgs = invocation.ArgumentList.Arguments
                .Select(arg => LowerExpression(model, arg.Expression))
                .ToList();
            return EmitEventInvocation(eventSymbol, eventArgs, invocation);
        }

        var receiver = LowerExpression(model, conditionalAccess.Expression);
        EnsureNonNull(receiver);
        return LowerExpression(model, conditionalAccess.WhenNotNull);
    }

    private void EmitDisposeCall(SemanticModel model, ITypeSymbol resourceTypeSymbol, HirValue resource, bool isAsync)
    {
        if (isAsync)
        {
            var disposeAsync = FindMethod(resourceTypeSymbol, "DisposeAsync", model.Compilation);
            if (disposeAsync is null)
                throw new NotSupportedException($"Type '{resourceTypeSymbol.ToDisplayString()}' does not implement DisposeAsync required for await using.");

            EmitInvocation(disposeAsync!, resource, Array.Empty<HirValue>());
            return;
        }

        var dispose = FindMethod(resourceTypeSymbol, "Dispose", model.Compilation);
        if (dispose is null)
            throw new NotSupportedException($"Type '{resourceTypeSymbol.ToDisplayString()}' does not implement Dispose required for using statement.");

        EmitInvocation(dispose!, resource, Array.Empty<HirValue>());
    }

    private static IMethodSymbol? FindMethod(ITypeSymbol type, string name, Compilation compilation)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            var candidate = current.GetMembers(name)
                .OfType<IMethodSymbol>()
                .FirstOrDefault(m => m.Parameters.Length == 0);
            if (candidate is not null)
                return candidate;
        }

        return compilation.GetTypeByMetadataName("System.IDisposable")?
            .GetMembers(name)
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.Parameters.Length == 0);
    }

    private HirValue ResolveIndexerAssignment(SemanticModel model, ElementAccessExpressionSyntax elementAccess, HirValue value)
    {
        var target = LowerExpression(model, elementAccess.Expression);
        var typeInfo = model.GetTypeInfo(elementAccess.Expression).ConvertedType;
        var indices = elementAccess.ArgumentList.Arguments.Select(a => LowerExpression(model, a.Expression)).ToArray();

        if (typeInfo is IArrayTypeSymbol)
        {
            if (indices.Length != 1)
                throw new NotSupportedException("Only single-dimension arrays are supported.");

            var store = new HirArraySet(target, indices[0], value);
            _hirBuilder!.Append(store);
            return value;
        }

        if (model.GetSymbolInfo(elementAccess).Symbol is IPropertySymbol indexer && indexer.SetMethod is not null)
        {
            var args = indices.Concat(new[] { value }).ToArray();
            EmitInvocation(indexer.SetMethod, target, args);
            return value;
        }

        throw new NotSupportedException("Indexer assignment is not supported for this target.");
    }

}
