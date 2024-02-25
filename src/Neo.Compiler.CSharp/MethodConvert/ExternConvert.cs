// Copyright (C) 2015-2023 The Neo Project.
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
using Neo.Cryptography;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using scfx::Neo.SmartContract.Framework.Attributes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Neo.Compiler;

partial class MethodConvert
{

    private void ConvertExtern()
    {
        _inline = true;
        AttributeData? contractAttribute = Symbol.ContainingType.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(ContractAttribute));
        if (contractAttribute is null)
        {
            bool emitted = false;
            foreach (AttributeData attribute in Symbol.GetAttributes())
            {
                switch (attribute.AttributeClass!.Name)
                {
                    case nameof(OpCodeAttribute):
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
                    case nameof(SyscallAttribute):
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
                    case nameof(CallingConventionAttribute):
                        emitted = true;
                        _callingConvention = (CallingConvention)attribute.ConstructorArguments[0].Value!;
                        break;
                }
            }
            if (Symbol.ToString()?.StartsWith("System.Array.Empty") == true)
            {
                emitted = true;
                AddInstruction(OpCode.NEWARRAY0);
            }
            else if (Symbol.ToString()?.Equals("Neo.SmartContract.Framework.Services.Runtime.Debug(string)") == true)
            {
                _context.AddEvent(new AbiEvent(Symbol, "Debug", new SmartContract.Manifest.ContractParameterDefinition() { Name = "message", Type = ContractParameterType.String }), false);
            }
            if (!emitted) throw new CompilationException(Symbol, DiagnosticId.ExternMethod, $"Unknown method: {Symbol}");
        }
        else
        {
            UInt160 hash = UInt160.Parse((string)contractAttribute.ConstructorArguments[0].Value!);
            if (Symbol.MethodKind == MethodKind.PropertyGet)
            {
                AttributeData? attribute = Symbol.AssociatedSymbol!.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(ContractHashAttribute));
                if (attribute is not null)
                {
                    Push(hash.ToArray());
                    return;
                }
            }
            string method = Symbol.GetDisplayName(true);
            ushort parametersCount = (ushort)Symbol.Parameters.Length;
            bool hasReturnValue = !Symbol.ReturnsVoid || Symbol.MethodKind == MethodKind.Constructor;
            Call(hash, method, parametersCount, hasReturnValue);
        }
    }
}
