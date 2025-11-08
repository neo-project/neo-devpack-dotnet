using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.HIR.Optimization;

/// <summary>
/// Eliminates redundant null and bounds guards that are dominated by identical checks along every incoming path.
/// </summary>
internal sealed class HirGuardHoistingPass : IHirPass
{
    private readonly struct GuardSignature : IEquatable<GuardSignature>
    {
        private readonly GuardKind _kind;
        private readonly HirValue _primary;
        private readonly HirValue? _secondary;
        private readonly HirFailPolicy _policy;

        internal GuardSignature(GuardKind kind, HirValue primary, HirValue? secondary, HirFailPolicy policy)
        {
            _kind = kind;
            _primary = primary;
            _secondary = secondary;
            _policy = policy;
        }

        public bool Equals(GuardSignature other)
        {
            return _kind == other._kind
                && ReferenceEquals(_primary, other._primary)
                && ReferenceEquals(_secondary, other._secondary)
                && _policy == other._policy;
        }

        public override bool Equals(object? obj) => obj is GuardSignature other && Equals(other);

        public override int GetHashCode()
        {
            var hash = (int)_kind;
            hash = (hash * 397) ^ RuntimeHelpers.GetHashCode(_primary);
            hash = (hash * 397) ^ (_secondary is null ? 0 : RuntimeHelpers.GetHashCode(_secondary));
            hash = (hash * 397) ^ (int)_policy;
            return hash;
        }
    }

    private enum GuardKind : byte
    {
        Null,
        Bounds
    }

    public bool Run(HirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var predecessors = HirControlFlow.BuildPredecessors(function);
        var inSets = new Dictionary<HirBlock, HashSet<GuardSignature>>();
        var outSets = new Dictionary<HirBlock, HashSet<GuardSignature>>();

        foreach (var block in function.Blocks)
        {
            inSets[block] = new HashSet<GuardSignature>();
            outSets[block] = new HashSet<GuardSignature>();
        }

        var modified = false;
        var changed = true;

        while (changed)
        {
            changed = false;

            foreach (var block in function.Blocks)
            {
                var entry = ComputeEntrySet(block, predecessors, outSets);
                if (!SetEquals(inSets[block], entry))
                {
                    inSets[block] = entry;
                    changed = true;
                }

                var exit = new HashSet<GuardSignature>(entry);
                for (int i = 0; i < block.Instructions.Count; i++)
                {
                    switch (block.Instructions[i])
                    {
                        case HirNullCheck nullCheck when nullCheck.Policy != HirFailPolicy.Assume:
                            {
                                var signature = new GuardSignature(GuardKind.Null, nullCheck.Reference, null, nullCheck.Policy);
                                if (exit.Contains(signature))
                                {
                                    block.RemoveInstructionAt(i);
                                    i--;
                                    modified = true;
                                    continue;
                                }

                                exit.Add(signature);
                                break;
                            }

                        case HirBoundsCheck boundsCheck when boundsCheck.Policy != HirFailPolicy.Assume:
                            {
                                var signature = new GuardSignature(GuardKind.Bounds, boundsCheck.Index, boundsCheck.Length, boundsCheck.Policy);
                                if (exit.Contains(signature))
                                {
                                    block.RemoveInstructionAt(i);
                                    i--;
                                    modified = true;
                                    continue;
                                }

                                exit.Add(signature);
                                break;
                            }
                    }
                }

                if (!SetEquals(outSets[block], exit))
                {
                    outSets[block] = exit;
                    changed = true;
                }
            }
        }

        return modified;
    }

    private static HashSet<GuardSignature> ComputeEntrySet(
        HirBlock block,
        Dictionary<HirBlock, HashSet<HirBlock>> predecessors,
        Dictionary<HirBlock, HashSet<GuardSignature>> outSets)
    {
        if (!predecessors.TryGetValue(block, out var preds) || preds.Count == 0)
            return new HashSet<GuardSignature>();

        HashSet<GuardSignature>? result = null;
        foreach (var pred in preds)
        {
            if (!outSets.TryGetValue(pred, out var predSet))
                continue;

            if (result is null)
            {
                result = new HashSet<GuardSignature>(predSet);
            }
            else
            {
                result.IntersectWith(predSet);
            }
        }

        return result ?? new HashSet<GuardSignature>();
    }

    private static bool SetEquals(HashSet<GuardSignature> left, HashSet<GuardSignature> right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left.Count != right.Count)
            return false;
        foreach (var entry in left)
        {
            if (!right.Contains(entry))
                return false;
        }

        return true;
    }
}
