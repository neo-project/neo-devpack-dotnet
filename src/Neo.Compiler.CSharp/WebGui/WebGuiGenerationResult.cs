// Copyright (C) 2015-2025 The Neo Project.
//
// WebGuiGenerationResult.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;

namespace Neo.Compiler.WebGui
{
    /// <summary>
    /// Result of web GUI generation
    /// </summary>
    public class WebGuiGenerationResult
    {
        /// <summary>
        /// Whether the generation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if generation failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Path to the main HTML file
        /// </summary>
        public string? HtmlFilePath { get; set; }

        /// <summary>
        /// Directory containing all generated files
        /// </summary>
        public string? OutputDirectory { get; set; }

        /// <summary>
        /// List of all generated files
        /// </summary>
        public List<string> GeneratedFiles { get; set; } = new();

        /// <summary>
        /// URL to access the web GUI (if served)
        /// </summary>
        public string? AccessUrl { get; set; }

        /// <summary>
        /// Generated HTML content
        /// </summary>
        public string? HtmlContent { get; set; }

        /// <summary>
        /// Generation statistics
        /// </summary>
        public WebGuiStatistics Statistics { get; set; } = new();

        /// <summary>
        /// Creates a successful result
        /// </summary>
        public static WebGuiGenerationResult CreateSuccess(string htmlFilePath, string outputDirectory)
        {
            return new WebGuiGenerationResult
            {
                Success = true,
                HtmlFilePath = htmlFilePath,
                OutputDirectory = outputDirectory
            };
        }

        /// <summary>
        /// Creates a failed result
        /// </summary>
        public static WebGuiGenerationResult Failure(string errorMessage)
        {
            return new WebGuiGenerationResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Statistics about web GUI generation
    /// </summary>
    public class WebGuiStatistics
    {
        /// <summary>
        /// Number of contract methods processed
        /// </summary>
        public int MethodCount { get; set; }

        /// <summary>
        /// Number of events processed
        /// </summary>
        public int EventCount { get; set; }

        /// <summary>
        /// Total size of generated files in bytes
        /// </summary>
        public long TotalFileSize { get; set; }

        /// <summary>
        /// Generation time in milliseconds
        /// </summary>
        public long GenerationTimeMs { get; set; }

        /// <summary>
        /// Number of generated files
        /// </summary>
        public int FileCount { get; set; }
    }
}
