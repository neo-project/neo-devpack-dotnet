using System;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle exception operations
    /// </summary>
    public static class ExceptionOperations
    {
        /// <summary>
        /// Generate a random try-catch block
        /// </summary>
        public static string GenerateTryCatch(this FragmentGenerator generator)
        {
            Random random = new Random();
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Try-catch block");
            sb.AppendLine($"string {resultName};");
            sb.AppendLine($"try");
            sb.AppendLine("{");
            sb.AppendLine($"    // Code that might throw an exception");
            sb.AppendLine($"    {resultName} = \"Success\";");
            sb.AppendLine($"    if ({generator.GenerateBooleanLiteral()})");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        throw new Exception(\"Test exception\");");
            sb.AppendLine($"    }}");
            sb.AppendLine("}");
            sb.AppendLine($"catch (Exception ex)");
            sb.AppendLine("{");
            sb.AppendLine($"    // Handle the exception");
            sb.AppendLine($"    {resultName} = $\"Exception: {{ex.Message}}\";");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random try-catch-finally block
        /// </summary>
        public static string GenerateTryCatchFinally(this FragmentGenerator generator)
        {
            Random random = new Random();
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Try-catch-finally block");
            sb.AppendLine($"string {resultName};");
            sb.AppendLine($"try");
            sb.AppendLine("{");
            sb.AppendLine($"    // Code that might throw an exception");
            sb.AppendLine($"    {resultName} = \"Success\";");
            sb.AppendLine($"    if ({generator.GenerateBooleanLiteral()})");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        throw new Exception(\"Test exception\");");
            sb.AppendLine($"    }}");
            sb.AppendLine("}");
            sb.AppendLine($"catch (Exception ex)");
            sb.AppendLine("{");
            sb.AppendLine($"    // Handle the exception");
            sb.AppendLine($"    {resultName} = $\"Exception: {{ex.Message}}\";");
            sb.AppendLine("}");
            sb.AppendLine($"finally");
            sb.AppendLine("{");
            sb.AppendLine($"    // Cleanup code that always executes");
            sb.AppendLine($"    Runtime.Log(\"Finally block executed\");");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random throw statement
        /// </summary>
        public static string GenerateThrowStatement(this FragmentGenerator generator)
        {
            Random random = new Random();
            string conditionName = generator.GenerateIdentifier("condition");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Throw statement");
            sb.AppendLine($"bool {conditionName} = {generator.GenerateBooleanLiteral()};");
            sb.AppendLine($"if ({conditionName})");
            sb.AppendLine("{");
            sb.AppendLine($"    throw new Exception(\"Test exception\");");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random throw expression
        /// </summary>
        public static string GenerateThrowExpression(this FragmentGenerator generator)
        {
            Random random = new Random();
            string inputName = generator.GenerateIdentifier("input");
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Throw expression");
            sb.AppendLine($"string {inputName} = null;");
            sb.AppendLine($"string {resultName} = {inputName} ?? throw new Exception(\"Input is null\");");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random exception filter
        /// </summary>
        public static string GenerateExceptionFilter(this FragmentGenerator generator)
        {
            Random random = new Random();
            string resultName = generator.GenerateIdentifier("result");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Exception filter");
            sb.AppendLine($"string {resultName};");
            sb.AppendLine($"try");
            sb.AppendLine("{");
            sb.AppendLine($"    // Code that might throw an exception");
            sb.AppendLine($"    {resultName} = \"Success\";");
            sb.AppendLine($"    throw new Exception(\"Test exception\");");
            sb.AppendLine("}");
            sb.AppendLine($"catch (Exception ex) when (ex.Message.Contains(\"Test\"))");
            sb.AppendLine("{");
            sb.AppendLine($"    // Handle specific exceptions");
            sb.AppendLine($"    {resultName} = $\"Filtered exception: {{ex.Message}}\";");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
