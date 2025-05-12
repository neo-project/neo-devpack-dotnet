using System;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle control flow operations
    /// </summary>
    public static class ControlFlowOperations
    {
        /// <summary>
        /// Generate a random switch statement
        /// </summary>
        public static string GenerateSwitchStatement(this FragmentGenerator generator)
        {
            Random random = new Random();
            string varName = generator.GenerateIdentifier("value");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Switch statement");
            sb.AppendLine($"int {varName} = {random.Next(1, 4)};");
            sb.AppendLine($"string {resultName};");
            sb.AppendLine($"switch ({varName})");
            sb.AppendLine("{");
            sb.AppendLine($"    case 1:");
            sb.AppendLine($"        {resultName} = \"One\";");
            sb.AppendLine($"        break;");
            sb.AppendLine($"    case 2:");
            sb.AppendLine($"        {resultName} = \"Two\";");
            sb.AppendLine($"        break;");
            sb.AppendLine($"    case 3:");
            sb.AppendLine($"        {resultName} = \"Three\";");
            sb.AppendLine($"        break;");
            sb.AppendLine($"    default:");
            sb.AppendLine($"        {resultName} = \"Other\";");
            sb.AppendLine($"        break;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random switch expression
        /// </summary>
        public static string GenerateSwitchExpression(this FragmentGenerator generator)
        {
            Random random = new Random();
            string varName = generator.GenerateIdentifier("value");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Switch expression");
            sb.AppendLine($"int {varName} = {random.Next(1, 4)};");
            sb.AppendLine($"string {resultName} = {varName} switch");
            sb.AppendLine("{");
            sb.AppendLine($"    1 => \"One\",");
            sb.AppendLine($"    2 => \"Two\",");
            sb.AppendLine($"    3 => \"Three\",");
            sb.AppendLine($"    _ => \"Other\"");
            sb.AppendLine("};");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random while loop
        /// </summary>
        public static string GenerateWhileLoop(this FragmentGenerator generator)
        {
            Random random = new Random();
            string counterName = generator.GenerateIdentifier("counter");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// While loop");
            sb.AppendLine($"int {counterName} = 0;");
            sb.AppendLine($"int {resultName} = 0;");
            sb.AppendLine($"while ({counterName} < 5)");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} += {counterName};");
            sb.AppendLine($"    {counterName}++;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random do-while loop
        /// </summary>
        public static string GenerateDoWhileLoop(this FragmentGenerator generator)
        {
            Random random = new Random();
            string counterName = generator.GenerateIdentifier("counter");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Do-while loop");
            sb.AppendLine($"int {counterName} = 0;");
            sb.AppendLine($"int {resultName} = 0;");
            sb.AppendLine($"do");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} += {counterName};");
            sb.AppendLine($"    {counterName}++;");
            sb.AppendLine($"}} while ({counterName} < 5);");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random foreach loop
        /// </summary>
        public static string GenerateForeachLoop(this FragmentGenerator generator)
        {
            Random random = new Random();
            string arrayName = generator.GenerateIdentifier("array");
            string itemName = generator.GenerateIdentifier("item");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Foreach loop");
            sb.AppendLine($"int[] {arrayName} = new int[] {{ 1, 2, 3, 4, 5 }};");
            sb.AppendLine($"int {resultName} = 0;");
            sb.AppendLine($"foreach (int {itemName} in {arrayName})");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} += {itemName};");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random break statement
        /// </summary>
        public static string GenerateBreakStatement(this FragmentGenerator generator)
        {
            Random random = new Random();
            string counterName = generator.GenerateIdentifier("counter");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Break statement");
            sb.AppendLine($"int {counterName} = 0;");
            sb.AppendLine($"int {resultName} = 0;");
            sb.AppendLine($"while (true)");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} += {counterName};");
            sb.AppendLine($"    {counterName}++;");
            sb.AppendLine($"    if ({counterName} >= 5)");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        break;");
            sb.AppendLine($"    }}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random continue statement
        /// </summary>
        public static string GenerateContinueStatement(this FragmentGenerator generator)
        {
            Random random = new Random();
            string counterName = generator.GenerateIdentifier("counter");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Continue statement");
            sb.AppendLine($"int {counterName} = 0;");
            sb.AppendLine($"int {resultName} = 0;");
            sb.AppendLine($"while ({counterName} < 10)");
            sb.AppendLine("{");
            sb.AppendLine($"    {counterName}++;");
            sb.AppendLine($"    if ({counterName} % 2 == 0)");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        continue;");
            sb.AppendLine($"    }}");
            sb.AppendLine($"    {resultName} += {counterName};");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random goto statement
        /// </summary>
        public static string GenerateGotoStatement(this FragmentGenerator generator)
        {
            Random random = new Random();
            string counterName = generator.GenerateIdentifier("counter");
            string resultName = generator.GenerateIdentifier("result");
            string labelName = generator.GenerateIdentifier("label");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Goto statement");
            sb.AppendLine($"int {counterName} = 0;");
            sb.AppendLine($"int {resultName} = 0;");
            sb.AppendLine($"{resultName} += 10;");
            sb.AppendLine($"if ({resultName} > 5)");
            sb.AppendLine("{");
            sb.AppendLine($"    goto {labelName};");
            sb.AppendLine("}");
            sb.AppendLine($"{resultName} += 20;");
            sb.AppendLine($"{labelName}:");
            sb.AppendLine($"{resultName} += 5;");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random ternary conditional operator
        /// </summary>
        public static string GenerateTernaryOperator(this FragmentGenerator generator)
        {
            Random random = new Random();
            string conditionName = generator.GenerateIdentifier("condition");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Ternary conditional operator");
            sb.AppendLine($"bool {conditionName} = {generator.GenerateBooleanLiteral()};");
            sb.AppendLine($"string {resultName} = {conditionName} ? \"True\" : \"False\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random pattern matching statement
        /// </summary>
        public static string GeneratePatternMatching(this FragmentGenerator generator)
        {
            Random random = new Random();
            string objName = generator.GenerateIdentifier("obj");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Pattern matching");
            sb.AppendLine($"object {objName} = \"test\";");
            sb.AppendLine($"string {resultName};");
            sb.AppendLine($"if ({objName} is string text)");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} = $\"String with length {{text.Length}}\";");
            sb.AppendLine("}");
            sb.AppendLine($"else if ({objName} is int number)");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} = $\"Number: {{number}}\";");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} = \"Unknown type\";");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random property pattern matching
        /// </summary>
        public static string GeneratePropertyPatternMatching(this FragmentGenerator generator)
        {
            Random random = new Random();
            string objName = generator.GenerateIdentifier("obj");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Property pattern matching");
            sb.AppendLine($"// This would normally use a class with properties");
            sb.AppendLine($"// For testing, we'll use a tuple");
            sb.AppendLine($"(string Name, int Age) {objName} = (\"John\", 30);");
            sb.AppendLine($"string {resultName};");
            sb.AppendLine($"if ({objName} is {{ Name: \"John\", Age: 30 }})");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} = \"Exact match\";");
            sb.AppendLine("}");
            sb.AppendLine($"else if ({objName} is {{ Name: \"John\" }})");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} = \"Name match\";");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine($"    {resultName} = \"No match\";");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
