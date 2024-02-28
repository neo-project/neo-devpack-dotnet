namespace Neo.Compiler
{
    public class CompilationSourceReferences
    {
        /// <summary>
        /// Packages
        /// </summary>
        public (string packageName, string packageVersion)[]? Packages { get; set; } = null;

        /// <summary>
        /// Projects
        /// </summary>
        public string[]? Projects { get; set; } = null;
    }
}
