using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.SmartContract.Fuzzer.StaticAnalysis
{
    /// <summary>
    /// Static analyzer for C# smart contract source code using Roslyn.
    /// </summary>
    public class CSharpStaticAnalyzer : IStaticAnalyzer
    {
        private readonly string _sourcePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpStaticAnalyzer"/> class.
        /// </summary>
        /// <param name="sourcePath">Path to the C# source file.</param>
        public CSharpStaticAnalyzer(string sourcePath)
        {
            _sourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
        }

        /// <summary>
        /// Analyzes the C# source code and returns hints for fuzzing.
        /// </summary>
        /// <returns>A collection of static analysis hints.</returns>
        public IEnumerable<StaticAnalysisHint> Analyze()
        {
            if (!File.Exists(_sourcePath))
                throw new FileNotFoundException($"Source file not found: {_sourcePath}");

            string sourceCode = File.ReadAllText(_sourcePath);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            var hints = new List<StaticAnalysisHint>();

            // Find all method declarations
            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var method in methodDeclarations)
            {
                // Check for methods with parameters
                if (method.ParameterList.Parameters.Count > 0)
                {
                    hints.Add(new StaticAnalysisHint
                    {
                        FilePath = _sourcePath,
                        LineNumber = GetLineNumber(method, syntaxTree),
                        RiskType = "MethodWithParameters",
                        Description = $"Method {method.Identifier.Text} has {method.ParameterList.Parameters.Count} parameters",
                        Priority = 50,
                        MethodName = method.Identifier.Text
                    });

                    // Check for integer parameters (potential overflow/underflow)
                    foreach (var parameter in method.ParameterList.Parameters)
                    {
                        if (IsIntegerType(parameter.Type))
                        {
                            hints.Add(new StaticAnalysisHint
                            {
                                FilePath = _sourcePath,
                                LineNumber = GetLineNumber(parameter, syntaxTree),
                                RiskType = "IntegerParameter",
                                Description = $"Integer parameter {parameter.Identifier.Text} in method {method.Identifier.Text} (potential overflow/underflow)",
                                Priority = 70,
                                MethodName = method.Identifier.Text,
                                ParameterName = parameter.Identifier.Text
                            });
                        }
                    }
                }

                // Check for storage operations
                var storageAccesses = method.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(IsStorageOperation);

                foreach (var storageAccess in storageAccesses)
                {
                    hints.Add(new StaticAnalysisHint
                    {
                        FilePath = _sourcePath,
                        LineNumber = GetLineNumber(storageAccess, syntaxTree),
                        RiskType = "StorageOperation",
                        Description = $"Storage operation in method {method.Identifier.Text}",
                        Priority = 60,
                        MethodName = method.Identifier.Text
                    });
                }

                // Check for arithmetic operations
                var binaryExpressions = method.DescendantNodes()
                    .OfType<BinaryExpressionSyntax>()
                    .Where(IsArithmeticOperation);

                foreach (var binaryExpression in binaryExpressions)
                {
                    hints.Add(new StaticAnalysisHint
                    {
                        FilePath = _sourcePath,
                        LineNumber = GetLineNumber(binaryExpression, syntaxTree),
                        RiskType = "ArithmeticOperation",
                        Description = $"Arithmetic operation ({binaryExpression.OperatorToken.Text}) in method {method.Identifier.Text} (potential overflow/underflow)",
                        Priority = 65,
                        MethodName = method.Identifier.Text
                    });
                }

                // Check for witness checks
                var witnessChecks = method.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(IsWitnessCheck);

                if (!witnessChecks.Any() && HasStorageWrite(method))
                {
                    hints.Add(new StaticAnalysisHint
                    {
                        FilePath = _sourcePath,
                        LineNumber = GetLineNumber(method, syntaxTree),
                        RiskType = "MissingWitnessCheck",
                        Description = $"Method {method.Identifier.Text} writes to storage but has no witness check",
                        Priority = 90,
                        MethodName = method.Identifier.Text
                    });
                }
            }

            return hints;
        }

        private static int GetLineNumber(SyntaxNode node, SyntaxTree syntaxTree)
        {
            return syntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
        }

        private static bool IsIntegerType(TypeSyntax? type)
        {
            if (type == null)
                return false;

            string typeName = type.ToString();
            return typeName == "int" || typeName == "uint" || typeName == "long" || typeName == "ulong" ||
                   typeName == "byte" || typeName == "sbyte" || typeName == "short" || typeName == "ushort" ||
                   typeName == "BigInteger" || typeName == "System.Numerics.BigInteger";
        }

        private static bool IsStorageOperation(InvocationExpressionSyntax invocation)
        {
            string? methodName = invocation.Expression.ToString();
            return methodName.Contains("Storage.Put") || methodName.Contains("Storage.Get") ||
                   methodName.Contains("Storage.Delete") || methodName.Contains("Storage.Find");
        }

        private static bool IsArithmeticOperation(BinaryExpressionSyntax binary)
        {
            return binary.Kind() == SyntaxKind.AddExpression ||
                   binary.Kind() == SyntaxKind.SubtractExpression ||
                   binary.Kind() == SyntaxKind.MultiplyExpression ||
                   binary.Kind() == SyntaxKind.DivideExpression;
        }

        private static bool IsWitnessCheck(InvocationExpressionSyntax invocation)
        {
            string? methodName = invocation.Expression.ToString();
            return methodName.Contains("Runtime.CheckWitness") || methodName.Contains("Runtime.GetCallingScriptHash");
        }

        private static bool HasStorageWrite(MethodDeclarationSyntax method)
        {
            return method.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Any(invocation =>
                {
                    string? methodName = invocation.Expression.ToString();
                    return methodName.Contains("Storage.Put") || methodName.Contains("Storage.Delete");
                });
        }
    }
}
