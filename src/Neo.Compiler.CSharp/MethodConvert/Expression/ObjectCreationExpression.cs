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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void ConvertObjectCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
    {
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        if (type.TypeKind == TypeKind.Delegate)
        {
            ConvertDelegateCreationExpression(model, expression);
            return;
        }
        IMethodSymbol constructor = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList?.Arguments ?? (IReadOnlyList<ArgumentSyntax>)Array.Empty<ArgumentSyntax>();
        if (TryProcessSystemConstructors(model, constructor, arguments))
            return;
        bool needCreateObject = !type.DeclaringSyntaxReferences.IsEmpty && !constructor.IsExtern;
        if (needCreateObject)
        {
            // an optimization to avoid PACK + billions of SETITEM
            if (TryOptimizedObjectCreation(model, expression, type, constructor))
                return;
            CreateObject(model, type);
        }
        if (!constructor.DeclaringSyntaxReferences.IsEmpty)
            CallInstanceMethod(model, constructor, needCreateObject, arguments);
        if (expression.Initializer is not null)
            ConvertObjectCreationExpressionInitializer(model, expression.Initializer);
    }

    /// <summary>
    /// Check whether necessary to include the constructor instructions in the compiled contract
    /// </summary>
    /// <param name="convert">var (convert, _) = GetMethodConvertAndCallingConvention(model, constructor);</param>
    /// <returns></returns>
    public static bool CanSkipConstructor(MethodConvert? convert)
    {
        if (convert == null)
            return false;  // special complex cases like virtual methods
        if (convert.Instructions.Count >= 1)
        {
            Instruction ret = convert.Instructions[0];
            if (ret.OpCode == OpCode.RET)
                return true;
        }
        if (convert.Instructions.Count >= 2)
        {
            // INITSLOT 0 locals, 1 args
            // RET
            Instruction initslot = convert.Instructions[0];
            Instruction ret = convert.Instructions[1];
            if (initslot.OpCode == OpCode.INITSLOT && initslot.Operand?[0] == 0 && initslot.Operand[1] == 1
                && ret.OpCode == OpCode.RET)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Handles new MyClass() { PropertyA = "A", Property2 = 2, } in a GAS-efficient way
    /// Do not initialize MyClass() by PACKing default values and then SETITEM for { PropertyA = "A", Property2 = 2, }
    /// Just PACK the final values when constructor of MyClass is not needed
    /// </summary>
    /// <param name="model"></param>
    /// <param name="expression"></param>
    /// <param name="type"></param>
    /// <param name="constructor"></param>
    /// <returns></returns>
    private bool TryOptimizedObjectCreation(SemanticModel model, BaseObjectCreationExpressionSyntax expression,
        ITypeSymbol type, IMethodSymbol constructor)
    {
        if (expression.Initializer == null || expression.Initializer.IsKind(SyntaxKind.CollectionInitializerExpression))
            return false;
        var (convert, methodCallingConvention) = GetMethodConvertAndCallingConvention(model, constructor);
        if (!CanSkipConstructor(convert))
            return false;
        // no constructor needed
        var members = type.GetAllMembers().Where(p => !p.IsStatic).ToArray();
        var fields = members.OfType<IFieldSymbol>().ToArray();
        Dictionary<int, ExpressionSyntax> indexToValue = new();
        foreach (ExpressionSyntax e in expression.Initializer.Expressions)
        {
            if (e is not AssignmentExpressionSyntax ae)
                throw new CompilationException(expression.Initializer, DiagnosticId.SyntaxNotSupported, $"Unsupported initializer: {expression.Initializer}");
            ISymbol symbol = model.GetSymbolInfo(ae.Left).Symbol!;
            if (symbol is not IFieldSymbol field)
                return false;
            int index = Array.IndexOf(field.ContainingType.GetFields(), field);
            indexToValue.Add(index, ae.Right);
        }
        var virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
        int needVirtualMethodTable = 0;
        if (!type.IsRecord && virtualMethods.Length > 0)
        {
            needVirtualMethodTable += 1;
            byte vTableIndex = _context.AddVTable(type);
            AccessSlot(OpCode.LDSFLD, vTableIndex);
        }
        for (int i = fields.Length - 1; i >= 0; --i)
            if (indexToValue.TryGetValue(i, out ExpressionSyntax? right))
                ConvertExpression(model, right);
            else
                PushDefault(fields[i].Type);
        Push(fields.Length + needVirtualMethodTable);
        AddInstruction(type.IsValueType || type.IsRecord ? OpCode.PACKSTRUCT : OpCode.PACK);
        return true;
    }

    private void ConvertObjectCreationExpressionInitializer(SemanticModel model, InitializerExpressionSyntax initializer)
    {
        // Handle different types of initializer expressions:
        //
        // ObjectInitializerExpression:
        // Example: new Person { Name = "John", Age = 30 }
        // Used for initializing properties of an object.
        //
        // CollectionInitializerExpression:
        // Example: new List<int> { 1, 2, 3 }
        // Used for initializing collections like lists or sets.
        //
        // ArrayInitializerExpression:
        // Example: new int[] { 1, 2, 3 }
        // Used for initializing arrays.
        //
        // ComplexElementInitializerExpression:
        // Example: new Dictionary<string, int> { { "one", 1 }, { "two", 2 } }
        // Used for initializing complex elements like dictionary entries.
        //
        // NullLiteralExpression:
        // Example: new Person { Name = null }
        // Used when explicitly setting a property to null in an initializer.

        if (initializer.IsKind(SyntaxKind.CollectionInitializerExpression))
        {
            ITypeSymbol type;
            if (initializer.Expressions.Count > 0)
            {
                var firstExpression = initializer.Expressions[0];
                var typeInfo = model.GetTypeInfo(firstExpression);
                type = typeInfo.Type!;
            }
            else
            {
                // Handle empty collection case if necessary
                throw new CompilationException(initializer, DiagnosticId.SyntaxNotSupported, "Cannot determine item type from an empty collection initializer.");
            }

            AddInstruction(OpCode.DROP);
            if (type.SpecialType == SpecialType.System_Byte)
            {
                var values = initializer.Expressions.Select(p => model.GetConstantValue(p)).ToArray();
                if (values.Any(p => !p.HasValue))
                {
                    Push(values.Length);
                    AddInstruction(OpCode.NEWBUFFER);
                    for (var i = 0; i < initializer.Expressions.Count; i++)
                    {
                        AddInstruction(OpCode.DUP);
                        Push(i);
                        ConvertExpression(model, initializer.Expressions[i]);
                        AddInstruction(OpCode.SETITEM);
                    }
                }
                else
                {
                    var data = values.Select(p => (byte)System.Convert.ChangeType(p.Value, typeof(byte))!).ToArray();
                    Push(data);
                    ChangeType(VM.Types.StackItemType.Buffer);
                }
            }
            else
            {
                for (var i = initializer.Expressions.Count - 1; i >= 0; i--)
                    ConvertExpression(model, initializer.Expressions[i]);
                Push(initializer.Expressions.Count);
                AddInstruction(OpCode.PACK);
            }
            return;
        }

        foreach (ExpressionSyntax e in initializer.Expressions)
        {
            if (e is not AssignmentExpressionSyntax ae)
                throw new CompilationException(initializer, DiagnosticId.SyntaxNotSupported, $"Unsupported initializer: {initializer}");
            ISymbol symbol = model.GetSymbolInfo(ae.Left).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    AddInstruction(OpCode.DUP);
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    Push(index);
                    ConvertExpression(model, ae.Right);
                    AddInstruction(OpCode.SETITEM);
                    break;
                case IPropertySymbol property:
                    // Special handling for Map and List initialization is required due to their unique initialization syntax and behavior.
                    // Map and List properties are not defined explicitly like regular types

                    // Examples:
                    // Map: new Map<string, int> { ["key"] = 42 };
                    //      This is equivalent to: map["key"] = 42;
                    // Regular: new MyClass { Property = value };
                    //      This uses the standard property setter.
                    if (property.ContainingType.Name is "Map")
                    {
                        // Duplicate the object reference for Map and List
                        AddInstruction(OpCode.DUP);

                        if (ae.Left is ImplicitElementAccessSyntax elementAccess)
                        {
                            ConvertExpression(model, elementAccess.ArgumentList.Arguments[0].Expression);
                        }
                        else
                        {
                            ConvertExpression(model, ae.Left);
                        }
                        // Convert the value to be assigned (for both Map and List)
                        ConvertExpression(model, ae.Right);
                        AddInstruction(OpCode.SETITEM);
                    }
                    else
                    {
                        // For regular properties:
                        ConvertExpression(model, ae.Right);
                        AddInstruction(OpCode.OVER);
                        CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
                    }
                    break;
                default:
                    throw new CompilationException(ae.Left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
            }
        }
    }

    private void ConvertDelegateCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
    {
        if (expression.ArgumentList!.Arguments.Count != 1)
            throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported delegate: {expression}");
        IMethodSymbol symbol = (IMethodSymbol)model.GetSymbolInfo(expression.ArgumentList.Arguments[0].Expression).Symbol!;
        if (!symbol.IsStatic)
            throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {symbol}");
        InvokeMethod(model, symbol);
    }
}
