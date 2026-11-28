// Copyright (C) 2015-2026 The Neo Project.
//
// SystemCall.Enum.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Gets the module symbol for error reporting purposes.
    /// </summary>
    /// <param name="symbol">The symbol to get the module for</param>
    /// <param name="symbolInfo">Symbol information for error messages</param>
    /// <returns>The module symbol</returns>
    /// <exception cref="CompilationException">Thrown when module metadata is not available</exception>
    private static IModuleSymbol GetSymbolMetadataModule(ISymbol symbol, string symbolInfo)
    {
        var metadata = symbol.Locations.FirstOrDefault()?.MetadataModule;
        if (metadata is null)
            throw new CompilationException(symbol, DiagnosticId.InvalidArgument, $"Unexpected symbol location metadata module for {symbolInfo}");
        return metadata;
    }

    private static IFieldSymbol[] GetEnumFields(INamedTypeSymbol enumTypeSymbol)
    {
        return enumTypeSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false })
            .ToArray();
    }

    private static IFieldSymbol[] GetSortedEnumFields(INamedTypeSymbol enumTypeSymbol)
    {
        int bits = enumTypeSymbol.EnumUnderlyingType?.SpecialType switch
        {
            SpecialType.System_SByte or SpecialType.System_Byte => 8,
            SpecialType.System_Int16 or SpecialType.System_UInt16 => 16,
            SpecialType.System_Int32 or SpecialType.System_UInt32 => 32,
            SpecialType.System_Int64 or SpecialType.System_UInt64 => 64,
            _ => throw new ArgumentOutOfRangeException(nameof(enumTypeSymbol)),
        };
        BigInteger mask = (BigInteger.One << bits) - 1;
        return GetEnumFields(enumTypeSymbol)
            .OrderBy(field => ToBigIntegerConstant(field.ConstantValue) & mask)
            .ToArray();
    }

    /// <summary>
    /// Handles the Enum.Parse method by parsing a string representation into an enum value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Parses member names, decimal values, and comma-separated names; throws on invalid input.
    /// </remarks>
    private static void HandleEnumParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, false, false, false);
    }

    /// <summary>
    /// Handles the Enum.Parse method with case-insensitive parsing support.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Parses member names, decimal values, and comma-separated names with optional case folding.
    /// </remarks>
    private static void HandleEnumParseIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, false, true, false);
    }

    /// <summary>
    /// Handles the Enum.TryParse method with case-insensitive parsing and out parameter support.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Parses an enum value with optional case folding and stores it in the out parameter.
    /// </remarks>
    private static void HandleEnumTryParseIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, false, true, true);
    }

    /// <summary>
    /// Handles the Enum.IsDefined method by checking if a string name exists in the enum.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iterates through enum members, compares names, returns true if found
    /// </remarks>
    private static void HandleEnumIsDefinedByName(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax)?.Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.IsDefined"), DiagnosticId.InvalidArgument, "Enum.IsDefined requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();
        var endTarget = new JumpTarget();
        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                                   // Duplicate input name
            methodConvert.Push(t.Name);                            // Push enum name
            methodConvert.Equal();                                 // Compare strings

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);                    // If not equal, check next
            methodConvert.Push(true);                              // Push true (enum name is defined)
            methodConvert.JumpAlwaysLong(endTarget);

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        methodConvert.Push(false);                                 // Push false (enum name is not defined)
        endTarget.Instruction = methodConvert.Nip();                // Remove the duplicated input name
    }

    /// <summary>
    /// Handles the Enum.GetName method by returning the name for a given enum value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iterates through enum members, compares values, returns matching name or null
    /// </remarks>
    private static void HandleEnumGetName(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the Enum value
        var enumType = symbol.Parameters[0].Type;
        if (enumType is not INamedTypeSymbol enumTypeSymbol)
        {
            throw new CompilationException(symbol.Locations.FirstOrDefault()!.MetadataModule!, DiagnosticId.InvalidType, "Unable to determine enum type");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        var cases = enumMembers
            .Select(t => (ToBigIntegerConstant(t.ConstantValue), (Action)(() => methodConvert.Push(t.Name))))
            .ToArray();

        methodConvert.EmitSwitch(
            valueEmitter: () => { },  // value is already on the stack
            cases: cases,
            defaultBody: () => methodConvert.PushNull());
    }

    /// <summary>
    /// Converts a boxed enum constant value to <see cref="BigInteger"/> while preserving
    /// the full integral domain of every possible enum backing type, including
    /// <see langword="ulong"/> values above <see cref="long.MaxValue"/>.
    /// </summary>
    private static BigInteger ToBigIntegerConstant(object? constantValue) => constantValue switch
    {
        sbyte v => v,
        byte v => v,
        short v => v,
        ushort v => v,
        int v => v,
        uint v => v,
        long v => v,
        ulong v => v,
        _ => throw new ArgumentOutOfRangeException(nameof(constantValue), constantValue, "Unsupported enum backing type"),
    };

    /// <summary>
    /// Handles the Enum.GetName method with explicit type parameter.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Similar to GetName but uses explicit type from typeof argument
    /// </remarks>
    private static void HandleEnumGetNameWithType(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is not null && arguments.Count > 0)
        {
            if ((arguments[0] as ArgumentSyntax)?.Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.GetName"), DiagnosticId.InvalidArgument, "Enum.GetName requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments[0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();
        var endTarget = new JumpTarget();
        methodConvert.Nip();                     // Drop the enum type
        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                 // Duplicate input value
            methodConvert.Push(t.ConstantValue); // Push enum value, the enum value is an int value

            var nextCheck = new JumpTarget();
            methodConvert.Jump(OpCode.JMPNE, nextCheck);  // If not numeric equal, check next

            methodConvert.Drop();       // Remove the duplicated input value
            methodConvert.Push(t.Name); // Push enum name
            methodConvert.JumpAlwaysLong(endTarget);

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        methodConvert.Drop();     // Remove the input value
        methodConvert.PushNull(); // Push null (no matching enum name)
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles the Enum.TryParse method by attempting to parse a string into an enum value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Parses an enum value, stores it in the out parameter, and returns a success flag.
    /// </remarks>
    private static void HandleEnumTryParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, false, false, true);
    }

    /// <summary>
    /// Handles the Enum.GetNames method by returning all enum member names as an array.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Pushes all enum names onto stack then packs them into an array
    /// </remarks>
    private static void HandleEnumGetNames(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax)?.Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.GetNames"), DiagnosticId.InvalidArgument, "Enum.GetNames requires one argument");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = GetSortedEnumFields((INamedTypeSymbol)enumTypeSymbol);

        methodConvert.Drop(); // Consume the typeof(enum) argument before pushing the result.

        // Create an array of names
        foreach (IFieldSymbol m in enumMembers.Reverse())          // PACK works in a reversed way
            methodConvert.Push(m.Name);
        methodConvert.Pack(enumMembers.Length);
    }

    /// <summary>
    /// Handles the Enum.GetValues method by returning all enum member values as an array.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Pushes all enum constant values onto stack then packs them into an array
    /// </remarks>
    private static void HandleEnumGetValues(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax)?.Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.GetValues"), DiagnosticId.InvalidArgument, "Enum.GetValues requires one argument");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = GetSortedEnumFields((INamedTypeSymbol)enumTypeSymbol);

        methodConvert.Drop(); // Consume the typeof(enum) argument before pushing the result.

        // Create an array of values
        foreach (IFieldSymbol m in enumMembers.Reverse())          // PACK works in a reversed way
            methodConvert.Push(m.ConstantValue);
        methodConvert.Pack(enumMembers.Length);
    }

    /// <summary>
    /// Handles the Enum.IsDefined method by checking if a value or name exists in the enum.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Determines if checking by name or value, iterates through enum members for matches
    /// </remarks>
    private static void HandleEnumIsDefined(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 0 })
        {
            if ((arguments[0] as ArgumentSyntax)?.Expression is TypeOfExpressionSyntax typeOfExpression)
            {
                var typeInfo = model.GetTypeInfo(typeOfExpression.Type);
                enumTypeSymbol = typeInfo.Type;
            }
            else
            {
                throw new CompilationException(arguments[0], DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
            }
        }
        else
        {
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.IsDefined"), DiagnosticId.InvalidArgument, "Enum.IsDefined requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        if (arguments[1] is not ArgumentSyntax argument) // unexpected case
            throw new CompilationException(arguments[1], DiagnosticId.InvalidArgument, "Invalid second argument");

        var valueType = model.GetTypeInfo(argument.Expression).Type;

        // We need to compare the input value against the enum values
        var endTarget = new JumpTarget();

        // here add check logic to verify if valueType is string
        var isName = valueType is INamedTypeSymbol { Name: "String" };

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                                   // Duplicate value to compare
            if (isName)
            {
                methodConvert.Push(t.Name);
            }
            else
            {
                methodConvert.Push(t.ConstantValue);
            }

            var nextCheck = new JumpTarget();
            methodConvert.Equal();                             // Stack: [..., isEqual]
            methodConvert.JumpIfNot(nextCheck);

            methodConvert.Push(true);                          // Set result to true
            methodConvert.JumpAlwaysLong(endTarget);
            nextCheck.Instruction = methodConvert.Nop();
        }

        methodConvert.Push(false);
        endTarget.Instruction = methodConvert.Nip();   // Remove the EnumType and duplicated value
        methodConvert.Nip();
    }

    private static INamedTypeSymbol EnsureEnumTypeArgument(IMethodSymbol symbol, int index, SyntaxNode? errorNode)
    {
        if (symbol.TypeArguments.Length <= index) throw new CompilationException(symbol, DiagnosticId.InvalidType, "Generic enum operation requires an enum type argument.");
        if (symbol.TypeArguments[index] is not INamedTypeSymbol enumType)
            throw new CompilationException(symbol, DiagnosticId.InvalidType, "Generic enum operation requires an enum type argument.");
        if (enumType.TypeKind != TypeKind.Enum)
        {
            if (errorNode is not null)
                throw new CompilationException(errorNode, DiagnosticId.InvalidType, "Type argument must be an enum.");
            throw new CompilationException(symbol, DiagnosticId.InvalidType, "Type argument must be an enum.");
        }
        return enumType;
    }

    private static void HandleEnumToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is null)
            throw new CompilationException(symbol, DiagnosticId.InvalidArgument, "Enum.ToString requires an instance.");

        var enumType = model.GetTypeInfo(instanceExpression).Type as INamedTypeSymbol
                       ?? throw new CompilationException(symbol, DiagnosticId.InvalidType, "Unable to determine enum type for ToString.");
        if (enumType.TypeKind != TypeKind.Enum)
            throw new CompilationException(symbol, DiagnosticId.InvalidType, "Enum.ToString is only supported on enum values.");

        var enumMembers = GetEnumFields(enumType);
        if (TryGetIntegerConstant(model, instanceExpression, out BigInteger value))
        {
            var member = enumMembers.FirstOrDefault(m => value.Equals(ToBigIntegerConstant(m.ConstantValue)));
            methodConvert.Push(member is not null ? member.Name : value.ToString("d", CultureInfo.InvariantCulture));
            return;
        }

        methodConvert.ConvertExpression(model, instanceExpression);

        var endTarget = new JumpTarget();
        foreach (var member in enumMembers)
        {
            methodConvert.Dup();
            methodConvert.Push(member.ConstantValue);

            var next = new JumpTarget();
            methodConvert.JumpIfNotEqual(next);
            methodConvert.Drop();
            methodConvert.Push(member.Name);
            methodConvert.JumpAlwaysLong(endTarget);
            next.Instruction = methodConvert.Nop();
        }

        // Fallback to numeric representation
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleEnumHasFlag(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is null)
            throw new CompilationException(symbol, DiagnosticId.InvalidArgument, "Enum.HasFlag requires an instance.");

        methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Stack: [value, flag]
        methodConvert.Tuck(); // [flag, value, flag]
        methodConvert.And();  // [flag, value']

        // The HasFlag method returns the result of the following Boolean expression: (thisInstance & flag) == flag
        methodConvert.NumEqual();
    }

    private static void HandleEnumParseGeneric(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, true, false, false);
    }

    private static void HandleEnumParseGenericIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, true, true, false);
    }

    private static void HandleEnumTryParseGeneric(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, true, false, true);
    }

    private static void HandleEnumTryParseGenericIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleEnumParseCore(methodConvert, model, symbol, instanceExpression, arguments, true, true, true);
    }

    private static void HandleEnumGetValuesGeneric(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var enumType = EnsureEnumTypeArgument(symbol, 0, instanceExpression);
        var members = GetSortedEnumFields(enumType);

        foreach (var member in members.Reverse())
            methodConvert.Push(member.ConstantValue);

        methodConvert.Pack(members.Length);
    }

    private static void HandleEnumGetNamesGeneric(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var enumType = EnsureEnumTypeArgument(symbol, 0, instanceExpression);
        var members = GetSortedEnumFields(enumType);

        foreach (var member in members.Reverse())
            methodConvert.Push(member.Name);

        methodConvert.Pack(members.Length);
    }

    private static void HandleEnumParseCore(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments,
        bool generic, bool hasIgnoreCase, bool tryParse)
    {
        using var scope = methodConvert.PreserveAnonymousVariables();
        var enumType = generic
            ? EnsureEnumTypeArgument(symbol, 0, instanceExpression)
            : GetEnumTypeArgument(model, symbol, arguments);

        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        byte outIndex = 0;
        if (tryParse)
        {
            int parameterIndex = generic ? (hasIgnoreCase ? 2 : 1) : (hasIgnoreCase ? 3 : 2);
            if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[parameterIndex], out outIndex))
                throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");
            methodConvert.Drop();
        }

        if (hasIgnoreCase)
        {
            byte ignoreSlot = methodConvert.AddAnonymousVariable();
            methodConvert.AccessSlot(OpCode.STLOC, ignoreSlot);
            if (!generic) methodConvert.Nip();
            methodConvert.AccessSlot(OpCode.LDLOC, ignoreSlot);
        }
        else
        {
            if (!generic) methodConvert.Nip();
            methodConvert.Push(false);
        }

        EmitEnumParseValue(methodConvert, enumType);
        if (tryParse)
        {
            methodConvert.Swap();
            methodConvert.AccessSlot(OpCode.STSFLD, outIndex);
        }
        else
        {
            JumpTarget parsed = new();
            methodConvert.JumpIfTrue(parsed);
            methodConvert.Drop();
            methodConvert.Push("No such enum value");
            methodConvert.Throw();
            parsed.Instruction = methodConvert.Nop();
        }
    }

    private static INamedTypeSymbol GetEnumTypeArgument(SemanticModel model, IMethodSymbol symbol,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not { Count: > 0 } ||
            (arguments[0] as ArgumentSyntax)?.Expression is not TypeOfExpressionSyntax typeOfExpression)
            throw new CompilationException(symbol, DiagnosticId.InvalidArgument, "First argument must be a typeof(enum)");
        if (model.GetTypeInfo(typeOfExpression.Type).Type is not INamedTypeSymbol { TypeKind: TypeKind.Enum } enumType)
            throw new CompilationException(arguments[0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        return enumType;
    }

    /// <summary>Consumes a string and ignoreCase flag, and leaves the parsed value and success flag.</summary>
    private static void EmitEnumParseValue(MethodConvert methodConvert, INamedTypeSymbol enumType)
    {
        using var scope = methodConvert.PreserveAnonymousVariables();
        byte ignoreSlot = methodConvert.AddAnonymousVariable();
        byte inputSlot = methodConvert.AddAnonymousVariable();
        byte partsSlot = methodConvert.AddAnonymousVariable();
        byte indexSlot = methodConvert.AddAnonymousVariable();
        byte valueSlot = methodConvert.AddAnonymousVariable();
        byte tokenSlot = methodConvert.AddAnonymousVariable();

        methodConvert.AccessSlot(OpCode.STLOC, ignoreSlot);
        methodConvert.AccessSlot(OpCode.STLOC, inputSlot);
        JumpTarget failure = new();
        JumpTarget success = new();
        JumpTarget end = new();
        JumpTarget loop = new();
        JumpTarget matched = new();

        methodConvert.AccessSlot(OpCode.LDLOC, inputSlot);
        methodConvert.IsNull();
        methodConvert.JumpIfTrueLong(failure);

        // Preserve the inexpensive exact-name path for the common case.
        methodConvert.AccessSlot(OpCode.LDLOC, inputSlot);
        methodConvert.AccessSlot(OpCode.STLOC, tokenSlot);
        JumpTarget skipFastUpper = new();
        methodConvert.AccessSlot(OpCode.LDLOC, ignoreSlot);
        methodConvert.JumpIfFalseLong(skipFastUpper);
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        ConvertToUpper(methodConvert);
        methodConvert.AccessSlot(OpCode.STLOC, tokenSlot);
        skipFastUpper.Instruction = methodConvert.Nop();
        foreach (var member in GetEnumFields(enumType))
        {
            EmitEnumNameMatch(methodConvert, tokenSlot, ignoreSlot, member);
            JumpTarget next = new();
            methodConvert.JumpIfFalse(next);
            methodConvert.Push(member.ConstantValue);
            methodConvert.Push(true);
            methodConvert.JumpAlwaysLong(end);
            next.Instruction = methodConvert.Nop();
        }

        methodConvert.Push(",");
        methodConvert.AccessSlot(OpCode.LDLOC, inputSlot);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "stringSplit", 2, true);
        methodConvert.AccessSlot(OpCode.STLOC, partsSlot);
        methodConvert.Push0();
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);
        methodConvert.Push0();
        methodConvert.AccessSlot(OpCode.STLOC, valueSlot);

        loop.Instruction = methodConvert.Nop();
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, partsSlot);
        methodConvert.Size();
        methodConvert.Lt();
        methodConvert.JumpIfFalseLong(success);

        methodConvert.AccessSlot(OpCode.LDLOC, partsSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.PickItem();
        HandleStringTrim(methodConvert, null!, null!, null!, null);
        methodConvert.AccessSlot(OpCode.STLOC, tokenSlot);

        JumpTarget skipUpper = new();
        methodConvert.AccessSlot(OpCode.LDLOC, ignoreSlot);
        methodConvert.JumpIfFalseLong(skipUpper);
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        ConvertToUpper(methodConvert);
        methodConvert.AccessSlot(OpCode.STLOC, tokenSlot);
        skipUpper.Instruction = methodConvert.Nop();

        foreach (var member in GetEnumFields(enumType))
        {
            EmitEnumNameMatch(methodConvert, tokenSlot, ignoreSlot, member);
            JumpTarget next = new();
            methodConvert.JumpIfFalse(next);
            methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
            methodConvert.Push(member.ConstantValue);
            methodConvert.Or();
            methodConvert.AccessSlot(OpCode.STLOC, valueSlot);
            methodConvert.JumpAlwaysLong(matched);
            next.Instruction = methodConvert.Nop();
        }

        // A numeric representation is accepted only as a complete, single value.
        methodConvert.AccessSlot(OpCode.LDLOC, partsSlot);
        methodConvert.Size();
        methodConvert.Push1();
        methodConvert.Jump(OpCode.JMPNE_L, failure);
        byte digitIndexSlot = EmitIntegerStringValidation(methodConvert, tokenSlot, failure);
        EmitEnumNumericLengthValidation(methodConvert, tokenSlot, digitIndexSlot, failure);
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.Dup();
        var backingType = enumType.EnumUnderlyingType?.SpecialType;
        (BigInteger min, BigInteger max) = backingType switch
        {
            SpecialType.System_SByte => (sbyte.MinValue, sbyte.MaxValue),
            SpecialType.System_Byte => (byte.MinValue, byte.MaxValue),
            SpecialType.System_Int16 => (short.MinValue, short.MaxValue),
            SpecialType.System_UInt16 => (ushort.MinValue, ushort.MaxValue),
            SpecialType.System_Int32 => (int.MinValue, int.MaxValue),
            SpecialType.System_UInt32 => (uint.MinValue, uint.MaxValue),
            SpecialType.System_Int64 => (long.MinValue, long.MaxValue),
            SpecialType.System_UInt64 => (BigInteger.Zero, new BigInteger(ulong.MaxValue)),
            _ => throw new ArgumentOutOfRangeException(nameof(enumType)),
        };
        methodConvert.Within(min, max);
        JumpTarget numericInRange = new();
        methodConvert.JumpIfTrue(numericInRange);
        methodConvert.Drop();
        methodConvert.JumpAlwaysLong(failure);
        numericInRange.Instruction = methodConvert.AccessSlot(OpCode.STLOC, valueSlot);
        methodConvert.JumpAlwaysLong(success);

        matched.Instruction = methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.Inc();
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);
        methodConvert.JumpAlwaysLong(loop);

        success.Instruction = methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.Push(true);
        methodConvert.JumpAlwaysLong(end);
        failure.Instruction = methodConvert.Push0();
        methodConvert.Push(false);
        end.Instruction = methodConvert.Nop();
    }

    /// <summary>Keep atoi's result within the VM's 32-byte integer limit before checking the enum backing type.</summary>
    private static void EmitEnumNumericLengthValidation(MethodConvert methodConvert, byte tokenSlot, byte indexSlot, JumpTarget failure)
    {
        JumpTarget sign = new();
        JumpTarget scan = new();
        JumpTarget nonZero = new();
        JumpTarget done = new();

        methodConvert.Push0();
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        methodConvert.Push0();
        methodConvert.PickItem();
        methodConvert.Push((ushort)'-');
        methodConvert.JumpIfEqual(sign);
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        methodConvert.Push0();
        methodConvert.PickItem();
        methodConvert.Push((ushort)'+');
        methodConvert.JumpIfEqual(sign);
        methodConvert.JumpAlwaysLong(scan);

        sign.Instruction = methodConvert.Push1();
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);
        scan.Instruction = methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        methodConvert.Size();
        methodConvert.Jump(OpCode.JMPGE_L, done);
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.PickItem();
        methodConvert.Push((ushort)'0');
        methodConvert.JumpIfNotEqual(nonZero);
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.Inc();
        methodConvert.AccessSlot(OpCode.STLOC, indexSlot);
        methodConvert.JumpAlwaysLong(scan);

        nonZero.Instruction = methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        methodConvert.Size();
        methodConvert.AccessSlot(OpCode.LDLOC, indexSlot);
        methodConvert.Sub();
        methodConvert.Push(20); // ulong.MaxValue is the widest possible enum value (20 decimal digits).
        methodConvert.Jump(OpCode.JMPGT_L, failure);
        done.Instruction = methodConvert.Nop();
    }

    private static void EmitEnumNameMatch(MethodConvert methodConvert, byte tokenSlot, byte ignoreSlot, IFieldSymbol member)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, tokenSlot);
        methodConvert.AccessSlot(OpCode.LDLOC, ignoreSlot);
        JumpTarget caseSensitive = new();
        JumpTarget compare = new();
        methodConvert.JumpIfFalse(caseSensitive);
        methodConvert.Push(member.Name.ToUpperInvariant());
        methodConvert.Jump(compare);
        caseSensitive.Instruction = methodConvert.Push(member.Name);
        compare.Instruction = methodConvert.Equal();
    }

    private static bool TryGetConstantArgument<T>(SemanticModel model, IMethodSymbol symbol,
        string parameterName, IReadOnlyList<SyntaxNode>? arguments, out T? value)
    {
        value = default;
        if (arguments is null) return false;

        foreach (var item in MapArgumentsToParameters(model, symbol, arguments))
        {
            if (item.Key.Name != parameterName) continue;

            var constant = model.GetConstantValue(item.Value.Expression);
            if (constant.HasValue && constant.Value is T v)
            {
                value = v;
                return true;
            }
            return false;
        }
        return false;
    }
}
