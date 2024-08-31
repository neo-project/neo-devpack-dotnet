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
            ConvertFieldBackedProperty(property);
            if (attribute is not null)
                ConvertStorageBackedProperty(property, attribute);
        }
    }

    private void ConvertFieldBackedProperty(IPropertySymbol property)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        if (Symbol.IsStatic)
        {
            IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
            byte backingFieldIndex = Context.AddStaticField(backingField);
            switch (Symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    _instructionsBuilder.LdSFld(backingFieldIndex);
                    break;
                case MethodKind.PropertySet:
                    if (!_inline) _instructionsBuilder.LdArg(0);
                    _instructionsBuilder.StSFld(backingFieldIndex);
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
                    if (!_inline) _instructionsBuilder.LdArg(0);
                    _instructionsBuilder.Push(backingFieldIndex);
                    _instructionsBuilder.PickItem();
                    break;
                case MethodKind.PropertySet:
                    if (_inline)
                    {
                        _instructionsBuilder.Push(backingFieldIndex);
                        _instructionsBuilder.Rot();
                    }
                    else
                    {
                        _instructionsBuilder.LdArg(0);
                        _instructionsBuilder.Push(backingFieldIndex);
                        _instructionsBuilder.LdArg(1);
                    }
                    _instructionsBuilder.SetItem();
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
        if (Symbol.MethodKind == MethodKind.PropertyGet)
        {
            JumpTarget endTarget = new();
            if (Symbol.IsStatic)
            {
                // AddInstruction(OpCode.DUP);
                _instructionsBuilder.IsNull();
                // Ensure that no object was sent
                _instructionsBuilder.JmpIfNotL(endTarget);
            }
            else
            {
                // Check class
                _instructionsBuilder.JmpIfL(endTarget);
            }
            _instructionsBuilder.Push(key);
            CallInteropMethod(ApplicationEngine.System_Storage_GetReadOnlyContext);
            CallInteropMethod(ApplicationEngine.System_Storage_Get);
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
                    // Replace NULL with 0
                    _instructionsBuilder.Dup();
                    _instructionsBuilder.IsNull();
                    JumpTarget ifFalse = new();
                    _instructionsBuilder.JmpIfNotL(ifFalse);
                    {
                        _instructionsBuilder.Drop();
                        _instructionsBuilder.Push0();
                    }
                    _instructionsBuilder.AddTarget(ifFalse);
                    break;
                case "String":
                case "ByteString":
                case "UInt160":
                case "UInt256":
                case "ECPoint":
                    break;
                default:
                    CallContractMethod(NativeContract.StdLib.Hash, "deserialize", 1, true);
                    break;
            }
            _instructionsBuilder.Dup();
            if (Symbol.IsStatic)
            {
                IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
                byte backingFieldIndex = Context.AddStaticField(backingField);
                _instructionsBuilder.StSFld(backingFieldIndex);
            }
            else
            {
                fields = fields.Where(p => !p.IsStatic).ToArray();
                int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                _instructionsBuilder.LdArg(0);
                _instructionsBuilder.Push(backingFieldIndex);
                _instructionsBuilder.Rot();
                _instructionsBuilder.SetItem();
            }
            _instructionsBuilder.AddTarget(endTarget);
        }
        else
        {
            if (Symbol.IsStatic)
                _instructionsBuilder.LdArg(0);
            else
                _instructionsBuilder.LdArg(1);
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
            _instructionsBuilder.Push(key);
            CallInteropMethod(ApplicationEngine.System_Storage_GetContext);
            CallInteropMethod(ApplicationEngine.System_Storage_Put);
        }
    }
}
