// Copyright (C) 2015-2026 The Neo Project.
//
// DiagnosticCatalogConsistencyTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class DiagnosticCatalogConsistencyTests
    {
        private static readonly IReadOnlyList<AnalyzerDiagnostic> AnalyzerDiagnostics = LoadAnalyzerDiagnostics();
        private static readonly IReadOnlyList<ReleaseEvent> ReleaseEvents = LoadReleaseEvents();
        private static readonly IReadOnlyDictionary<string, ActiveRule> ActiveRules = ApplyReleaseEvents(ReleaseEvents);

        [TestMethod]
        public void EveryAnalyzer_ShouldExposeAtLeastOneDiagnostic()
        {
            var analyzersWithoutDiagnostics = typeof(FloatUsageAnalyzer).Assembly.GetTypes()
                .Where(IsConcreteAnalyzer)
                .Select(CreateAnalyzer)
                .Where(analyzer => analyzer.SupportedDiagnostics.IsEmpty)
                .Select(analyzer => analyzer.GetType().Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(
                0,
                analyzersWithoutDiagnostics.Length,
                "Analyzers without supported diagnostics:" + Environment.NewLine +
                string.Join(Environment.NewLine, analyzersWithoutDiagnostics));
        }

        [TestMethod]
        public void DiagnosticIds_ShouldBeUnique()
        {
            var duplicates = AnalyzerDiagnostics
                .GroupBy(entry => entry.Descriptor.Id, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => $"{group.Key}: {string.Join(", ", group.Select(entry => entry.AnalyzerName).OrderBy(name => name, StringComparer.Ordinal))}")
                .OrderBy(entry => entry, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(
                0,
                duplicates.Length,
                "Duplicate analyzer diagnostic IDs:" + Environment.NewLine +
                string.Join(Environment.NewLine, duplicates));
        }

        [TestMethod]
        public void EveryDiagnostic_ShouldBeReleaseTracked()
        {
            var untracked = AnalyzerDiagnostics
                .Where(entry => !ActiveRules.ContainsKey(entry.Descriptor.Id))
                .Select(entry => $"{entry.Descriptor.Id}: {entry.AnalyzerName}")
                .OrderBy(entry => entry, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(
                0,
                untracked.Length,
                "Analyzer diagnostics missing from the active release catalog:" + Environment.NewLine +
                string.Join(Environment.NewLine, untracked));
        }

        [TestMethod]
        public void EveryActiveReleaseEntry_ShouldHaveADiagnosticDescriptor()
        {
            var descriptorIds = AnalyzerDiagnostics
                .Select(entry => entry.Descriptor.Id)
                .ToHashSet(StringComparer.Ordinal);
            var orphaned = ActiveRules
                .Where(entry => !descriptorIds.Contains(entry.Key))
                .Select(entry => $"{entry.Value.Source}:{entry.Value.LineNumber}: {entry.Key}")
                .OrderBy(entry => entry, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(
                0,
                orphaned.Length,
                "Active release entries without a diagnostic descriptor:" + Environment.NewLine +
                string.Join(Environment.NewLine, orphaned));
        }

        [TestMethod]
        public void ReleaseMetadata_ShouldMatchDiagnosticDescriptors()
        {
            var mismatches = AnalyzerDiagnostics
                .Where(entry => ActiveRules.ContainsKey(entry.Descriptor.Id))
                .Select(entry => new
                {
                    Analyzer = entry,
                    Release = ActiveRules[entry.Descriptor.Id]
                })
                .Where(entry => !MetadataMatchesDescriptor(entry.Release.Metadata, entry.Analyzer.Descriptor))
                .Select(entry =>
                    $"{entry.Analyzer.Descriptor.Id}: descriptor={FormatDescriptor(entry.Analyzer.Descriptor)}, " +
                    $"release={FormatMetadata(entry.Release.Metadata)} ({entry.Release.Source}:{entry.Release.LineNumber})")
                .OrderBy(entry => entry, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(
                0,
                mismatches.Length,
                "Analyzer release metadata mismatches:" + Environment.NewLine +
                string.Join(Environment.NewLine, mismatches));
        }

        [TestMethod]
        public void UnsupportedSyntaxDiagnostics_ShouldLinkToDocumentedAnchors()
        {
            const string helpLinkBase =
                "https://github.com/neo-project/neo-devpack-dotnet/blob/master-n3/docs/diagnostics/unsupported-syntax.md#";
            var analyzerDirectory = FindAnalyzerDirectory();
            var documentationPath = Path.GetFullPath(Path.Combine(
                analyzerDirectory,
                "..",
                "..",
                "docs",
                "diagnostics",
                "unsupported-syntax.md"));
            var documentation = File.ReadAllText(documentationPath);

            foreach (var descriptor in new UnsupportedSyntaxAnalyzer().SupportedDiagnostics)
            {
                var anchor = descriptor.Id.ToLowerInvariant();
                Assert.AreEqual(helpLinkBase + anchor, descriptor.HelpLinkUri, descriptor.Id);
                StringAssert.Contains(
                    documentation,
                    $"<a id=\"{anchor}\"></a>",
                    $"Missing documentation anchor for {descriptor.Id}.");
            }
        }

        [TestMethod]
        public void RemovedRule_ShouldNotRequireALiveDescriptor()
        {
            var events = ParseReleaseCatalogs(
                new ReleaseCatalog("AnalyzerReleases.Shipped.md", false, Lines("""
                    ## Release 1.0
                    ### New Rules
                    Rule ID | Category | Severity | Notes
                    --------|----------|----------|------
                    NC4998 | Usage | Error | Removed later
                    """)),
                new ReleaseCatalog("AnalyzerReleases.Unshipped.md", true, Lines("""
                    ### Removed Rules
                    Rule ID | Category | Severity | Notes
                    --------|----------|----------|------
                    NC4998 | Usage | Error | No longer reported
                    """)));

            var activeRules = ApplyReleaseEvents(events);

            Assert.IsFalse(activeRules.ContainsKey("NC4998"));
        }

        [TestMethod]
        public void ChangedRule_ShouldUseTheLatestMetadata()
        {
            var events = ParseReleaseCatalogs(
                new ReleaseCatalog("AnalyzerReleases.Shipped.md", false, Lines("""
                    ## Release 2.0
                    ### Changed Rules
                    Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
                    --------|--------------|--------------|--------------|--------------|------
                    NC4997 | Security | Error | Usage | Warning | Updated metadata
                    ## Release 1.0
                    ### New Rules
                    Rule ID | Category | Severity | Notes
                    --------|----------|----------|------
                    NC4997 | Usage | Warning | Initial metadata
                    """)));

            var activeRule = ApplyReleaseEvents(events)["NC4997"];

            Assert.AreEqual("Security", activeRule.Metadata.Category);
            Assert.AreEqual(DiagnosticSeverity.Error, activeRule.Metadata.Severity);
            Assert.IsTrue(activeRule.Metadata.IsEnabledByDefault);
        }

        [TestMethod]
        public void DisabledRule_ShouldTrackEnabledByDefaultState()
        {
            var events = ParseReleaseCatalogs(
                new ReleaseCatalog("AnalyzerReleases.Shipped.md", false, Lines("""
                    ## Release 1.0
                    ### New Rules
                    Rule ID | Category | Severity | Notes
                    --------|----------|----------|------
                    NC4996 | Usage | Disabled | Disabled by default
                    """)));

            var metadata = ApplyReleaseEvents(events)["NC4996"].Metadata;

            Assert.IsFalse(metadata.IsEnabledByDefault);
            Assert.IsNull(metadata.Severity);
        }

        [TestMethod]
        public void DuplicateRuleEventsInOneRelease_ShouldFail()
        {
            var catalog = new ReleaseCatalog("AnalyzerReleases.Unshipped.md", true, Lines("""
                ### New Rules
                Rule ID | Category | Severity | Notes
                --------|----------|----------|------
                NC4995 | Usage | Error | First entry
                NC4995 | Usage | Error | Duplicate entry
                """));

            Assert.ThrowsException<InvalidDataException>(() => ParseReleaseCatalogs(catalog));
        }

        private static IReadOnlyList<AnalyzerDiagnostic> LoadAnalyzerDiagnostics()
        {
            return typeof(FloatUsageAnalyzer).Assembly.GetTypes()
                .Where(IsConcreteAnalyzer)
                .OrderBy(type => type.FullName, StringComparer.Ordinal)
                .Select(CreateAnalyzer)
                .SelectMany(analyzer => analyzer.SupportedDiagnostics.Select(descriptor =>
                    new AnalyzerDiagnostic(analyzer.GetType().Name, descriptor)))
                .ToArray();
        }

        private static bool IsConcreteAnalyzer(Type type) =>
            !type.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(type);

        private static DiagnosticAnalyzer CreateAnalyzer(Type type) =>
            Activator.CreateInstance(type) as DiagnosticAnalyzer
            ?? throw new InvalidOperationException($"Unable to instantiate analyzer '{type.FullName}'.");

        private static IReadOnlyList<ReleaseEvent> LoadReleaseEvents()
        {
            var analyzerDirectory = FindAnalyzerDirectory();
            string[] catalogNames = ["AnalyzerReleases.Shipped.md", "AnalyzerReleases.Unshipped.md"];
            var catalogs = catalogNames
                .Select(name => Path.Combine(analyzerDirectory, name))
                .Where(File.Exists)
                .Select(path => new ReleaseCatalog(
                    Path.GetFileName(path),
                    path.EndsWith(".Unshipped.md", StringComparison.Ordinal),
                    File.ReadAllLines(path)))
                .ToArray();

            return ParseReleaseCatalogs(catalogs);
        }

        private static IReadOnlyList<ReleaseEvent> ParseReleaseCatalogs(params ReleaseCatalog[] catalogs)
        {
            var events = new List<ReleaseEvent>();
            foreach (var catalog in catalogs)
            {
                Version? releaseVersion = null;
                string? releaseName = catalog.IsUnshipped ? "Unshipped" : null;
                ReleaseEventKind? eventKind = null;

                for (var index = 0; index < catalog.Lines.Length; index++)
                {
                    var originalLine = catalog.Lines[index];
                    var line = originalLine.Trim();
                    var releaseMatch = Regex.Match(
                        line,
                        @"^##\s+Release\s+([^\s]+)\s*$",
                        RegexOptions.CultureInvariant);
                    if (releaseMatch.Success)
                    {
                        if (catalog.IsUnshipped)
                        {
                            throw new InvalidDataException(
                                $"Unshipped catalog must not declare a release at {catalog.Source}:{index + 1}.");
                        }

                        releaseName = releaseMatch.Groups[1].Value;
                        releaseVersion = ParseReleaseVersion(releaseName, catalog.Source, index + 1);
                        eventKind = null;
                        continue;
                    }

                    var sectionMatch = Regex.Match(
                        line,
                        @"^###\s+(New|Changed|Removed)\s+Rules\s*$",
                        RegexOptions.CultureInvariant);
                    if (sectionMatch.Success)
                    {
                        eventKind = Enum.Parse<ReleaseEventKind>(sectionMatch.Groups[1].Value);
                        continue;
                    }

                    if (!Regex.IsMatch(line, @"^NC\d{4}\b", RegexOptions.CultureInvariant))
                        continue;

                    if (releaseName is null || eventKind is null)
                    {
                        throw new InvalidDataException(
                            $"Diagnostic entry is outside a release section at {catalog.Source}:{index + 1}: {originalLine}");
                    }

                    events.Add(ParseReleaseEvent(
                        line,
                        eventKind.Value,
                        releaseName,
                        releaseVersion,
                        catalog.IsUnshipped,
                        catalog.Source,
                        index + 1));
                }
            }

            if (events.Count == 0)
                throw new InvalidDataException("The analyzer release catalogs do not contain any diagnostic entries.");

            var conflicts = events
                .GroupBy(entry => (entry.ReleaseName, entry.Id))
                .Where(group => group.Count() > 1)
                .Select(group =>
                    $"{group.Key.Id} in {group.Key.ReleaseName}: " +
                    string.Join(", ", group.Select(entry => $"{entry.Source}:{entry.LineNumber} ({entry.Kind})")))
                .OrderBy(entry => entry, StringComparer.Ordinal)
                .ToArray();
            if (conflicts.Length != 0)
            {
                throw new InvalidDataException(
                    "Conflicting entries in one analyzer release:" + Environment.NewLine +
                    string.Join(Environment.NewLine, conflicts));
            }

            return events;
        }

        private static ReleaseEvent ParseReleaseEvent(
            string line,
            ReleaseEventKind eventKind,
            string releaseName,
            Version? releaseVersion,
            bool isUnshipped,
            string source,
            int lineNumber)
        {
            var columns = line.Split('|').Select(column => column.Trim()).ToArray();
            var expectedColumns = eventKind == ReleaseEventKind.Changed ? 6 : 4;
            if (columns.Length < expectedColumns || !Regex.IsMatch(columns[0], @"^NC\d{4}$", RegexOptions.CultureInvariant))
            {
                throw new InvalidDataException(
                    $"Malformed {eventKind} rule entry at {source}:{lineNumber}: {line}");
            }

            RuleMetadata? newMetadata = null;
            RuleMetadata? previousMetadata = null;
            switch (eventKind)
            {
                case ReleaseEventKind.New:
                    newMetadata = ParseMetadata(columns[1], columns[2], source, lineNumber);
                    break;
                case ReleaseEventKind.Changed:
                    newMetadata = ParseMetadata(columns[1], columns[2], source, lineNumber);
                    previousMetadata = ParseMetadata(columns[3], columns[4], source, lineNumber);
                    break;
                case ReleaseEventKind.Removed:
                    previousMetadata = ParseMetadata(columns[1], columns[2], source, lineNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventKind), eventKind, "Unknown release event kind.");
            }

            return new ReleaseEvent(
                columns[0],
                eventKind,
                newMetadata,
                previousMetadata,
                releaseName,
                releaseVersion,
                isUnshipped,
                source,
                lineNumber);
        }

        private static IReadOnlyDictionary<string, ActiveRule> ApplyReleaseEvents(IReadOnlyList<ReleaseEvent> events)
        {
            var activeRules = new Dictionary<string, ActiveRule>(StringComparer.Ordinal);
            var introducedIds = new HashSet<string>(StringComparer.Ordinal);
            var orderedEvents = events
                .OrderBy(entry => entry.IsUnshipped)
                .ThenBy(entry => entry.ReleaseVersion)
                .ThenBy(entry => entry.LineNumber)
                .ToArray();

            foreach (var releaseEvent in orderedEvents)
            {
                switch (releaseEvent.Kind)
                {
                    case ReleaseEventKind.New:
                        if (!introducedIds.Add(releaseEvent.Id))
                        {
                            throw new InvalidDataException(
                                $"Rule {releaseEvent.Id} is introduced more than once at " +
                                $"{releaseEvent.Source}:{releaseEvent.LineNumber}.");
                        }

                        activeRules.Add(
                            releaseEvent.Id,
                            new ActiveRule(
                                releaseEvent.NewMetadata!,
                                releaseEvent.Source,
                                releaseEvent.LineNumber));
                        break;
                    case ReleaseEventKind.Changed:
                        var changedRule = GetActiveRule(activeRules, releaseEvent);
                        EnsurePreviousMetadataMatches(changedRule.Metadata, releaseEvent);
                        activeRules[releaseEvent.Id] = new ActiveRule(
                            releaseEvent.NewMetadata!,
                            releaseEvent.Source,
                            releaseEvent.LineNumber);
                        break;
                    case ReleaseEventKind.Removed:
                        var removedRule = GetActiveRule(activeRules, releaseEvent);
                        EnsurePreviousMetadataMatches(removedRule.Metadata, releaseEvent);
                        activeRules.Remove(releaseEvent.Id);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return activeRules;
        }

        private static ActiveRule GetActiveRule(
            IReadOnlyDictionary<string, ActiveRule> activeRules,
            ReleaseEvent releaseEvent)
        {
            if (activeRules.TryGetValue(releaseEvent.Id, out var activeRule))
                return activeRule;

            throw new InvalidDataException(
                $"{releaseEvent.Kind} entry for inactive rule {releaseEvent.Id} at " +
                $"{releaseEvent.Source}:{releaseEvent.LineNumber}.");
        }

        private static void EnsurePreviousMetadataMatches(
            RuleMetadata activeMetadata,
            ReleaseEvent releaseEvent)
        {
            if (activeMetadata == releaseEvent.PreviousMetadata)
                return;

            throw new InvalidDataException(
                $"Previous metadata for {releaseEvent.Id} does not match its active metadata at " +
                $"{releaseEvent.Source}:{releaseEvent.LineNumber}: " +
                $"active={FormatMetadata(activeMetadata)}, release={FormatMetadata(releaseEvent.PreviousMetadata!)}.");
        }

        private static RuleMetadata ParseMetadata(
            string category,
            string severity,
            string source,
            int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new InvalidDataException($"Missing rule category at {source}:{lineNumber}.");

            if (severity is "Disabled" or "Disable")
                return new RuleMetadata(category, null, false);

            if (!Enum.TryParse<DiagnosticSeverity>(severity, out var parsedSeverity))
                throw new InvalidDataException($"Invalid rule severity '{severity}' at {source}:{lineNumber}.");

            return new RuleMetadata(category, parsedSeverity, true);
        }

        private static Version ParseReleaseVersion(string value, string source, int lineNumber)
        {
            var coreVersion = value.Split('-', '+')[0];
            var parts = coreVersion.Split('.');
            if (parts.Length is < 2 or > 4 || parts.Any(part => !int.TryParse(part, out _)))
                throw new InvalidDataException($"Invalid release version '{value}' at {source}:{lineNumber}.");

            var numbers = parts.Select(int.Parse).ToArray();
            return new Version(
                numbers[0],
                numbers[1],
                numbers.Length > 2 ? numbers[2] : 0,
                numbers.Length > 3 ? numbers[3] : 0);
        }

        private static bool MetadataMatchesDescriptor(RuleMetadata metadata, DiagnosticDescriptor descriptor) =>
            string.Equals(metadata.Category, descriptor.Category, StringComparison.Ordinal) &&
            metadata.IsEnabledByDefault == descriptor.IsEnabledByDefault &&
            (!metadata.IsEnabledByDefault || metadata.Severity == descriptor.DefaultSeverity);

        private static string FormatDescriptor(DiagnosticDescriptor descriptor) =>
            $"{descriptor.Category}/" +
            (descriptor.IsEnabledByDefault ? descriptor.DefaultSeverity : "Disabled");

        private static string FormatMetadata(RuleMetadata metadata) =>
            $"{metadata.Category}/" +
            (metadata.IsEnabledByDefault ? metadata.Severity : "Disabled");

        private static string[] Lines(string value) =>
            value.Replace("\r", string.Empty, StringComparison.Ordinal).Split('\n');

        private static string FindAnalyzerDirectory()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null)
            {
                var candidateDirectory = Path.Combine(
                    directory.FullName,
                    "src",
                    "Neo.SmartContract.Analyzer");
                var shippedCatalog = Path.Combine(candidateDirectory, "AnalyzerReleases.Shipped.md");
                if (File.Exists(shippedCatalog)) return candidateDirectory;

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Unable to locate the analyzer release catalog directory from the test output directory.");
        }

        private sealed record AnalyzerDiagnostic(string AnalyzerName, DiagnosticDescriptor Descriptor);

        private sealed record ReleaseCatalog(string Source, bool IsUnshipped, string[] Lines);

        private sealed record RuleMetadata(
            string Category,
            DiagnosticSeverity? Severity,
            bool IsEnabledByDefault);

        private sealed record ReleaseEvent(
            string Id,
            ReleaseEventKind Kind,
            RuleMetadata? NewMetadata,
            RuleMetadata? PreviousMetadata,
            string ReleaseName,
            Version? ReleaseVersion,
            bool IsUnshipped,
            string Source,
            int LineNumber);

        private sealed record ActiveRule(RuleMetadata Metadata, string Source, int LineNumber);

        private enum ReleaseEventKind
        {
            New,
            Changed,
            Removed
        }
    }
}
