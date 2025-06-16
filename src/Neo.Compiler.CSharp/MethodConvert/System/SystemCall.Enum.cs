// Copyright (C) 2015-2025 The Neo Project.
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
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    /// <summary>
    /// Handles the Enum.Parse method by parsing a string representation into an enum value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Iterates through enum members, compares string names, returns matching value or throws
    /// </remarks>
    private static void HandleEnumParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
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
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.Parse"), DiagnosticId.InvalidArgument, "Enum.Parse requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        foreach (var t in enumMembers)
        {
            // Duplicate inputString
            methodConvert.Dup();                                   // Stack: [type, inputString,inputString]

            // Push enum name
            methodConvert.Push(t.Name);                            // Stack: [type, inputString,inputString, enumName]

            // Equal comparison
            methodConvert.Equal();                                 // Stack: [type,inputString, isEqual]

            var nextCheck = new JumpTarget();
            // If not equal, discard duplicated inputString and proceed to next
            methodConvert.Jump(OpCode.JMPIFNOT, nextCheck);

            // If equal:
            // Push enum value
            methodConvert.Push(t.ConstantValue);                   // Stack: [type, inputString, enumValue]
            methodConvert.Reverse3();
            methodConvert.Drop(2);
            methodConvert.Ret();

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        // Remove the inputString from the stack
        methodConvert.Drop();
        methodConvert.Push("No such enum value");
        methodConvert.Throw();
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
    /// Algorithm: Similar to Parse but with optional case-insensitive string comparison using uppercase conversion
    /// </remarks>
    private static void HandleEnumParseIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
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
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.Parse"), DiagnosticId.InvalidArgument, "Enum.Parse requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        var ignoreCase = new JumpTarget();
        var ignoreCase2 = new JumpTarget();
        methodConvert.JumpIfNot(ignoreCase);
        ConvertToUpper(methodConvert);                             // Convert inputString to upper case
        ignoreCase.Instruction = methodConvert.Nop();
        foreach (var t in enumMembers)
        {
            // Duplicate inputString
            methodConvert.Dup();                                   // Stack: [..., inputString, inputString]
            methodConvert.LdArg1();
            methodConvert.JumpIfNot(ignoreCase2);
            JumpTarget endCase = new JumpTarget();
            // Push enum name
            methodConvert.Push(t.Name.ToUpper());                  // Stack: [..., inputString, inputString, enumName]
            methodConvert.Jump(endCase);
            ignoreCase2.Instruction = methodConvert.Nop();
            methodConvert.Push(t.Name);
            endCase.Instruction = methodConvert.Nop();

            // Equal comparison
            methodConvert.Equal();                                 // Stack: [..., inputString, isEqual]

            var nextCheck = new JumpTarget();
            // If not equal, discard duplicated inputString and proceed to next
            methodConvert.Jump(OpCode.JMPIFNOT, nextCheck);

            // If equal:
            // Remove the duplicated inputString from the stack
            methodConvert.Drop(3);
            // Push enum value
            methodConvert.Push(t.ConstantValue);
            methodConvert.Ret();

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        // Remove the inputString from the stack
        methodConvert.Drop(2);
        methodConvert.Push("No such enum value");
        methodConvert.Throw();
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
    /// Algorithm: Attempts to parse enum with case-insensitive option, stores result in out parameter
    /// </remarks>
    private static void HandleEnumTryParseIgnoreCase(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        // Get the enum type from the first argument (typeof(enum))
        ITypeSymbol? enumTypeSymbol = null;
        if (arguments is { Count: > 2 })
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
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.TryParse"), DiagnosticId.InvalidArgument, "Enum.TryParse requires at least three arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[3], out var index))
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();
        var ignoreCase = new JumpTarget();
        var ignoreCase2 = new JumpTarget();
        methodConvert.Drop();
        methodConvert.JumpIfNot(ignoreCase);
        methodConvert.Swap();
        methodConvert.Drop();
        ConvertToUpper(methodConvert);                             // Convert inputString to upper case
        ignoreCase.Instruction = methodConvert.Nop();
        foreach (var t in enumMembers)
        {
            // Duplicate inputString
            methodConvert.Dup();                                   // Stack: [..., inputString, inputString]
            methodConvert.LdArg1();
            methodConvert.JumpIfNot(ignoreCase2);
            JumpTarget endCase = new JumpTarget();
            // Push enum name
            methodConvert.Push(t.Name.ToUpper());                  // Stack: [..., inputString, inputString, enumName]
            methodConvert.Jump(endCase);
            ignoreCase2.Instruction = methodConvert.Nop();
            methodConvert.Push(t.Name);
            endCase.Instruction = methodConvert.Nop();

            // Equal comparison
            methodConvert.Equal();                                 // Stack: [..., inputString, isEqual]

            var nextCheck = new JumpTarget();
            // If not equal, discard duplicated inputString and proceed to next
            methodConvert.Jump(OpCode.JMPIFNOT, nextCheck);

            // If equal:
            // Remove the duplicated inputString from the stack
            methodConvert.Drop(2);
            // Push enum value
            methodConvert.Push(t.ConstantValue);
            // Store the result in the out parameter
            methodConvert.AccessSlot(OpCode.STSFLD, index);
            // Push true to indicate success
            methodConvert.Push(true);
            methodConvert.Ret();

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        // Remove the inputString from the stack
        methodConvert.Drop(2);
        // Push default value (0) for the out parameter
        methodConvert.Push(0);
        methodConvert.AccessSlot(OpCode.STSFLD, index);
        // Push false to indicate failure
        methodConvert.Push(false);
        methodConvert.Ret();
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

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                                   // Duplicate input name
            methodConvert.Push(t.Name);                            // Push enum name
            methodConvert.Equal();                                 // Compare strings

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);                    // If not equal, check next

            methodConvert.Drop();                                  // Remove the duplicated input name
            methodConvert.Push(true);                              // Push true (enum name is defined)
            methodConvert.Ret();                                   // Return true

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        methodConvert.Drop();                                      // Remove the input name
        methodConvert.Push(false);                                 // Push false (enum name is not defined)
        methodConvert.Ret();                                       // Return false
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

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                                   // Duplicate input value
            methodConvert.Push(t.ConstantValue);                   // Push enum value
            methodConvert.Equal();                                 // Compare values

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);                    // If not equal, check next

            methodConvert.Drop();                                  // Remove the duplicated input value
            methodConvert.Push(t.Name);                            // Push enum name
            methodConvert.Ret();                                   // Return enum name

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        methodConvert.Drop();                                      // Remove the input value
        methodConvert.PushNull();                                  // Push null (no matching enum name)
        methodConvert.Ret();                                       // Return null
    }

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
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.GetName"), DiagnosticId.InvalidArgument, "Enum.GetName requires at least two arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments![0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                                   // Duplicate input value
            methodConvert.Push(t.ConstantValue);                   // Push enum value
            methodConvert.Equal();                                 // Compare values

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);                    // If not equal, check next

            methodConvert.Drop(2);                                 // Remove the duplicated input value
            methodConvert.Push(t.Name);                            // Push enum name
            methodConvert.Ret();                                   // Return enum name

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        methodConvert.Drop(2);                                     // Remove the input value
        methodConvert.PushNull();                                  // Push null (no matching enum name)
        methodConvert.Ret();                                       // Return null
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
    /// Algorithm: Attempts to parse enum name, stores result in out parameter, returns success flag
    /// </remarks>
    private static void HandleEnumTryParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[2], out var index))
            throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

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
            throw new CompilationException(GetSymbolMetadataModule(symbol, "Enum.TryParse"), DiagnosticId.InvalidArgument, "Enum.TryParse requires at least three arguments");
        }

        if (enumTypeSymbol is not { TypeKind: TypeKind.Enum })
        {
            throw new CompilationException(arguments[0], DiagnosticId.InvalidType, "Unable to determine enum type from first argument");
        }

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        JumpTarget endTarget = new();
        methodConvert.Drop();
        foreach (var t in enumMembers)
        {
            methodConvert.Dup();                                   // Stack: [..., inputString, inputString]
            methodConvert.Push(t.Name);                            // Stack: [..., inputString, inputString, enumName]

            // Equal comparison
            methodConvert.Equal();                                 // Stack: [..., inputString, isEqual]

            JumpTarget nextCheck = new();
            methodConvert.JumpIfNot(nextCheck);

            // If equal:
            methodConvert.Drop(2);                                 // Remove the inputString
            methodConvert.Push(t.ConstantValue);                   // Stack: [..., enumValue]
            methodConvert.AccessSlot(OpCode.STSFLD, index);        // Store enum value in out parameter
            methodConvert.Push(true);                              // Stack: [..., true]
            methodConvert.Ret();

            nextCheck.Instruction = methodConvert.Nop();
        }

        // No match found
        methodConvert.Drop(2);                                     // Remove the inputString
        methodConvert.Push(0);                                     // Default enum value
        methodConvert.AccessSlot(OpCode.STSFLD, index);            // Store default value in out parameter
        methodConvert.Push(false);                                 // Success flag set to false

        endTarget.Instruction = methodConvert.Nop();
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

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        // Create an array of names
        foreach (IFieldSymbol m in enumMembers.Reverse())          // PACK works in a reversed way
            methodConvert.Push(m.Name);
        methodConvert.Pack(enumMembers.Length);

        methodConvert.Ret();
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

        var enumMembers = enumTypeSymbol.GetMembers().OfType<IFieldSymbol>()
            .Where(field => field is { HasConstantValue: true, IsImplicitlyDeclared: false }).ToArray();

        // Create an array of values
        foreach (IFieldSymbol m in enumMembers.Reverse())          // PACK works in a reversed way
            methodConvert.Push(m.ConstantValue);
        methodConvert.Pack(enumMembers.Length);

        methodConvert.Ret();
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
        var endLabel = new JumpTarget();
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

            // Equal comparison
            methodConvert.Equal();                                 // Stack: [..., isEqual]

            var nextCheck = new JumpTarget();
            methodConvert.JumpIfNot(nextCheck);

            // If equal, set result to true
            methodConvert.Drop();
            methodConvert.Drop();                                  // Remove the false
            methodConvert.Push(true);                              // Set result to true
            methodConvert.Ret();
            nextCheck.Instruction = methodConvert.Nop();
        }
        methodConvert.Drop();
        methodConvert.Drop();                                      // Remove the duplicated value
        methodConvert.Push(false);
    }
}
