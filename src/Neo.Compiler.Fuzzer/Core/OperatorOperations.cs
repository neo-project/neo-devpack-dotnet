using System;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle operator operations
    /// </summary>
    public static class OperatorOperations
    {
        /// <summary>
        /// Generate random arithmetic operators
        /// </summary>
        public static string GenerateArithmeticOperators(this FragmentGenerator generator)
        {
            Random random = new Random();
            string x = generator.GenerateIdentifier("x");
            string y = generator.GenerateIdentifier("y");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Arithmetic operators");
            sb.AppendLine($"int {x} = {random.Next(1, 100)};");
            sb.AppendLine($"int {y} = {random.Next(1, 100)};");
            sb.AppendLine($"int {result}1 = {x} + {y}; // Addition");
            sb.AppendLine($"int {result}2 = {x} - {y}; // Subtraction");
            sb.AppendLine($"int {result}3 = {x} * {y}; // Multiplication");
            sb.AppendLine($"int {result}4 = {x} / {y}; // Division");
            sb.AppendLine($"int {result}5 = {x} % {y}; // Modulus");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random comparison operators
        /// </summary>
        public static string GenerateComparisonOperators(this FragmentGenerator generator)
        {
            Random random = new Random();
            string x = generator.GenerateIdentifier("x");
            string y = generator.GenerateIdentifier("y");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Comparison operators");
            sb.AppendLine($"int {x} = {random.Next(1, 100)};");
            sb.AppendLine($"int {y} = {random.Next(1, 100)};");
            sb.AppendLine($"bool {result}1 = {x} == {y}; // Equal to");
            sb.AppendLine($"bool {result}2 = {x} != {y}; // Not equal to");
            sb.AppendLine($"bool {result}3 = {x} < {y}; // Less than");
            sb.AppendLine($"bool {result}4 = {x} > {y}; // Greater than");
            sb.AppendLine($"bool {result}5 = {x} <= {y}; // Less than or equal to");
            sb.AppendLine($"bool {result}6 = {x} >= {y}; // Greater than or equal to");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random logical operators
        /// </summary>
        public static string GenerateLogicalOperators(this FragmentGenerator generator)
        {
            Random random = new Random();
            string a = generator.GenerateIdentifier("a");
            string b = generator.GenerateIdentifier("b");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Logical operators");
            sb.AppendLine($"bool {a} = {generator.GenerateBooleanLiteral()};");
            sb.AppendLine($"bool {b} = {generator.GenerateBooleanLiteral()};");
            sb.AppendLine($"bool {result}1 = {a} && {b}; // Logical AND");
            sb.AppendLine($"bool {result}2 = {a} || {b}; // Logical OR");
            sb.AppendLine($"bool {result}3 = !{a}; // Logical NOT");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random bitwise operators
        /// </summary>
        public static string GenerateBitwiseOperators(this FragmentGenerator generator)
        {
            Random random = new Random();
            string x = generator.GenerateIdentifier("x");
            string y = generator.GenerateIdentifier("y");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Bitwise operators");
            sb.AppendLine($"int {x} = {random.Next(1, 100)};");
            sb.AppendLine($"int {y} = {random.Next(1, 100)};");
            sb.AppendLine($"int {result}1 = {x} & {y}; // Bitwise AND");
            sb.AppendLine($"int {result}2 = {x} | {y}; // Bitwise OR");
            sb.AppendLine($"int {result}3 = {x} ^ {y}; // Bitwise XOR");
            sb.AppendLine($"int {result}4 = ~{x}; // Bitwise NOT");
            sb.AppendLine($"int {result}5 = {x} << 2; // Left shift");
            sb.AppendLine($"int {result}6 = {x} >> 2; // Right shift");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random assignment operators
        /// </summary>
        public static string GenerateAssignmentOperators(this FragmentGenerator generator)
        {
            Random random = new Random();
            string x = generator.GenerateIdentifier("x");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Assignment operators");
            sb.AppendLine($"int {x} = {random.Next(1, 100)};");
            sb.AppendLine($"{x} += 5; // Addition assignment");
            sb.AppendLine($"{x} -= 3; // Subtraction assignment");
            sb.AppendLine($"{x} *= 2; // Multiplication assignment");
            sb.AppendLine($"{x} /= 4; // Division assignment");
            sb.AppendLine($"{x} %= 3; // Modulus assignment");
            sb.AppendLine($"{x} &= 0xFF; // Bitwise AND assignment");
            sb.AppendLine($"{x} |= 0x0F; // Bitwise OR assignment");
            sb.AppendLine($"{x} ^= 0xF0; // Bitwise XOR assignment");
            sb.AppendLine($"{x} <<= 2; // Left shift assignment");
            sb.AppendLine($"{x} >>= 1; // Right shift assignment");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random increment and decrement operators
        /// </summary>
        public static string GenerateIncrementDecrementOperators(this FragmentGenerator generator)
        {
            Random random = new Random();
            string x = generator.GenerateIdentifier("x");
            string y = generator.GenerateIdentifier("y");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Increment and decrement operators");
            sb.AppendLine($"int {x} = {random.Next(1, 100)};");
            sb.AppendLine($"int {y};");
            sb.AppendLine($"{y} = {x}++; // Post-increment");
            sb.AppendLine($"{y} = ++{x}; // Pre-increment");
            sb.AppendLine($"{y} = {x}--; // Post-decrement");
            sb.AppendLine($"{y} = --{x}; // Pre-decrement");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random checked and unchecked expressions
        /// </summary>
        public static string GenerateCheckedUncheckedExpressions(this FragmentGenerator generator)
        {
            Random random = new Random();
            string x = generator.GenerateIdentifier("x");
            string result1 = generator.GenerateIdentifier("result");
            string result2 = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Checked and unchecked expressions");
            sb.AppendLine($"int {x} = {random.Next(1000, 2000)};");
            sb.AppendLine($"int {result1};");
            sb.AppendLine($"int {result2};");
            sb.AppendLine($"checked");
            sb.AppendLine("{");
            sb.AppendLine($"    // This would throw an exception if overflow occurs");
            sb.AppendLine($"    {result1} = {x} + 1000;");
            sb.AppendLine("}");
            sb.AppendLine($"unchecked");
            sb.AppendLine("{");
            sb.AppendLine($"    // This would wrap around if overflow occurs");
            sb.AppendLine($"    {result2} = {x} + 1000;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random sizeof operator usage
        /// </summary>
        public static string GenerateSizeofOperator(this FragmentGenerator generator)
        {
            Random random = new Random();
            string result = generator.GenerateIdentifier("size");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Sizeof operator");
            sb.AppendLine($"int {result}1 = sizeof(int); // Size of int");
            sb.AppendLine($"int {result}2 = sizeof(byte); // Size of byte");
            sb.AppendLine($"int {result}3 = sizeof(long); // Size of long");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random is and as operators
        /// </summary>
        public static string GenerateIsAsOperators(this FragmentGenerator generator)
        {
            Random random = new Random();
            string obj = generator.GenerateIdentifier("obj");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Is and as operators");
            sb.AppendLine($"object {obj} = \"test\";");
            sb.AppendLine($"bool {result}1 = {obj} is string; // Is operator");
            sb.AppendLine($"bool {result}2 = {obj} is int; // Is operator");
            sb.AppendLine($"string {result}3 = {obj} as string; // As operator");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random default literal expression
        /// </summary>
        public static string GenerateDefaultLiteralExpression(this FragmentGenerator generator)
        {
            Random random = new Random();
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Default literal expression");
            sb.AppendLine($"int {result}1 = default; // Default int (0)");
            sb.AppendLine($"bool {result}2 = default; // Default bool (false)");
            sb.AppendLine($"string {result}3 = default; // Default string (null)");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random out variable declaration
        /// </summary>
        public static string GenerateOutVariableDeclaration(this FragmentGenerator generator)
        {
            Random random = new Random();
            string input = generator.GenerateIdentifier("input");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Out variable declaration");
            sb.AppendLine($"string {input} = \"42\";");
            sb.AppendLine($"if (int.TryParse({input}, out int result))");
            sb.AppendLine("{");
            sb.AppendLine($"    // Use the result variable");
            sb.AppendLine($"    int doubled = result * 2;");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
