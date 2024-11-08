using Microsoft.CodeAnalysis;
using System.IO;

namespace Neo.Compiler
{
    public class LocationInformation
    {
        /// <summary>
        /// Source Location
        /// </summary>
        public Location? SourceLocation { get; set; }

        /// <summary>
        /// Compiler Origin
        /// </summary>
        public string? CompilerLocation { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceLocation">Source Location</param>
        /// <param name="compilerLocation">Compiler Location</param>
        public LocationInformation(Location? sourceLocation, string? compilerLocation)
        {
            SourceLocation = sourceLocation;
            CompilerLocation = compilerLocation;
        }

        /// <summary>
        /// Build compiler location
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <param name="caller">Caller</param>
        /// <returns>Compiler location</returns>
        public static string? BuildCompilerLocation(int lineNumber, string? callerPath, string? caller)
        {
            if (string.IsNullOrEmpty(callerPath)) return null;
            if (string.IsNullOrEmpty(caller)) return null;

            return $"{caller}({Path.GetFileName(callerPath)}@{lineNumber})";
        }
    }
}
