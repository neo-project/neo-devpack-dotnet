// Copyright (C) 2015-2026 The Neo Project.
//
// ArtifactDiffReporter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler
{
    public enum ArtifactDiffSeverity
    {
        Info,
        Warning,
        Breaking
    }

    public enum ArtifactDiffCategory
    {
        Nef,
        Manifest,
        Abi,
        Permission,
        Standard
    }

    public sealed class ArtifactDiffChange
    {
        public ArtifactDiffSeverity Severity { get; }
        public ArtifactDiffCategory Category { get; }
        public string Description { get; }

        public ArtifactDiffChange(ArtifactDiffSeverity severity, ArtifactDiffCategory category, string description)
        {
            Severity = severity;
            Category = category;
            Description = description;
        }
    }

    public sealed class ArtifactDiffReport
    {
        public IReadOnlyList<ArtifactDiffChange> Changes { get; }
        public bool HasBreakingChanges => Changes.Any(u => u.Severity == ArtifactDiffSeverity.Breaking);
        public bool HasWarnings => Changes.Any(u => u.Severity == ArtifactDiffSeverity.Warning);

        public ArtifactDiffReport(IEnumerable<ArtifactDiffChange> changes)
        {
            Changes = changes.ToArray();
        }
    }

    public static class ArtifactDiffReporter
    {
        private const int ColumnWidth = 24;
        private const string Separator = "------------------------------------------------------------";

        public static ArtifactDiffReport Compare(ContractManifest oldManifest, ContractManifest newManifest)
        {
            return Compare(null, oldManifest, null, newManifest);
        }

        public static ArtifactDiffReport Compare(NefFile? oldNef, ContractManifest oldManifest, NefFile? newNef, ContractManifest newManifest)
        {
            ArgumentNullException.ThrowIfNull(oldManifest);
            ArgumentNullException.ThrowIfNull(newManifest);

            List<ArtifactDiffChange> changes = [];

            CompareNef(oldNef, newNef, changes);
            CompareManifest(oldManifest, newManifest, changes);
            CompareMethods(oldManifest.Abi.Methods, newManifest.Abi.Methods, changes);
            CompareEvents(oldManifest.Abi.Events, newManifest.Abi.Events, changes);
            CompareStandards(oldManifest.SupportedStandards, newManifest.SupportedStandards, changes);
            ComparePermissions(oldManifest.Permissions, newManifest.Permissions, changes);

            return new ArtifactDiffReport(changes);
        }

        public static void Print(ArtifactDiffReport report, TextWriter output)
        {
            ArgumentNullException.ThrowIfNull(report);
            ArgumentNullException.ThrowIfNull(output);

            output.WriteLine();
            output.WriteLine("Artifact Diff Report");
            output.WriteLine(Separator);
            output.WriteLine($"{"Breaking changes:",-ColumnWidth}{FormatYesNo(report.HasBreakingChanges)}");
            output.WriteLine($"{"Warnings:",-ColumnWidth}{FormatYesNo(report.HasWarnings)}");
            output.WriteLine($"{"Total changes:",-ColumnWidth}{report.Changes.Count}");
            output.WriteLine();

            if (report.Changes.Count == 0)
            {
                output.WriteLine("No artifact changes detected.");
                output.WriteLine();
                return;
            }

            foreach (ArtifactDiffChange change in report.Changes
                         .OrderByDescending(u => u.Severity)
                         .ThenBy(u => u.Category)
                         .ThenBy(u => u.Description, StringComparer.Ordinal))
            {
                output.WriteLine($"[{FormatSeverity(change.Severity)}] {FormatCategory(change.Category)}: {change.Description}");
            }
            output.WriteLine();
        }

        private static void CompareNef(NefFile? oldNef, NefFile? newNef, List<ArtifactDiffChange> changes)
        {
            if (oldNef is null || newNef is null)
                return;

            if (oldNef.CheckSum != newNef.CheckSum)
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Nef, $"Checksum changed: {FormatChecksum(oldNef.CheckSum)} => {FormatChecksum(newNef.CheckSum)}"));

            if (!oldNef.Script.Span.SequenceEqual(newNef.Script.Span))
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Nef, $"Script size changed: {oldNef.Script.Length} bytes => {newNef.Script.Length} bytes"));

            if (!string.Equals(oldNef.Compiler, newNef.Compiler, StringComparison.Ordinal))
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Nef, $"Compiler changed: {oldNef.Compiler} => {newNef.Compiler}"));
        }

        private static void CompareManifest(ContractManifest oldManifest, ContractManifest newManifest, List<ArtifactDiffChange> changes)
        {
            if (!string.Equals(oldManifest.Name, newManifest.Name, StringComparison.Ordinal))
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Warning, ArtifactDiffCategory.Manifest, $"Name changed: {oldManifest.Name} => {newManifest.Name}"));
        }

        private static void CompareMethods(ContractMethodDescriptor[] oldMethods, ContractMethodDescriptor[] newMethods, List<ArtifactDiffChange> changes)
        {
            Dictionary<string, ContractMethodDescriptor[]> oldByName = GroupMethodsByName(oldMethods);
            Dictionary<string, ContractMethodDescriptor[]> newByName = GroupMethodsByName(newMethods);

            foreach (string removed in oldByName.Keys.Except(newByName.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                foreach (ContractMethodDescriptor method in oldByName[removed].OrderBy(FormatMethod, StringComparer.Ordinal))
                    changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Breaking, ArtifactDiffCategory.Abi, $"Removed method: {FormatMethod(method)}"));
            }

            foreach (string added in newByName.Keys.Except(oldByName.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                foreach (ContractMethodDescriptor method in newByName[added].OrderBy(FormatMethod, StringComparer.Ordinal))
                    changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Abi, $"Added method: {FormatMethod(method)}"));
            }

            foreach (string name in oldByName.Keys.Intersect(newByName.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                ContractMethodDescriptor[] oldGroup = oldByName[name];
                ContractMethodDescriptor[] newGroup = newByName[name];
                if (oldGroup.Length != 1 || newGroup.Length != 1)
                {
                    CompareOverloadedMethods(oldGroup, newGroup, changes);
                    continue;
                }

                ContractMethodDescriptor oldMethod = oldGroup[0];
                ContractMethodDescriptor newMethod = newGroup[0];
                if (!SameMethodSignature(oldMethod, newMethod))
                {
                    changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Breaking, ArtifactDiffCategory.Abi, $"Changed method signature: {FormatMethod(oldMethod)} => {FormatMethod(newMethod)}"));
                }
                else if (oldMethod.Safe != newMethod.Safe)
                {
                    ArtifactDiffSeverity severity = oldMethod.Safe ? ArtifactDiffSeverity.Warning : ArtifactDiffSeverity.Info;
                    changes.Add(new ArtifactDiffChange(severity, ArtifactDiffCategory.Abi, $"Changed safe flag: {name}: {FormatYesNo(oldMethod.Safe)} => {FormatYesNo(newMethod.Safe)}"));
                }
            }
        }

        private static void CompareEvents(ContractEventDescriptor[] oldEvents, ContractEventDescriptor[] newEvents, List<ArtifactDiffChange> changes)
        {
            Dictionary<string, ContractEventDescriptor[]> oldByName = GroupEventsByName(oldEvents);
            Dictionary<string, ContractEventDescriptor[]> newByName = GroupEventsByName(newEvents);

            foreach (string removed in oldByName.Keys.Except(newByName.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                foreach (ContractEventDescriptor ev in oldByName[removed].OrderBy(FormatEvent, StringComparer.Ordinal))
                    changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Breaking, ArtifactDiffCategory.Abi, $"Removed event: {FormatEvent(ev)}"));
            }

            foreach (string added in newByName.Keys.Except(oldByName.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                foreach (ContractEventDescriptor ev in newByName[added].OrderBy(FormatEvent, StringComparer.Ordinal))
                    changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Abi, $"Added event: {FormatEvent(ev)}"));
            }

            foreach (string name in oldByName.Keys.Intersect(newByName.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                ContractEventDescriptor[] oldGroup = oldByName[name];
                ContractEventDescriptor[] newGroup = newByName[name];
                if (oldGroup.Length != 1 || newGroup.Length != 1)
                {
                    CompareOverloadedEvents(oldGroup, newGroup, changes);
                    continue;
                }

                ContractEventDescriptor oldEvent = oldGroup[0];
                ContractEventDescriptor newEvent = newGroup[0];
                if (!SameParameters(oldEvent.Parameters, newEvent.Parameters))
                    changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Breaking, ArtifactDiffCategory.Abi, $"Changed event signature: {FormatEvent(oldEvent)} => {FormatEvent(newEvent)}"));
            }
        }

        private static void CompareOverloadedMethods(ContractMethodDescriptor[] oldMethods, ContractMethodDescriptor[] newMethods, List<ArtifactDiffChange> changes)
        {
            Dictionary<string, ContractMethodDescriptor> oldByParameters = GroupMethodsByParameterTypes(oldMethods);
            Dictionary<string, ContractMethodDescriptor> newByParameters = GroupMethodsByParameterTypes(newMethods);

            foreach (string removed in oldByParameters.Keys.Except(newByParameters.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Breaking, ArtifactDiffCategory.Abi, $"Removed method: {FormatMethod(oldByParameters[removed])}"));
            }

            foreach (string added in newByParameters.Keys.Except(oldByParameters.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Abi, $"Added method: {FormatMethod(newByParameters[added])}"));
            }

            foreach (string key in oldByParameters.Keys.Intersect(newByParameters.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                ContractMethodDescriptor oldMethod = oldByParameters[key];
                ContractMethodDescriptor newMethod = newByParameters[key];
                if (oldMethod.ReturnType != newMethod.ReturnType)
                {
                    changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Breaking, ArtifactDiffCategory.Abi, $"Changed method signature: {FormatMethod(oldMethod)} => {FormatMethod(newMethod)}"));
                }
                else if (oldMethod.Safe != newMethod.Safe)
                {
                    ArtifactDiffSeverity severity = oldMethod.Safe ? ArtifactDiffSeverity.Warning : ArtifactDiffSeverity.Info;
                    changes.Add(new ArtifactDiffChange(severity, ArtifactDiffCategory.Abi, $"Changed safe flag: {FormatMethodIdentity(oldMethod)}: {FormatYesNo(oldMethod.Safe)} => {FormatYesNo(newMethod.Safe)}"));
                }
            }
        }

        private static void CompareOverloadedEvents(ContractEventDescriptor[] oldEvents, ContractEventDescriptor[] newEvents, List<ArtifactDiffChange> changes)
        {
            Dictionary<string, ContractEventDescriptor> oldByParameters = GroupEventsByParameterTypes(oldEvents);
            Dictionary<string, ContractEventDescriptor> newByParameters = GroupEventsByParameterTypes(newEvents);

            foreach (string removed in oldByParameters.Keys.Except(newByParameters.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Breaking, ArtifactDiffCategory.Abi, $"Removed event: {FormatEvent(oldByParameters[removed])}"));
            }

            foreach (string added in newByParameters.Keys.Except(oldByParameters.Keys, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Abi, $"Added event: {FormatEvent(newByParameters[added])}"));
            }
        }

        private static Dictionary<string, ContractMethodDescriptor[]> GroupMethodsByName(ContractMethodDescriptor[] methods)
        {
            return methods
                .GroupBy(u => u.Name, StringComparer.Ordinal)
                .ToDictionary(u => u.Key, u => u.ToArray(), StringComparer.Ordinal);
        }

        private static Dictionary<string, ContractEventDescriptor[]> GroupEventsByName(ContractEventDescriptor[] events)
        {
            return events
                .GroupBy(u => u.Name, StringComparer.Ordinal)
                .ToDictionary(u => u.Key, u => u.ToArray(), StringComparer.Ordinal);
        }

        private static Dictionary<string, ContractMethodDescriptor> GroupMethodsByParameterTypes(ContractMethodDescriptor[] methods)
        {
            return methods
                .GroupBy(FormatParameterKey, StringComparer.Ordinal)
                .ToDictionary(u => u.Key, u => u.First(), StringComparer.Ordinal);
        }

        private static Dictionary<string, ContractEventDescriptor> GroupEventsByParameterTypes(ContractEventDescriptor[] events)
        {
            return events
                .GroupBy(FormatParameterKey, StringComparer.Ordinal)
                .ToDictionary(u => u.Key, u => u.First(), StringComparer.Ordinal);
        }

        private static void CompareStandards(string[] oldStandards, string[] newStandards, List<ArtifactDiffChange> changes)
        {
            foreach (string removed in oldStandards.Except(newStandards, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Warning, ArtifactDiffCategory.Standard, $"Removed supported standard: {removed}"));
            }

            foreach (string added in newStandards.Except(oldStandards, StringComparer.Ordinal).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Standard, $"Added supported standard: {added}"));
            }
        }

        private static void ComparePermissions(ContractPermission[] oldPermissions, ContractPermission[] newPermissions, List<ArtifactDiffChange> changes)
        {
            SortedSet<string> oldSet = new(oldPermissions.Select(FormatPermission), StringComparer.Ordinal);
            SortedSet<string> newSet = new(newPermissions.Select(FormatPermission), StringComparer.Ordinal);

            foreach (string removed in oldSet.Except(newSet).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Info, ArtifactDiffCategory.Permission, $"Removed permission: {removed}"));
            }

            foreach (string added in newSet.Except(oldSet).OrderBy(u => u, StringComparer.Ordinal))
            {
                changes.Add(new ArtifactDiffChange(ArtifactDiffSeverity.Warning, ArtifactDiffCategory.Permission, $"Added permission: {added}"));
            }
        }

        private static bool SameMethodSignature(ContractMethodDescriptor oldMethod, ContractMethodDescriptor newMethod)
        {
            return oldMethod.ReturnType == newMethod.ReturnType &&
                   SameParameters(oldMethod.Parameters, newMethod.Parameters);
        }

        private static bool SameParameters(ContractParameterDefinition[] oldParameters, ContractParameterDefinition[] newParameters)
        {
            if (oldParameters.Length != newParameters.Length)
                return false;

            for (int i = 0; i < oldParameters.Length; i++)
            {
                if (oldParameters[i].Type != newParameters[i].Type)
                    return false;
            }

            return true;
        }

        private static string FormatMethod(ContractMethodDescriptor method)
        {
            return $"{method.Name}({FormatParameters(method.Parameters)}) -> {method.ReturnType}";
        }

        private static string FormatMethodIdentity(ContractMethodDescriptor method)
        {
            return $"{method.Name}({FormatParameters(method.Parameters)})";
        }

        private static string FormatEvent(ContractEventDescriptor ev)
        {
            return $"{ev.Name}({FormatParameters(ev.Parameters)})";
        }

        private static string FormatParameterKey(ContractMethodDescriptor method)
        {
            return FormatParameters(method.Parameters);
        }

        private static string FormatParameterKey(ContractEventDescriptor ev)
        {
            return FormatParameters(ev.Parameters);
        }

        private static string FormatParameters(ContractParameterDefinition[] parameters)
        {
            return parameters.Length == 0
                ? string.Empty
                : string.Join(", ", parameters.Select(u => u.Type.ToString()));
        }

        private static string FormatPermission(ContractPermission permission)
        {
            string contract = permission.Contract.ToJson().AsString();
            string methods = permission.Methods.IsWildcard
                ? "*"
                : string.Join(",", permission.Methods.OrderBy(u => u, StringComparer.Ordinal));
            return $"{contract}::{methods}";
        }

        private static string FormatChecksum(uint checksum)
        {
            return $"0x{checksum:x8}";
        }

        private static string FormatYesNo(bool value)
        {
            return value ? "yes" : "no";
        }

        private static string FormatSeverity(ArtifactDiffSeverity severity)
        {
            return severity.ToString().ToLowerInvariant();
        }

        private static string FormatCategory(ArtifactDiffCategory category)
        {
            return category == ArtifactDiffCategory.Abi ? "ABI" : category.ToString();
        }
    }
}
