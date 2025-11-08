using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Removes redundant or provably unnecessary guard instructions and deduplicates identical checks within a block.
/// </summary>
internal sealed class MirGuardOptimizationPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            var nullGuards = new Dictionary<(MirValue Reference, MirGuardFail Fail, MirBlock? Target), MirGuardNull>(new GuardKeyComparer());
            var boundsGuards = new Dictionary<(MirValue Index, MirValue Length, MirGuardFail Fail, MirBlock? Target), MirGuardBounds>(new GuardKeyComparer());

            var instructions = block.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                switch (instructions[i])
                {
                    case MirGuardNull guardNull:
                        {
                            if (IsReferenceStaticallyNonNull(guardNull.Reference))
                            {
                                instructions.RemoveAt(i);
                                changed = true;
                                i--;
                                break;
                            }

                            var key = (guardNull.Reference, guardNull.Fail, guardNull.FailTarget);
                            if (nullGuards.ContainsKey(key))
                            {
                                instructions.RemoveAt(i);
                                changed = true;
                                i--;
                            }
                            else
                            {
                                nullGuards[key] = guardNull;
                            }
                        }
                        break;

                    case MirGuardBounds guardBounds:
                        {
                            if (TryGetInt(guardBounds.Index, out var index) &&
                                TryGetInt(guardBounds.Length, out var length) &&
                                index >= BigInteger.Zero && index < length)
                            {
                                instructions.RemoveAt(i);
                                changed = true;
                                i--;
                                break;
                            }

                            var key = (guardBounds.Index, guardBounds.Length, guardBounds.Fail, guardBounds.FailTarget);
                            if (boundsGuards.ContainsKey(key))
                            {
                                instructions.RemoveAt(i);
                                changed = true;
                                i--;
                            }
                            else
                            {
                                boundsGuards[key] = guardBounds;
                            }
                        }
                        break;
                }
            }
        }

        return changed;
    }

    private static bool IsReferenceStaticallyNonNull(MirValue value)
    {
        return value switch
        {
            MirConstInt => true,
            MirConstBool => true,
            MirConstByteString => true,
            MirConstBuffer => true,
            _ => false
        };
    }

    private static bool TryGetInt(MirValue value, out BigInteger constant)
    {
        switch (value)
        {
            case MirConstInt constInt:
                constant = constInt.Value;
                return true;
        }

        constant = default;
        return false;
    }

    private sealed class GuardKeyComparer :
        IEqualityComparer<(MirValue Reference, MirGuardFail Fail, MirBlock? Target)>,
        IEqualityComparer<(MirValue Index, MirValue Length, MirGuardFail Fail, MirBlock? Target)>
    {
        public bool Equals((MirValue Reference, MirGuardFail Fail, MirBlock? Target) x, (MirValue Reference, MirGuardFail Fail, MirBlock? Target) y)
        {
            return ReferenceEquals(x.Reference, y.Reference) && x.Fail == y.Fail && ReferenceEquals(x.Target, y.Target);
        }

        public int GetHashCode((MirValue Reference, MirGuardFail Fail, MirBlock? Target) obj)
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + RuntimeHelpers.GetHashCode(obj.Reference);
                hash = hash * 31 + (int)obj.Fail;
                hash = hash * 31 + (obj.Target is null ? 0 : RuntimeHelpers.GetHashCode(obj.Target));
                return hash;
            }
        }

        public bool Equals((MirValue Index, MirValue Length, MirGuardFail Fail, MirBlock? Target) x, (MirValue Index, MirValue Length, MirGuardFail Fail, MirBlock? Target) y)
        {
            return ReferenceEquals(x.Index, y.Index)
                && ReferenceEquals(x.Length, y.Length)
                && x.Fail == y.Fail
                && ReferenceEquals(x.Target, y.Target);
        }

        public int GetHashCode((MirValue Index, MirValue Length, MirGuardFail Fail, MirBlock? Target) obj)
        {
            unchecked
            {
                var hash = 23;
                hash = hash * 31 + RuntimeHelpers.GetHashCode(obj.Index);
                hash = hash * 31 + RuntimeHelpers.GetHashCode(obj.Length);
                hash = hash * 31 + (int)obj.Fail;
                hash = hash * 31 + (obj.Target is null ? 0 : RuntimeHelpers.GetHashCode(obj.Target));
                return hash;
            }
        }
    }
}
