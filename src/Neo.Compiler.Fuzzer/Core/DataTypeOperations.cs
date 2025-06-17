using System;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle data type operations
    /// </summary>
    public static class DataTypeOperations
    {
        /// <summary>
        /// Generate a random char declaration
        /// </summary>
        public static string GenerateCharDeclaration(this FragmentGenerator generator)
        {
            Random random = new Random();
            string varName = generator.GenerateIdentifier("char");

            // Generate a random ASCII character (avoiding control characters and quotes)
            char c = (char)random.Next(65, 90); // A-Z

            return $"char {varName} = '{c}';";
        }

        /// <summary>
        /// Generate a random struct declaration
        /// </summary>
        public static string GenerateStructDeclaration(this FragmentGenerator generator)
        {
            Random random = new Random();
            string structName = generator.GenerateIdentifier("Struct");
            string fieldName1 = generator.GenerateIdentifier("field");
            string fieldName2 = generator.GenerateIdentifier("field");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Struct declaration would normally be at class level");
            sb.AppendLine($"// Example of how a struct would be declared:");
            sb.AppendLine($"// public struct {structName}");
            sb.AppendLine($"// {{");
            sb.AppendLine($"//     public int {fieldName1};");
            sb.AppendLine($"//     public string {fieldName2};");
            sb.AppendLine($"// }}");
            sb.AppendLine($"// For testing, we'll just create a variable");
            sb.AppendLine($"string {structName}Name = \"{structName}\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random tuple declaration and usage
        /// </summary>
        public static string GenerateTupleDeclaration(this FragmentGenerator generator)
        {
            Random random = new Random();
            string tupleName = generator.GenerateIdentifier("tuple");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Tuple declaration and usage");
            sb.AppendLine($"(string, int) {tupleName} = (\"test\", 42);");
            sb.AppendLine($"string {tupleName}Item1 = {tupleName}.Item1;");
            sb.AppendLine($"int {tupleName}Item2 = {tupleName}.Item2;");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random tuple deconstruction
        /// </summary>
        public static string GenerateTupleDeconstruction(this FragmentGenerator generator)
        {
            Random random = new Random();
            string name = generator.GenerateIdentifier("name");
            string age = generator.GenerateIdentifier("age");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Tuple deconstruction");
            sb.AppendLine($"(string {name}, int {age}) = (\"John\", 30);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random nullable type declaration
        /// </summary>
        public static string GenerateNullableTypeDeclaration(this FragmentGenerator generator)
        {
            Random random = new Random();
            string[] types = { "int", "bool", "byte", "long" };
            string type = types[random.Next(types.Length)];
            string varName = generator.GenerateIdentifier("nullable");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Nullable type declaration");
            sb.AppendLine($"{type}? {varName} = null;");
            sb.AppendLine($"if ({varName} == null)");
            sb.AppendLine("{");
            if (type == "bool")
            {
                sb.AppendLine($"    {varName} = {generator.GenerateBooleanLiteral()};");
            }
            else
            {
                sb.AppendLine($"    {varName} = {generator.GenerateIntegerLiteral(type)};");
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random null-coalescing operator usage
        /// </summary>
        public static string GenerateNullCoalescingOperator(this FragmentGenerator generator)
        {
            Random random = new Random();
            string varName = generator.GenerateIdentifier("value");
            string defaultName = generator.GenerateIdentifier("default");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Null-coalescing operator usage");
            sb.AppendLine($"string {varName} = null;");
            sb.AppendLine($"string {defaultName} = \"default\";");
            sb.AppendLine($"string result = {varName} ?? {defaultName};");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random null-conditional operator usage
        /// </summary>
        public static string GenerateNullConditionalOperator(this FragmentGenerator generator)
        {
            Random random = new Random();
            string varName = generator.GenerateIdentifier("text");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Null-conditional operator usage");
            sb.AppendLine($"string {varName} = \"test\";");
            sb.AppendLine($"int? length = {varName}?.Length;");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random null-coalescing assignment operator usage
        /// </summary>
        public static string GenerateNullCoalescingAssignment(this FragmentGenerator generator)
        {
            Random random = new Random();
            string varName = generator.GenerateIdentifier("value");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Null-coalescing assignment operator usage");
            sb.AppendLine($"string {varName} = null;");
            sb.AppendLine($"{varName} ??= \"default\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random range and index usage
        /// </summary>
        public static string GenerateRangeAndIndexUsage(this FragmentGenerator generator)
        {
            Random random = new Random();
            string arrayName = generator.GenerateIdentifier("array");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Range and index usage");
            sb.AppendLine($"int[] {arrayName} = new int[] {{ 1, 2, 3, 4, 5 }};");
            sb.AppendLine($"// Neo N3 doesn't support index from end (^) or range (..) operators");
            sb.AppendLine($"// int last = {arrayName}[^1]; // Last element");
            sb.AppendLine($"int last = {arrayName}[{arrayName}.Length - 1]; // Last element");
            sb.AppendLine($"// int[] slice = {arrayName}[1..4]; // Slice from index 1 to 3");
            sb.AppendLine($"// Instead, we need to create a new array manually");
            sb.AppendLine($"int[] slice = new int[] {{ {arrayName}[1], {arrayName}[2], {arrayName}[3] }};");

            return sb.ToString();
        }
    }
}
