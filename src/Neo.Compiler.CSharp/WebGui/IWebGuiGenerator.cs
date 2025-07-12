// Copyright (C) 2015-2025 The Neo Project.
//
// IWebGuiGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Manifest;

namespace Neo.Compiler.WebGui
{
    /// <summary>
    /// Interface for generating interactive web-based GUIs for compiled smart contracts
    /// </summary>
    public interface IWebGuiGenerator
    {
        /// <summary>
        /// Generates a complete web-based GUI for a compiled contract
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="manifest">Contract manifest containing ABI information</param>
        /// <param name="nefBytes">Compiled NEF file bytes</param>
        /// <param name="contractHash">Contract hash (if deployed)</param>
        /// <param name="outputDirectory">Directory to generate the web files</param>
        /// <param name="options">Web GUI generation options</param>
        /// <returns>Web GUI generation result</returns>
        WebGuiGenerationResult GenerateWebGui(
            string contractName,
            ContractManifest manifest,
            byte[] nefBytes,
            string? contractHash = null,
            string? outputDirectory = null,
            WebGuiOptions? options = null);

        /// <summary>
        /// Generates just the HTML content without writing to files
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="manifest">Contract manifest containing ABI information</param>
        /// <param name="nefBytes">Compiled NEF file bytes</param>
        /// <param name="contractHash">Contract hash (if deployed)</param>
        /// <param name="options">Web GUI generation options</param>
        /// <returns>Generated HTML content</returns>
        string GenerateHtmlContent(
            string contractName,
            ContractManifest manifest,
            byte[] nefBytes,
            string? contractHash = null,
            WebGuiOptions? options = null);
    }
}
