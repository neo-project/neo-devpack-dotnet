using Neo.Json;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpCode = Neo.VM.OpCode;

namespace Neo.Disassembler.CSharp;

public static class Disassembler
{
    private static readonly Regex RangeRegex = new(@"(\d+)\-(\d+)", RegexOptions.Compiled);

    public static List<Instruction> ConvertScriptToInstructions(byte[] script)
    {
        var res = EnumerateInstructions(script);

        return res.Select(x => x.instruction).ToList();
    }

    public static List<(int address, Instruction instruction)> ConvertMethodToInstructions(NefFile nef, int start, int end)
    {
        var instructions = EnumerateInstructions(nef.Script).ToList();
        return instructions
            .Where(ai => ai.address >= start && ai.address <= end)
            .Select(ai => (ai.address - start, ai.instruction))
            .ToList();
    }

    public static JObject? GetMethod(string name, int argCount, JToken debugInfo)
    {
        name = name.Length == 0 ? string.Empty : string.Concat(name[0].ToString().ToUpper(), name.AsSpan(1));  // first letter uppercase

        foreach (var method in (JArray)debugInfo["methods"]!)
        {
            var methodName = method!["name"]!.AsString().Split(",")[1];
            methodName = methodName.Length == 0 ? string.Empty : string.Concat(methodName[0].ToString().ToUpper(), methodName.AsSpan(1));  // first letter uppercase

            if (methodName == name)
            {
                if (method["params"] is not JArray jparams)
                {
                    continue;
                }

                // excluding `this` of an object
                var count = jparams.FirstOrDefault()?.AsString() == "this,Any,0" ? jparams.Count - 1 : jparams.Count;
                if (count != argCount) continue;

                return method as JObject;
            }
        }
        return null;
    }

    public static (int start, int end) GetMethodStartEndAddress(JToken debugInfoMethod)
    {
        if (debugInfoMethod["range"] is not JString range) return (-1, -1);
        var rangeGroups = RangeRegex.Match(range.AsString()).Groups;
        return (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
    }

    private static IEnumerable<(int address, Instruction instruction)> EnumerateInstructions(this Script script)
    {
        var address = 0;
        var opcode = OpCode.PUSH0;
        Instruction instruction;
        for (; address < script.Length; address += instruction.Size)
        {
            instruction = script.GetInstruction(address);
            opcode = instruction.OpCode;
            yield return (address, instruction);
        }
        if (opcode != OpCode.RET)
            yield return (address, Instruction.RET);
    }

    public static string InstructionToString(this Instruction instruction, bool addPrice = true)
    {
        var opcode = instruction.OpCode.ToString();
        var operand = instruction.Operand;

        var addprice = 0L;
        string ret;

        if (operand.IsEmpty || operand.Length == 0)
        {
            ret = $"OpCode.{opcode}";
        }
        else
        {
            var operandString = BitConverter.ToString(operand.ToArray()).Replace("-", "");

            switch (instruction.OpCode)
            {
                case OpCode.CONVERT:
                    {
                        ret = $"OpCode.{opcode} {operandString} '{(StackItemType)operand.Span[0]}'";
                        break;
                    }
                case OpCode.SYSCALL:
                    {
                        var descriptor = ApplicationEngine.GetInteropDescriptor(instruction.TokenU32);
                        addprice += descriptor.FixedPrice;
                        ret = $"OpCode.{opcode} {operandString} '{descriptor.Name}'";
                        break;
                    }
                default:
                    {
                        ret = $"OpCode.{opcode} {operandString}";
                        break;
                    }
            }
        }

        if (!addPrice) return ret;

        var fixedPrice = ApplicationEngine.OpCodePriceTable[(byte)instruction.OpCode] + addprice;
        return $"{ret} [{fixedPrice} datoshi]";
    }
}
