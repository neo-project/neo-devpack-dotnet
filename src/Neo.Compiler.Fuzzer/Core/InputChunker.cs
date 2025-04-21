using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Utility class for breaking large inputs into smaller chunks for more efficient processing.
    /// </summary>
    public static class InputChunker
    {
        /// <summary>
        /// Break a large byte array into smaller chunks
        /// </summary>
        /// <param name="input">The input byte array</param>
        /// <param name="maxChunkSize">Maximum size of each chunk</param>
        /// <returns>List of byte array chunks</returns>
        public static List<byte[]> ChunkByteArray(byte[] input, int maxChunkSize = 1024)
        {
            if (input == null || input.Length == 0)
                return new List<byte[]>();

            var result = new List<byte[]>();

            for (int i = 0; i < input.Length; i += maxChunkSize)
            {
                int chunkSize = Math.Min(maxChunkSize, input.Length - i);
                var chunk = new byte[chunkSize];
                Array.Copy(input, i, chunk, 0, chunkSize);
                result.Add(chunk);
            }

            return result;
        }

        /// <summary>
        /// Break a large string into smaller chunks
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="maxChunkSize">Maximum size of each chunk</param>
        /// <returns>List of string chunks</returns>
        public static List<string> ChunkString(string input, int maxChunkSize = 1024)
        {
            if (string.IsNullOrEmpty(input))
                return new List<string>();

            var result = new List<string>();

            for (int i = 0; i < input.Length; i += maxChunkSize)
            {
                int chunkSize = Math.Min(maxChunkSize, input.Length - i);
                result.Add(input.Substring(i, chunkSize));
            }

            return result;
        }

        /// <summary>
        /// Break a large contract into smaller method chunks
        /// </summary>
        /// <param name="contractCode">The contract code</param>
        /// <param name="maxMethodSize">Maximum size of each method in characters</param>
        /// <returns>Modified contract code with smaller methods</returns>
        public static string ChunkContractMethods(string contractCode, int maxMethodSize = 500)
        {
            if (string.IsNullOrEmpty(contractCode))
                return contractCode;

            // Simple heuristic to break large methods into smaller ones
            // This is a basic implementation and might need refinement for complex contracts

            StringBuilder result = new StringBuilder();
            string[] lines = contractCode.Split('\n');
            StringBuilder currentMethod = new StringBuilder();
            bool inMethod = false;
            int methodSize = 0;
            int subMethodCount = 1;
            string currentMethodName = "";

            foreach (string line in lines)
            {
                // Check if this line starts a method
                if (!inMethod && line.Contains("public") && line.Contains("(") && !line.Contains(";"))
                {
                    inMethod = true;
                    currentMethod.Clear();
                    methodSize = 0;

                    // Extract method name
                    int nameStart = line.LastIndexOf(' ', line.IndexOf('(')) + 1;
                    int nameEnd = line.IndexOf('(');
                    if (nameStart > 0 && nameEnd > nameStart)
                    {
                        currentMethodName = line.Substring(nameStart, nameEnd - nameStart);
                    }
                    else
                    {
                        currentMethodName = "Method";
                    }

                    currentMethod.AppendLine(line);
                    methodSize += line.Length;
                }
                // Check if this line ends a method
                else if (inMethod && line.Trim() == "}")
                {
                    currentMethod.AppendLine(line);
                    methodSize += line.Length;

                    // If method is too large, break it into smaller methods
                    if (methodSize > maxMethodSize)
                    {
                        string breakdownMethod = BreakdownLargeMethod(currentMethod.ToString(), currentMethodName, subMethodCount);
                        result.AppendLine(breakdownMethod);
                        subMethodCount++;
                    }
                    else
                    {
                        result.AppendLine(currentMethod.ToString());
                    }

                    inMethod = false;
                }
                // Inside a method
                else if (inMethod)
                {
                    currentMethod.AppendLine(line);
                    methodSize += line.Length;
                }
                // Outside a method
                else
                {
                    result.AppendLine(line);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Break down a large method into smaller helper methods
        /// </summary>
        private static string BreakdownLargeMethod(string methodCode, string methodName, int subMethodCount)
        {
            // This is a simplified implementation
            // A more sophisticated approach would analyze the method structure

            StringBuilder result = new StringBuilder();
            string[] lines = methodCode.Split('\n');

            // Extract method signature and opening brace
            int openingBraceIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("{"))
                {
                    openingBraceIndex = i;
                    break;
                }
            }

            // Extract method body (excluding opening and closing braces)
            List<string> methodBody = new List<string>();
            for (int i = openingBraceIndex + 1; i < lines.Length - 1; i++)
            {
                methodBody.Add(lines[i]);
            }

            // Create the main method that calls helper methods
            result.AppendLine(lines[0]); // Method signature
            result.AppendLine(lines[openingBraceIndex]); // Opening brace

            // Split the method body into chunks
            int chunkSize = Math.Max(1, methodBody.Count / 3); // Aim for 3 helper methods
            for (int i = 0; i < methodBody.Count; i += chunkSize)
            {
                int currentChunk = i / chunkSize + 1;
                string helperMethodName = $"{methodName}_Helper{subMethodCount}_{currentChunk}";

                // Add call to helper method
                result.AppendLine($"    {helperMethodName}();");

                // Create helper method
                StringBuilder helperMethod = new StringBuilder();
                helperMethod.AppendLine($"    private static void {helperMethodName}()");
                helperMethod.AppendLine("    {");

                // Add chunk of the original method body to helper method
                int end = Math.Min(i + chunkSize, methodBody.Count);
                for (int j = i; j < end; j++)
                {
                    helperMethod.AppendLine(methodBody[j]);
                }

                helperMethod.AppendLine("    }");

                // Add helper method after the main method
                methodBody.AddRange(helperMethod.ToString().Split('\n'));
            }

            result.AppendLine(lines[lines.Length - 1]); // Closing brace

            return result.ToString();
        }

        /// <summary>
        /// Break a large feature set into smaller groups for more manageable testing
        /// </summary>
        /// <param name="features">Dictionary of feature generators</param>
        /// <param name="maxFeaturesPerGroup">Maximum number of features per group</param>
        /// <returns>List of feature groups</returns>
        public static List<Dictionary<string, Func<string>>> ChunkFeatures(
            Dictionary<string, Func<string>> features,
            int maxFeaturesPerGroup = 5)
        {
            if (features == null || features.Count == 0)
                return new List<Dictionary<string, Func<string>>>();

            var result = new List<Dictionary<string, Func<string>>>();
            var keys = features.Keys.ToList();

            for (int i = 0; i < keys.Count; i += maxFeaturesPerGroup)
            {
                var group = new Dictionary<string, Func<string>>();
                int end = Math.Min(i + maxFeaturesPerGroup, keys.Count);

                for (int j = i; j < end; j++)
                {
                    string key = keys[j];
                    group[key] = features[key];
                }

                result.Add(group);
            }

            return result;
        }
    }
}
