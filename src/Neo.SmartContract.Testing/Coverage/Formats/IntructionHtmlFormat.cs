using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class IntructionHtmlFormat : CoverageFormatBase
    {
        /// <summary>
        /// Entries
        /// </summary>
        public (CoveredContract, CoveredMethod[])[] Entries { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="methods">Methods</param>
        public IntructionHtmlFormat(CoveredContract contract, params CoveredMethod[] methods)
        {
            Entries = new (CoveredContract, CoveredMethod[])[] { (contract, methods) };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entries">Entries</param>
        public IntructionHtmlFormat(IEnumerable<(CoveredContract, CoveredMethod[])> entries)
        {
            Entries = entries.ToArray();
        }

        public override void WriteReport(Action<string, Action<Stream>> writeAttachement)
        {
            writeAttachement("coverage.cobertura.html", stream =>
            {
                using var writer = new StreamWriter(stream)
                {
                    NewLine = "\n"
                };
                WriteReport(writer);
                writer.Flush();
            });
        }

        private void WriteReport(StreamWriter writer)
        {
            writer.WriteLine(@"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<title>NEF coverage Report</title>
<style>
    body { font-family: Arial, sans-serif; margin: 0; padding: 0; }
    .bar { background-color: #f2f2f2; padding: 10px; cursor: pointer; }
    .hash { float: left; }
    .method-name { float: left; }
    .coverage { float: right; display: inline-block; width: 100px; text-align: right; }
    .method { cursor: pointer; margin-top: 5px; padding: 2px; }
    .details { display: none; padding-left: 20px; }
    .container { padding-left: 20px; }
    .opcode { margin-left: 20px; position: relative; padding: 2px; margin-bottom: 2px; display: flex; align-items: center; }
    .hit { background-color: #eafaea; } /* Light green for hits */
    .no-hit { background-color: #ffcccc; } /* Light red for no hits */
    .hits { margin-left: 5px; font-size: 0.6em; margin-right: 10px; }
    .branch { margin-left: 5px; font-size: 0.6em; margin-right: }
    .icon { margin-right: 5px; }

    .high-coverage { background-color: #ccffcc; } /* Lighter green for high coverage */
    .medium-coverage { background-color: #ffffcc; } /* Yellow for medium coverage */
    .low-coverage { background-color: #ffcccc; } /* Lighter red for low coverage */
</style>
</head>
<body>
");

            foreach ((var contract, var methods) in Entries)
            {
                writer.WriteLine($@"
<div class=""bar"">
    <div class=""hash"">{contract.Name}</div>
    <div class=""coverage"">&nbsp;{contract.CoveredBranchPercentage:P2}&nbsp;</div>
    <div class=""coverage"">&nbsp;{contract.CoveredLinesPercentage:P2}&nbsp;</div>
    <div style=""clear: both;""></div>
</div>
<div class=""container"">
");

                foreach (var method in methods.OrderBy(u => u.Method.Name).OrderByDescending(u => u.CoveredLinesPercentage))
                {
                    var kind = "low";
                    if (method.CoveredLinesPercentage > 0.7M) kind = "medium";
                    if (method.CoveredLinesPercentage > 0.8M) kind = "high";

                    writer.WriteLine($@"
<div class=""method {kind}-coverage"">
    <div class=""method-name"">{method.Method}</div>
    <div class=""coverage"">&nbsp;{method.CoveredBranchPercentage:P2}&nbsp;</div>
    <div class=""coverage"">&nbsp;{method.CoveredLinesPercentage:P2}&nbsp;</div>
    <div style=""clear: both;""></div>
</div>
");
                    writer.WriteLine($@"<div class=""details"">");

                    foreach (var hit in method.Lines)
                    {
                        var noHit = hit.Hits == 0 ? "no-" : "";
                        var icon = hit.Hits == 0 ? "✘" : "✔";
                        var branch = "";

                        if (contract.TryGetBranch(hit.Offset, out var b))
                        {
                            branch = $" <span class=\"branch\">[ᛦ {b.Hits}/{b.Count}]</span>";
                        }

                        writer.WriteLine($@"<div class=""opcode {noHit}hit""><span class=""icon"">{icon}</span><span class=""hits"">{hit.Hits} Hits</span>{hit.Description}{branch}</div>");
                    }

                    writer.WriteLine($@"</div>");
                }

                writer.WriteLine($@"</div>");
            }

            writer.WriteLine(@"
<script>
    document.querySelector('.bar').addEventListener('click', () => {
        const container = document.querySelector('.container');
        container.style.display = container.style.display === 'none' ? 'block' : 'none';
    });

    document.querySelectorAll('.method').forEach(item => {
        item.addEventListener('click', function() {
            const details = this.nextElementSibling;
            if(details.style.display === '' || details.style.display === 'none') {
                details.style.display = 'block';
            } else {
                details.style.display = 'none';
            }
        });
    });
</script>
</body>
</html>
");
        }
    }
}
