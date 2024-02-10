using Neo.SmartContract.Manifest;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.TestEngine
{
    public class Artifacts
    {
        /// <summary>
        /// Create source code from contract Abi
        /// </summary>
        /// <param name="name">Contract name</param>
        /// <param name="abi">Abi</param>
        /// <returns>Source</returns>
        public static string CreateSourceFromManifest(string name, ContractAbi abi)
        {
            StringBuilder sourceCode = new();

            sourceCode.AppendLine("using Neo");
            sourceCode.AppendLine("using System.Numerics;");
            sourceCode.AppendLine("");
            sourceCode.AppendLine("namespace Neo.TestEngine.Contracts;");
            sourceCode.AppendLine("");
            sourceCode.AppendLine($"public abstract class {name} : Neo.SmartContract.TestEngine.Mocks.SmartContract");
            sourceCode.AppendLine("{");

            // Create constructor

            sourceCode.AppendLine("#region Constructor for internal use only");
            sourceCode.AppendLine($"    internal {name}(Neo.SmartContract.TestEngine.Mocks.SmartContract.TestEngine testEngine) : base(testEngine) {{}}");
            sourceCode.AppendLine("#endregion");

            // Crete methods

            if (abi.Methods.Any(u => u.Safe))
            {
                sourceCode.AppendLine("#region Safe methods");

                foreach (var method in abi.Methods.Where(u => u.Safe).OrderBy(u => u.Name))
                {
                    // This method can't be called, so avoid them

                    if (method.Name.StartsWith("_")) continue;

                    sourceCode.Append(CreateSourceMethodFromManifest(method));
                }

                sourceCode.AppendLine("#endregion");
            }

            if (abi.Methods.Any(u => !u.Safe))
            {
                sourceCode.AppendLine("#region Unsafe methods");

                foreach (var method in abi.Methods.Where(u => !u.Safe).OrderBy(u => u.Name))
                {
                    // This method can't be called, so avoid them

                    if (method.Name.StartsWith("_")) continue;

                    sourceCode.Append(CreateSourceMethodFromManifest(method));
                }
                sourceCode.AppendLine("#endregion");
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
