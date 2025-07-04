using System.Text.RegularExpressions;

namespace Neo.SmartContract.Template
{
    /// <summary>
    /// Processes template files and replaces placeholder values
    /// </summary>
    public static class TemplateProcessor
    {
        private static readonly Dictionary<string, string> TemplateVariables = new()
        {
            { "TemplateNeoVersion", "3.8.1-*" },
            { "TemplateDotNetVersion", "9.0.0" },
            { "TemplateFrameworkVersion", "net9.0" }
        };

        /// <summary>
        /// Process a template file and replace all template variables
        /// </summary>
        public static string ProcessTemplate(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            var result = content;

            foreach (var variable in TemplateVariables)
            {
                result = result.Replace(variable.Key, variable.Value);
            }

            return result;
        }

        /// <summary>
        /// Process a template file on disk
        /// </summary>
        public static async Task ProcessTemplateFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var content = await File.ReadAllTextAsync(filePath);
            var processedContent = ProcessTemplate(content);

            if (content != processedContent)
            {
                await File.WriteAllTextAsync(filePath, processedContent);
            }
        }

        /// <summary>
        /// Process all template files in a directory recursively
        /// </summary>
        public static async Task ProcessTemplateDirectoryAsync(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return;

            var templateExtensions = new[] { ".cs", ".csproj", ".json", ".sln", ".md" };

            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories)
                .Where(f => templateExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .ToArray();

            var tasks = files.Select(ProcessTemplateFileAsync);
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Get the current Neo version for templates
        /// </summary>
        public static string GetNeoVersion()
        {
            return TemplateVariables["TemplateNeoVersion"];
        }

        /// <summary>
        /// Set template variables (for testing or customization)
        /// </summary>
        public static void SetTemplateVariable(string key, string value)
        {
            TemplateVariables[key] = value;
        }
    }
}