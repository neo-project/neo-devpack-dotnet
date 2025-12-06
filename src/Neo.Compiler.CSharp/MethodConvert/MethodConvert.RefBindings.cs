// Copyright (C) 2015-2025 The Neo Project.
//
// MethodConvert.RefBindings.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    // Ref locals and ref/out arguments can alias many different storage locations. We keep a lookup
    // table that maps the Roslyn symbol to the instructions required to load/store that location so
    // every use site just asks for the binding instead of re-emitting the discovery logic.
    private readonly Dictionary<ISymbol, RefBinding> _refLocalBindings = new(SymbolEqualityComparer.Default);

    private void SetRefBinding(ILocalSymbol symbol, RefBinding binding)
    {
        _refLocalBindings[symbol] = binding;
        if (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, symbol))
            _refLocalBindings[symbol.OriginalDefinition] = binding;
    }

    private void RemoveRefBinding(ILocalSymbol symbol)
    {
        if (_refLocalBindings.TryGetValue(symbol, out RefBinding binding))
        {
            ReleaseRefBinding(binding);
            _refLocalBindings.Remove(symbol);
        }

        if (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, symbol))
            _refLocalBindings.Remove(symbol.OriginalDefinition);
    }

    private bool TryGetRefBinding(ISymbol symbol, out RefBinding binding)
    {
        if (_refLocalBindings.TryGetValue(symbol, out binding))
            return true;
        if (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, symbol))
            return _refLocalBindings.TryGetValue(symbol.OriginalDefinition, out binding);
        return false;
    }

    private bool HasRefBinding(ISymbol symbol)
        => _refLocalBindings.ContainsKey(symbol) ||
           (!SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, symbol) &&
            _refLocalBindings.ContainsKey(symbol.OriginalDefinition));

    private readonly struct RefBinding
    {
        private RefBinding(RefBindingKind kind, byte localSlot = 0, byte arraySlot = 0, byte indexSlot = 0,
            byte instanceSlot = 0, int fieldIndex = -1, byte staticFieldIndex = 0, IParameterSymbol? parameter = null)
        {
            Kind = kind;
            LocalSlot = localSlot;
            ArraySlot = arraySlot;
            IndexSlot = indexSlot;
            InstanceSlot = instanceSlot;
            FieldIndex = fieldIndex;
            StaticFieldIndex = staticFieldIndex;
            ParameterSymbol = parameter;
        }

        public RefBindingKind Kind { get; }
        public byte LocalSlot { get; }
        public byte ArraySlot { get; }
        public byte IndexSlot { get; }
        public byte InstanceSlot { get; }
        public int FieldIndex { get; }
        public byte StaticFieldIndex { get; }
        public IParameterSymbol? ParameterSymbol { get; }

        public static RefBinding Local(byte slot)
            => new RefBinding(RefBindingKind.LocalSlot, localSlot: slot);

        public static RefBinding FromParameter(IParameterSymbol parameter)
            => new RefBinding(RefBindingKind.Parameter, parameter: parameter);

        public static RefBinding StaticField(byte fieldIndex)
            => new RefBinding(RefBindingKind.StaticField, staticFieldIndex: fieldIndex);

        public static RefBinding InstanceField(byte instanceSlot, int fieldIndex)
            => new RefBinding(RefBindingKind.InstanceField, instanceSlot: instanceSlot, fieldIndex: fieldIndex);

        public static RefBinding ArrayElement(byte arraySlot, byte indexSlot)
            => new RefBinding(RefBindingKind.ArrayElement, arraySlot: arraySlot, indexSlot: indexSlot);
    }

    private enum RefBindingKind
    {
        LocalSlot,
        Parameter,
        StaticField,
        InstanceField,
        ArrayElement
    }

    private Instruction LoadRefBinding(RefBinding binding)
    {
        return binding.Kind switch
        {
            RefBindingKind.LocalSlot => AccessSlot(OpCode.LDLOC, binding.LocalSlot),
            RefBindingKind.Parameter => LdArgSlot(binding.ParameterSymbol!),
            RefBindingKind.StaticField => AccessSlot(OpCode.LDSFLD, binding.StaticFieldIndex),
            RefBindingKind.InstanceField => LoadInstanceFieldBinding(binding),
            RefBindingKind.ArrayElement => LoadArrayElementBinding(binding),
            _ => throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported ref binding kind '{binding.Kind}'.")
        };
    }

    private Instruction StoreRefBinding(RefBinding binding)
    {
        return binding.Kind switch
        {
            RefBindingKind.LocalSlot => AccessSlot(OpCode.STLOC, binding.LocalSlot),
            RefBindingKind.Parameter => StArgSlot(binding.ParameterSymbol!),
            RefBindingKind.StaticField => AccessSlot(OpCode.STSFLD, binding.StaticFieldIndex),
            RefBindingKind.InstanceField => StoreInstanceFieldBinding(binding),
            RefBindingKind.ArrayElement => StoreArrayElementBinding(binding),
            _ => throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported ref binding kind '{binding.Kind}'.")
        };
    }

    private Instruction LoadInstanceFieldBinding(RefBinding binding)
    {
        AccessSlot(OpCode.LDLOC, binding.InstanceSlot);
        Push(binding.FieldIndex);
        return AddInstruction(OpCode.PICKITEM);
    }

    private Instruction StoreInstanceFieldBinding(RefBinding binding)
    {
        AccessSlot(OpCode.LDLOC, binding.InstanceSlot);
        Push(binding.FieldIndex);
        AddInstruction(OpCode.ROT);
        return AddInstruction(OpCode.SETITEM);
    }

    private Instruction LoadArrayElementBinding(RefBinding binding)
    {
        AccessSlot(OpCode.LDLOC, binding.ArraySlot);
        AccessSlot(OpCode.LDLOC, binding.IndexSlot);
        return AddInstruction(OpCode.PICKITEM);
    }

    private Instruction StoreArrayElementBinding(RefBinding binding)
    {
        AccessSlot(OpCode.LDLOC, binding.ArraySlot);
        AccessSlot(OpCode.LDLOC, binding.IndexSlot);
        AddInstruction(OpCode.ROT);
        return AddInstruction(OpCode.SETITEM);
    }

    private void ReleaseRefBinding(RefBinding binding)
    {
        switch (binding.Kind)
        {
            case RefBindingKind.ArrayElement:
                RemoveAnonymousVariable(binding.ArraySlot);
                RemoveAnonymousVariable(binding.IndexSlot);
                break;
            case RefBindingKind.InstanceField:
                RemoveAnonymousVariable(binding.InstanceSlot);
                break;
        }
    }

    private void BindRefLocal(SemanticModel model, ILocalSymbol symbol, RefExpressionSyntax refExpression)
    {
        if (HasRefBinding(symbol))
            RemoveRefBinding(symbol);
        // Ref assignments (`ref alias = ref source`) simply attach a new binding to the local;
        // subsequent loads/stores will reuse whatever storage CreateRefBinding resolved here.
        RefBinding binding = CreateRefBinding(model, refExpression.Expression);
        SetRefBinding(symbol, binding);
    }

    private RefBinding CreateRefBinding(SemanticModel model, ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax identifier => CreateIdentifierRefBinding(model, identifier),
            MemberAccessExpressionSyntax memberAccess => CreateMemberAccessRefBinding(model, memberAccess),
            ElementAccessExpressionSyntax elementAccess => CreateElementAccessRefBinding(model, elementAccess),
            RefExpressionSyntax nestedRef => CreateRefBinding(model, nestedRef.Expression),
            _ => throw CompilationException.UnsupportedSyntax(expression,
                $"Unsupported ref expression node '{expression.GetType().Name}'. Only locals, parameters, fields, and single-dimensional array elements are supported.")
        };
    }

    private RefBinding CreateIdentifierRefBinding(SemanticModel model, IdentifierNameSyntax identifier)
    {
        ISymbol? symbol = model.GetSymbolInfo(identifier).Symbol;
        if (symbol is null)
            throw CompilationException.UnsupportedSyntax(identifier, $"Unable to resolve identifier '{identifier.Identifier.ValueText}' in ref expression.");

        return symbol switch
        {
            ILocalSymbol local => CreateLocalRefBinding(local),
            IParameterSymbol parameter => RefBinding.FromParameter(parameter),
            IFieldSymbol field => CreateFieldRefBinding(model, field, instanceExpression: null),
            _ => throw CompilationException.UnsupportedSyntax(identifier,
                $"Identifier '{identifier.Identifier.ValueText}' of type '{symbol.GetType().Name}' cannot be used in a ref expression.")
        };
    }

    private RefBinding CreateLocalRefBinding(ILocalSymbol local)
    {
        if (TryGetRefBinding(local, out RefBinding existing))
            return CloneRefBinding(existing);

        if (!_localVariables.TryGetValue(local, out byte slot))
            throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Local '{local.Name}' is not available in the current scope.");

        return RefBinding.Local(slot);
    }

    private RefBinding CreateMemberAccessRefBinding(SemanticModel model, MemberAccessExpressionSyntax memberAccess)
    {
        ISymbol? symbol = model.GetSymbolInfo(memberAccess).Symbol;
        if (symbol is IFieldSymbol field)
            return CreateFieldRefBinding(model, field, memberAccess.Expression);

        throw CompilationException.UnsupportedSyntax(memberAccess,
            $"Member access '{memberAccess}' with symbol type '{symbol?.GetType().Name}' cannot be used in a ref expression. Only fields are allowed.");
    }

    private RefBinding CreateFieldRefBinding(SemanticModel model, IFieldSymbol field, ExpressionSyntax? instanceExpression)
    {
        if (field.IsStatic)
        {
            byte index = _context.AddStaticField(field);
            return RefBinding.StaticField(index);
        }

        // Instance fields require the receiver to stay live for the lifetime of the ref binding.
        // We materialize the instance once, stash it in a temp slot, and record both the slot and
        // field offset so loads/stores can reproduce the same PICKITEM/SETITEM sequence later.
        byte instanceSlot = AddAnonymousVariable();
        if (instanceExpression is null)
        {
            AddInstruction(OpCode.LDARG0);
        }
        else
        {
            ConvertExpression(model, instanceExpression);
        }

        AccessSlot(OpCode.STLOC, instanceSlot);
        int fieldIndex = Array.IndexOf(field.ContainingType.GetFields(), field);
        if (fieldIndex < 0)
            throw new CompilationException(field, DiagnosticId.SyntaxNotSupported, $"Field '{field.Name}' was not found on containing type '{field.ContainingType.Name}'.");
        return RefBinding.InstanceField(instanceSlot, fieldIndex);
    }

    private RefBinding CreateElementAccessRefBinding(SemanticModel model, ElementAccessExpressionSyntax elementAccess)
    {
        if (elementAccess.ArgumentList.Arguments.Count != 1)
            throw CompilationException.UnsupportedSyntax(elementAccess, "Only single-dimensional element access can be bound by ref.");

        byte arraySlot = AddAnonymousVariable();
        ConvertExpression(model, elementAccess.Expression);
        AccessSlot(OpCode.STLOC, arraySlot);

        byte indexSlot = AddAnonymousVariable();
        ConvertExpression(model, elementAccess.ArgumentList.Arguments[0].Expression);
        AccessSlot(OpCode.STLOC, indexSlot);

        return RefBinding.ArrayElement(arraySlot, indexSlot);
    }

    private RefBinding CloneRefBinding(RefBinding binding)
    {
        return binding.Kind switch
        {
            RefBindingKind.LocalSlot => RefBinding.Local(binding.LocalSlot),
            RefBindingKind.Parameter => RefBinding.FromParameter(binding.ParameterSymbol!),
            RefBindingKind.StaticField => RefBinding.StaticField(binding.StaticFieldIndex),
            RefBindingKind.InstanceField => CloneInstanceFieldBinding(binding),
            RefBindingKind.ArrayElement => CloneArrayElementBinding(binding),
            _ => throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported ref binding kind '{binding.Kind}' for cloning.")
        };
    }

    private RefBinding CloneInstanceFieldBinding(RefBinding binding)
    {
        byte instanceSlot = AddAnonymousVariable();
        AccessSlot(OpCode.LDLOC, binding.InstanceSlot);
        AccessSlot(OpCode.STLOC, instanceSlot);
        return RefBinding.InstanceField(instanceSlot, binding.FieldIndex);
    }

    private RefBinding CloneArrayElementBinding(RefBinding binding)
    {
        byte arraySlot = AddAnonymousVariable();
        byte indexSlot = AddAnonymousVariable();
        AccessSlot(OpCode.LDLOC, binding.ArraySlot);
        AccessSlot(OpCode.STLOC, arraySlot);
        AccessSlot(OpCode.LDLOC, binding.IndexSlot);
        AccessSlot(OpCode.STLOC, indexSlot);
        return RefBinding.ArrayElement(arraySlot, indexSlot);
    }
}
