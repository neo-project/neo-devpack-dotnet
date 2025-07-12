// Copyright (C) 2015-2025 The Neo Project.
//
// WebGuiOptions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.WebGui
{
    /// <summary>
    /// Options for web GUI generation
    /// </summary>
    public class WebGuiOptions
    {
        /// <summary>
        /// Default RPC endpoint URL
        /// </summary>
        public string RpcEndpoint { get; set; } = "https://neo.coz.io:443";

        /// <summary>
        /// Default network magic number
        /// </summary>
        public uint NetworkMagic { get; set; } = 860833102; // TestNet

        /// <summary>
        /// Include transaction history monitoring
        /// </summary>
        public bool IncludeTransactionHistory { get; set; } = true;

        /// <summary>
        /// Include balance monitoring
        /// </summary>
        public bool IncludeBalanceMonitoring { get; set; } = true;

        /// <summary>
        /// Include method invocation interface
        /// </summary>
        public bool IncludeMethodInvocation { get; set; } = true;

        /// <summary>
        /// Include contract state monitoring
        /// </summary>
        public bool IncludeStateMonitoring { get; set; } = true;

        /// <summary>
        /// Include event monitoring
        /// </summary>
        public bool IncludeEventMonitoring { get; set; } = true;

        /// <summary>
        /// Auto-refresh interval in seconds
        /// </summary>
        public int RefreshInterval { get; set; } = 30;

        /// <summary>
        /// Use dark theme
        /// </summary>
        public bool DarkTheme { get; set; } = false;

        /// <summary>
        /// Custom CSS styles
        /// </summary>
        public string? CustomCss { get; set; }

        /// <summary>
        /// Custom JavaScript code
        /// </summary>
        public string? CustomJavaScript { get; set; }

        /// <summary>
        /// Include wallet connection capabilities
        /// </summary>
        public bool IncludeWalletConnection { get; set; } = true;

        /// <summary>
        /// Show advanced developer features
        /// </summary>
        public bool ShowAdvancedFeatures { get; set; } = false;
    }
}
