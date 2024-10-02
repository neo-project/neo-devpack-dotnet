using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Neo.Json;
using Neo.SmartContract;
using Neo.VM;

namespace Neo.Disassembler.CSharp;

public static class Disassembler
{
    private static readonly Regex RangeRegex = new(@"(\d+)\-(\d+)", RegexOptions.Compiled);

    public static List<Instruction> ConvertScriptToInstructions(byte[] script)
    {
        var res = EnumerateInstructions(script);

        return res.Select(x => x.instruction).ToList();
    }

    public static List<(int address, Instruction instruction)> ConvertMethodToInstructions(NefFile nef, JToken DebugInfo, string method)
    {
        var (start, end) = GetMethodStartEndAddress(method, DebugInfo);
        var instructions = EnumerateInstructions(nef.Script).ToList();
        return instructions.Where(
            ai => ai.address >= start && ai.address <= end).Select(ai => (ai.address - start, ai.instruction)).ToList();
    }

    private static (int start, int end) GetMethodStartEndAddress(string name, JToken debugInfo)
    {
        name = name.Length == 0 ? string.Empty : string.Concat(name[0].ToString().ToUpper(), name.AsSpan(1));  // first letter uppercase
        int start = -1, end = -1;
        foreach (var method in (JArray)debugInfo["methods"]!)
        {
            var methodName = method!["name"]!.AsString().Split(",")[1];
            if (methodName == name)
            {
                var rangeGroups = RangeRegex.Match(method["range"]!.AsString()).Groups;
                (start, end) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
            }
        }
        return (start, end);
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
}
