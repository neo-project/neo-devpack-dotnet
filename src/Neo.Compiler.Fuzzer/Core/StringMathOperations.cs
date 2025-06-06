using System;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle string and math operations
    /// </summary>
    public static class StringMathOperations
    {
        /// <summary>
        /// Generate random string operations
        /// </summary>
        public static string GenerateStringOperations(this FragmentGenerator generator)
        {
            Random random = new Random();
            string str = generator.GenerateIdentifier("str");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// String operations");
            sb.AppendLine($"string {str} = \"Hello, World!\";");
            sb.AppendLine($"int {result}1 = {str}.Length; // String length");
            sb.AppendLine($"string {result}2 = {str}.Substring(0, 5); // Substring");
            sb.AppendLine($"string {result}3 = {str}.ToUpper(); // ToUpper");
            sb.AppendLine($"string {result}4 = {str}.ToLower(); // ToLower");
            sb.AppendLine($"bool {result}5 = {str}.Contains(\"World\"); // Contains");
            sb.AppendLine($"bool {result}6 = {str}.StartsWith(\"Hello\"); // StartsWith");
            sb.AppendLine($"bool {result}7 = {str}.EndsWith(\"!\"); // EndsWith");
            sb.AppendLine($"int {result}8 = {str}.IndexOf(\"World\"); // IndexOf");
            sb.AppendLine($"string {result}9 = {str}.Replace(\"World\", \"Neo\"); // Replace");
            sb.AppendLine($"string {result}10 = {str}.Trim(); // Trim");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random string concatenation
        /// </summary>
        public static string GenerateStringConcatenation(this FragmentGenerator generator)
        {
            Random random = new Random();
            string str1 = generator.GenerateIdentifier("str");
            string str2 = generator.GenerateIdentifier("str");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// String concatenation");
            sb.AppendLine($"string {str1} = \"Hello\";");
            sb.AppendLine($"string {str2} = \"World\";");
            sb.AppendLine($"string {result}1 = {str1} + \", \" + {str2} + \"!\"; // Using + operator");
            sb.AppendLine($"string {result}2 = string.Concat({str1}, \", \", {str2}, \"!\"); // Using Concat");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random string interpolation
        /// </summary>
        public static string GenerateStringInterpolation(this FragmentGenerator generator)
        {
            Random random = new Random();
            string name = generator.GenerateIdentifier("name");
            string age = generator.GenerateIdentifier("age");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// String interpolation");
            sb.AppendLine($"string {name} = \"John\";");
            sb.AppendLine($"int {age} = 30;");
            sb.AppendLine($"string {result} = $\"My name is {{{name}}} and I am {{{age}}} years old.\";");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random string splitting
        /// </summary>
        public static string GenerateStringSplitting(this FragmentGenerator generator)
        {
            Random random = new Random();
            string str = generator.GenerateIdentifier("str");
            string parts = generator.GenerateIdentifier("parts");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// String splitting");
            sb.AppendLine($"string {str} = \"apple,banana,orange\";");
            sb.AppendLine($"string[] {parts} = {str}.Split(',');");
            sb.AppendLine($"string first = {parts}[0]; // \"apple\"");
            sb.AppendLine($"string second = {parts}[1]; // \"banana\"");
            sb.AppendLine($"string third = {parts}[2]; // \"orange\"");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random string joining
        /// </summary>
        public static string GenerateStringJoining(this FragmentGenerator generator)
        {
            Random random = new Random();
            string fruits = generator.GenerateIdentifier("fruits");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// String joining");
            sb.AppendLine($"string[] {fruits} = new string[] {{ \"apple\", \"banana\", \"orange\" }};");
            sb.AppendLine($"string {result} = string.Join(\", \", {fruits});");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random char operations
        /// </summary>
        public static string GenerateCharOperations(this FragmentGenerator generator)
        {
            Random random = new Random();
            string str = generator.GenerateIdentifier("str");
            string c = generator.GenerateIdentifier("c");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Char operations");
            sb.AppendLine($"string {str} = \"Hello\";");
            sb.AppendLine($"char {c} = {str}[0]; // 'H'");
            sb.AppendLine($"bool {result}1 = char.IsLetter({c}); // IsLetter");
            sb.AppendLine($"bool {result}2 = char.IsDigit({c}); // IsDigit");
            sb.AppendLine($"bool {result}3 = char.IsWhiteSpace({c}); // IsWhiteSpace");
            sb.AppendLine($"bool {result}4 = char.IsUpper({c}); // IsUpper");
            sb.AppendLine($"bool {result}5 = char.IsLower({c}); // IsLower");
            sb.AppendLine($"char {result}6 = char.ToUpper({c}); // ToUpper");
            sb.AppendLine($"char {result}7 = char.ToLower({c}); // ToLower");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random math operations
        /// </summary>
        public static string GenerateMathOperations(this FragmentGenerator generator)
        {
            Random random = new Random();
            string x = generator.GenerateIdentifier("x");
            string y = generator.GenerateIdentifier("y");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Math operations");
            sb.AppendLine($"int {x} = {random.Next(1, 100)};");
            sb.AppendLine($"int {y} = {random.Next(1, 100)};");
            sb.AppendLine($"int {result}1 = Math.Abs(-{x}); // Absolute value");
            sb.AppendLine($"int {result}2 = Math.Max({x}, {y}); // Maximum");
            sb.AppendLine($"int {result}3 = Math.Min({x}, {y}); // Minimum");
            sb.AppendLine($"int {result}4 = Math.Sign(-{x}); // Sign (-1, 0, or 1)");
            sb.AppendLine($"int {result}5 = Math.Clamp({x}, 10, 50); // Clamp value between min and max");

            return sb.ToString();
        }

        // BigInteger operations removed due to System.Numerics reference issues

        /// <summary>
        /// Generate random type conversion operations
        /// </summary>
        public static string GenerateTypeConversionOperations(this FragmentGenerator generator)
        {
            Random random = new Random();
            string value = generator.GenerateIdentifier("value");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Type conversion operations");
            sb.AppendLine($"int {value} = {random.Next(1, 100)};");
            sb.AppendLine($"long {result}1 = (long){value}; // Explicit casting int to long");
            sb.AppendLine($"short {result}2 = (short){value}; // Explicit casting int to short");
            sb.AppendLine($"byte {result}3 = (byte){value}; // Explicit casting int to byte");
            sb.AppendLine($"sbyte {result}4 = (sbyte){value}; // Explicit casting int to sbyte");
            sb.AppendLine($"uint {result}5 = (uint){value}; // Explicit casting int to uint");
            sb.AppendLine($"ulong {result}6 = (ulong){value}; // Explicit casting int to ulong");
            sb.AppendLine($"ushort {result}7 = (ushort){value}; // Explicit casting int to ushort");
            sb.AppendLine($"char {result}8 = (char){value}; // Explicit casting int to char");
            sb.AppendLine($"string {result}9 = {value}.ToString(); // Converting int to string");

            return sb.ToString();
        }

        /// <summary>
        /// Generate random ByteString operations
        /// </summary>
        public static string GenerateByteStringOperations(this FragmentGenerator generator)
        {
            Random random = new Random();
            string str = generator.GenerateIdentifier("str");
            string bytes = generator.GenerateIdentifier("bytes");
            string result = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// ByteString operations");
            sb.AppendLine($"string {str} = \"Hello\";");
            sb.AppendLine($"ByteString {bytes} = {str}.ToByteArray().ToByteString();");
            sb.AppendLine($"ByteString {result}1 = ByteString.Empty; // Empty ByteString");
            sb.AppendLine($"int {result}2 = {bytes}.Length; // ByteString length");
            sb.AppendLine($"byte {result}3 = {bytes}[0]; // Access ByteString element");
            sb.AppendLine($"// ByteString doesn't have a Range method in Neo.SmartContract.Framework");
            sb.AppendLine($"// ByteString {result}4 = {bytes}.Range(1, 3); // ByteString range");
            sb.AppendLine($"ByteString {result}5 = {bytes}.Concat({bytes}); // Concatenate ByteStrings");

            return sb.ToString();
        }
    }
}
