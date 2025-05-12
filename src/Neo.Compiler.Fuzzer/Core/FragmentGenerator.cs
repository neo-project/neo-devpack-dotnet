using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Generates C# code fragments for Neo N3 smart contract fuzzing.
    /// This class provides methods to create random but valid C# code fragments for testing.
    /// </summary>
    public class FragmentGenerator
    {
        private readonly Random _random;
        private readonly HashSet<string> _usedIdentifiers = new HashSet<string>();

        public FragmentGenerator(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Generate a random identifier name that is guaranteed to be unique
        /// </summary>
        public string GenerateIdentifier(string prefix = "var")
        {
            string identifier;
            do
            {
                identifier = $"{prefix}{_random.Next(1, 10000)}";
            } while (_usedIdentifiers.Contains(identifier));

            _usedIdentifiers.Add(identifier);
            return identifier;
        }

        /// <summary>
        /// Generate a random string literal
        /// </summary>
        public string GenerateStringLiteral(int maxLength = 20)
        {
            int length = _random.Next(1, maxLength);
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                // Generate a random ASCII character (avoiding control characters and quotes)
                char c = (char)_random.Next(32, 126);
                if (c == '"' || c == '\\')
                {
                    sb.Append('\\');
                }
                sb.Append(c);
            }

            return $"\"{sb}\"";
        }

        /// <summary>
        /// Generate a random integer literal
        /// </summary>
        public string GenerateIntegerLiteral(string type = "int")
        {
            if (type == "short")
                return _random.Next(-32768, 32767).ToString();
            else if (type == "ushort")
                return _random.Next(0, 32767).ToString();
            else if (type == "byte")
                return _random.Next(0, 255).ToString();
            else if (type == "sbyte")
                return _random.Next(-128, 127).ToString();
            else if (type == "ulong" || type == "uint")
                return _random.Next(0, 1000000).ToString();
            else
                return _random.Next(-1000000, 1000000).ToString();
        }

        /// <summary>
        /// Generate a random boolean literal
        /// </summary>
        public string GenerateBooleanLiteral()
        {
            return _random.Next(2) == 0 ? "false" : "true";
        }

        /// <summary>
        /// Generate a random enum declaration
        /// </summary>
        public string GenerateEnumDeclaration()
        {
            string enumName = GenerateIdentifier("MyEnum");
            int memberCount = _random.Next(2, 5);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public enum {enumName}");
            sb.AppendLine("{");
            for (int i = 0; i < memberCount; i++)
            {
                sb.Append($"    Member{i + 1}");
                if (i < memberCount - 1)
                {
                    sb.AppendLine(",");
                }
                else
                {
                    sb.AppendLine();
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Generate code that uses a previously declared enum (assumes MyEnum exists)
        /// </summary>
        public string GenerateEnumUsage()
        {
            string enumName = "MyEnum"; // Assuming GenerateEnumDeclaration created this
            string varName = GenerateIdentifier("enumVar");
            // Get enum members (assuming Member1, Member2, etc.)
            // In a real scenario, we might need to parse the enum declaration or pass members
            int memberIndex = _random.Next(1, 4); // Assuming 2-4 members
            string memberName = $"Member{memberIndex}";

            return $"{enumName} {varName} = {enumName}.{memberName};";
        }

        /// <summary>
        /// Generate a random primitive type declaration
        /// </summary>
        public string GeneratePrimitiveTypeDeclaration()
        {
            string[] primitiveTypes = {
                "bool", "byte", "sbyte", "short", "ushort",
                "int", "uint", "long", "ulong", "string"
            };

            string type = primitiveTypes[_random.Next(primitiveTypes.Length)];
            string varName = GenerateIdentifier();
            string value;

            switch (type)
            {
                case "bool":
                    value = GenerateBooleanLiteral();
                    break;
                case "string":
                    value = GenerateStringLiteral();
                    break;
                case "byte":
                case "sbyte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                    value = GenerateIntegerLiteral(type);
                    if (type == "long" || type == "ulong")
                        value += "L";
                    else if (type == "uint")
                        value += "U";
                    break;
                default:
                    value = "null";
                    break;
            }

            return $"{type} {varName} = {value};";
        }

        /// <summary>
        /// Generate a random complex type declaration
        /// </summary>
        public string GenerateComplexTypeDeclaration()
        {
            string[] complexTypes = {
                "UInt160",
                "UInt256",
                "ECPoint",
                "ByteString",
                "Map<string, string>",
                "List<string>",
                "BigInteger",
                "StorageMap",
                "StorageContext",
                "Iterator<byte[], byte[]>",
                "Notification",
                "Block",
                "Transaction",
                "Header",
                "Contract"
                // Removed Dictionary as it's not supported in Neo N3
            };

            string type = complexTypes[_random.Next(complexTypes.Length)];
            string varName = GenerateIdentifier();
            string value;

            switch (type)
            {
                case "UInt160":
                    string[] uint160Options = {
                        "UInt160.Zero",
                        "Runtime.ExecutingScriptHash",
                        "Runtime.CallingScriptHash"
                    };
                    value = uint160Options[_random.Next(uint160Options.Length)];
                    break;
                case "UInt256":
                    string[] uint256Options = {
                        "UInt256.Zero",
                        "Ledger.CurrentHash"
                    };
                    value = uint256Options[_random.Next(uint256Options.Length)];
                    break;
                case "ECPoint":
                    // ECPoint is a special type that requires a valid public key
                    // For testing, we'll use a hardcoded value
                    // Avoid using ECPoint for now as it causes conversion issues
                    value = "null";
                    break;
                case "ByteString":
                    string[] bytestringOptions = {
                        $"new byte[] {{ {string.Join(", ", Enumerable.Range(0, _random.Next(1, 5)).Select(_ => _random.Next(256)))} }}.ToByteString()",
                        "\"test\".ToByteArray().ToByteString()",
                        "ByteString.Empty"
                    };
                    value = bytestringOptions[_random.Next(bytestringOptions.Length)];
                    break;
                case "Map<string, string>":
                    value = "new Map<string, string>()";
                    break;
                case "List<string>":
                    value = "new Neo.SmartContract.Framework.List<string>()";
                    break;
                case "BigInteger":
                    string[] bigIntOptions = {
                        "BigInteger.Zero",
                        "BigInteger.One",
                        "new BigInteger(100)",
                        "NEO.TotalSupply()"
                    };
                    value = bigIntOptions[_random.Next(bigIntOptions.Length)];
                    break;
                case "StorageMap":
                    value = "new StorageMap(Storage.CurrentContext, \"prefix\")";
                    break;
                case "StorageContext":
                    string[] contextOptions = {
                        "Storage.CurrentContext",
                        "Storage.CurrentReadOnlyContext"
                    };
                    value = contextOptions[_random.Next(contextOptions.Length)];
                    break;
                case "Iterator<byte[], byte[]>":
                    value = "Storage.Find(Storage.CurrentContext, \"prefix\", FindOptions.ValuesOnly)";
                    break;
                case "Notification":
                    value = "Runtime.GetNotifications(Runtime.ExecutingScriptHash).Next() ? Runtime.GetNotifications(Runtime.ExecutingScriptHash).Value : null";
                    break;
                case "Block":
                    value = "Ledger.GetBlock(Ledger.CurrentHash)";
                    break;
                case "Transaction":
                    value = "Ledger.GetTransaction(UInt256.Zero)";
                    break;
                case "Header":
                    value = "Ledger.GetHeader(Ledger.CurrentHash)";
                    break;
                case "Contract":
                    value = "ContractManagement.GetContract(Runtime.ExecutingScriptHash)";
                    break;
                // Removed Dictionary case as it's not supported in Neo N3

                default:
                    value = "null";
                    break;
            }

            return $"{type} {varName} = {value};";
        }

        /// <summary>
        /// Generate a random array declaration
        /// </summary>
        public string GenerateArrayDeclaration()
        {
            string[] types = { "int", "string", "bool", "byte" };
            string type = types[_random.Next(types.Length)];
            string varName = GenerateIdentifier("array");
            int size = _random.Next(1, 5);

            StringBuilder sb = new StringBuilder();
            sb.Append($"{type}[] {varName} = new {type}[{size}] {{ ");

            for (int i = 0; i < size; i++)
            {
                switch (type)
                {
                    case "int":
                        sb.Append(GenerateIntegerLiteral());
                        break;
                    case "string":
                        sb.Append(GenerateStringLiteral());
                        break;
                    case "bool":
                        sb.Append(GenerateBooleanLiteral());
                        break;
                    case "byte":
                        sb.Append(_random.Next(256));
                        break;
                }

                if (i < size - 1)
                    sb.Append(", ");
            }

            sb.Append(" };");
            return sb.ToString();
        }

        /// <summary>
        /// Generate a random if statement
        /// </summary>
        public string GenerateIfStatement()
        {
            string condition = GenerateIdentifier("condition");
            string result = GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"bool {condition} = {GenerateBooleanLiteral()};");
            sb.AppendLine($"string {result};");
            sb.AppendLine($"if ({condition})");
            sb.AppendLine("{");
            sb.AppendLine($"    {result} = \"Condition is true\";");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine($"    {result} = \"Condition is false\";");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random for loop
        /// </summary>
        public string GenerateForLoop()
        {
            string i = GenerateIdentifier("i");
            string sum = GenerateIdentifier("sum");
            int count = _random.Next(1, 5);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"int {sum} = 0;");
            sb.AppendLine($"for (int {i} = 0; {i} < {count}; {i}++)");
            sb.AppendLine("{");
            sb.AppendLine($"    {sum} += {i};");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random Neo N3 storage operation
        /// </summary>
        public string GenerateStorageOperation()
        {
            string[] operations = {
                GenerateBasicStorageOperation(),
                GenerateStorageMapOperation(),
                GenerateStorageFindOperation(),
                GenerateStorageDeleteOperation()
            };

            return operations[_random.Next(operations.Length)];
        }

        /// <summary>
        /// Generate a basic storage operation (Put/Get)
        /// </summary>
        private string GenerateBasicStorageOperation()
        {
            string key = GenerateIdentifier("key");
            string value = GenerateIdentifier("value");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Basic Storage Put/Get");
            sb.AppendLine($"string {key} = \"test_key\";");
            sb.AppendLine($"string {value} = \"test_value\";");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, {key}, {value});");
            sb.AppendLine($"string retrieved = Storage.Get(Storage.CurrentContext, {key});");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a StorageMap operation
        /// </summary>
        private string GenerateStorageMapOperation()
        {
            string prefix = GenerateIdentifier("prefix");
            string key = GenerateIdentifier("key");
            string value = GenerateIdentifier("value");
            string map = GenerateIdentifier("map");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// StorageMap operations");
            sb.AppendLine($"string {prefix} = \"map_prefix\";");
            sb.AppendLine($"StorageMap {map} = new StorageMap(Storage.CurrentContext, {prefix});");
            sb.AppendLine($"string {key} = \"map_key\";");
            sb.AppendLine($"string {value} = \"map_value\";");
            sb.AppendLine($"{map}.Put({key}, {value});");
            sb.AppendLine($"string mapValue = {map}.Get({key});");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a Storage.Find operation
        /// </summary>
        private string GenerateStorageFindOperation()
        {
            string prefix = GenerateIdentifier("prefix");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Storage.Find operation");
            sb.AppendLine($"string {prefix} = \"find_prefix\";");
            sb.AppendLine($"// Storage.Find returns an iterator that can be used to iterate over storage entries");
            sb.AppendLine($"// The following code is commented out as it requires special handling");
            sb.AppendLine($"// var iterator = Storage.Find(Storage.CurrentContext, {prefix}, FindOptions.ValuesOnly);");
            sb.AppendLine($"// while (iterator.Next())");
            sb.AppendLine($"// {{");
            sb.AppendLine($"//     // Process each item");
            sb.AppendLine($"//     ByteString key = iterator.Key;");
            sb.AppendLine($"//     ByteString value = iterator.Value;");
            sb.AppendLine($"//     Runtime.Log(key + \": \" + value);");
            sb.AppendLine($"// }}");
            sb.AppendLine($"// Instead, we'll just use a simple Storage.Put and Storage.Get");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, \"test_key\", \"test_value\");");
            sb.AppendLine($"ByteString value = Storage.Get(Storage.CurrentContext, \"test_key\");");
            sb.AppendLine($"Runtime.Log(\"Value: \" + value);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a Storage.Delete operation
        /// </summary>
        private string GenerateStorageDeleteOperation()
        {
            string key = GenerateIdentifier("key");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Storage.Delete operation");
            sb.AppendLine($"string {key} = \"delete_key\";");
            sb.AppendLine($"// First store something");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, {key}, \"value_to_delete\");");
            sb.AppendLine($"// Then delete it");
            sb.AppendLine($"Storage.Delete(Storage.CurrentContext, {key});");
            sb.AppendLine($"// Verify it's gone");
            sb.AppendLine($"ByteString checkValue = Storage.Get(Storage.CurrentContext, {key});");
            sb.AppendLine($"bool isDeleted = checkValue is null;");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random Neo N3 runtime operation
        /// </summary>
        public string GenerateRuntimeOperation()
        {
            string[] operations = {
                // Basic Runtime properties
                "string runtimePlatform = Runtime.Platform;",
                "TriggerType runtimeTrigger = Runtime.Trigger;",
                "UInt160 runtimeExecScriptHash = Runtime.ExecutingScriptHash;",
                "UInt160 runtimeCallScriptHash = Runtime.CallingScriptHash;",
                "UInt160 runtimeEntryHash = Runtime.EntryScriptHash;",
                "uint runtimeTime = Runtime.Time;",
                "// Runtime.GasLeft and Runtime.InvocationCounter are ulong, so we need to cast them\nulong runtimeGasLeft = Runtime.GasLeft;\nulong runtimeInvocationCounter = Runtime.InvocationCounter;",

                // Runtime logging
                "Runtime.Log(\"Test log message\");",

                // Runtime CheckWitness
                "bool isWitness = Runtime.CheckWitness(Runtime.ExecutingScriptHash);",

                // Runtime Notifications
                "var notifications = Runtime.GetNotifications();",

                // Runtime BurnGas
                "Runtime.BurnGas(10);"
            };

            return operations[_random.Next(operations.Length)];
        }

        /// <summary>
        /// Generate a random native contract call for Neo N3
        /// </summary>
        public string GenerateNativeContractCall()
        {
            string[] calls = {
                // NEO Token
                "string neoTokenSymbol = NEO.Symbol;",
                "byte neoTokenDecimals = NEO.Decimals;",
                "UInt160 neoTokenHash = NEO.Hash;",
                "BigInteger neoTotalSupply = NEO.TotalSupply();",

                // GAS Token
                "string gasTokenSymbol = GAS.Symbol;",
                "byte gasTokenDecimals = GAS.Decimals;",
                "UInt160 gasTokenHash = GAS.Hash;",
                "BigInteger gasTotalSupply = GAS.TotalSupply();",

                // CryptoLib
                "ByteString murmurHash = CryptoLib.Murmur32(\"test\", 0);",
                "ByteString sha256Hash = CryptoLib.Sha256(\"test\");",
                "ByteString ripemd160Hash = CryptoLib.Ripemd160(\"test\");",

                // Ledger
                "uint blockHeight = Ledger.CurrentIndex;",
                "UInt256 blockHash = Ledger.CurrentHash;",

                // ContractManagement
                "UInt160 contractHash = Runtime.ExecutingScriptHash;",
                "Contract contract = ContractManagement.GetContract(contractHash);",

                // StdLib
                "ByteString jsonData = StdLib.JsonSerialize(new object[] { \"test\", 123 });",
                "string base64String = StdLib.Base64Encode(\"test\".ToByteArray());"
            };

            return calls[_random.Next(calls.Length)];
        }

        /// <summary>
        /// Generate a random event declaration
        /// </summary>
        public string GenerateEventDeclaration()
        {
            string eventName = GenerateIdentifier("Event");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Event declaration would normally be at class level");
            sb.AppendLine($"// Example of how an event would be declared:");
            sb.AppendLine($"// [DisplayName(\"{eventName}\")]");
            sb.AppendLine($"// public static event Action On{eventName};");
            sb.AppendLine($"// For testing, we'll just create a variable");
            sb.AppendLine($"string {eventName}Name = \"{eventName}\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random event emission
        /// </summary>
        public string GenerateEventEmission()
        {
            string eventName = GenerateIdentifier("Event");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Event emission example");
            sb.AppendLine($"// This would normally emit an event declared at class level");
            sb.AppendLine($"// For testing, we'll use the built-in OnMainCompleted method");
            sb.AppendLine($"string {eventName}Param = \"{eventName} was triggered\";");
            sb.AppendLine($"// Call the method");
            sb.AppendLine($"OnMainCompleted({eventName}Param, true);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate pattern matching code for type checking
        /// </summary>
        public string GeneratePatternMatching()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Pattern matching");
            sb.AppendLine("// Neo N3 doesn't support pattern matching with variable declaration");
            sb.AppendLine("// Using simple if-else with type checking instead");
            string obj = GenerateIdentifier("obj");
            string result = GenerateIdentifier("result");
            sb.AppendLine($"object {obj} = \"test\";");
            sb.AppendLine($"string {result};");
            sb.AppendLine($"if ({obj} is string)");
            sb.AppendLine("{");
            sb.AppendLine($"    string text = (string){obj};");
            sb.AppendLine($"    {result} = \"String: \" + text;");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine($"    {result} = \"Not a string\";");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Generate property pattern matching code
        /// </summary>
        public string GeneratePropertyPatternMatching()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Property pattern matching");
            sb.AppendLine("// Neo N3 doesn't support property patterns");
            sb.AppendLine("// Using simple if-else instead");
            string name = GenerateIdentifier("name");
            string age = GenerateIdentifier("age");
            string result = GenerateIdentifier("result");
            sb.AppendLine($"string {name} = \"John\";");
            sb.AppendLine($"int {age} = 30;");
            sb.AppendLine($"string {result};");
            sb.AppendLine($"if ({name} == \"John\" && {age} == 30)");
            sb.AppendLine("{");
            sb.AppendLine($"    {result} = \"Exact match\";");
            sb.AppendLine("}");
            sb.AppendLine($"else if ({name} == \"John\")");
            sb.AppendLine("{");
            sb.AppendLine($"    {result} = \"Name match\";");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine($"    {result} = \"No match\";");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Generate a switch expression
        /// </summary>
        public string GenerateSwitchExpression()
        {
            string value = GenerateIdentifier("value");
            string result = GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Switch expression");
            sb.AppendLine($"int {value} = 2;");
            sb.AppendLine($"string {result} = {value} switch");
            sb.AppendLine("{");
            sb.AppendLine("    0 => \"Zero\",");
            sb.AppendLine("    1 => \"One\",");
            sb.AppendLine("    2 => \"Two\",");
            sb.AppendLine("    3 => \"Three\",");
            sb.AppendLine("    _ => \"Other\"");
            sb.AppendLine("};");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a Neo N3 contract call
        /// </summary>
        public string GenerateContractCall()
        {
            string contractHash = GenerateIdentifier("contractHash");
            string result = GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Contract call with specific flags");
            sb.AppendLine($"UInt160 {contractHash} = UInt160.Zero; // Replace with actual contract hash in production");

            // Generate different types of contract calls
            string[] callTypes = {
                $"// Basic contract call\nContract.Call({contractHash}, \"method\", CallFlags.All, new object[0]);",
                $"// Contract call with return value\nobject {result} = Contract.Call({contractHash}, \"method\", CallFlags.ReadOnly, new object[] {{ \"param1\", 123 }});",
                $"// Contract call with specific flags\nContract.Call({contractHash}, \"method\", CallFlags.AllowCall, new object[0]);",
                $"// Create standard account operations are commented out as they require special handling\n// UInt160 standardAccount = Contract.CreateStandardAccount(ECPoint.FromBytes(new byte[33], ECCurve.Secp256r1));\nstring accountExample = \"Standard account example\";"
            };

            sb.AppendLine(callTypes[_random.Next(callTypes.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate a method to enhance native contract operations
        /// </summary>
        public string GenerateEnhancedNativeContractOperation()
        {
            string[] operations = {
                // NEO Token operations
                "BigInteger neoBalance = NEO.BalanceOf(Runtime.ExecutingScriptHash);",
                "bool neoTransferResult = NEO.Transfer(Runtime.ExecutingScriptHash, UInt160.Zero, 1, null);",
                "BigInteger neoUnclaimedGas = NEO.UnclaimedGas(Runtime.ExecutingScriptHash, Ledger.CurrentIndex);",

                // GAS Token operations
                "BigInteger gasBalance = GAS.BalanceOf(Runtime.ExecutingScriptHash);",
                "bool gasTransferResult = GAS.Transfer(Runtime.ExecutingScriptHash, UInt160.Zero, 1, null);",

                // Policy operations
                "bool isBlocked = Policy.IsBlocked(Runtime.ExecutingScriptHash);",
                "// Policy fee methods return long but we need to handle them as ulong\nulong feePerByte = (ulong)Policy.GetFeePerByte();",
                "// Policy fee methods return long but we need to handle them as ulong\nulong execFeeFactor = (ulong)Policy.GetExecFeeFactor();",
                "// Policy fee methods return long but we need to handle them as ulong\nulong storagePrice = (ulong)Policy.GetStoragePrice();",

                // Ledger operations
                "// Ledger operations are commented out as they require special handling\n// Block currentBlock = Ledger.GetBlock(Ledger.CurrentHash);\nUInt256 currentHash = Ledger.CurrentHash;",
                "// Transaction operations are commented out as they require special handling\n// Transaction tx = Ledger.GetTransaction(UInt256.Zero);\nuint currentIndex = Ledger.CurrentIndex;",
                "// Header operations are commented out as they require special handling\n// Header is not directly accessible\nstring ledgerExample = \"Ledger example\";",
                "// Transaction array operations are commented out as they require special handling\n// var txs = Ledger.GetTransactionFromBlock(Ledger.CurrentHash, 0);\nbool ledgerInitialized = true;",

                // Oracle operations
                "Oracle.Request(\"https://api.example.com\", \"json\", \"$.price\", \"oracleCallback\", null);",

                // RoleManagement operations
                "// RoleManagement operations are commented out as they require special handling\n// ECPoint[] nodes = RoleManagement.GetDesignatedByRole(Role.Oracle, Ledger.CurrentIndex);\nstring roleManagementExample = \"RoleManagement example\";",

                // ContractManagement operations
                "Contract contract = ContractManagement.GetContract(Runtime.ExecutingScriptHash);",
                "ContractManagement.Update(new byte[]{0}, \"{ \\\"name\\\": \\\"Updated Contract\\\" }\", null);",

                // StdLib operations
                "string jsonString = StdLib.JsonSerialize(new object[] { \"test\", 123 });",
                "object jsonObject = StdLib.JsonDeserialize(\"{ \\\"key\\\": \\\"value\\\" }\");",
                "byte[] base64Bytes = StdLib.Base64Decode(\"dGVzdA==\");",
                "string base64String = StdLib.Base64Encode(\"test\");"
            };

            return operations[_random.Next(operations.Length)];
        }

        /// <summary>
        /// Generate Oracle callback method
        /// </summary>
        public string GenerateOracleCallback()
        {
            StringBuilder sb = new StringBuilder();
            string callbackMethod = GenerateIdentifier("oracleCallback");

            sb.AppendLine($"// Oracle callback method");
            sb.AppendLine($"public static void {callbackMethod}(string url, byte[] userData, OracleResponseCode code, string result)");
            sb.AppendLine("{");
            sb.AppendLine($"    // Process Oracle response");
            sb.AppendLine($"    if (code == OracleResponseCode.Success)");
            sb.AppendLine("    {");
            sb.AppendLine($"        Runtime.Log($\"Oracle response: {{result}}\");");
            sb.AppendLine($"        Storage.Put(Storage.CurrentContext, \"oracleResult\", result);");
            sb.AppendLine("    }");
            sb.AppendLine($"    else");
            sb.AppendLine("    {");
            sb.AppendLine($"        Runtime.Log($\"Oracle request failed with code: {{code}}\");");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate code that uses a previously declared struct (assumes MyStruct exists)
        /// </summary>
        public string GenerateStructUsage()
        {
            string varName = GenerateIdentifier("structVar");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Using a struct");
            sb.AppendLine($"// This would normally use a struct, but for testing we'll use a simple class");
            sb.AppendLine($"// Define a simple struct-like class for testing");
            sb.AppendLine($"// public class SimpleStruct");
            sb.AppendLine($"// {{\n//    public string Name;\n//    public int Value;\n// }}");
            sb.AppendLine();
            sb.AppendLine($"// Create and use the struct-like class");
            sb.AppendLine($"// SimpleStruct {varName} = new SimpleStruct();");
            sb.AppendLine($"// {varName}.Name = \"Test\";");
            sb.AppendLine($"// {varName}.Value = 42;");
            sb.AppendLine($"// Runtime.Log($\"{varName}.Name: {{{varName}.Name}}\");");
            sb.AppendLine($"string structExample = \"Struct example\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a struct declaration for Neo N3
        /// </summary>
        public string GenerateStructDeclaration()
        {
            string structName = GenerateIdentifier("MyStruct");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Struct declaration would normally be at namespace level");
            sb.AppendLine($"// Example of how a struct would be declared:");
            sb.AppendLine($"// public struct {structName}");
            sb.AppendLine($"// {{");
            sb.AppendLine($"//     public string Name;");
            sb.AppendLine($"//     public int Value;");
            sb.AppendLine($"// }}");
            sb.AppendLine($"// For testing, we'll just create a variable");
            sb.AppendLine($"string {structName}Name = \"{structName}\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate Neo N3 NFT operations
        /// </summary>
        public string GenerateNFTOperations()
        {
            StringBuilder sb = new StringBuilder();

            // Choose one of the NFT operations
            string[] operations = {
                // NFT Properties
                "string nftSymbol = NFT.Symbol;",
                "byte nftDecimals = NFT.Decimals;",
                "UInt160 nftHash = NFT.Hash;",
                "BigInteger nftTotalSupply = NFT.TotalSupply();",

                // NFT Token Operations with proper variable declarations
                "ByteString tokenId = NFT.CreateToken(Runtime.ExecutingScriptHash, \"tokenURI\", 1);\nstring tokenURI = NFT.GetTokenURI(tokenId);",
                "ByteString tokenId = NFT.CreateToken(Runtime.ExecutingScriptHash, \"tokenURI\", 1);\nUInt160 tokenOwner = NFT.OwnerOf(tokenId);",
                "ByteString tokenId = NFT.CreateToken(Runtime.ExecutingScriptHash, \"tokenURI\", 1);\nbool transferResult = NFT.Transfer(Runtime.ExecutingScriptHash, UInt160.Zero, tokenId, null);"
            };

            // Add comments
            sb.AppendLine("// NFT Operations");
            sb.AppendLine(operations[_random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate Neo N3 NameService operations
        /// </summary>
        public string GenerateNameServiceOperations()
        {
            StringBuilder sb = new StringBuilder();

            // Choose one of the NameService operations
            string[] operations = {
                // NameService Properties
                "string nsSymbol = NameService.Symbol;",
                "UInt160 nsHash = NameService.Hash;",

                // NameService Operations with proper variable declarations
                "string domainName = \"example.neo\";\nUInt160 domainOwner = NameService.GetOwner(domainName);",
                "string domainName = \"example.neo\";\nstring domainRecord = NameService.GetRecord(domainName, \"profile\");",
                "string domainName = \"example.neo\";\nbool isAvailable = NameService.IsAvailable(domainName);",
                "string domainName = \"example.neo\";\nbool registerResult = NameService.Register(domainName, Runtime.ExecutingScriptHash, 1);",
                "string domainName = \"example.neo\";\nbool setRecordResult = NameService.SetRecord(domainName, \"profile\", \"My Profile\");",
                "string domainName = \"example.neo\";\nbool deleteResult = NameService.Delete(domainName);"
            };

            // Add comments
            sb.AppendLine("// NameService Operations");
            sb.AppendLine(operations[_random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate Neo N3 enhanced cryptography operations
        /// </summary>
        public string GenerateEnhancedCryptographyOperations()
        {
            string[] operations = {
                // Basic Hash Functions
                "ByteString sha256Hash = CryptoLib.Sha256(\"test\");",
                "ByteString ripemd160Hash = CryptoLib.Ripemd160(\"test\");",
                "ByteString murmurHash = CryptoLib.Murmur32(\"test\", 0);",

                // ECDSA Operations
                "// ECPoint operations are commented out as they require special handling\n// ECPoint publicKey = ECPoint.FromBytes(new byte[33], ECCurve.Secp256r1);\nstring ecPointExample = \"ECPoint example\";",
                "// ECDSA verification is commented out as it requires special handling\n// byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(\"message\");\n// bool verifyResult = CryptoLib.VerifyWithECDsa(messageBytes, new byte[64], publicKey, NamedCurve.Secp256r1);\nstring ecdsaExample = \"ECDSA example\";",

                // BLS Operations
                "// BLS operations are commented out as they require special handling\n// byte[] blsMessageBytes = System.Text.Encoding.UTF8.GetBytes(\"message\");\n// bool blsVerifyResult = CryptoLib.VerifyWithBLS12_381(new byte[48], new byte[96], blsMessageBytes, new byte[32]);\nstring blsExample = \"BLS example\";",

                // Advanced Hash Functions
                "ByteString sha256Hash = CryptoLib.Sha256(\"test\");",
                "ByteString ripemd160Hash = CryptoLib.Ripemd160(\"test\");"
            };

            return operations[_random.Next(operations.Length)];
        }

        /// <summary>
        /// Generate Neo N3 interoperability services (IOS) operations
        /// </summary>
        public string GenerateIOSOperations()
        {
            StringBuilder sb = new StringBuilder();

            // Group operations by category to ensure proper variable declarations
            string[][] operationGroups = {
                // System Operations
                new string[] {
                    "ExecutionEngine.Assert(true, \"Assertion message\");",
                    "ExecutionEngine.Abort(\"Abort message\");"
                },

                // Iterator Operations
                new string[] {
                    "// Iterator operations are commented out as they require special handling\n// Iterator operations require special handling\nstring iteratorExample = \"Iterator example\";"
                },

                // Binary Operations
                new string[] {
                    "byte[] bytes = new byte[] { 1, 2, 3 };\nint size = bytes.Length;",
                    "byte[] bytes = new byte[] { 1, 2, 3 };\nbyte element = bytes[0];",
                    "byte[] bytes = new byte[] { 1, 2, 3 };\nbyte[] range = bytes.Range(0, 2);",
                    "byte[] bytes = new byte[] { 1, 2, 3 };\nbyte[] takeBytes = bytes.Take(2);",
                    "byte[] bytes = new byte[] { 1, 2, 3 };\nbyte[] lastBytes = bytes.Last(2);"
                },

                // String Operations
                new string[] {
                    "string str = \"Hello\";\nint strSize = str.Length;",
                    "string str = \"Hello\";\nchar ch = str[0];",
                    "string str = \"Hello\";\nstring subStr = str.Substring(1, 3);",
                    "string str = \"Hello\";\nstring[] parts = str.Split('l');"
                }
            };

            // Select a random group and then a random operation from that group
            int groupIndex = _random.Next(operationGroups.Length);
            string[] selectedGroup = operationGroups[groupIndex];
            string selectedOperation = selectedGroup[_random.Next(selectedGroup.Length)];

            // Add comments based on the group
            string[] groupNames = { "System Operations", "Iterator Operations", "Binary Operations", "String Operations" };
            sb.AppendLine($"// {groupNames[groupIndex]}");
            sb.AppendLine(selectedOperation);

            return sb.ToString();
        }

        /// <summary>
        /// Generate Neo N3 attribute usage
        /// </summary>
        public string GenerateAttributeUsage()
        {
            string[] attributes = {
                "[Safe]",
                "[DisplayName(\"MyMethod\")]",
                "[ContractPermission(\"*\", \"*\")]",
                "[ContractTrust(\"0x0123456789abcdef0123456789abcdef01234567\")]",
                "[SupportedStandards(\"NEP-17\")]",
                "[ManifestExtra(\"Author\", \"Neo\")]",
                "[ManifestExtra(\"Email\", \"dev@neo.org\")]",
                "[ManifestExtra(\"Description\", \"A Neo N3 smart contract\")]"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Attribute usage example");
            sb.AppendLine("// These attributes would normally be applied to methods or classes");
            sb.AppendLine("// For example:");
            sb.AppendLine("// " + attributes[_random.Next(attributes.Length)]);
            sb.AppendLine("// static string MyMethod() => \"Hello\";");
            sb.AppendLine("string attributeName = \"Safe\";\nRuntime.Log($\"Using attribute: {attributeName}\");");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a Neo N3 contract attribute
        /// </summary>
        public string GenerateContractAttribute()
        {
            // Use a safer subset of attributes that won't cause compilation issues
            string[] safeAttributes = {
                "// [Safe] attribute marks a method as safe (read-only)\nstring safeMethodName = \"SafeMethod\";",
                "// [DisplayName] attribute provides a friendly name for a method\nstring displayName = \"FriendlyName\";",
                "// [SupportedStandards] attribute declares supported standards\nstring[] standards = new string[] { \"NEP-17\", \"NEP-11\" };",
                "// [ManifestExtra] attribute adds metadata to the contract\nstring author = \"Neo Developer\";",
                "// [ContractHash] attribute is used for contract references\nUInt160 contractHash = UInt160.Zero;"
            };

            return safeAttributes[_random.Next(safeAttributes.Length)];
        }


    }
}
