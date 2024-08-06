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

            if (Environment.GetEnvironmentVariable("COVERAGE_MERGE_JOIN") is string mergeWith &&
                !string.IsNullOrEmpty(mergeWith))
            {
                new CoverletJsonFormat([.. list]).Write(Environment.ExpandEnvironmentVariables(mergeWith), true);

                Console.WriteLine($"Coverage merged with: {mergeWith}");
            }

            // Ensure that the coverage is more than X% at the end of the tests

            Assert.IsTrue(coverage.CoveredLinesPercentage >= requiredCoverage, $"Coverage is {coverage.CoveredLinesPercentage:P2}, less than {requiredCoverage:P2}");
        }
    }
}
