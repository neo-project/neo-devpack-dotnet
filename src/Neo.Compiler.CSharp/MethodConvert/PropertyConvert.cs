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
using Neo.IO;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void ConvertNoBody(AccessorDeclarationSyntax syntax)
    {
        _callingConvention = CallingConvention.Cdecl;
        IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
        AttributeData? attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(StoredAttribute));
        using (InsertSequencePoint(syntax))
        {
            _inline = attribute is null;
            // Here we handle them separately, if store backed, its store backed.
            // we need to load value from store directly.
            if (attribute is not null)
            {
                ConvertStorageBackedProperty(property, attribute);
            }
            else
            {
                ConvertFieldBackedProperty(property);
            }
        }
    }

    private void ConvertFieldBackedProperty(IPropertySymbol property)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        var backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
        // We need to take care of contract fields as non-static.
        if (Symbol.IsStatic || !NeedInstanceConstructor(Symbol) || _context.ContractFields.Any(f =>
                SymbolEqualityComparer.Default.Equals(f.Field, backingField)))
        {
            // IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
            byte backingFieldIndex = _context.AddStaticField(backingField);
            switch (Symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    AccessSlot(OpCode.LDSFLD, backingFieldIndex);
                    break;
                case MethodKind.PropertySet:
                    if (!_inline) AccessSlot(OpCode.LDARG, 0);
                    AccessSlot(OpCode.STSFLD, backingFieldIndex);
                    break;
                default:
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
            }
        }
        else
        {
            fields = fields.Where(p => !p.IsStatic).ToArray();
            int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
            switch (Symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    if (!_inline) AccessSlot(OpCode.LDARG, 0);
                    Push(backingFieldIndex);
                    AddInstruction(OpCode.PICKITEM);
                    break;
                case MethodKind.PropertySet:
                    if (_inline)
                    {
                        Push(backingFieldIndex);
                        AddInstruction(OpCode.ROT);
                    }
                    else
                    {
                        AccessSlot(OpCode.LDARG, 0);
                        Push(backingFieldIndex);
                        AccessSlot(OpCode.LDARG, 1);
                    }
                    AddInstruction(OpCode.SETITEM);
                    break;
                default:
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
            }
        }
    }

    private byte[] GetStorageBackedKey(IPropertySymbol property, AttributeData attribute)
    {
        byte[] key;

        if (attribute.ConstructorArguments.Length == 0)
        {
            key = Utility.StrictUTF8.GetBytes(property.Name);
        }
        else
        {
            if (attribute.ConstructorArguments[0].Value is byte b)
            {
                key = [b];
            }
            else if (attribute.ConstructorArguments[0].Value is string s)
            {
                key = Utility.StrictUTF8.GetBytes(s);
            }
            else
            {
                throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unknown StorageBacked constructor: {Symbol}");
            }
        }
        return key;
    }

    private void ConvertStorageBackedProperty(IPropertySymbol property, AttributeData attribute)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        byte[] key = GetStorageBackedKey(property, attribute);

        IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;

        if (Symbol.MethodKind == MethodKind.PropertyGet)
        {
            // Step 1. Load the value from the store.
            Push(key);
            CallInteropMethod(ApplicationEngine.System_Storage_GetReadOnlyContext);
            CallInteropMethod(ApplicationEngine.System_Storage_Get);

            // Step 2. Check if the value is initialized.
            // If not, load the default/assigned value to the backing field.
            switch (property.Type.Name)
            {
                case "byte":
                case "sbyte":
                case "Byte":
                case "SByte":

                case "short":
                case "ushort":
                case "Int16":
                case "UInt16":

                case "int":
                case "uint":
                case "Int32":
                case "UInt32":

                case "long":
                case "ulong":
                case "Int64":
                case "UInt64":
                case "BigInteger":
                case "bool":
                    /// TODO: Default value for string
                    // Check Null
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL); // null means these value are not initialized, then we should load them from backing field
                    JumpTarget ifFalse = new();
                    Jump(OpCode.JMPIFNOT_L, ifFalse);
                    AddInstruction(OpCode.DROP); // Drop the DUPed value
                    if (Symbol.IsStatic || !NeedInstanceConstructor(Symbol) || _context.ContractFields.Any(f =>
                            SymbolEqualityComparer.Default.Equals(f.Field, backingField)))
                    {
                        byte backingFieldIndex = _context.AddStaticField(backingField);
                        AccessSlot(OpCode.LDSFLD, backingFieldIndex);
                    }
                    else if (NeedInstanceConstructor(Symbol))
                    {
                        AddInstruction(OpCode.DUP);
                        fields = fields.Where(p => !p.IsStatic).ToArray();
                        int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                        if (!_inline) AccessSlot(OpCode.LDARG, 0);
                        Push(backingFieldIndex);
                        AddInstruction(OpCode.PICKITEM);
                    }
                    ifFalse.Instruction = AddInstruction(OpCode.NOP);
                    break;
                case "String":
                case "ByteString":
                case "UInt160":
                case "UInt256":
                case "ECPoint":
                    // but for those whose default value is null, it is impossible to know if
                    // the value has being initialized or not.
                    // TODO: figure out a way to initialize storebacked fields when deploy.
                    break;
                default:
                    CallContractMethod(NativeContract.StdLib.Hash, "deserialize", 1, true);
                    break;
            }
        }
        else if (Symbol.MethodKind == MethodKind.PropertySet) // explicitly use `else if` instead of `if` to improve readability.
        {
            if (Symbol.IsStatic || !NeedInstanceConstructor(Symbol) || _context.ContractFields.Any(f =>
                    SymbolEqualityComparer.Default.Equals(f.Field, backingField)))
                AccessSlot(OpCode.LDARG, 0);
            else
                AccessSlot(OpCode.LDARG, 1);
            switch (property.Type.Name)
            {
                case "byte":
                case "sbyte":
                case "Byte":
                case "SByte":

                case "short":
                case "ushort":
                case "Int16":
                case "UInt16":

                case "int":
                case "uint":
                case "Int32":
                case "UInt32":

                case "long":
                case "ulong":
                case "Int64":
                case "UInt64":
                case "BigInteger":
                case "String":
                case "ByteString":
                case "UInt160":
                case "UInt256":
                case "ECPoint":
                    break;
                default:
                    CallContractMethod(NativeContract.StdLib.Hash, "serialize", 1, true);
                    break;
            }
            Push(key);
            CallInteropMethod(ApplicationEngine.System_Storage_GetContext);
            CallInteropMethod(ApplicationEngine.System_Storage_Put);
        }
    }
}
