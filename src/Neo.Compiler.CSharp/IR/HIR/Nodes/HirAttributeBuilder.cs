using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Neo.Compiler;

namespace Neo.Compiler.HIR;

internal static class HirAttributeBuilder
{
    private const string SafeAttributeName = "Neo.SmartContract.Framework.Attributes.SafeAttribute";
    private const string DisplayNameAttributeName = "System.ComponentModel.DisplayNameAttribute";
    private const string SyscallAttributeName = "Neo.SmartContract.Framework.Attributes.SyscallAttribute";
    private const string NoReentrantAttributeName = "Neo.SmartContract.Framework.Attributes.NoReentrantAttribute";
    private const string NoReentrantMethodAttributeName = "Neo.SmartContract.Framework.Attributes.NoReentrantMethodAttribute";
    private const string MethodImplAttributeName = "System.Runtime.CompilerServices.MethodImplAttribute";

    internal static IReadOnlyList<HirAttribute> Collect(CompilationOptions options, IMethodSymbol symbol)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));
        if (symbol is null)
            throw new ArgumentNullException(nameof(symbol));

        var attributes = new List<HirAttribute>();
        var duplicateGuard = new HashSet<string>(StringComparer.Ordinal);

        TryAddDisplayName(symbol, attributes, duplicateGuard);
        TryAddDisplayName(symbol.AssociatedSymbol, attributes, duplicateGuard);

        if (HasAttribute(symbol, SafeAttributeName) || HasAttribute(symbol.AssociatedSymbol, SafeAttributeName))
        {
            attributes.Add(new HirSafeAttribute());
            duplicateGuard.Add(nameof(HirSafeAttribute));
        }

        AppendSyscallTags(symbol, attributes, duplicateGuard);
        AppendSyscallTags(symbol.AssociatedSymbol, attributes, duplicateGuard);
        AppendNoReentrantAttributes(symbol, attributes, duplicateGuard);
        AppendNoReentrantAttributes(symbol.AssociatedSymbol, attributes, duplicateGuard);
        AppendInlineAttributes(symbol, attributes, duplicateGuard);
        AppendInlineAttributes(symbol.AssociatedSymbol, attributes, duplicateGuard);

        return attributes.Count == 0 ? Array.Empty<HirAttribute>() : attributes;
    }

    private static void TryAddDisplayName(ISymbol? symbol, List<HirAttribute> attributes, HashSet<string> duplicateGuard)
    {
        if (symbol is null)
            return;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null)
                continue;

            if (!string.Equals(
                    TrimGlobal(attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                    DisplayNameAttributeName,
                    StringComparison.Ordinal))
            {
                continue;
            }

            if (attribute.ConstructorArguments.Length == 0)
                continue;

            var name = attribute.ConstructorArguments[0].Value as string;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var guardKey = $"displayName:{name}";
            if (!duplicateGuard.Add(guardKey))
                return;

            attributes.Add(new HirMethodNameAttribute(name));
            break;
        }
    }

    private static bool HasAttribute(ISymbol? symbol, string fullyQualifiedName)
    {
        if (symbol is null)
            return false;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null)
                continue;

            var attributeName = TrimGlobal(attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            if (string.Equals(attributeName, fullyQualifiedName, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private static void AppendSyscallTags(
        ISymbol? symbol,
        List<HirAttribute> attributes,
        HashSet<string> duplicateGuard)
    {
        if (symbol is null)
            return;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null ||
                attribute.ConstructorArguments.Length == 0)
            {
                continue;
            }

            var attributeName = TrimGlobal(attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            if (!string.Equals(attributeName, SyscallAttributeName, StringComparison.Ordinal))
                continue;

            if (attribute.ConstructorArguments[0].Value is not string syscallName)
                continue;

            if (!HirIntrinsicCatalog.TryResolveSyscall(syscallName, out var metadata))
                continue;

            var guardKey = $"intrinsic:{metadata.Category}.{metadata.Name}";
            if (!duplicateGuard.Add(guardKey))
                continue;

            attributes.Add(new HirIntrinsicTagAttribute(metadata.Category, metadata.Name));
        }
    }

    private static void AppendNoReentrantAttributes(
        ISymbol? symbol,
        List<HirAttribute> attributes,
        HashSet<string> duplicateGuard)
    {
        if (symbol is null)
            return;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null)
                continue;

            var attributeName = TrimGlobal(attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            if (attributeName != NoReentrantAttributeName && attributeName != NoReentrantMethodAttributeName)
                continue;

            byte prefix = 0xFF;
            string key = "noReentrant";

            if (attribute.ConstructorArguments.Length > 0 &&
                attribute.ConstructorArguments[0].Value is byte prefixValue)
            {
                prefix = prefixValue;
            }

            if (attribute.ConstructorArguments.Length > 1 &&
                attribute.ConstructorArguments[1].Value is string keyValue &&
                !string.IsNullOrWhiteSpace(keyValue))
            {
                key = keyValue;
            }

            var guardKey = $"noreentrant:{prefix}:{key}";
            if (!duplicateGuard.Add(guardKey))
                continue;

            attributes.Add(new HirNoReentrantAttribute(prefix, key));
        }
    }

    private static void AppendInlineAttributes(
        ISymbol? symbol,
        List<HirAttribute> attributes,
        HashSet<string> duplicateGuard)
    {
        if (symbol is null)
            return;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null)
                continue;

            var attributeName = TrimGlobal(attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            if (!string.Equals(attributeName, MethodImplAttributeName, StringComparison.Ordinal))
                continue;

            var options = ExtractMethodImplOptions(attribute);
            if (options is null)
                continue;

            var aggressive = (options.Value & System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining) != 0;
            if (!aggressive)
                continue;

            if (!duplicateGuard.Add("inline:aggressive"))
                continue;

            attributes.Add(new HirInlineAttribute(true));
        }
    }

    private static System.Runtime.CompilerServices.MethodImplOptions? ExtractMethodImplOptions(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is int ctorValue)
            return (System.Runtime.CompilerServices.MethodImplOptions)ctorValue;

        foreach (var (name, typedValue) in attribute.NamedArguments)
        {
            if (string.Equals(name, "Value", StringComparison.Ordinal) && typedValue.Value is int namedValue)
                return (System.Runtime.CompilerServices.MethodImplOptions)namedValue;
        }

        return null;
    }

    private static string TrimGlobal(string value)
        => value.StartsWith("global::", StringComparison.Ordinal) ? value[8..] : value;
}
