using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Testing.Extensions
{
    public static class ArtifactExtensions
    {
        /// <summary>
        /// Get source code from contract Abi
        /// </summary>
        /// <param name="abi">Abi</param>
        /// <param name="name">Contract name</param>
        /// <param name="generateProperties">Generate properties</param>
        /// <returns>Source</returns>
        public static string GetArtifactsSource(this ContractAbi abi, string name, bool generateProperties = true)
        {
            StringBuilder sourceCode = new();

            sourceCode.AppendLine("using Neo.Cryptography.ECC;");
            sourceCode.AppendLine("using System.Collections.Generic;");
            sourceCode.AppendLine("using System.ComponentModel;");
            sourceCode.AppendLine("using System.Numerics;");
            sourceCode.AppendLine("");
            sourceCode.AppendLine("namespace Neo.SmartContract.Testing;");
            sourceCode.AppendLine("");
            sourceCode.AppendLine($"public abstract class {name} : Neo.SmartContract.Testing.SmartContract");
            sourceCode.AppendLine("{");

            // Crete events

            if (abi.Events.Any())
            {
                sourceCode.AppendLine("    #region Events");

                foreach (var ev in abi.Events.OrderBy(u => u.Name))
                {
                    sourceCode.Append(CreateSourceEventFromManifest(ev));
                }

                sourceCode.AppendLine("    #endregion");
            }

            // Create methods

            var methods = abi.Methods;

            if (generateProperties)
            {
                (methods, var properties) = ProcessAbiMethods(abi.Methods);

                if (properties.Any())
                {
                    sourceCode.AppendLine("    #region Properties");

                    foreach (var property in properties.OrderBy(u => u.getter.Name))
                    {
                        sourceCode.Append(CreateSourcePropertyFromManifest(property.getter, property.setter));
                    }

                    sourceCode.AppendLine("    #endregion");
                }
            }

            if (methods.Any(u => u.Safe))
            {
                sourceCode.AppendLine("    #region Safe methods");

                foreach (var method in methods.Where(u => u.Safe).OrderBy(u => u.Name))
                {
                    // This method can't be called, so avoid them

                    if (method.Name.StartsWith("_")) continue;

                    sourceCode.Append(CreateSourceMethodFromManifest(method));
                }

                sourceCode.AppendLine("    #endregion");
            }

            if (methods.Any(u => !u.Safe))
            {
                sourceCode.AppendLine("    #region Unsafe methods");

                foreach (var method in methods.Where(u => !u.Safe).OrderBy(u => u.Name))
                {
                    // This method can't be called, so avoid them

                    if (method.Name.StartsWith("_")) continue;

                    sourceCode.Append(CreateSourceMethodFromManifest(method));
                }
                sourceCode.AppendLine("    #endregion");
            }

            // Create constructor

            sourceCode.AppendLine("    #region Constructor for internal use only");
            sourceCode.AppendLine($"    protected {name}(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {{}}");
            sourceCode.AppendLine("    #endregion");

            sourceCode.AppendLine("}");

            if (Environment.NewLine.Length == 2)
            {
                return sourceCode.ToString().Replace("\r\n", "\n").Trim();
            }

            return sourceCode.ToString().TrimEnd();
        }

        private static (ContractMethodDescriptor[] methods, (ContractMethodDescriptor getter, ContractMethodDescriptor? setter)[] properties)
            ProcessAbiMethods(ContractMethodDescriptor[] methods)
        {
            List<ContractMethodDescriptor> methodList = new(methods);
            List<(ContractMethodDescriptor, ContractMethodDescriptor?)> properties = new();

            // Detect and extract properties, first find Safe && 0 args && return != void

            foreach (ContractMethodDescriptor getter in methods.Where(u => u.Safe && u.Parameters.Length == 0 && u.ReturnType != ContractParameterType.Void).ToArray())
            {
                // Find setter: setXXX && one arg && not safe && parameter = getter.return && return == void

                var setter = getter.Name.StartsWith("get") ? // Only find setter if start with get
                    methodList.FirstOrDefault(
                    u =>
                        u.Name == "set" + getter.Name[3..] &&
                        !u.Safe &&
                        u.Parameters.Length == 1 &&
                        u.Parameters[0].Type == getter.ReturnType &&
                        u.ReturnType == ContractParameterType.Void
                        ) : null;

                properties.Add((getter, setter));
                methodList.Remove(getter);

                if (setter != null)
                {
                    methodList.Remove(setter);
                }
            }

            return (methodList.ToArray(), properties.ToArray());
        }

        /// <summary>
        /// Create source code from event
        /// </summary>
        /// <param name="ev">Event</param>
        /// <returns>Source</returns>
        private static string CreateSourceEventFromManifest(ContractEventDescriptor ev)
        {
            StringBuilder sourceCode = new();

            sourceCode.Append($"    public delegate void del{ev.Name}(");

            bool isFirst = true;
            foreach (var arg in ev.Parameters)
            {
                if (!isFirst) sourceCode.Append(", ");
                else isFirst = false;

                sourceCode.Append($"{TypeToSource(arg.Type)} {EscapeName(arg.Name)}");
            }

            sourceCode.AppendLine(");");
            sourceCode.AppendLine($"    public event del{ev.Name}? {ev.Name};");

            return sourceCode.ToString();
        }

        /// <summary>
        /// Create source code from manifest property
        /// </summary>
        /// <param name="getter">Getter</param>
        /// <param name="setter">Setter</param>
        /// <returns>Source</returns>
        private static string CreateSourcePropertyFromManifest(ContractMethodDescriptor getter, ContractMethodDescriptor? setter)
        {
            var propertyName = getter.Name.StartsWith("get") ? getter.Name[3..] : getter.Name;
            var getset = setter is not null ? $"{{ [DisplayName(\"{getter.Name}\")] get; [DisplayName(\"{setter.Name}\")] set; }}" : $"{{ [DisplayName(\"{getter.Name}\")] get; }}";

            StringBuilder sourceCode = new();
            sourceCode.AppendLine($"    public abstract {TypeToSource(getter.ReturnType)} {EscapeName(propertyName)} {getset}");

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

            sourceCode.AppendLine($"    /// <summary>");
            sourceCode.AppendLine($"    /// {(method.Safe ? "Safe method" : "Unsafe method")}");
            sourceCode.AppendLine($"    /// </summary>");
            sourceCode.Append($"    public abstract {TypeToSource(method.ReturnType)} {EscapeName(method.Name)}(");

            bool isFirst = true;
            foreach (var arg in method.Parameters)
            {
                if (!isFirst) sourceCode.Append(", ");
                else isFirst = false;

                sourceCode.Append($"{TypeToSource(arg.Type)} {EscapeName(arg.Name)}");
            }

            sourceCode.AppendLine(");");

            return sourceCode.ToString();
        }

        /// <summary>
        /// Escape name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Escaped name</returns>
        private static string EscapeName(string name)
        {
            return name switch
            {
                "base" => "@" + name,
                "lock" => "@" + name,
                "params" => "@" + name,
                "struct" => "@" + name,
                "class" => "@" + name,

                _ => name
            };
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
