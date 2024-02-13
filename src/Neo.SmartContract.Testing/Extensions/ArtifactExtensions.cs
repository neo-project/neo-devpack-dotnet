using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Testing.Extensions
{
    public static class ArtifactExtensions
    {
        static readonly string[] _protectedWords = new string[] {
            "abstract", "as", "base", "bool", "break", "byte",
            "case", "catch", "char", "checked", "class", "const",
            "continue", "decimal", "default", "delegate", "do", "double",
            "else", "enum", "event", "explicit", "extern", "false",
            "finally", "fixed", "float", "for", "foreach", "goto",
            "if", "implicit", "in", "int", "interface", "internal",
            "is", "lock", "long", "namespace", "new", "null",
            "object", "operator", "out", "override", "params", "private",
            "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true", "try",
            "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
            "using", "virtual", "void", "volatile", "while"
        };

        /// <summary>
        /// Get source code from contract Abi
        /// </summary>
        /// <param name="abi">Abi</param>
        /// <param name="name">Contract name</param>
        /// <param name="generateProperties">Generate properties</param>
        /// <returns>Source</returns>
        public static string GetArtifactsSource(this ContractAbi abi, string name, bool generateProperties = true)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };

            sourceCode.WriteLine("using Neo.Cryptography.ECC;");
            sourceCode.WriteLine("using System.Collections.Generic;");
            sourceCode.WriteLine("using System.ComponentModel;");
            sourceCode.WriteLine("using System.Numerics;");
            sourceCode.WriteLine("");
            sourceCode.WriteLine("namespace Neo.SmartContract.Testing;");
            sourceCode.WriteLine("");
            sourceCode.WriteLine($"public abstract class {name} : Neo.SmartContract.Testing.SmartContract");
            sourceCode.WriteLine("{");

            // Crete events

            if (abi.Events.Any())
            {
                sourceCode.WriteLine("    #region Events");

                foreach (var ev in abi.Events.OrderBy(u => u.Name))
                {
                    sourceCode.Write(CreateSourceEventFromManifest(ev));
                }

                sourceCode.WriteLine("    #endregion");
            }

            // Create methods

            var methods = abi.Methods;

            if (generateProperties)
            {
                (methods, var properties) = ProcessAbiMethods(abi.Methods);

                if (properties.Any())
                {
                    sourceCode.WriteLine("    #region Properties");

                    foreach (var property in properties.OrderBy(u => u.getter.Name))
                    {
                        sourceCode.Write(CreateSourcePropertyFromManifest(property.getter, property.setter));
                    }

                    sourceCode.WriteLine("    #endregion");
                }
            }

            if (methods.Any(u => u.Safe))
            {
                sourceCode.WriteLine("    #region Safe methods");

                foreach (var method in methods.Where(u => u.Safe).OrderBy(u => u.Name))
                {
                    // This method can't be called, so avoid them

                    if (method.Name.StartsWith("_")) continue;

                    sourceCode.Write(CreateSourceMethodFromManifest(method));
                }

                sourceCode.WriteLine("    #endregion");
            }

            if (methods.Any(u => !u.Safe))
            {
                sourceCode.WriteLine("    #region Unsafe methods");

                foreach (var method in methods.Where(u => !u.Safe).OrderBy(u => u.Name))
                {
                    // This method can't be called, so avoid them

                    if (method.Name.StartsWith("_")) continue;

                    sourceCode.Write(CreateSourceMethodFromManifest(method));
                }
                sourceCode.WriteLine("    #endregion");
            }

            // Create constructor

            sourceCode.WriteLine("    #region Constructor for internal use only");
            sourceCode.WriteLine($"    protected {name}(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) {{ }}");
            sourceCode.WriteLine("    #endregion");

            sourceCode.WriteLine("}");

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
            var evName = TongleLowercase(EscapeName(ev.Name));
            if (!evName.StartsWith("On")) evName = "On" + evName;

            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };

            sourceCode.Write($"    public delegate void del{ev.Name}(");

            var isFirst = true;
            foreach (var arg in ev.Parameters)
            {
                if (!isFirst) sourceCode.Write(", ");
                else isFirst = false;

                sourceCode.Write($"{TypeToSource(arg.Type)} {EscapeName(arg.Name)}");
            }

            sourceCode.WriteLine(");");
            if (ev.Name != evName)
            {
                sourceCode.WriteLine($"    [DisplayName(\"{ev.Name}\")]");
            }
            sourceCode.WriteLine($"    public event del{ev.Name}? {evName};");

            return builder.ToString();
        }

        /// <summary>
        /// Create source code from manifest property
        /// </summary>
        /// <param name="getter">Getter</param>
        /// <param name="setter">Setter</param>
        /// <returns>Source</returns>
        private static string CreateSourcePropertyFromManifest(ContractMethodDescriptor getter, ContractMethodDescriptor? setter)
        {
            var propertyName = TongleLowercase(EscapeName(getter.Name.StartsWith("get") ? getter.Name[3..] : getter.Name));
            var getset = setter is not null ? $"{{ [DisplayName(\"{getter.Name}\")] get; [DisplayName(\"{setter.Name}\")] set; }}" : $"{{ [DisplayName(\"{getter.Name}\")] get; }}";

            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };
            sourceCode.WriteLine($"    public abstract {TypeToSource(getter.ReturnType)} {propertyName} {getset}");

            return builder.ToString();
        }

        /// <summary>
        /// Create source code from manifest method
        /// </summary>
        /// <param name="method">Contract method</param>
        /// <returns>Source</returns>
        private static string CreateSourceMethodFromManifest(ContractMethodDescriptor method)
        {
            var methodName = TongleLowercase(EscapeName(method.Name));

            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };

            sourceCode.WriteLine($"    /// <summary>");
            sourceCode.WriteLine($"    /// {(method.Safe ? "Safe method" : "Unsafe method")}");
            sourceCode.WriteLine($"    /// </summary>");
            if (method.Name != methodName)
            {
                sourceCode.WriteLine($"    [DisplayName(\"{method.Name}\")]");
            }
            sourceCode.Write($"    public abstract {TypeToSource(method.ReturnType)} {methodName}(");

            var isFirst = true;
            for (int x = 0; x < method.Parameters.Length; x++)
            {
                if (!isFirst) sourceCode.Write(", ");
                else isFirst = false;

                var isLast = x == method.Parameters.Length - 1;
                var arg = method.Parameters[x];

                if (isLast && arg.Type == ContractParameterType.Any)
                {
                    // it will be object X, we can add a default value

                    sourceCode.Write($"{TypeToSource(arg.Type)}? {EscapeName(arg.Name)} = null");
                }
                else
                {
                    sourceCode.Write($"{TypeToSource(arg.Type)} {EscapeName(arg.Name)}");
                }
            }


            sourceCode.WriteLine(");");

            return builder.ToString();
        }

        private static string TongleLowercase(string value)
        {
            if (value.Length == 0)
            {
                return value;
            }

            if (char.IsLower(value[0]))
            {
                return value[0].ToString().ToUpperInvariant() + value[1..];
            }

            return value;
        }

        /// <summary>
        /// Escape name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Escaped name</returns>
        private static string EscapeName(string name)
        {
            if (_protectedWords.Contains(name))
                return "@" + name;

            return name;
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
                ContractParameterType.Array => "IList<object>",
                ContractParameterType.Map => "IDictionary<object, object>",
                ContractParameterType.Void => "void",
                _ => "object",
            };
        }
    }
}
