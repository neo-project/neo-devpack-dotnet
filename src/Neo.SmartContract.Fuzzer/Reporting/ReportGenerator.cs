using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Neo.SmartContract.Fuzzer.Controller;

namespace Neo.SmartContract.Fuzzer.Reporting
{
    /// <summary>
    /// Generates reports of fuzzing results in various formats.
    /// </summary>
    public class ReportGenerator
    {
        private readonly string _outputDirectory;
        private readonly List<string> _formats;
        private readonly FuzzerConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportGenerator"/> class.
        /// </summary>
        /// <param name="outputDirectory">The directory where reports will be saved.</param>
        /// <param name="formats">The formats to generate reports in.</param>
        /// <param name="config">The fuzzer configuration.</param>
        public ReportGenerator(string outputDirectory, List<string> formats, FuzzerConfiguration config)
        {
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));
            _formats = formats ?? throw new ArgumentNullException(nameof(formats));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // Create output directory if it doesn't exist
            Directory.CreateDirectory(_outputDirectory);
        }

        /// <summary>
        /// Generates reports for the fuzzing results.
        /// </summary>
        /// <param name="issues">The issues found during fuzzing.</param>
        /// <param name="status">The status of the fuzzing session.</param>
        public void GenerateReports(List<IssueReport> issues, FuzzingStatus status)
        {
            foreach (var format in _formats)
            {
                switch (format.ToLowerInvariant())
                {
                    case "json":
                        GenerateJsonReport(issues, status);
                        break;
                    case "html":
                        GenerateHtmlReport(issues, status);
                        break;
                    case "markdown":
                    case "md":
                        GenerateMarkdownReport(issues, status);
                        break;
                    case "text":
                    case "txt":
                        GenerateTextReport(issues, status);
                        break;
                    case "junit":
                    case "xml":
                        GenerateJUnitReport(issues, status);
                        break;
                    default:
                        Console.WriteLine($"Warning: Unsupported report format '{format}'");
                        break;
                }
            }
        }

        private void GenerateJsonReport(List<IssueReport> issues, FuzzingStatus status)
        {
            var report = new
            {
                Contract = Path.GetFileName(_config.NefPath),
                Timestamp = DateTime.Now,
                Duration = status.ElapsedTime,
                TotalExecutions = status.TotalExecutions,
                SuccessfulExecutions = status.SuccessfulExecutions,
                FailedExecutions = status.FailedExecutions,
                IssuesFound = issues.Count,
                CodeCoverage = status.CodeCoverage,
                Issues = issues,
                Configuration = _config
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(report, options);
            string filePath = Path.Combine(_outputDirectory, "report.json");
            File.WriteAllText(filePath, json);

            Console.WriteLine($"JSON report generated: {filePath}");
        }

        private void GenerateHtmlReport(List<IssueReport> issues, FuzzingStatus status)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("  <meta charset=\"UTF-8\">");
            html.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("  <title>Neo Smart Contract Fuzzing Report</title>");
            html.AppendLine("  <style>");
            html.AppendLine("    body { font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #333; }");
            html.AppendLine("    h1, h2, h3 { color: #0066cc; }");
            html.AppendLine("    .container { max-width: 1200px; margin: 0 auto; }");
            html.AppendLine("    .summary { background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin-bottom: 20px; }");
            html.AppendLine("    .issue { background-color: #fff; border: 1px solid #ddd; padding: 15px; margin-bottom: 15px; border-radius: 5px; }");
            html.AppendLine("    .issue-critical { border-left: 5px solid #ff0000; }");
            html.AppendLine("    .issue-high { border-left: 5px solid #ff9900; }");
            html.AppendLine("    .issue-medium { border-left: 5px solid #ffcc00; }");
            html.AppendLine("    .issue-low { border-left: 5px solid #00cc00; }");
            html.AppendLine("    .details { margin-top: 10px; }");
            html.AppendLine("    .details pre { background-color: #f9f9f9; padding: 10px; overflow-x: auto; }");
            html.AppendLine("    table { width: 100%; border-collapse: collapse; }");
            html.AppendLine("    th, td { text-align: left; padding: 8px; border-bottom: 1px solid #ddd; }");
            html.AppendLine("    th { background-color: #f2f2f2; }");
            html.AppendLine("  </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("  <div class=\"container\">");
            html.AppendLine("    <h1>Neo Smart Contract Fuzzing Report</h1>");

            // Summary
            html.AppendLine("    <div class=\"summary\">");
            html.AppendLine("      <h2>Summary</h2>");
            html.AppendLine("      <table>");
            html.AppendLine($"        <tr><th>Contract</th><td>{Path.GetFileName(_config.NefPath)}</td></tr>");
            html.AppendLine($"        <tr><th>Timestamp</th><td>{DateTime.Now}</td></tr>");
            html.AppendLine($"        <tr><th>Duration</th><td>{status.ElapsedTime}</td></tr>");
            html.AppendLine($"        <tr><th>Total Executions</th><td>{status.TotalExecutions}</td></tr>");
            html.AppendLine($"        <tr><th>Successful Executions</th><td>{status.SuccessfulExecutions}</td></tr>");
            html.AppendLine($"        <tr><th>Failed Executions</th><td>{status.FailedExecutions}</td></tr>");
            html.AppendLine($"        <tr><th>Issues Found</th><td>{issues.Count}</td></tr>");
            html.AppendLine($"        <tr><th>Code Coverage</th><td>{status.CodeCoverage:P2}</td></tr>");
            html.AppendLine("      </table>");
            html.AppendLine("    </div>");

            // Issues
            html.AppendLine("    <h2>Issues</h2>");
            if (issues.Count == 0)
            {
                html.AppendLine("    <p>No issues found.</p>");
            }
            else
            {
                foreach (var issue in issues)
                {
                    string severityClass = $"issue-{issue.Severity.ToString().ToLowerInvariant()}";
                    html.AppendLine($"    <div class=\"issue {severityClass}\">");
                    html.AppendLine($"      <h3>[{issue.Severity}] {issue.IssueType}</h3>");
                    html.AppendLine($"      <p><strong>Method:</strong> {issue.MethodName}</p>");
                    html.AppendLine($"      <p><strong>Description:</strong> {issue.Description}</p>");

                    if (!string.IsNullOrEmpty(issue.ExceptionMessage))
                    {
                        html.AppendLine($"      <p><strong>Exception:</strong> {issue.ExceptionType}: {issue.ExceptionMessage}</p>");
                    }

                    html.AppendLine("      <div class=\"details\">");
                    html.AppendLine("        <h4>Test Case</h4>");
                    if (issue.MinimizedTestCase != null)
                    {
                        html.AppendLine("        <pre>");
                        html.AppendLine(issue.SerializedMinimizedTestCase);
                        html.AppendLine("        </pre>");
                    }
                    else if (issue.TestCase != null)
                    {
                        html.AppendLine("        <pre>");
                        html.AppendLine(issue.SerializedTestCase);
                        html.AppendLine("        </pre>");
                    }

                    if (issue.Remediation != null)
                    {
                        html.AppendLine("        <h4>Remediation</h4>");
                        html.AppendLine($"        <p>{issue.Remediation}</p>");
                    }

                    html.AppendLine("      </div>");
                    html.AppendLine("    </div>");
                }
            }

            html.AppendLine("  </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            string filePath = Path.Combine(_outputDirectory, "report.html");
            File.WriteAllText(filePath, html.ToString());

            Console.WriteLine($"HTML report generated: {filePath}");
        }

        private void GenerateMarkdownReport(List<IssueReport> issues, FuzzingStatus status)
        {
            var md = new StringBuilder();
            md.AppendLine("# Neo Smart Contract Fuzzing Report");
            md.AppendLine();
            md.AppendLine($"Generated: {DateTime.Now}");
            md.AppendLine();

            // Summary
            md.AppendLine("## Summary");
            md.AppendLine();
            md.AppendLine($"- **Contract:** {Path.GetFileName(_config.NefPath)}");
            md.AppendLine($"- **Duration:** {status.ElapsedTime}");
            md.AppendLine($"- **Total Executions:** {status.TotalExecutions}");
            md.AppendLine($"- **Successful Executions:** {status.SuccessfulExecutions}");
            md.AppendLine($"- **Failed Executions:** {status.FailedExecutions}");
            md.AppendLine($"- **Issues Found:** {issues.Count}");
            md.AppendLine($"- **Code Coverage:** {status.CodeCoverage:P2}");
            md.AppendLine();

            // Issues
            md.AppendLine("## Issues");
            md.AppendLine();

            if (issues.Count == 0)
            {
                md.AppendLine("No issues found.");
            }
            else
            {
                foreach (var issue in issues)
                {
                    md.AppendLine($"### [{issue.Severity}] {issue.IssueType}");
                    md.AppendLine();
                    md.AppendLine($"**Method:** {issue.MethodName}");
                    md.AppendLine();
                    md.AppendLine($"**Description:** {issue.Description}");
                    md.AppendLine();

                    if (!string.IsNullOrEmpty(issue.ExceptionMessage))
                    {
                        md.AppendLine($"**Exception:** {issue.ExceptionType}: {issue.ExceptionMessage}");
                        md.AppendLine();
                    }

                    md.AppendLine("#### Test Case");
                    md.AppendLine();
                    if (issue.MinimizedTestCase != null)
                    {
                        md.AppendLine("```json");
                        md.AppendLine(issue.SerializedMinimizedTestCase);
                        md.AppendLine("```");
                    }
                    else if (issue.TestCase != null)
                    {
                        md.AppendLine("```json");
                        md.AppendLine(issue.SerializedTestCase);
                        md.AppendLine("```");
                    }
                    md.AppendLine();

                    if (issue.Remediation != null)
                    {
                        md.AppendLine("#### Remediation");
                        md.AppendLine();
                        md.AppendLine(issue.Remediation);
                        md.AppendLine();
                    }

                    md.AppendLine("---");
                    md.AppendLine();
                }
            }

            string filePath = Path.Combine(_outputDirectory, "report.md");
            File.WriteAllText(filePath, md.ToString());

            Console.WriteLine($"Markdown report generated: {filePath}");
        }

        private void GenerateTextReport(List<IssueReport> issues, FuzzingStatus status)
        {
            var text = new StringBuilder();
            text.AppendLine("Neo Smart Contract Fuzzing Report");
            text.AppendLine("=================================");
            text.AppendLine();
            text.AppendLine($"Generated: {DateTime.Now}");
            text.AppendLine();

            // Summary
            text.AppendLine("Summary");
            text.AppendLine("-------");
            text.AppendLine($"Contract: {Path.GetFileName(_config.NefPath)}");
            text.AppendLine($"Duration: {status.ElapsedTime}");
            text.AppendLine($"Total Executions: {status.TotalExecutions}");
            text.AppendLine($"Successful Executions: {status.SuccessfulExecutions}");
            text.AppendLine($"Failed Executions: {status.FailedExecutions}");
            text.AppendLine($"Issues Found: {issues.Count}");
            text.AppendLine($"Code Coverage: {status.CodeCoverage:P2}");
            text.AppendLine();

            // Issues
            text.AppendLine("Issues");
            text.AppendLine("------");
            text.AppendLine();

            if (issues.Count == 0)
            {
                text.AppendLine("No issues found.");
            }
            else
            {
                foreach (var issue in issues)
                {
                    text.AppendLine($"[{issue.Severity}] {issue.IssueType}");
                    text.AppendLine(new string('-', issue.IssueType.Length + issue.Severity.ToString().Length + 3));
                    text.AppendLine();
                    text.AppendLine($"Method: {issue.MethodName}");
                    text.AppendLine();
                    text.AppendLine($"Description: {issue.Description}");
                    text.AppendLine();

                    if (!string.IsNullOrEmpty(issue.ExceptionMessage))
                    {
                        text.AppendLine($"Exception: {issue.ExceptionType}: {issue.ExceptionMessage}");
                        text.AppendLine();
                    }

                    text.AppendLine("Test Case:");
                    if (issue.MinimizedTestCase != null)
                    {
                        text.AppendLine(issue.SerializedMinimizedTestCase);
                    }
                    else if (issue.TestCase != null)
                    {
                        text.AppendLine(issue.SerializedTestCase);
                    }
                    text.AppendLine();

                    if (issue.Remediation != null)
                    {
                        text.AppendLine("Remediation:");
                        text.AppendLine(issue.Remediation);
                        text.AppendLine();
                    }

                    text.AppendLine(new string('=', 80));
                    text.AppendLine();
                }
            }

            string filePath = Path.Combine(_outputDirectory, "report.txt");
            File.WriteAllText(filePath, text.ToString());

            Console.WriteLine($"Text report generated: {filePath}");
        }

        private void GenerateJUnitReport(List<IssueReport> issues, FuzzingStatus status)
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<testsuites>");

            // Group issues by method
            var issuesByMethod = issues.GroupBy(i => i.MethodName).ToDictionary(g => g.Key, g => g.ToList());

            // Add a test suite for each method
            foreach (var method in issuesByMethod.Keys)
            {
                int failures = issuesByMethod[method].Count;
                xml.AppendLine($"  <testsuite name=\"{method}\" tests=\"1\" failures=\"{failures}\" errors=\"0\" skipped=\"0\" time=\"{status.ElapsedTime.TotalSeconds}\">");

                // Add a test case for each issue
                foreach (var issue in issuesByMethod[method])
                {
                    xml.AppendLine($"    <testcase name=\"{issue.IssueType}\" classname=\"{Path.GetFileNameWithoutExtension(_config.NefPath)}.{method}\">");
                    xml.AppendLine($"      <failure message=\"{issue.Description}\" type=\"{issue.Severity}\">");
                    xml.AppendLine($"        {issue.ExceptionMessage}");
                    xml.AppendLine("      </failure>");
                    xml.AppendLine("    </testcase>");
                }

                xml.AppendLine("  </testsuite>");
            }

            // Add a test suite for methods without issues
            xml.AppendLine($"  <testsuite name=\"NoIssues\" tests=\"1\" failures=\"0\" errors=\"0\" skipped=\"0\" time=\"{status.ElapsedTime.TotalSeconds}\">");
            xml.AppendLine($"    <testcase name=\"NoIssues\" classname=\"{Path.GetFileNameWithoutExtension(_config.NefPath)}\">");
            xml.AppendLine("    </testcase>");
            xml.AppendLine("  </testsuite>");

            xml.AppendLine("</testsuites>");

            string filePath = Path.Combine(_outputDirectory, "report.xml");
            File.WriteAllText(filePath, xml.ToString());

            Console.WriteLine($"JUnit XML report generated: {filePath}");
        }
    }
}
