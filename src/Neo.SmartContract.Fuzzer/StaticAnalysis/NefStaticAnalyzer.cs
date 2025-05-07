using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Fuzzer.StaticAnalysis
{
    /// <summary>
    /// Static analyzer for NEF (Neo Executable Format) files.
    /// </summary>
    public class NefStaticAnalyzer : IStaticAnalyzer
    {
        private readonly byte[] _nefBytes;
        private readonly ContractManifest _manifest;

        /// <summary>
        /// Initializes a new instance of the <see cref="NefStaticAnalyzer"/> class.
        /// </summary>
        /// <param name="nefBytes">The NEF file bytes.</param>
        /// <param name="manifest">The contract manifest.</param>
        public NefStaticAnalyzer(byte[] nefBytes, ContractManifest manifest)
        {
            _nefBytes = nefBytes ?? throw new ArgumentNullException(nameof(nefBytes));
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        /// <summary>
        /// Analyzes the NEF file and returns hints for fuzzing.
        /// </summary>
        /// <returns>A collection of static analysis hints.</returns>
        public IEnumerable<StaticAnalysisHint> Analyze()
        {
            var hints = new List<StaticAnalysisHint>();

            // Analyze methods from manifest
            foreach (var method in _manifest.Abi.Methods)
            {
                // Check for methods with parameters
                if (method.Parameters.Length > 0)
                {
                    hints.Add(new StaticAnalysisHint
                    {
                        FilePath = "NEF Analysis",
                        LineNumber = 0,
                        RiskType = "MethodWithParameters",
                        Description = $"Method {method.Name} has {method.Parameters.Length} parameters",
                        Priority = 50,
                        MethodName = method.Name
                    });

                    // Check for integer parameters (potential overflow/underflow)
                    foreach (var parameter in method.Parameters)
                    {
                        if (IsIntegerType(parameter.Type.ToString()))
                        {
                            hints.Add(new StaticAnalysisHint
                            {
                                FilePath = "NEF Analysis",
                                LineNumber = 0,
                                RiskType = "IntegerParameter",
                                Description = $"Integer parameter {parameter.Name} in method {method.Name} (potential overflow/underflow)",
                                Priority = 70,
                                MethodName = method.Name,
                                ParameterName = parameter.Name
                            });
                        }
                    }
                }

                // Check for methods that return boolean (potential access control)
                if (method.ReturnType.ToString() == "Boolean")
                {
                    hints.Add(new StaticAnalysisHint
                    {
                        FilePath = "NEF Analysis",
                        LineNumber = 0,
                        RiskType = "BooleanReturn",
                        Description = $"Method {method.Name} returns Boolean (potential access control)",
                        Priority = 60,
                        MethodName = method.Name
                    });
                }
            }

            // Analyze permissions
            if (_manifest.Permissions != null)
            {
                foreach (var permission in _manifest.Permissions)
                {
                    if (permission.Contract.ToString() == "*")
                    {
                        hints.Add(new StaticAnalysisHint
                        {
                            FilePath = "NEF Analysis",
                            LineNumber = 0,
                            RiskType = "WildcardPermission",
                            Description = $"Contract has wildcard permission for contract {permission.Contract}",
                            Priority = 80
                        });
                    }
                }
            }

            // Basic NEF analysis
            try
            {
                var nef = Neo.SmartContract.NefFile.Parse(_nefBytes);

                // Check script size
                if (nef.Script.Length > 10000)
                {
                    hints.Add(new StaticAnalysisHint
                    {
                        FilePath = "NEF Analysis",
                        LineNumber = 0,
                        RiskType = "LargeScript",
                        Description = $"Script size is large ({nef.Script.Length} bytes)",
                        Priority = 40
                    });
                }

                // Perform detailed script analysis
                var scriptHints = AnalyzeScriptPatterns(nef.Script.ToArray());
                hints.AddRange(scriptHints);
            }
            catch (Exception ex)
            {
                hints.Add(new StaticAnalysisHint
                {
                    FilePath = "NEF Analysis",
                    LineNumber = 0,
                    RiskType = "ParseError",
                    Description = $"Error parsing NEF file: {ex.Message}",
                    Priority = 30
                });
            }

            return hints;
        }

        private static bool IsIntegerType(string type)
        {
            return type == "Integer" || type == "Hash160" || type == "Hash256" ||
                   type == "ByteArray" || type == "PublicKey";
        }

        private static bool ContainsOpCode(byte[] script, Neo.VM.OpCode opCode)
        {
            // Create a script object to properly parse the bytecode
            var scriptObj = new Neo.VM.Script(script);

            // Iterate through the script using proper instruction parsing
            for (int ip = 0; ip < script.Length;)
            {
                try
                {
                    // Get the instruction at the current position
                    var instruction = scriptObj.GetInstruction(ip);

                    // Check if this instruction matches the target opcode
                    if (instruction.OpCode == opCode)
                        return true;

                    // Move to the next instruction
                    ip += instruction.Size;
                }
                catch (Exception)
                {
                    // If we encounter an error parsing an instruction, move to the next byte
                    // This can happen if the script is malformed
                    ip++;
                }
            }

            return false;
        }

        /// <summary>
        /// Analyzes a script for potentially dangerous patterns
        /// </summary>
        private static List<StaticAnalysisHint> AnalyzeScriptPatterns(byte[] script)
        {
            var hints = new List<StaticAnalysisHint>();
            var scriptObj = new Neo.VM.Script(script);

            // Track the stack depth (simplified)
            int stackDepth = 0;

            // Track jump targets for control flow analysis
            var jumpTargets = new HashSet<int>();

            // First pass: identify all jump targets
            for (int ip = 0; ip < script.Length;)
            {
                try
                {
                    var instruction = scriptObj.GetInstruction(ip);

                    // Check for jump instructions and record their targets
                    if (instruction.OpCode == Neo.VM.OpCode.JMP ||
                        instruction.OpCode == Neo.VM.OpCode.JMPIF ||
                        instruction.OpCode == Neo.VM.OpCode.JMPIFNOT)
                    {
                        // Get the jump offset
                        int offset = instruction.TokenI8;
                        int targetIp = ip + offset;

                        // Record the jump target
                        if (targetIp >= 0 && targetIp < script.Length)
                        {
                            jumpTargets.Add(targetIp);
                        }
                        else
                        {
                            // Invalid jump target
                            hints.Add(new StaticAnalysisHint
                            {
                                FilePath = "Script Analysis",
                                LineNumber = ip,
                                RiskType = "InvalidJump",
                                Description = $"Jump instruction at position {ip} targets invalid position {targetIp}",
                                Priority = 80
                            });
                        }
                    }

                    ip += instruction.Size;
                }
                catch (Exception)
                {
                    ip++;
                }
            }

            // Second pass: analyze instructions
            for (int ip = 0; ip < script.Length;)
            {
                try
                {
                    var instruction = scriptObj.GetInstruction(ip);

                    // Check for potentially dangerous patterns

                    // 1. Check for SYSCALL instructions
                    if (instruction.OpCode == Neo.VM.OpCode.SYSCALL)
                    {
                        // Get the syscall number
                        uint syscallNumber = instruction.TokenU32;

                        // Add a hint about the syscall
                        hints.Add(new StaticAnalysisHint
                        {
                            FilePath = "Script Analysis",
                            LineNumber = ip,
                            RiskType = "Syscall",
                            Description = $"SYSCALL instruction at position {ip} with number {syscallNumber}",
                            Priority = 40
                        });
                    }

                    // 2. Check for potential stack underflow
                    int stackEffect = GetStackEffect(instruction.OpCode);
                    stackDepth += stackEffect;

                    if (stackDepth < 0 && !jumpTargets.Contains(ip))
                    {
                        // Potential stack underflow
                        hints.Add(new StaticAnalysisHint
                        {
                            FilePath = "Script Analysis",
                            LineNumber = ip,
                            RiskType = "StackUnderflow",
                            Description = $"Potential stack underflow at position {ip} with instruction {instruction.OpCode}",
                            Priority = 90
                        });
                    }

                    // 3. Check for infinite loops
                    if (instruction.OpCode == Neo.VM.OpCode.JMP)
                    {
                        int offset = instruction.TokenI8;
                        if (offset <= 0)
                        {
                            // Backward jump or self-jump could create an infinite loop
                            hints.Add(new StaticAnalysisHint
                            {
                                FilePath = "Script Analysis",
                                LineNumber = ip,
                                RiskType = "PotentialInfiniteLoop",
                                Description = $"Backward jump at position {ip} could create an infinite loop",
                                Priority = 70
                            });
                        }
                    }

                    // 4. Check for storage operations
                    if (instruction.OpCode == Neo.VM.OpCode.SYSCALL)
                    {
                        string syscallToken = instruction.TokenString;
                        string? syscallName = GetSyscallName(syscallToken);

                        // Check for storage operations
                        if (syscallName != null && syscallName.Contains("Storage."))
                        {
                            // Track storage operations
                            hints.Add(new StaticAnalysisHint
                            {
                                FilePath = "Script Analysis",
                                LineNumber = ip,
                                RiskType = "StorageOperation",
                                Description = $"Storage operation at position {ip}: {syscallName}",
                                Priority = 60
                            });

                            // Check for missing witness checks before storage writes
                            if (syscallName.Contains("Storage.Put") ||
                                syscallName.Contains("Storage.Delete"))
                            {
                                // Check if there's a witness check before this operation
                                bool hasWitnessCheck = HasWitnessCheckBefore(script, ip);
                                if (!hasWitnessCheck)
                                {
                                    hints.Add(new StaticAnalysisHint
                                    {
                                        FilePath = "Script Analysis",
                                        LineNumber = ip,
                                        RiskType = "MissingWitnessCheck",
                                        Description = $"Storage write operation at position {ip} without witness check",
                                        Priority = 90
                                    });
                                }
                            }
                        }

                        // 5. Check for reentrancy vulnerabilities
                        if (syscallName != null && syscallName.Contains("Contract.Call"))
                        {
                            // Check if there are storage writes after this contract call
                            bool hasStorageWriteAfter = HasStorageWriteAfter(script, ip);
                            if (hasStorageWriteAfter)
                            {
                                hints.Add(new StaticAnalysisHint
                                {
                                    FilePath = "Script Analysis",
                                    LineNumber = ip,
                                    RiskType = "PotentialReentrancy",
                                    Description = $"Contract call at position {ip} followed by storage write - potential reentrancy vulnerability",
                                    Priority = 95
                                });
                            }
                        }
                    }

                    ip += instruction.Size;
                }
                catch (Exception)
                {
                    ip++;
                }
            }

            return hints;
        }

        /// <summary>
        /// Gets the effect on stack size for a given opcode (simplified)
        /// </summary>
        private static int GetStackEffect(Neo.VM.OpCode opCode)
        {
            // This is a simplified implementation
            // A complete implementation would account for all opcodes

            switch (opCode)
            {
                // Instructions that push values onto the stack
                case Neo.VM.OpCode.PUSH0:
                case Neo.VM.OpCode.PUSHDATA1:
                case Neo.VM.OpCode.PUSHDATA2:
                case Neo.VM.OpCode.PUSHDATA4:
                case Neo.VM.OpCode.PUSHM1:
                case Neo.VM.OpCode.PUSH1:
                case Neo.VM.OpCode.PUSH2:
                case Neo.VM.OpCode.PUSH3:
                case Neo.VM.OpCode.PUSH4:
                case Neo.VM.OpCode.PUSH5:
                case Neo.VM.OpCode.PUSH6:
                case Neo.VM.OpCode.PUSH7:
                case Neo.VM.OpCode.PUSH8:
                case Neo.VM.OpCode.PUSH9:
                case Neo.VM.OpCode.PUSH10:
                case Neo.VM.OpCode.PUSH11:
                case Neo.VM.OpCode.PUSH12:
                case Neo.VM.OpCode.PUSH13:
                case Neo.VM.OpCode.PUSH14:
                case Neo.VM.OpCode.PUSH15:
                case Neo.VM.OpCode.PUSH16:
                    return 1;

                // Instructions that pop values from the stack
                case Neo.VM.OpCode.DROP:
                    return -1;
                case Neo.VM.OpCode.NIP:
                    return -1;

                // Instructions that operate on two values and produce one result
                case Neo.VM.OpCode.ADD:
                case Neo.VM.OpCode.SUB:
                case Neo.VM.OpCode.MUL:
                case Neo.VM.OpCode.DIV:
                case Neo.VM.OpCode.MOD:
                case Neo.VM.OpCode.AND:
                case Neo.VM.OpCode.OR:
                case Neo.VM.OpCode.XOR:
                case Neo.VM.OpCode.EQUAL:
                case Neo.VM.OpCode.NOTEQUAL:
                case Neo.VM.OpCode.LT:
                case Neo.VM.OpCode.GT:
                case Neo.VM.OpCode.LE:
                case Neo.VM.OpCode.GE:
                    return -1; // Pops 2, pushes 1

                // Instructions that duplicate stack items
                case Neo.VM.OpCode.DUP:
                    return 1;

                // Default case for other instructions
                default:
                    return 0; // Assume no net effect for other instructions
            }
        }

        /// <summary>
        /// Gets the syscall name from a syscall token
        /// </summary>
        private static string? GetSyscallName(string syscallToken)
        {
            // This is a simplified mapping of common Neo syscalls
            // A complete implementation would include all syscalls

            // Define a dictionary of known syscall tokens to names
            var syscalls = new Dictionary<string, string>
            {
                // Storage syscalls
                { "System.Storage.Get", "Storage.Get" },
                { "System.Storage.Put", "Storage.Put" },
                { "System.Storage.Delete", "Storage.Delete" },
                { "System.Storage.Find", "Storage.Find" },

                // Runtime syscalls
                { "System.Runtime.CheckWitness", "Runtime.CheckWitness" },
                { "System.Runtime.GetTime", "Runtime.GetTime" },
                { "System.Runtime.Notify", "Runtime.Notify" },
                { "System.Runtime.Log", "Runtime.Log" },

                // Contract syscalls
                { "System.Contract.Call", "Contract.Call" },
                { "System.Contract.CallEx", "Contract.CallEx" },
                { "System.Contract.Create", "Contract.Create" },
                { "System.Contract.Update", "Contract.Update" },

                // Crypto syscalls
                { "System.Crypto.SHA1", "Crypto.SHA1" },
                { "System.Crypto.SHA256", "Crypto.SHA256" },
                { "System.Crypto.VerifySignature", "Crypto.VerifySignature" }
            };

            // Try to get the syscall name
            if (syscalls.TryGetValue(syscallToken, out var name))
            {
                return name;
            }

            // If not found, return null
            return null;
        }

        /// <summary>
        /// Checks if there's a witness check before a given position in the script
        /// </summary>
        private static bool HasWitnessCheckBefore(byte[] script, int position)
        {
            var scriptObj = new Neo.VM.Script(script);

            // Scan the script from the beginning to the given position
            for (int ip = 0; ip < position;)
            {
                try
                {
                    var instruction = scriptObj.GetInstruction(ip);

                    // Check for Runtime.CheckWitness syscall
                    if (instruction.OpCode == Neo.VM.OpCode.SYSCALL)
                    {
                        string syscallToken = instruction.TokenString;
                        string? syscallName = GetSyscallName(syscallToken);

                        if (syscallName == "Runtime.CheckWitness")
                        {
                            return true;
                        }
                    }

                    ip += instruction.Size;
                }
                catch (Exception)
                {
                    ip++;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there's a storage write after a given position in the script
        /// </summary>
        private static bool HasStorageWriteAfter(byte[] script, int position)
        {
            var scriptObj = new Neo.VM.Script(script);

            // Scan the script from the given position to the end
            for (int ip = position; ip < script.Length;)
            {
                try
                {
                    var instruction = scriptObj.GetInstruction(ip);

                    // Check for Storage.Put or Storage.Delete syscall
                    if (instruction.OpCode == Neo.VM.OpCode.SYSCALL)
                    {
                        string syscallToken = instruction.TokenString;
                        string? syscallName = GetSyscallName(syscallToken);

                        if (syscallName == "Storage.Put" || syscallName == "Storage.Delete")
                        {
                            return true;
                        }
                    }

                    ip += instruction.Size;
                }
                catch (Exception)
                {
                    ip++;
                }
            }

            return false;
        }
    }
}
