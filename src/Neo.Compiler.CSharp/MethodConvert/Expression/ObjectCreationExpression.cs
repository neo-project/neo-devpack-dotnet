// Copyright (C) 2015-2026 The Neo Project.
//
// ObjectCreationExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
        INamedTypeSymbol? systemIndex = model.Compilation
            .GetSpecialType(SpecialType.System_Object)
            .ContainingAssembly
            .GetTypeByMetadataName("System.Index");
        if (SymbolEqualityComparer.Default.Equals(type, systemIndex))
            throw CompilationException.UnsupportedSyntax(expression, "System.Index construction is not supported. Use an int index, or inline '^' in an element or range access.");
        if (type.TypeKind == TypeKind.Delegate)
        {
            ConvertDelegateCreationExpression(model, expression);
            return;
        }
        IMethodSymbol constructor = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        IReadOnlyList<ArgumentSyntax> arguments = expression.ArgumentList?.Arguments ?? (IReadOnlyList<ArgumentSyntax>)Array.Empty<ArgumentSyntax>();
        if (TryProcessSystemConstructors(model, constructor, arguments))
            return;
        var bigIntegerType = model.Compilation.GetTypeByMetadataName("System.Numerics.BigInteger");
        if (SymbolEqualityComparer.Default.Equals(type, bigIntegerType))
        {
            throw new CompilationException(
                expression,
                DiagnosticId.BigIntegerCreation,
                $"BigInteger constructor '{constructor}' is not supported. Only BigInteger(byte[]) is supported; use BigInteger.Zero for zero or an implicit conversion for integral values.");
        }
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
        List<(int Index, ExpressionSyntax Value)> initializerValues = new();
        foreach (ExpressionSyntax e in expression.Initializer.Expressions)
        {
            if (e is not AssignmentExpressionSyntax ae)
                throw CompilationException.UnsupportedSyntax(expression.Initializer, $"Unsupported object initializer syntax. Use assignment expressions like '{{ Field = value, Property = value }}' for object initialization.");
            ISymbol symbol = model.GetSymbolInfo(ae.Left).Symbol!;
            if (symbol is not IFieldSymbol field)
                return false;
            int index = GetInstanceFieldIndex(field);
            indexToValue.Add(index, ae.Right);
            initializerValues.Add((index, ae.Right));
        }
        if (!CanUseFieldOrderPackedInitializer(model, initializerValues))
            return false;
        var virtualMethods = members.OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
        bool needVirtualMethodTable = !type.IsRecord && virtualMethods.Length > 0;

        // Build the packed slots in field-index order (initializer value or default), with the
        // optional virtual method table as the final slot. Routing through
        // EmitPackedItemsLeftToRight preserves left-to-right evaluation order when the
        // initializer values have side effects, instead of evaluating them in reverse (#1685).
        // The virtual method table load and field defaults are side-effect free, so the
        // all-constant case still emits the same instructions as before.
        var slots = new List<(Action Emit, bool CanDefer)>(fields.Length + (needVirtualMethodTable ? 1 : 0));
        for (int i = 0; i < fields.Length; i++)
        {
            if (indexToValue.TryGetValue(i, out ExpressionSyntax? right))
            {
                ExpressionSyntax value = right;
                slots.Add((() => ConvertExpression(model, value), CanDeferExpressionEmission(model, value)));
            }
            else
            {
                ITypeSymbol fieldType = fields[i].Type;
                slots.Add((() => PushDefault(fieldType), true));
            }
        }
        if (needVirtualMethodTable)
        {
            byte vTableIndex = _context.AddVTable(type);
            slots.Add((() => AccessSlot(OpCode.LDSFLD, vTableIndex), true));
        }

        EmitPackedItemsLeftToRight(
            slots,
            slot => slot.Emit(),
            type.IsValueType || type.IsRecord ? OpCode.PACKSTRUCT : OpCode.PACK,
            slot => slot.CanDefer);
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
                throw CompilationException.UnsupportedSyntax(initializer, "Cannot determine item type from empty collection initializer. Add at least one element or specify the type explicitly.");
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
                    PushAsBuffer(data);
                }
            }
            else
            {
                // Preserve left-to-right evaluation order of the elements when they have
                // side effects, instead of emitting them in reverse (see #1685).
                EmitPackedItemsLeftToRight(
                    initializer.Expressions.ToArray(),
                    expression => ConvertExpression(model, expression),
                    OpCode.PACK,
                    expression => CanDeferExpressionEmission(model, expression));
            }
            return;
        }

        foreach (ExpressionSyntax e in initializer.Expressions)
        {
            if (e is not AssignmentExpressionSyntax ae)
                throw CompilationException.UnsupportedSyntax(initializer, $"Unsupported collection initializer syntax. Use assignment expressions like '{{ item1, item2 }}' for collections or '{{ key = value }}' for dictionaries.");
            ISymbol symbol = model.GetSymbolInfo(ae.Left).Symbol!;
            switch (symbol)
            {
                case IFieldSymbol field:
                    AddInstruction(OpCode.DUP);
                    int index = GetInstanceFieldIndex(field);
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
                    throw CompilationException.UnsupportedSyntax(ae.Left, $"Unsupported member '{symbol.Name}' in object initializer. Only fields and properties can be initialized.");
            }
        }
    }

    private static bool CanUseFieldOrderPackedInitializer(SemanticModel model, IReadOnlyList<(int Index, ExpressionSyntax Value)> initializerValues)
    {
        bool hasSideEffects = initializerValues.Any(value => !CanDeferExpressionEmission(model, value.Value));
        if (!hasSideEffects)
            return true;

        for (int i = 1; i < initializerValues.Count; i++)
            if (initializerValues[i].Index < initializerValues[i - 1].Index)
                return false;

        return true;
    }

    private void ConvertDelegateCreationExpression(SemanticModel model, BaseObjectCreationExpressionSyntax expression)
    {
        if (expression.ArgumentList!.Arguments.Count != 1)
            throw CompilationException.UnsupportedSyntax(expression, $"Delegate constructor requires exactly one argument. Use 'new Action(MethodName)' or similar patterns.");
        IMethodSymbol symbol = (IMethodSymbol)model.GetSymbolInfo(expression.ArgumentList.Arguments[0].Expression).Symbol!;
        if (!symbol.IsStatic)
            throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {symbol}");
        InvokeMethod(model, symbol);
    }
}
