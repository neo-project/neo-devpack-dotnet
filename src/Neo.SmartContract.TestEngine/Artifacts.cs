using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Neo.SmartContract.Manifest;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.SmartContract.TestEngine
{
    public class Artifacts
    {
        /// <summary>
        /// Create artifacts for NefFile
        /// </summary>
        /// <param name="manifest">Contract manifest</param>
        /// <param name="outputPath">Ouptut path for artifacts</param>
        public static void CreateArtifacts(ContractManifest manifest, string outputPath)
        {
            var dependencies = new string[] {
                Path.GetFullPath(typeof(Artifacts).Assembly.Location),
                Path.GetFullPath(typeof(UInt160).Assembly.Location),
                Path.GetFullPath(typeof(BigInteger).Assembly.Location)
            };

            // Compose source code
            var sourceCode = CreateSourceFromManifest(manifest);

            // Create a C# Code Provider
            CSharpCodeProvider codeProvider = new();

            // Set compiler parameters
            CompilerParameters parameters = new()
            {
                GenerateExecutable = false, // We want a DLL, not an executable
                OutputAssembly = outputPath // Set the output path for the DLL
            };

            // Add referenced assemblies, if any
            foreach (var dependency in dependencies)
            {
                parameters.ReferencedAssemblies.Add(dependency);
            }

            // Compile the code
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, sourceCode);

            // Check for compilation errors
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine("Error ({0}): {1}", error.ErrorNumber, error.ErrorText);
                }
                throw new InvalidOperationException("Compilation failed with errors.");
            }
        }

        /// <summary>
        /// Create source code from manifest
        /// </summary>
        /// <param name="manifest">Manifest</param>
        /// <returns>Source</returns>
        public static string CreateSourceFromManifest(ContractManifest manifest)
        {
            StringBuilder sourceCode = new();

            sourceCode.AppendLine("using Neo");
            sourceCode.AppendLine("using System.Numerics;");
            sourceCode.AppendLine("");
            sourceCode.AppendLine("namespace Neo.TestEngine.Contracts;");
            sourceCode.AppendLine("");
            sourceCode.AppendLine($"public class {manifest.Name} : Neo.SmartContract.TestEngine.Mocks.SmartContract");
            sourceCode.AppendLine("{");

            // Create constructor

            sourceCode.AppendLine($"    internal {manifest.Name}(Neo.SmartContract.TestEngine.Mocks.SmartContract.TestEngine testEngine) : base(testEngine) {{}}");

            // Crete methods

            foreach (var method in manifest.Abi.Methods)
            {
                // This method can't be called, so avoid them

                if (method.Name.StartsWith("_")) continue;

                sourceCode.Append(CreateSourceMethodFromManifest(method));
            }

            sourceCode.AppendLine("}");

            return sourceCode.ToString();
        }

        /// <summary>
        /// Create source code from manifest method
        /// </summary>
        /// <param name="method">Contract method</param>
        /// <returns>Source</returns>
        private static string CreateSourceMethodFromManifest(ContractMethodDescriptor method)
        {
            StringBuilder sourceCode = new();

            sourceCode.Append($"    public abstract {TypeToSource(method.ReturnType)} {method.Name} (");

            bool isFirst = true;
            foreach (var arg in method.Parameters)
            {
                if (!isFirst) sourceCode.Append(", ");
                else isFirst = false;

                sourceCode.Append($"{TypeToSource(arg.Type)} {arg.Name}");
            }

            sourceCode.AppendLine(");");

            return sourceCode.ToString();
        }

        /// <summary>
        /// Type to source
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>c# type</returns>
        private static string TypeToSource(ContractParameterType type)
        {
            return type switch
            {
                ContractParameterType.Boolean => "bool",
                ContractParameterType.Integer => "BigInteger",
                ContractParameterType.String => "string",
                ContractParameterType.Hash160 => "UInt160",
                ContractParameterType.Hash256 => "UInt256",
                ContractParameterType.PublicKey => "ECPoint",
                ContractParameterType.ByteArray => "byte[]",
                ContractParameterType.Signature => "byte[]",
                ContractParameterType.Array => "List<object>",
                ContractParameterType.Map => "IDictionary<object,object>",
                ContractParameterType.Void => "void",
                _ => "object",
            };
        }
    }
}
