using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Compiler.HIR;

namespace Neo.Compiler.HIR.Import;

internal sealed partial class HirMethodImporter
{
    private void InitializeHirBuilder()
    {
        if (!_context.Options.EnableHir)
            return;

        var module = _context.HirModule;
        if (module is null)
            return;

        var signature = BuildHirSignature();
        var functionName = _symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var function = module.GetOrAddFunction(functionName, signature);

        _hirBuilder = new HirBuilder(function);
        _hirThisArgument = null;
        _hirBlockCounters.Clear();
        _hirTempCounter = 0;
        _hirCurrentState = new HirSsaState();

        _hirLocals.Clear();
        _hirArguments.Clear();
        _hirSequencePoints.Clear();
        _hirBreakTargets.Clear();
        _hirContinueTargets.Clear();
        _hirTryScopes.Clear();
        _hirReturnBlock = null;
        _hirReturnValueSlot = null;

        InitializeHirArguments(signature);
    }

    private HirSignature BuildHirSignature()
    {
        var parameterTypes = new List<HirType>();

        if (!_symbol.IsStatic)
            parameterTypes.Add(MapType(_symbol.ContainingType));

        foreach (var parameter in _symbol.Parameters)
            parameterTypes.Add(MapType(parameter.Type));

        var returnType = _symbol.ReturnsVoid ? HirType.VoidType : MapType(_symbol.ReturnType);
        var attributes = HirAttributeBuilder.Collect(_context.Options, _symbol);
        return new HirSignature(parameterTypes, returnType, attributes);
    }

    private void InitializeHirArguments(HirSignature signature)
    {
        if (_hirBuilder is null)
            return;

        int index = 0;

        if (!_symbol.IsStatic)
        {
            var thisType = signature.ParameterTypes[index];
            _hirThisArgument = new HirArgument("this", thisType, index);
            _hirCurrentState?.Assign(_symbol, _hirThisArgument);
            index++;
        }

        foreach (var parameter in _symbol.Parameters)
        {
            var type = signature.ParameterTypes[index];
            var argument = new HirArgument(parameter.Name, type, index);
            _hirArguments[parameter] = argument;
            AssignParameterSymbol(parameter, argument);
            index++;
        }
    }

    private void ConvertToHir(SemanticModel model)
    {
        if (_hirBuilder is null)
            return;

        if (_symbol.DeclaringSyntaxReferences.Length == 0)
            return;

        foreach (var reference in _symbol.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();
            switch (syntax)
            {
                case MethodDeclarationSyntax methodSyntax:
                    ConvertMethodSyntax(model, methodSyntax);
                    return;
                case AccessorDeclarationSyntax accessorSyntax:
                    ConvertAccessorSyntax(model, accessorSyntax);
                    return;
                case LocalFunctionStatementSyntax localFunction:
                    ConvertLocalFunctionSyntax(model, localFunction);
                    return;
                case ConstructorDeclarationSyntax ctorSyntax:
                    ConvertConstructorSyntax(model, ctorSyntax);
                    return;
                case OperatorDeclarationSyntax operatorSyntax:
                    ConvertOperatorSyntax(model, operatorSyntax);
                    return;
                case ConversionOperatorDeclarationSyntax conversionSyntax:
                    ConvertConversionOperatorSyntax(model, conversionSyntax);
                    return;
            }
        }

        throw new NotSupportedException($"HIR import does not yet support symbol kind '{_symbol.Kind}'.");
    }

    private void ConvertMethodSyntax(SemanticModel model, MethodDeclarationSyntax methodSyntax)
    {
        if (methodSyntax.ExpressionBody is not null)
        {
            EmitExpressionBodyReturn(model, methodSyntax.ExpressionBody.Expression, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
            return;
        }

        if (methodSyntax.Body is not null)
        {
            var (_, terminated) = ConvertBlock(model, methodSyntax.Body);
            EnsureBlockTerminated(terminated, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
        }

        // No body; nothing to import.
    }

    private void ConvertAccessorSyntax(SemanticModel model, AccessorDeclarationSyntax accessorSyntax)
    {
        if (accessorSyntax.ExpressionBody is not null)
        {
            EmitExpressionBodyReturn(model, accessorSyntax.ExpressionBody.Expression, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
            return;
        }

        if (accessorSyntax.Body is not null)
        {
            var (_, terminated) = ConvertBlock(model, accessorSyntax.Body);
            EnsureBlockTerminated(terminated, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
        }

        // No body; nothing to import.
    }

    private void ConvertLocalFunctionSyntax(SemanticModel model, LocalFunctionStatementSyntax localFunction)
    {
        if (localFunction.ExpressionBody is not null)
        {
            EmitExpressionBodyReturn(model, localFunction.ExpressionBody.Expression, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
            return;
        }

        if (localFunction.Body is not null)
        {
            var (_, terminated) = ConvertBlock(model, localFunction.Body);
            EnsureBlockTerminated(terminated, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
        }

        // No body; nothing to import.
    }

    private void ConvertConstructorSyntax(SemanticModel model, ConstructorDeclarationSyntax ctorSyntax)
    {
        if (ctorSyntax.ExpressionBody is not null)
        {
            EmitExpressionBodyReturn(model, ctorSyntax.ExpressionBody.Expression, returnsVoid: true, null);
            return;
        }

        if (ctorSyntax.Body is not null)
        {
            var (_, terminated) = ConvertBlock(model, ctorSyntax.Body);
            EnsureBlockTerminated(terminated, true, null);
        }

        // No body; nothing to import.
    }

    private void ConvertOperatorSyntax(SemanticModel model, OperatorDeclarationSyntax operatorSyntax)
    {
        if (operatorSyntax.ExpressionBody is not null)
        {
            EmitExpressionBodyReturn(model, operatorSyntax.ExpressionBody.Expression, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
            return;
        }

        if (operatorSyntax.Body is not null)
        {
            var (_, terminated) = ConvertBlock(model, operatorSyntax.Body);
            EnsureBlockTerminated(terminated, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
        }

        // No body; nothing to import.
    }

    private void ConvertConversionOperatorSyntax(SemanticModel model, ConversionOperatorDeclarationSyntax conversionSyntax)
    {
        if (conversionSyntax.ExpressionBody is not null)
        {
            EmitExpressionBodyReturn(model, conversionSyntax.ExpressionBody.Expression, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
            return;
        }

        if (conversionSyntax.Body is not null)
        {
            var (_, terminated) = ConvertBlock(model, conversionSyntax.Body);
            EnsureBlockTerminated(terminated, _symbol.ReturnsVoid, _symbol.ReturnsVoid ? null : MapType(_symbol.ReturnType));
        }

        // No body; nothing to import.
    }

    private HirType MapType(ITypeSymbol? symbol)
    {
        if (symbol is null)
            return HirType.UnknownType;

        symbol = UnwrapNullable(symbol);

        if (_hirTypeCache.TryGetValue(symbol, out var cached))
            return cached;

        HirType result = symbol switch
        {
            IArrayTypeSymbol arrayType => MapArrayType(arrayType),
            IPointerTypeSymbol => HirType.BufferType,
            INamedTypeSymbol namedType => MapNamedType(namedType),
            _ => HirType.UnknownType
        };

        if (!_hirTypeCache.ContainsKey(symbol))
            _hirTypeCache[symbol] = result;

        return result;
    }

    private static ITypeSymbol UnwrapNullable(ITypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol named &&
            named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T &&
            named.TypeArguments.Length == 1)
        {
            return named.TypeArguments[0];
        }

        return symbol;
    }

    private HirType MapNamedType(INamedTypeSymbol named)
    {
        if (named is null)
            return HirType.UnknownType;

        if (_hirTypeCache.TryGetValue(named, out var cached))
            return cached;

        if (named.SpecialType != SpecialType.None)
        {
            switch (named.SpecialType)
            {
                case SpecialType.System_Void:
                    return CacheType(named, HirType.VoidType);
                case SpecialType.System_Boolean:
                    return CacheType(named, HirType.BoolType);
                case SpecialType.System_String:
                    return CacheType(named, HirType.ByteStringType);
                case SpecialType.System_Char:
                    return CacheType(named, new HirIntType(16, false));
                case SpecialType.System_SByte:
                    return CacheType(named, new HirIntType(8, true));
                case SpecialType.System_Byte:
                    return CacheType(named, new HirIntType(8, false));
                case SpecialType.System_Int16:
                    return CacheType(named, new HirIntType(16, true));
                case SpecialType.System_UInt16:
                    return CacheType(named, new HirIntType(16, false));
                case SpecialType.System_Int32:
                    return CacheType(named, new HirIntType(32, true));
                case SpecialType.System_UInt32:
                    return CacheType(named, new HirIntType(32, false));
                case SpecialType.System_Int64:
                    return CacheType(named, new HirIntType(64, true));
                case SpecialType.System_UInt64:
                    return CacheType(named, new HirIntType(64, false));
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Decimal:
                    throw new NotSupportedException("Floating-point types are not supported in Neo smart contracts.");
                case SpecialType.System_Object:
                    return CacheType(named, HirType.UnknownType);
                default:
                    return CacheType(named, HirType.IntType);
            }
        }

        var qualifiedName = TrimGlobal(named.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        // Treat DevPack interop handles and framework-specific aliases.
        if (qualifiedName is "Neo.SmartContract.Framework.ByteString")
            return CacheType(named, HirType.ByteStringType);

        if (qualifiedName is "Neo.SmartContract.Framework.Buffer")
            return CacheType(named, HirType.BufferType);

        if (qualifiedName is "Neo.SmartContract.Framework.Services.Storage.StorageContext")
            return CacheType(named, new HirInteropHandleType("StorageContext"));

        if (qualifiedName is "Neo.SmartContract.Framework.Services.Storage.StorageMap")
            return CacheType(named, new HirInteropHandleType("StorageMap"));

        if (qualifiedName.StartsWith("Neo.SmartContract.Framework.Services.Iterator", StringComparison.Ordinal))
        {
            if (named.TypeArguments.Length == 1)
                return CacheType(named, new HirIteratorType(MapType(named.TypeArguments[0])));
            return CacheType(named, new HirIteratorType());
        }

        if (DerivesFrom(named, "Neo.SmartContract.Framework.ByteString"))
            return CacheType(named, HirType.ByteStringType);

        if (qualifiedName is "System.Numerics.BigInteger")
            return CacheType(named, HirType.IntType);

        if (ImplementsInterface(named, "Neo.SmartContract.Framework.IApiInterface"))
            return CacheType(named, new HirInteropHandleType(named.Name));

        if (named.TypeKind == TypeKind.Enum)
            return CacheType(named, HirType.IntType);

        if (named.IsReferenceType && named.Name is "ByteString")
            return CacheType(named, HirType.ByteStringType);

        if (named.TypeKind == TypeKind.Struct || named.TypeKind == TypeKind.Class || named.TypeKind == TypeKind.Interface)
        {
            if (_hirTypeCache.TryGetValue(named, out var existing))
                return existing;

            var placeholderFields = new List<HirField>();
            var placeholder = new HirStructType(placeholderFields);
            _hirTypeCache[named] = placeholder;

            foreach (var field in named.GetMembers()
                         .OfType<IFieldSymbol>()
                         .Where(f => !f.IsStatic)
                         .OrderBy(f => f.Name, StringComparer.Ordinal))
            {
                placeholderFields.Add(new HirField(field.Name, MapType(field.Type), !field.Type.IsValueType));
            }

            return placeholder;
        }

        return CacheType(named, HirType.UnknownType);
    }

    private HirType MapArrayType(IArrayTypeSymbol arrayType)
    {
        if (arrayType is null)
            return HirType.UnknownType;

        if (arrayType.Rank == 1 &&
            arrayType.ElementType.SpecialType == SpecialType.System_Byte)
        {
            return HirType.BufferType;
        }

        return new HirArrayType(
            MapType(arrayType.ElementType),
            arrayType.ElementNullableAnnotation == NullableAnnotation.Annotated);
    }

    private HirType CacheType(ITypeSymbol symbol, HirType type)
    {
        _hirTypeCache[symbol] = type;
        return type;
    }

    private static bool ImplementsInterface(INamedTypeSymbol symbol, string fullyQualifiedInterfaceName)
    {
        if (symbol is null)
            return false;

        var target = TrimGlobal(fullyQualifiedInterfaceName);
        foreach (var iface in symbol.AllInterfaces)
        {
            if (TrimGlobal(iface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) == target)
                return true;
        }

        return false;
    }

    private static bool DerivesFrom(INamedTypeSymbol symbol, string fullyQualifiedBaseName)
    {
        if (symbol is null)
            return false;

        var target = TrimGlobal(fullyQualifiedBaseName);
        var current = symbol.BaseType;
        while (current is not null)
        {
            if (TrimGlobal(current.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) == target)
                return true;
            current = current.BaseType;
        }

        return false;
    }

    private static string TrimGlobal(string value)
        => value.StartsWith("global::", StringComparison.Ordinal) ? value[8..] : value;
}
