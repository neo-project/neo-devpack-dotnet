using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle runtime operations
    /// </summary>
    public static class RuntimeOperationsExtensions
    {
        /// <summary>
        /// Generate a random runtime operation
        /// </summary>
        public static string GenerateEnhancedRuntimeOperation(this FragmentGenerator generator)
        {
            Random random = new Random();
            string[] runtimeOperationTypes = {
                "BasicRuntimeProperties",
                "RuntimeNotifications",
                "RuntimeLogging",
                "RuntimeCheckWitness",
                "RuntimeGasOperations",
                "RuntimeRandomOperations"
            };

            string operationType = runtimeOperationTypes[random.Next(runtimeOperationTypes.Length)];

            switch (operationType)
            {
                case "BasicRuntimeProperties":
                    return GenerateBasicRuntimeProperties(generator);
                case "RuntimeNotifications":
                    return GenerateRuntimeNotifications(generator);
                case "RuntimeLogging":
                    return GenerateRuntimeLogging(generator);
                case "RuntimeCheckWitness":
                    return GenerateRuntimeCheckWitness(generator);
                case "RuntimeGasOperations":
                    return GenerateRuntimeGasOperations(generator);
                case "RuntimeRandomOperations":
                    return GenerateRuntimeRandomOperations(generator);
                default:
                    return GenerateBasicRuntimeProperties(generator);
            }
        }

        /// <summary>
        /// Generate basic runtime properties
        /// </summary>
        private static string GenerateBasicRuntimeProperties(FragmentGenerator generator)
        {
            Random random = new Random();
            string[] properties = {
                "string runtimePlatform = Runtime.Platform;",
                "TriggerType runtimeTrigger = Runtime.Trigger;",
                "UInt160 runtimeExecScriptHash = Runtime.ExecutingScriptHash;",
                "UInt160 runtimeCallScriptHash = Runtime.CallingScriptHash;",
                "UInt160 runtimeEntryHash = Runtime.EntryScriptHash;",
                "uint runtimeTime = Runtime.Time;",
                "uint runtimeInvocationCounter = Runtime.InvocationCounter;",
                "object runtimeContainer = Runtime.ScriptContainer;"
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Basic Runtime properties");

            // Select 1 random property to avoid variable name conflicts
            sb.AppendLine(properties[random.Next(properties.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// Generate runtime notifications
        /// </summary>
        private static string GenerateRuntimeNotifications(FragmentGenerator generator)
        {
            string eventName = generator.GenerateIdentifier("event");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Runtime notifications");
            sb.AppendLine($"// Create an event name");
            sb.AppendLine($"string {eventName} = \"TestEvent\";");
            sb.AppendLine($"// Emit a notification");
            sb.AppendLine($"// Using OnMainCompleted event as an example");
            sb.AppendLine($"OnMainCompleted({eventName}, true);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate runtime logging
        /// </summary>
        private static string GenerateRuntimeLogging(FragmentGenerator generator)
        {
            string message = generator.GenerateStringLiteral();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Runtime logging");
            sb.AppendLine($"Runtime.Log({message});");

            return sb.ToString();
        }

        /// <summary>
        /// Generate runtime check witness
        /// </summary>
        private static string GenerateRuntimeCheckWitness(FragmentGenerator generator)
        {
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Runtime check witness");
            sb.AppendLine($"bool {result} = Runtime.CheckWitness(Runtime.ExecutingScriptHash);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate runtime gas operations
        /// </summary>
        private static string GenerateRuntimeGasOperations(FragmentGenerator generator)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Runtime gas operations");
            sb.AppendLine("Runtime.BurnGas(1); // Burn 1 GAS");

            return sb.ToString();
        }

        /// <summary>
        /// Generate runtime random operations
        /// </summary>
        private static string GenerateRuntimeRandomOperations(FragmentGenerator generator)
        {
            string randomVar = generator.GenerateIdentifier("random");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Runtime random operations");
            sb.AppendLine($"ulong {randomVar} = (ulong)Runtime.GetRandom();");
            sb.AppendLine($"ulong boundedRandom = {randomVar} % 100; // Random number between 0 and 99");

            return sb.ToString();
        }
    }
}
