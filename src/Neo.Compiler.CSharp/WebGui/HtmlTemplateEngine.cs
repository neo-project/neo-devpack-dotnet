// Copyright (C) 2015-2025 The Neo Project.
//
// HtmlTemplateEngine.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler.WebGui
{
    /// <summary>
    /// Template engine for generating HTML, CSS, and JavaScript for contract web GUIs
    /// </summary>
    internal class HtmlTemplateEngine
    {
        public string GenerateHtml(HtmlTemplateData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrWhiteSpace(data.ContractName))
                throw new ArgumentException("Contract name cannot be null or empty", nameof(data));
            if (data.Manifest == null)
                throw new ArgumentException("Manifest cannot be null", nameof(data));

            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine($"    <title>{data.ContractName} - Neo Smart Contract Dashboard</title>");
            html.AppendLine("    <link rel=\"stylesheet\" href=\"styles.css\">");
            html.AppendLine("    <link href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css\" rel=\"stylesheet\">");
            html.AppendLine("    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/axios/1.6.0/axios.min.js\"></script>");
            html.AppendLine("    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/chart.js/3.9.1/chart.min.js\"></script>");
            html.AppendLine("</head>");
            html.AppendLine("<body" + (data.Options.DarkTheme ? " class=\"dark-theme\"" : "") + ">");

            // Header
            GenerateHeader(html, data);

            // Main content container
            html.AppendLine("    <div class=\"container\">");

            // Navigation tabs
            GenerateNavigationTabs(html, data.Options);

            // Contract info section
            GenerateContractInfoSection(html, data);

            if (data.Options.IncludeMethodInvocation)
            {
                GenerateMethodInvocationSection(html, data);
            }

            if (data.Options.IncludeBalanceMonitoring)
            {
                GenerateBalanceMonitoringSection(html, data);
            }

            if (data.Options.IncludeTransactionHistory)
            {
                GenerateTransactionHistorySection(html, data);
            }

            if (data.Options.IncludeEventMonitoring)
            {
                GenerateEventMonitoringSection(html, data);
            }

            if (data.Options.IncludeStateMonitoring)
            {
                GenerateStateMonitoringSection(html, data);
            }

            if (data.Options.IncludeWalletConnection)
            {
                GenerateWalletConnectionSection(html, data);
            }

            html.AppendLine("    </div>");

            // Footer
            GenerateFooter(html, data);

            // Scripts
            html.AppendLine("    <script src=\"contract.js\"></script>");

            if (!string.IsNullOrEmpty(data.Options.CustomJavaScript))
            {
                html.AppendLine("    <script>");
                html.AppendLine(data.Options.CustomJavaScript);
                html.AppendLine("    </script>");
            }

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private void GenerateHeader(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <header class=\"header\">");
            html.AppendLine("        <div class=\"header-content\">");
            html.AppendLine("            <div class=\"logo\">");
            html.AppendLine("                <i class=\"fab fa-ethereum\"></i>");
            html.AppendLine($"                <h1>{data.ContractName}</h1>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class=\"header-actions\">");
            html.AppendLine("                <button id=\"theme-toggle\" class=\"btn btn-ghost\">");
            html.AppendLine("                    <i class=\"fas fa-moon\"></i>");
            html.AppendLine("                </button>");
            html.AppendLine("                <button id=\"refresh-all\" class=\"btn btn-primary\">");
            html.AppendLine("                    <i class=\"fas fa-sync-alt\"></i> Refresh");
            html.AppendLine("                </button>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </header>");
        }

        private void GenerateNavigationTabs(StringBuilder html, WebGuiOptions options)
        {
            html.AppendLine("    <nav class=\"nav-tabs\">");
            html.AppendLine("        <button class=\"tab-button active\" data-tab=\"overview\">Overview</button>");

            if (options.IncludeMethodInvocation)
                html.AppendLine("        <button class=\"tab-button\" data-tab=\"methods\">Methods</button>");

            if (options.IncludeBalanceMonitoring)
                html.AppendLine("        <button class=\"tab-button\" data-tab=\"balance\">Balance</button>");

            if (options.IncludeTransactionHistory)
                html.AppendLine("        <button class=\"tab-button\" data-tab=\"transactions\">Transactions</button>");

            if (options.IncludeEventMonitoring)
                html.AppendLine("        <button class=\"tab-button\" data-tab=\"events\">Events</button>");

            if (options.IncludeStateMonitoring)
                html.AppendLine("        <button class=\"tab-button\" data-tab=\"state\">State</button>");

            if (options.IncludeWalletConnection)
                html.AppendLine("        <button class=\"tab-button\" data-tab=\"wallet\">Wallet</button>");

            html.AppendLine("    </nav>");
        }

        private void GenerateContractInfoSection(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <div id=\"overview\" class=\"tab-content active\">");
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h2><i class=\"fas fa-info-circle\"></i> Contract Information</h2>");
            html.AppendLine("            <div class=\"info-grid\">");
            html.AppendLine($"                <div class=\"info-item\">");
            html.AppendLine($"                    <label>Contract Name:</label>");
            html.AppendLine($"                    <span>{data.ContractName}</span>");
            html.AppendLine($"                </div>");

            if (!string.IsNullOrEmpty(data.ContractHash))
            {
                html.AppendLine($"                <div class=\"info-item\">");
                html.AppendLine($"                    <label>Contract Hash:</label>");
                html.AppendLine($"                    <span class=\"hash\">{data.ContractHash}</span>");
                html.AppendLine($"                </div>");
            }

            html.AppendLine($"                <div class=\"info-item\">");
            html.AppendLine($"                    <label>Methods:</label>");
            html.AppendLine($"                    <span>{data.Manifest.Abi.Methods.Length}</span>");
            html.AppendLine($"                </div>");

            html.AppendLine($"                <div class=\"info-item\">");
            html.AppendLine($"                    <label>Events:</label>");
            html.AppendLine($"                    <span>{data.Manifest.Abi.Events.Length}</span>");
            html.AppendLine($"                </div>");

            html.AppendLine($"                <div class=\"info-item\">");
            html.AppendLine($"                    <label>Generated:</label>");
            html.AppendLine($"                    <span>{data.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC</span>");
            html.AppendLine($"                </div>");

            html.AppendLine("            </div>");
            html.AppendLine("        </div>");

            // Contract ABI details
            GenerateAbiDetails(html, data.Manifest);

            html.AppendLine("    </div>");
        }

        private void GenerateAbiDetails(StringBuilder html, ContractManifest manifest)
        {
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h3><i class=\"fas fa-code\"></i> Contract ABI</h3>");

            // Methods
            if (manifest.Abi.Methods.Length > 0)
            {
                html.AppendLine("            <h4>Methods</h4>");
                html.AppendLine("            <div class=\"method-list\">");

                foreach (var method in manifest.Abi.Methods)
                {
                    html.AppendLine("                <div class=\"method-item\">");
                    html.AppendLine($"                    <div class=\"method-signature\">");
                    html.AppendLine($"                        <span class=\"method-name\">{method.Name}</span>");
                    html.AppendLine($"                        <span class=\"method-params\">(");

                    for (int i = 0; i < method.Parameters.Length; i++)
                    {
                        var param = method.Parameters[i];
                        if (i > 0) html.Append(", ");
                        html.Append($"{param.Type} {param.Name}");
                    }

                    html.AppendLine($")</span>");
                    html.AppendLine($"                        <span class=\"return-type\">→ {method.ReturnType}</span>");
                    if (method.Safe)
                        html.AppendLine($"                        <span class=\"safe-badge\">SAFE</span>");
                    html.AppendLine($"                    </div>");
                    html.AppendLine("                </div>");
                }

                html.AppendLine("            </div>");
            }

            // Events
            if (manifest.Abi.Events.Length > 0)
            {
                html.AppendLine("            <h4>Events</h4>");
                html.AppendLine("            <div class=\"event-list\">");

                foreach (var eventItem in manifest.Abi.Events)
                {
                    html.AppendLine("                <div class=\"event-item\">");
                    html.AppendLine($"                    <div class=\"event-signature\">");
                    html.AppendLine($"                        <span class=\"event-name\">{eventItem.Name}</span>");
                    html.AppendLine($"                        <span class=\"event-params\">(");

                    for (int i = 0; i < eventItem.Parameters.Length; i++)
                    {
                        var param = eventItem.Parameters[i];
                        if (i > 0) html.Append(", ");
                        html.Append($"{param.Type} {param.Name}");
                    }

                    html.AppendLine($")</span>");
                    html.AppendLine($"                    </div>");
                    html.AppendLine("                </div>");
                }

                html.AppendLine("            </div>");
            }

            html.AppendLine("        </div>");
        }

        private void GenerateMethodInvocationSection(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <div id=\"methods\" class=\"tab-content\">");
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h2><i class=\"fas fa-play-circle\"></i> Method Invocation</h2>");
            html.AppendLine("            <div class=\"method-invocation\">");
            html.AppendLine("                <div class=\"method-selector\">");
            html.AppendLine("                    <label for=\"method-select\">Select Method:</label>");
            html.AppendLine("                    <select id=\"method-select\">");

            foreach (var method in data.Manifest.Abi.Methods.Where(m => !m.Name.StartsWith("_")))
            {
                html.AppendLine($"                        <option value=\"{method.Name}\">{method.Name}</option>");
            }

            html.AppendLine("                    </select>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div id=\"method-parameters\" class=\"method-parameters\"></div>");
            html.AppendLine("                <div class=\"method-actions\">");
            html.AppendLine("                    <button id=\"invoke-method\" class=\"btn btn-primary\">");
            html.AppendLine("                        <i class=\"fas fa-paper-plane\"></i> Invoke Method");
            html.AppendLine("                    </button>");
            html.AppendLine("                    <button id=\"test-invoke\" class=\"btn btn-secondary\">");
            html.AppendLine("                        <i class=\"fas fa-flask\"></i> Test Invoke");
            html.AppendLine("                    </button>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div id=\"method-result\" class=\"method-result\"></div>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        private void GenerateBalanceMonitoringSection(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <div id=\"balance\" class=\"tab-content\">");
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h2><i class=\"fas fa-wallet\"></i> Balance Monitoring</h2>");
            html.AppendLine("            <div class=\"balance-grid\">");
            html.AppendLine("                <div class=\"balance-card\">");
            html.AppendLine("                    <h3>GAS Balance</h3>");
            html.AppendLine("                    <div id=\"gas-balance\" class=\"balance-amount\">Loading...</div>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"balance-card\">");
            html.AppendLine("                    <h3>NEO Balance</h3>");
            html.AppendLine("                    <div id=\"neo-balance\" class=\"balance-amount\">Loading...</div>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"balance-card\">");
            html.AppendLine("                    <h3>Contract Balance</h3>");
            html.AppendLine("                    <div id=\"contract-balance\" class=\"balance-amount\">Loading...</div>");
            html.AppendLine("                </div>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class=\"balance-chart\">");
            html.AppendLine("                <canvas id=\"balanceChart\" width=\"400\" height=\"200\"></canvas>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        private void GenerateTransactionHistorySection(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <div id=\"transactions\" class=\"tab-content\">");
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h2><i class=\"fas fa-history\"></i> Transaction History</h2>");
            html.AppendLine("            <div class=\"transaction-filters\">");
            html.AppendLine("                <input type=\"text\" id=\"tx-filter\" placeholder=\"Filter by hash, method, or address...\">");
            html.AppendLine("                <button id=\"clear-filter\" class=\"btn btn-secondary\">Clear</button>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div id=\"transaction-list\" class=\"transaction-list\"></div>");
            html.AppendLine("            <div class=\"pagination\">");
            html.AppendLine("                <button id=\"prev-page\" class=\"btn btn-ghost\">Previous</button>");
            html.AppendLine("                <span id=\"page-info\">Page 1</span>");
            html.AppendLine("                <button id=\"next-page\" class=\"btn btn-ghost\">Next</button>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        private void GenerateEventMonitoringSection(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <div id=\"events\" class=\"tab-content\">");
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h2><i class=\"fas fa-bell\"></i> Event Monitoring</h2>");
            html.AppendLine("            <div class=\"event-controls\">");
            html.AppendLine("                <button id=\"start-monitoring\" class=\"btn btn-success\">");
            html.AppendLine("                    <i class=\"fas fa-play\"></i> Start Monitoring");
            html.AppendLine("                </button>");
            html.AppendLine("                <button id=\"stop-monitoring\" class=\"btn btn-danger\" disabled>");
            html.AppendLine("                    <i class=\"fas fa-stop\"></i> Stop Monitoring");
            html.AppendLine("                </button>");
            html.AppendLine("                <button id=\"clear-events\" class=\"btn btn-secondary\">");
            html.AppendLine("                    <i class=\"fas fa-trash\"></i> Clear Events");
            html.AppendLine("                </button>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div id=\"event-list\" class=\"event-list\"></div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        private void GenerateStateMonitoringSection(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <div id=\"state\" class=\"tab-content\">");
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h2><i class=\"fas fa-database\"></i> Contract State</h2>");
            html.AppendLine("            <div class=\"state-viewer\">");
            html.AppendLine("                <div class=\"state-controls\">");
            html.AppendLine("                    <input type=\"text\" id=\"storage-key\" placeholder=\"Storage key...\">");
            html.AppendLine("                    <button id=\"get-storage\" class=\"btn btn-primary\">Get Value</button>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div id=\"storage-result\" class=\"storage-result\"></div>");
            html.AppendLine("                <div class=\"state-dump\">");
            html.AppendLine("                    <button id=\"dump-state\" class=\"btn btn-secondary\">Dump All State</button>");
            html.AppendLine("                    <div id=\"state-dump-result\" class=\"state-dump-result\"></div>");
            html.AppendLine("                </div>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        private void GenerateWalletConnectionSection(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <div id=\"wallet\" class=\"tab-content\">");
            html.AppendLine("        <div class=\"card\">");
            html.AppendLine("            <h2><i class=\"fas fa-link\"></i> Wallet Connection</h2>");
            html.AppendLine("            <div class=\"wallet-connection\">");
            html.AppendLine("                <div class=\"wallet-status\">");
            html.AppendLine("                    <div id=\"wallet-info\" class=\"wallet-info\">No wallet connected</div>");
            html.AppendLine("                    <button id=\"connect-wallet\" class=\"btn btn-primary\">Connect Wallet</button>");
            html.AppendLine("                    <button id=\"disconnect-wallet\" class=\"btn btn-secondary\" style=\"display: none;\">Disconnect</button>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"wallet-details\" style=\"display: none;\">");
            html.AppendLine("                    <div class=\"wallet-detail\">");
            html.AppendLine("                        <label>Address:</label>");
            html.AppendLine("                        <span id=\"wallet-address\"></span>");
            html.AppendLine("                    </div>");
            html.AppendLine("                    <div class=\"wallet-detail\">");
            html.AppendLine("                        <label>Balance:</label>");
            html.AppendLine("                        <span id=\"wallet-balance\"></span>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
        }

        private void GenerateFooter(StringBuilder html, HtmlTemplateData data)
        {
            html.AppendLine("    <footer class=\"footer\">");
            html.AppendLine("        <div class=\"footer-content\">");
            html.AppendLine("            <p>Generated by Neo DevPack Compiler</p>");
            html.AppendLine($"            <p>Generated on {data.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC</p>");
            html.AppendLine("        </div>");
            html.AppendLine("    </footer>");
        }

        public string GenerateCssContent(WebGuiOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return CssTemplates.GetMainCss(options);
        }

        public string GenerateJavaScriptContent(string contractName, ContractManifest manifest, WebGuiOptions options)
        {
            if (string.IsNullOrWhiteSpace(contractName))
                throw new ArgumentException("Contract name cannot be null or empty", nameof(contractName));
            if (manifest == null)
                throw new ArgumentNullException(nameof(manifest));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return JavaScriptTemplates.GetMainScript(contractName, manifest, options);
        }
    }
}
