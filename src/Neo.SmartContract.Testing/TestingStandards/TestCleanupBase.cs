// Copyright (C) 2015-2026 The Neo Project.
//
// TestCleanupBase.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Coverage.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Neo.SmartContract.Testing.TestingStandards
{
    public abstract class TestCleanupBase
    {
        protected static void EnsureCoverageInternal(Assembly assembly, IEnumerable<(Type type, NeoDebugInfo? dbgInfo)> debugInfos, decimal requiredCoverage = 0.9M)
        {
            // Join here all of your coverage sources
            CoverageBase? coverage = null;
            var allTypes = assembly.GetTypes();
            var list = new List<(CoveredContract Contract, NeoDebugInfo DebugInfo)>();

            foreach (var infos in debugInfos)
            {
                Type type = typeof(TestBase<>).MakeGenericType(infos.type);
                CoveredContract? cov = null;

                if (infos.dbgInfo != null)
                {
                    foreach (var aType in allTypes)
                    {
                        if (type.IsAssignableFrom(aType))
                        {
                            cov = type.GetProperty("Coverage")!.GetValue(null) as CoveredContract;
                            Assert.IsNotNull(cov, $"{infos.type} coverage can't be null");

                            // It doesn't require join, because we have only one UnitTest class per contract

                            coverage += cov;
                            list.Add((cov, infos.dbgInfo));
                            break;
                        }
                    }
                }

                if (cov is null)
                {
                    Console.Error.WriteLine($"Coverage not found for {infos.type}");
                }
            }

            // Ensure we have coverage

            Assert.IsNotNull(coverage, $"Coverage can't be null");

            // Dump current coverage

            Console.WriteLine(coverage.Dump(DumpFormat.Console));
            File.WriteAllText("coverage.instruction.html", coverage.Dump(DumpFormat.Html));

            // Write the cobertura format

            File.WriteAllText("coverage.cobertura.xml", new CoberturaFormat([.. list]).Dump());

            // Write the report to the specific path

            CoverageReporting.CreateReport("coverage.cobertura.xml", "./coverageReport/");

            // Merge coverlet json

            if (ResolveCoverageMergePath() is string mergeWith)
            {
                new CoverletJsonFormat([.. list]).Write(mergeWith, true);

                Console.WriteLine($"Coverage merged with: {mergeWith}");
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= requiredCoverage, $"Coverage is {coverage.CoveredLinesPercentage:P2}, less than {requiredCoverage:P2}");
        }

        private static string? ResolveCoverageMergePath()
        {
            var direct = NormalizeCoveragePath(Environment.GetEnvironmentVariable("COVERAGE_MERGE_JOIN"));
            if (direct is not null)
                return direct;

            // CI uses an env var with the MSBuild argument used for coverlet merge:
            // COVERLET_MERGE_WITH=/p:MergeWith=/path/to/coverage.json
            var mergeWithArg = Environment.GetEnvironmentVariable("COVERLET_MERGE_WITH");
            if (string.IsNullOrWhiteSpace(mergeWithArg))
                return null;

            const string Prefix = "/p:MergeWith=";
            var index = mergeWithArg.IndexOf(Prefix, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return null;

            return NormalizeMsBuildArgumentValue(mergeWithArg[(index + Prefix.Length)..]);
        }

        private static string? NormalizeMsBuildArgumentValue(string value)
        {
            value = value.Trim();
            if (value.Length == 0)
                return null;

            if (value[0] is '"' or '\'')
            {
                var quote = value[0];
                var endQuote = value.IndexOf(quote, 1);
                if (endQuote <= 1)
                    return null;

                value = value[1..endQuote];
            }
            else
            {
                var spaceIndex = value.IndexOf(' ');
                if (spaceIndex >= 0)
                    value = value[..spaceIndex];
            }

            return NormalizeCoveragePath(value);
        }

        private static string? NormalizeCoveragePath(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim().Trim('"', '\'');
            if (value.Length == 0)
                return null;

            return Environment.ExpandEnvironmentVariables(value);
        }
    }
}
