using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.MIR.Optimization;

/// <summary>
/// Tracks per-block container versions to eliminate redundant length and membership queries across maps and arrays.
/// </summary>
internal sealed class MirContainerVersioningPass : IMirPass
{
    public bool Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var changed = false;

        foreach (var block in function.Blocks)
        {
            var arrayLenCache = new Dictionary<MirValue, MirValue>(ReferenceComparer.Instance);
            var mapLenCache = new Dictionary<MirValue, MirValue>(ReferenceComparer.Instance);
            var mapHasCache = new Dictionary<(MirValue Map, MirValue Key), MirValue>(new MapHasComparer());

            var instructions = block.Instructions;
            for (int i = 0; i < instructions.Count; i++)
            {
                var inst = instructions[i];
                switch (inst)
                {
                    case MirArrayLen arrayLen:
                        if (arrayLenCache.TryGetValue(arrayLen.Array, out var cachedArrayLen))
                        {
                            instructions.RemoveAt(i);
                            MirValueRewriter.Replace(function, arrayLen, cachedArrayLen);
                            i--;
                            changed = true;
                        }
                        else
                        {
                            arrayLenCache[arrayLen.Array] = arrayLen;
                        }
                        break;

                    case MirArraySet arraySet:
                        arrayLenCache.Remove(arraySet.Array);
                        break;

                    case MirMapLen mapLen:
                        if (mapLenCache.TryGetValue(mapLen.Map, out var cachedMapLen))
                        {
                            instructions.RemoveAt(i);
                            MirValueRewriter.Replace(function, mapLen, cachedMapLen);
                            i--;
                            changed = true;
                        }
                        else
                        {
                            mapLenCache[mapLen.Map] = mapLen;
                        }
                        break;

                    case MirMapHas mapHas:
                        var key = (mapHas.Map, mapHas.Key);
                        if (mapHasCache.TryGetValue(key, out var cachedHas))
                        {
                            instructions.RemoveAt(i);
                            MirValueRewriter.Replace(function, mapHas, cachedHas);
                            i--;
                            changed = true;
                        }
                        else
                        {
                            mapHasCache[key] = mapHas;
                        }
                        break;

                    case MirMapSet mapSet:
                        mapLenCache.Remove(mapSet.Map);
                        InvalidateMapHas(mapHasCache, mapSet.Map);
                        break;

                    case MirMapDelete mapDelete:
                        mapLenCache.Remove(mapDelete.Map);
                        InvalidateMapHas(mapHasCache, mapDelete.Map);
                        break;

                    case MirMapNew mapNew:
                        mapLenCache.Clear();
                        mapHasCache.Clear();
                        break;

                    default:
                        if (IsStorageBarrier(inst))
                        {
                            arrayLenCache.Clear();
                            mapLenCache.Clear();
                            mapHasCache.Clear();
                        }
                        break;
                }
            }
        }

        return changed;
    }

    private static void InvalidateMapHas(Dictionary<(MirValue Map, MirValue Key), MirValue> cache, MirValue map)
    {
        var keysToRemove = new List<(MirValue, MirValue)>();
        foreach (var entry in cache.Keys)
        {
            if (ReferenceEquals(entry.Map, map))
                keysToRemove.Add(entry);
        }

        foreach (var key in keysToRemove)
            cache.Remove(key);
    }

    private sealed class MapHasComparer : IEqualityComparer<(MirValue Map, MirValue Key)>
    {
        public bool Equals((MirValue Map, MirValue Key) x, (MirValue Map, MirValue Key) y)
        {
            return ReferenceEquals(x.Map, y.Map) && ReferenceEquals(x.Key, y.Key);
        }

        public int GetHashCode((MirValue Map, MirValue Key) obj)
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + RuntimeHelpers.GetHashCode(obj.Map);
                hash = hash * 31 + RuntimeHelpers.GetHashCode(obj.Key);
                return hash;
            }
        }
    }

    private sealed class ReferenceComparer : IEqualityComparer<MirValue>
    {
        internal static ReferenceComparer Instance { get; } = new ReferenceComparer();

        public bool Equals(MirValue? x, MirValue? y) => ReferenceEquals(x, y);

        public int GetHashCode(MirValue obj) => obj is null ? 0 : RuntimeHelpers.GetHashCode(obj);
    }

    private static bool IsStorageBarrier(MirInst inst)
    {
        if (inst is null)
            return false;

        if ((inst.Effect & MirEffect.StorageWrite) != MirEffect.None)
            return true;
        if (inst is MirSyscall syscall && (syscall.Effect & MirEffect.StorageWrite) != MirEffect.None)
            return true;

        return false;
    }
}
