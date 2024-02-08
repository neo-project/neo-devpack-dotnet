using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Neo.Optimizer
{
    public static class DumpNef
    {
        static readonly Lazy<IReadOnlyDictionary<uint, string>> sysCallNames = new(
            () => ApplicationEngine.Services.ToImmutableDictionary(kvp => kvp.Value.Hash, kvp => kvp.Value.Name));

        public static string Unzip(string path)
        {
            using FileStream zippedBuffer = File.OpenRead(path);
            using var archive = new ZipArchive(zippedStream, ZipArchiveMode.Read, false, Encoding.UTF8);
            var entry = archive.Entries.FirstOrDefault();
            if (entry != null)
            {
                using var unzippedEntryStream = entry.Open();
                using var ms = new MemoryStream();
                unzippedEntryStream.CopyTo(ms);
                var unzippedArray = ms.ToArray();
                return Encoding.UTF8.GetString(unzippedArray);
            }
            throw new ArgumentException("No file found in zip archive");
        }

        public static void Zip(string path, byte[] content, string innerFilename)
        {
            using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
            using ZipArchive archive = new(fs, ZipArchiveMode.Create);
            using Stream stream = archive.CreateEntry(innerFilename).Open();
            stream.Write(content);
        }

        public static string GetInstructionAddressPadding(this Script script)
        {
            var digitCount = EnumerateInstructions(script).Last().address switch
            {
                var x when x < 10 => 1,
                var x when x < 100 => 2,
                var x when x < 1000 => 3,
                var x when x < 10000 => 4,
                var x when x <= ushort.MaxValue => 5,
                _ => throw new Exception($"Max script length is {ushort.MaxValue} bytes"),
            };
            return new string('0', digitCount);
        }

        public static string WriteInstruction(int address, Instruction instruction, string padString, MethodToken[] tokens)
        {
            string result = "";
            try
            {
                result += $"{address.ToString(padString)}";
                result += $" {instruction.OpCode}";
                if (!instruction.Operand.IsEmpty)
                    result += $" {instruction.GetOperandString()}";

                var comment = instruction.GetComment(address, tokens);
                if (comment.Length > 0)
                    result += $" # {comment}";
            }
            finally { }
            return result;
        }

        public static IEnumerable<(int address, Instruction instruction)> EnumerateInstructions(this Script script, bool print = false)
        {
            int address = 0;
            OpCode opcode = OpCode.PUSH0;
            Instruction instruction;
            for (; address < script.Length; address += instruction.Size)
            {
                instruction = script.GetInstruction(address);
                opcode = instruction.OpCode;
                if (print)
                    Console.WriteLine(WriteInstruction(address, instruction, "0000", new MethodToken[] { }));
                yield return (address, instruction);
            }
            if (opcode != OpCode.RET)
                yield return (address, Instruction.RET);
        }


        public static string GetOperandString(this Instruction instruction) => BitConverter.ToString(instruction.Operand.Span.ToArray());

        public static string GetComment(this Instruction instruction, int ip, MethodToken[]? tokens = null)
        {
            tokens ??= Array.Empty<MethodToken>();

            switch (instruction.OpCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    return $"{new BigInteger(instruction.Operand.Span)}";
                case OpCode.PUSHA:
                    return $"{checked(ip + instruction.TokenI32)}";
                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                    {
                        var text = System.Text.Encoding.UTF8.GetString(instruction.Operand.Span)
                            .Replace("\r", "\"\\r\"").Replace("\n", "\"\\n\"");
                        if (instruction.Operand.Length == 20)
                        {
                            return $"as script hash: {new UInt160(instruction.Operand.Span)}, as text: \"{text}\"";
                        }
                        return $"as text: \"{text}\"";
                    }
                case OpCode.JMP:
                case OpCode.JMPIF:
                case OpCode.JMPIFNOT:
                case OpCode.JMPEQ:
                case OpCode.JMPNE:
                case OpCode.JMPGT:
                case OpCode.JMPGE:
                case OpCode.JMPLT:
                case OpCode.JMPLE:
                case OpCode.CALL:
                    return OffsetComment(instruction.TokenI8);
                case OpCode.JMP_L:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE_L:
                case OpCode.CALL_L:
                    return OffsetComment(instruction.TokenI32);
                case OpCode.CALLT:
                    {
                        int index = instruction.TokenU16;
                        if (index >= tokens.Length)
                            return $"Unknown token {instruction.TokenU16}";
                        var token = tokens[index];
                        var contract = NativeContract.Contracts.SingleOrDefault(c => c.Hash == token.Hash);
                        var tokenName = contract is null ? $"{token.Hash}" : contract.Name;
                        return $"{tokenName}.{token.Method} token call";
                    }
                case OpCode.TRY:
                    return TryComment(instruction.TokenI8, instruction.TokenI8_1);
                case OpCode.TRY_L:
                    return TryComment(instruction.TokenI32, instruction.TokenI32_1);
                case OpCode.ENDTRY:
                    return OffsetComment(instruction.TokenI8);
                case OpCode.ENDTRY_L:
                    return OffsetComment(instruction.TokenI32);
                case OpCode.SYSCALL:
                    return sysCallNames.Value.TryGetValue(instruction.TokenU32, out var name)
                        ? $"{name} SysCall"
                        : $"Unknown SysCall {instruction.TokenU32}";
                case OpCode.INITSSLOT:
                    return $"{instruction.TokenU8} static variables";
                case OpCode.INITSLOT:
                    return $"{instruction.TokenU8} local variables, {instruction.TokenU8_1} arguments";
                case OpCode.LDSFLD:
                case OpCode.STSFLD:
                case OpCode.LDLOC:
                case OpCode.STLOC:
                case OpCode.LDARG:
                case OpCode.STARG:
                    return $"Slot index {instruction.TokenU8}";
                case OpCode.NEWARRAY_T:
                case OpCode.ISTYPE:
                case OpCode.CONVERT:
                    return $"{(VM.Types.StackItemType)instruction.TokenU8} type";
                default:
                    return string.Empty;
            }

            string OffsetComment(int offset) => $"pos: {checked(ip + offset)} (offset: {offset})";
            string TryComment(int catchOffset, int finallyOffset)
            {
                var builder = new System.Text.StringBuilder();
                builder.Append(catchOffset == 0 ? "no catch block, " : $"catch {OffsetComment(catchOffset)}, ");
                builder.Append(finallyOffset == 0 ? "no finally block" : $"finally {OffsetComment(finallyOffset)}");
                return builder.ToString();
            }
        }

        public static string GenerateDumpNef(NefFile nef, JToken debugInfo)
        {
            Script script = nef.Script;
            List<(int, Instruction)> addressAndInstructionsList = script.EnumerateInstructions().ToList();
            Dictionary<int, Instruction> addressToInstruction = new();
            foreach ((int a, Instruction i) in addressAndInstructionsList)
                addressToInstruction.Add(a, i);
            Dictionary<int, string> methodStartAddrToName = new();
            Dictionary<int, string> methodEndAddrToName = new();
            Dictionary<int, (int docId, int startLine, int startCol, int endLine, int endCol)> newAddrToSequencePoint = new();
            Regex documentRegex = new Regex(@"\[(\d+)\](\d+)\:(\d+)\-(\d+)\:(\d+)", RegexOptions.Compiled);
            Regex rangeRegex = new Regex(@"(\d+)\-(\d+)", RegexOptions.Compiled);
            Regex sequencePointRegex = new Regex(@"(\d+)(\[\d+\]\d+\:\d+\-\d+\:\d+)", RegexOptions.Compiled);
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                GroupCollection rangeGroups = rangeRegex.Match(method!["range"]!.AsString()).Groups;
                (int methodStartAddr, int methodEndAddr) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
                methodStartAddrToName.Add(methodStartAddr, method!["id"]!.AsString());  // TODO: same format of method name as dumpnef
                methodEndAddrToName.Add(methodEndAddr, method["id"]!.AsString());

                foreach (JToken? sequencePoint in (JArray)method!["sequence-points"]!)
                {
                    GroupCollection sequencePointGroups = sequencePointRegex.Match(sequencePoint!.AsString()).Groups;
                    GroupCollection documentGroups = documentRegex.Match(sequencePointGroups[2].ToString()).Groups;
                    newAddrToSequencePoint.Add(int.Parse(sequencePointGroups[1].Value), (
                        int.Parse(documentGroups[1].ToString()),
                        int.Parse(documentGroups[2].ToString()),
                        int.Parse(documentGroups[3].ToString()),
                        int.Parse(documentGroups[4].ToString()),
                        int.Parse(documentGroups[5].ToString())
                    ));
                }
            }

            Dictionary<string, string[]> docPathToContent = new();
            string dumpnef = "";
            foreach ((int a, Instruction i) in script.EnumerateInstructions(/*print: true*/).ToList())
            {
                if (methodStartAddrToName.ContainsKey(a))
                    dumpnef += $"# Method Start {methodStartAddrToName[a]}\n";
                if (methodEndAddrToName.ContainsKey(a))
                    dumpnef += $"# Method End {methodEndAddrToName[a]}\n";
                if (newAddrToSequencePoint.ContainsKey(a))
                {
                    var docInfo = newAddrToSequencePoint[a];
                    string docPath = debugInfo["documents"]![docInfo.docId]!.AsString();
                    if (debugInfo["document-root"] != null)
                        docPath = Path.Combine(debugInfo["document-root"]!.AsString(), docPath);
                    if (!docPathToContent.ContainsKey(docPath))
                        docPathToContent.Add(docPath, File.ReadAllLines(docPath).ToArray());
                    if (docInfo.startLine == docInfo.endLine)
                        dumpnef += $"# Code {Path.GetFileName(docPath)} line {docInfo.startLine}: \"{docPathToContent[docPath][docInfo.startLine - 1][(docInfo.startCol - 1)..(docInfo.endCol - 1)]}\"\n";
                    else
                        for (int lineIndex = docInfo.startLine; lineIndex <= docInfo.endLine; lineIndex++)
                        {
                            string src;
                            if (lineIndex == docInfo.startLine)
                                src = docPathToContent[docPath][lineIndex - 1][(docInfo.startCol - 1)..].Trim();
                            else if (lineIndex == docInfo.endLine)
                                src = docPathToContent[docPath][lineIndex - 1][..(docInfo.endCol - 1)].Trim();
                            else
                                src = docPathToContent[docPath][lineIndex - 1].Trim();
                            dumpnef += $"# Code {Path.GetFileName(docPath)} line {docInfo.startLine}: \"{src}\"\n";
                        }
                }
                if (a < script.Length)
                    dumpnef += $"{WriteInstruction(a, script.GetInstruction(a), script.GetInstructionAddressPadding(), nef.Tokens)}\n";
            }
            return dumpnef;
        }
    }
}
