using Neo.VM.Types;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Represents a test case for fuzzing a smart contract method.
    /// </summary>
    public class TestCase
    {
        /// <summary>
        /// Gets or sets the name of the method being tested.
        /// </summary>
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parameters for the method call.
        /// </summary>
        [JsonIgnore]
        public StackItem[] Parameters { get; set; } = System.Array.Empty<StackItem>();

        /// <summary>
        /// Gets or sets the serialized parameters for JSON serialization.
        /// </summary>
        public string[] SerializedParameters
        {
            get => Parameters.Select(p => ContractExecutor.ConvertStackItemToJson(p).ToString()).ToArray();
            set
            {
                if (value == null)
                {
                    Parameters = System.Array.Empty<StackItem>();
                    return;
                }

                Parameters = new StackItem[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    try
                    {
                        using JsonDocument doc = JsonDocument.Parse(value[i]);
                        Parameters[i] = ContractExecutor.ConvertJsonElementToStackItem(doc.RootElement, "Any");
                    }
                    catch
                    {
                        // If parsing fails, use the string as is
                        Parameters[i] = new ByteString(System.Text.Encoding.UTF8.GetBytes(value[i]));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the energy value for this test case.
        /// Higher energy values indicate more interesting test cases.
        /// </summary>
        public double Energy { get; set; } = 1.0;

        /// <summary>
        /// Gets or sets the iteration number when this test case was generated.
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when this test case was created.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Creates a clone of this test case.
        /// </summary>
        /// <returns>A new instance of <see cref="TestCase"/> with the same values.</returns>
        public TestCase Clone()
        {
            return new TestCase
            {
                MethodName = MethodName,
                Parameters = Parameters.Select(p => p.DeepCopy()).ToArray(),
                Energy = Energy,
                Iteration = Iteration,
                Timestamp = Timestamp
            };
        }

        /// <summary>
        /// Converts the test case to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the test case.</returns>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// Creates a test case from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>A new instance of <see cref="TestCase"/>.</returns>
        public static TestCase FromJson(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TestCase>(json) ?? throw new System.Text.Json.JsonException("Failed to deserialize test case");
        }
    }
}
