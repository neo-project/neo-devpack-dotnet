using Microsoft.CodeAnalysis;
using System.IO;

namespace Neo.Compiler
{
    public class LocationInformation
    {
        /// <summary>
        /// Source Location
        /// </summary>
        public SourceLocation? Source { get; set; }

        /// <summary>
        /// Compiler Origin
        /// </summary>
        public CompilerLocation? Compiler { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceLocation">Source Location</param>
        /// <param name="compilerLocation">Compiler Location</param>
        public LocationInformation(Location? sourceLocation, CompilerLocation? compilerLocation)
        {
            Source = sourceLocation is not null ? new SourceLocation(sourceLocation) : null;
            Compiler = compilerLocation;
        }

        /// <summary>
        /// Build compiler location
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <param name="caller">Caller</param>
        /// <returns>Compiler location</returns>
        public static CompilerLocation? BuildCompilerLocation(int lineNumber, string? callerPath, string? caller)
        {
            if (string.IsNullOrEmpty(callerPath)) return null;
            if (string.IsNullOrEmpty(caller)) return null;

            return new CompilerLocation()
            {
                File = Path.GetFileName(callerPath),
                Line = lineNumber,
                Method = caller
            };
        }
    }
}
