using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle native contract operations
    /// </summary>
    public static class NativeContractOperationsExtensions
    {
        /// <summary>
        /// Generate a random native contract operation
        /// </summary>
        public static string GenerateEnhancedNativeContractOperation(this FragmentGenerator generator)
        {
            Random random = new Random();
            string[] contractTypes = {
                "NEO",
                "GAS",
                "ContractManagement",
                "CryptoLib",
                "Ledger",
                "Oracle",
                "Policy",
                "RoleManagement",
                "StdLib"
            };

            string contractType = contractTypes[random.Next(contractTypes.Length)];

            switch (contractType)
            {
                case "NEO":
                    return GenerateNEOContractOperation(generator);
                case "GAS":
                    return GenerateGASContractOperation(generator);
                case "ContractManagement":
                    return GenerateContractManagementOperation(generator);
                case "CryptoLib":
                    return GenerateCryptoLibOperation(generator);
                case "Ledger":
                    return GenerateLedgerOperation(generator);
                case "Oracle":
                    return GenerateOracleOperation(generator);
                case "Policy":
                    return GeneratePolicyOperation(generator);
                case "RoleManagement":
                    return GenerateRoleManagementOperation(generator);
                case "StdLib":
                    return GenerateStdLibOperation(generator);
                default:
                    return GenerateNEOContractOperation(generator);
            }
        }

        /// <summary>
        /// Generate NEO contract operations
        /// </summary>
        private static string GenerateNEOContractOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 neoTokenHash = NEO.Hash;",
                "string neoTokenSymbol = NEO.Symbol;",
                "byte neoTokenDecimals = NEO.Decimals;"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// NEO native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate GAS contract operations
        /// </summary>
        private static string GenerateGASContractOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 gasTokenHash = GAS.Hash;",
                "string gasTokenSymbol = GAS.Symbol;",
                "byte gasTokenDecimals = GAS.Decimals;"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// GAS native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate ContractManagement operations
        /// </summary>
        private static string GenerateContractManagementOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 contractManagementHash = ContractManagement.Hash;",
                "long minimumDeploymentFee = ContractManagement.GetMinimumDeploymentFee();",
                "Contract contract = ContractManagement.GetContract(Runtime.ExecutingScriptHash);",
                "bool hasMethod = ContractManagement.HasMethod(Runtime.ExecutingScriptHash, \"Main\", 0);"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// ContractManagement native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate CryptoLib operations
        /// </summary>
        private static string GenerateCryptoLibOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 cryptoLibHash = CryptoLib.Hash;",
                "ByteString sha256Hash = CryptoLib.Sha256(\"test\");",
                "ByteString ripemd160Hash = CryptoLib.Ripemd160(\"test\");",
                "uint murmur32Hash = CryptoLib.Murmur32(\"test\", 0);"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// CryptoLib native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate Ledger operations
        /// </summary>
        private static string GenerateLedgerOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 ledgerHash = Ledger.Hash;",
                "UInt256 currentHash = Ledger.CurrentHash;",
                "uint currentIndex = Ledger.CurrentIndex;",
                "ulong txHeight = Ledger.GetTransactionHeight(UInt256.Zero);",
                "VMState vmState = Ledger.GetTransactionVMState(UInt256.Zero);"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Ledger native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate Oracle operations
        /// </summary>
        private static string GenerateOracleOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 oracleHash = Oracle.Hash;",
                "uint minimumResponseFee = Oracle.MinimumResponseFee;",
                "long price = Oracle.GetPrice();"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Oracle native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate Policy operations
        /// </summary>
        private static string GeneratePolicyOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 policyHash = Policy.Hash;",
                "long policyFeePerByte = Policy.GetFeePerByte();",
                "uint policyExecFeeFactor = Policy.GetExecFeeFactor();",
                "uint storagePrice = Policy.GetStoragePrice();",
                "bool isBlocked = Policy.IsBlocked(Runtime.ExecutingScriptHash);"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Policy native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate RoleManagement operations
        /// </summary>
        private static string GenerateRoleManagementOperation(FragmentGenerator generator)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// RoleManagement native contract operations");
            sb.AppendLine("UInt160 roleManagementHash = RoleManagement.Hash;");

            return sb.ToString();
        }

        /// <summary>
        /// Generate StdLib operations
        /// </summary>
        private static string GenerateStdLibOperation(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] operations = {
                "UInt160 stdLibHash = StdLib.Hash;",
                "string base64String = StdLib.Base64Encode(\"test\"); ByteString base64Decoded = StdLib.Base64Decode(base64String);",
                "string intString = StdLib.Itoa(42); int parsedInt = StdLib.Atoi(\"42\");"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// StdLib native contract operations");

            // Select 1 random operation to avoid variable name conflicts
            sb.AppendLine(operations[random.Next(operations.Length)]);

            return sb.ToString();
        }
    }
}
