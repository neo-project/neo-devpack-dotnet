using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle contract features
    /// </summary>
    public static class ContractFeaturesExtensions
    {
        /// <summary>
        /// Generate a random contract attribute
        /// </summary>
        public static string GenerateContractAttribute(this FragmentGenerator generator)
        {
            Random random = new Random();
            string[] attributes = {
                "[DisplayName(\"TestContract\")]",
                "[ContractDescription(\"A test contract for Neo N3\")]",
                "[ContractAuthor(\"Neo Community\", \"dev@neo.org\")]",
                "[ContractVersion(\"1.0.0\")]",
                "[ContractPermission(Permission.Any, Method.Any)]",
                "[ContractTrust(\"*\")]",
                "[ContractHash]",
                "[Safe]",
                "[NoReentrant]"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Contract attributes would normally be placed at the class level");
            sb.AppendLine("// Example of a contract attribute:");
            sb.AppendLine("// " + attributes[random.Next(attributes.Length)]);
            sb.AppendLine("// For testing, we'll just create a variable");
            sb.AppendLine("string attributeName = \"ContractAttribute\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random contract call
        /// </summary>
        public static string GenerateContractCall(this FragmentGenerator generator)
        {
            Random random = new Random();
            string[] callTypes = {
                "BasicContractCall",
                "ContractCallWithParams",
                "ContractCallWithFlags",
                "ContractCallWithResult"
            };

            string callType = callTypes[random.Next(callTypes.Length)];

            switch (callType)
            {
                case "BasicContractCall":
                    return GenerateBasicContractCall(generator);
                case "ContractCallWithParams":
                    return GenerateContractCallWithParams(generator);
                case "ContractCallWithFlags":
                    return GenerateContractCallWithFlags(generator);
                case "ContractCallWithResult":
                    return GenerateContractCallWithResult(generator);
                default:
                    return GenerateBasicContractCall(generator);
            }
        }

        /// <summary>
        /// Generate a basic contract call
        /// </summary>
        private static string GenerateBasicContractCall(FragmentGenerator generator)
        {
            string contractHash = generator.GenerateIdentifier("contractHash");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Basic contract call");
            sb.AppendLine($"UInt160 {contractHash} = UInt160.Zero; // Replace with actual contract hash in production");
            sb.AppendLine($"Contract.Call({contractHash}, \"method\", CallFlags.All, new object[0]);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a contract call with parameters
        /// </summary>
        private static string GenerateContractCallWithParams(FragmentGenerator generator)
        {
            string contractHash = generator.GenerateIdentifier("contractHash");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Contract call with parameters");
            sb.AppendLine($"UInt160 {contractHash} = UInt160.Zero; // Replace with actual contract hash in production");
            sb.AppendLine($"object[] parameters = new object[] {{ \"param1\", 123, true }};");
            sb.AppendLine($"Contract.Call({contractHash}, \"methodWithParams\", CallFlags.All, parameters);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a contract call with specific flags
        /// </summary>
        private static string GenerateContractCallWithFlags(FragmentGenerator generator)
        {
            Random random = new Random();
            string contractHash = generator.GenerateIdentifier("contractHash");

            string[] flags = {
                "CallFlags.None",
                "CallFlags.ReadOnly",
                "CallFlags.WriteStates",
                "CallFlags.AllowCall",
                "CallFlags.AllowNotify",
                "CallFlags.States",
                "CallFlags.All"
            };

            string flag = flags[random.Next(flags.Length)];

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Contract call with specific flags");
            sb.AppendLine($"UInt160 {contractHash} = UInt160.Zero; // Replace with actual contract hash in production");
            sb.AppendLine($"Contract.Call({contractHash}, \"method\", {flag}, new object[0]);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a contract call with result handling
        /// </summary>
        private static string GenerateContractCallWithResult(FragmentGenerator generator)
        {
            string contractHash = generator.GenerateIdentifier("contractHash");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Contract call with result handling");
            sb.AppendLine($"UInt160 {contractHash} = UInt160.Zero; // Replace with actual contract hash in production");
            sb.AppendLine($"object {result} = Contract.Call({contractHash}, \"method\", CallFlags.All, new object[0]);");
            sb.AppendLine($"if ({result} is string stringResult)");
            sb.AppendLine("{");
            sb.AppendLine("    // Handle string result");
            sb.AppendLine("}");
            sb.AppendLine($"else if ({result} is int intResult)");
            sb.AppendLine("{");
            sb.AppendLine("    // Handle int result");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a stored property
        /// </summary>
        public static string GenerateStoredProperty(this FragmentGenerator generator)
        {
            string propertyName = generator.GenerateIdentifier("Property");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// This is a stored property that will be persisted in the blockchain");
            sb.AppendLine($"string {propertyName}Key = \"property_{propertyName}\"; // Key for the property");
            sb.AppendLine($"// Store a value");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, {propertyName}Key, \"test value\");");
            sb.AppendLine($"// Retrieve the value");
            sb.AppendLine($"string {propertyName}Value = Storage.Get(Storage.CurrentContext, {propertyName}Key);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a contract method with attributes
        /// </summary>
        public static string GenerateContractMethod(this FragmentGenerator generator)
        {
            Random random = new Random();
            string methodName = generator.GenerateIdentifier("Method");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Contract method call example");
            sb.AppendLine($"// This would normally be a separate method in the contract");
            sb.AppendLine($"// Example of how to call a method with parameters");
            sb.AppendLine($"bool result = true; // Simulating a method call result");
            sb.AppendLine($"string param1 = \"test parameter\";");
            sb.AppendLine($"int param2 = 42;");
            sb.AppendLine($"// Process the parameters");
            sb.AppendLine($"if (param1.Length > 0 && param2 > 0)");
            sb.AppendLine("{");
            sb.AppendLine("    // Do something with the parameters");
            sb.AppendLine("    result = true;");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine("    // Handle invalid parameters");
            sb.AppendLine("    result = false;");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
