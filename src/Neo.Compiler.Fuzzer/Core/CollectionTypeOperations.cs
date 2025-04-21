using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Provides enhanced operations for collection types like arrays, lists, maps, and dictionaries.
    /// This class implements the enhancements outlined in the implementation plan for Neo.Compiler.Fuzzer.
    /// </summary>
    public class CollectionTypeOperations
    {
        private readonly Random _random;
        private readonly FragmentGenerator _fragmentGenerator;

        public CollectionTypeOperations(FragmentGenerator fragmentGenerator, int? seed = null)
        {
            _fragmentGenerator = fragmentGenerator;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Generate a multi-dimensional array declaration and initialization
        /// </summary>
        public string GenerateMultiDimensionalArray()
        {
            string[] types = { "int", "string", "bool", "byte" };
            string type = types[_random.Next(types.Length)];
            string varName = _fragmentGenerator.GenerateIdentifier("array");

            // Decide between 2D and 3D array
            bool is3D = _random.Next(2) == 0;

            if (is3D)
            {
                int dim1 = _random.Next(1, 3);
                int dim2 = _random.Next(1, 3);
                int dim3 = _random.Next(1, 3);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{type}[,,] {varName} = new {type}[{dim1}, {dim2}, {dim3}];")
                  .AppendLine("// Initialize some elements");

                // Initialize a few random elements
                for (int i = 0; i < Math.Min(3, dim1 * dim2 * dim3); i++)
                {
                    int idx1 = _random.Next(dim1);
                    int idx2 = _random.Next(dim2);
                    int idx3 = _random.Next(dim3);
                    string value = GenerateValueForType(type);
                    sb.AppendLine($"{varName}[{idx1}, {idx2}, {idx3}] = {value};");
                }

                return sb.ToString();
            }
            else
            {
                int dim1 = _random.Next(1, 4);
                int dim2 = _random.Next(1, 4);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{type}[,] {varName} = new {type}[{dim1}, {dim2}];")
                  .AppendLine("// Initialize some elements");

                // Initialize a few random elements
                for (int i = 0; i < Math.Min(4, dim1 * dim2); i++)
                {
                    int idx1 = _random.Next(dim1);
                    int idx2 = _random.Next(dim2);
                    string value = GenerateValueForType(type);
                    sb.AppendLine($"{varName}[{idx1}, {idx2}] = {value};");
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Generate a jagged array declaration and initialization
        /// </summary>
        public string GenerateJaggedArray()
        {
            string[] types = { "int", "string", "bool", "byte" };
            string type = types[_random.Next(types.Length)];
            string varName = _fragmentGenerator.GenerateIdentifier("jagged");

            int outerLength = _random.Next(2, 4);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{type}[][] {varName} = new {type}[{outerLength}][];");

            // Initialize the inner arrays
            for (int i = 0; i < outerLength; i++)
            {
                int innerLength = _random.Next(1, 4);
                sb.AppendLine($"{varName}[{i}] = new {type}[{innerLength}];");

                // Initialize some elements in the inner array
                for (int j = 0; j < innerLength; j++)
                {
                    string value = GenerateValueForType(type);
                    sb.AppendLine($"{varName}[{i}][{j}] = {value};");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate enhanced List operations
        /// </summary>
        public string GenerateEnhancedListOperations()
        {
            string[] types = { "int", "string", "bool", "UInt160" };
            string type = types[_random.Next(types.Length)];
            string varName = _fragmentGenerator.GenerateIdentifier("list");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Enhanced List<{type}> operations")
              .AppendLine($"var {varName} = new Neo.SmartContract.Framework.List<{type}>();");

            // Add operations
            int itemCount = _random.Next(2, 5);
            for (int i = 0; i < itemCount; i++)
            {
                string value = GenerateValueForType(type);
                sb.AppendLine($"{varName}.Add({value});");
            }

            // Add more complex operations
            string[] operations = {
                $"// Check if list contains an element\nbool contains = {varName}.Contains({GenerateValueForType(type)});",
                $"// Get list count\nint count = {varName}.Count;",
                $"// Get element at index\nvar element = {varName}[{_random.Next(Math.Max(1, itemCount))}];",
                $"// Remove element at index\n{varName}.RemoveAt({_random.Next(Math.Max(1, itemCount))});",
                $"// Clear the list\n{varName}.Clear();",
                $"// Insert element at index\n{varName}.Insert({_random.Next(Math.Max(1, itemCount))}, {GenerateValueForType(type)});"
            };

            // Add 2-3 random operations
            int opCount = _random.Next(2, 4);
            for (int i = 0; i < opCount; i++)
            {
                sb.AppendLine(operations[_random.Next(operations.Length)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate enhanced Map operations
        /// </summary>
        public string GenerateEnhancedMapOperations()
        {
            string[] keyTypes = { "string", "int", "UInt160" };
            string[] valueTypes = { "string", "int", "bool", "UInt160", "ByteString" };

            string keyType = keyTypes[_random.Next(keyTypes.Length)];
            string valueType = valueTypes[_random.Next(valueTypes.Length)];
            string varName = _fragmentGenerator.GenerateIdentifier("map");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Enhanced Map<{keyType}, {valueType}> operations")
              .AppendLine($"var {varName} = new Map<{keyType}, {valueType}>();");

            // Add key-value pairs
            int pairCount = _random.Next(2, 5);
            for (int i = 0; i < pairCount; i++)
            {
                string key = GenerateValueForType(keyType);
                string value = GenerateValueForType(valueType);
                sb.AppendLine($"{varName}[{key}] = {value};");
            }

            // Add more complex operations
            string[] operations = {
                $"// Check if map contains a key\nbool containsKey = {varName}.ContainsKey({GenerateValueForType(keyType)});",
                $"// Get map count\nint count = {varName}.Count;",
                $"// Remove a key\n{varName}.Remove({GenerateValueForType(keyType)});",
                $"// Clear the map\n{varName}.Clear();",
                $"// Get keys\nvar keys = {varName}.Keys;",
                $"// Get values\nvar values = {varName}.Values;"
            };

            // Add 2-3 random operations
            int opCount = _random.Next(2, 4);
            for (int i = 0; i < opCount; i++)
            {
                sb.AppendLine(operations[_random.Next(operations.Length)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate Dictionary operations
        /// </summary>
        public string GenerateDictionaryOperations()
        {
            string[] keyTypes = { "string", "int", "UInt160" };
            string[] valueTypes = { "string", "int", "bool", "UInt160", "ByteString" };

            string keyType = keyTypes[_random.Next(keyTypes.Length)];
            string valueType = valueTypes[_random.Next(valueTypes.Length)];
            string varName = _fragmentGenerator.GenerateIdentifier("dict");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Dictionary<{keyType}, {valueType}> operations")
              .AppendLine($"var {varName} = new Dictionary<{keyType}, {valueType}>();");

            // Add key-value pairs
            int pairCount = _random.Next(2, 5);
            for (int i = 0; i < pairCount; i++)
            {
                string key = GenerateValueForType(keyType);
                string value = GenerateValueForType(valueType);
                sb.AppendLine($"{varName}.Add({key}, {value});");
            }

            // Add more complex operations
            string[] operations = {
                $"// Check if dictionary contains a key\nbool containsKey = {varName}.ContainsKey({GenerateValueForType(keyType)});",
                $"// Get dictionary count\nint count = {varName}.Count;",
                $"// Remove a key\n{varName}.Remove({GenerateValueForType(keyType)});",
                $"// Clear the dictionary\n{varName}.Clear();",
                $"// Get value with TryGetValue\n{valueType} value; bool success = {varName}.TryGetValue({GenerateValueForType(keyType)}, out value);",
                $"// Access by key\nvar item = {varName}[{GenerateValueForType(keyType)}];"
            };

            // Add 2-3 random operations
            int opCount = _random.Next(2, 4);
            for (int i = 0; i < opCount; i++)
            {
                sb.AppendLine(operations[_random.Next(operations.Length)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate a random value for a given type
        /// </summary>
        private string GenerateValueForType(string type)
        {
            switch (type)
            {
                case "int":
                    return _random.Next(-100, 100).ToString();
                case "string":
                    return $"\"{_fragmentGenerator.GenerateStringLiteral(5).Replace("\"", "")}\"";
                case "bool":
                    return _random.Next(2) == 0 ? "false" : "true";
                case "byte":
                    return _random.Next(256).ToString();
                case "UInt160":
                    return "UInt160.Zero";
                case "ByteString":
                    return "ByteString.Empty";
                default:
                    return "default";
            }
        }

        /// <summary>
        /// Generate a random collection operation
        /// </summary>
        public string GenerateCollectionOperation()
        {
            string[] operations = {
                GenerateMultiDimensionalArray(),
                GenerateJaggedArray(),
                GenerateEnhancedListOperations(),
                GenerateEnhancedMapOperations(),
                GenerateDictionaryOperations()
            };

            return operations[_random.Next(operations.Length)];
        }
    }
}
