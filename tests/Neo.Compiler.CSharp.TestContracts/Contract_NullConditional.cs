using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_NullConditional : SmartContract.Framework.SmartContract
{
    private class Node
    {
        public Node? Child { get; set; }
        public Node? Sibling;
        public static Node? StaticChild { get; set; }
        public int Count { get; set; }
        public int Value;
        public int[]? Values;
        public Node? MaybeChild { get; set; }
        public Node? MaybeSibling;
        public int? MaybeCount { get; set; }
        public int? MaybeValue;
        public int?[]? MaybeValues;
    }

    private class IndexerBox
    {
        private readonly int[] _values = new[] { 3 };

        public int this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }
    }

    private class NullableIndexerBox
    {
        private readonly int?[] _values;

        public NullableIndexerBox(bool seedElement)
        {
            _values = new int?[] { seedElement ? 3 : null };
        }

        public int? this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }
    }

    private static int SideEffects;

    private static Node CreateNodeWithSideEffect()
    {
        SideEffects += 1;
        return new Node();
    }

    private static int CreateValueWithSideEffect()
    {
        SideEffects += 1;
        return 7;
    }

    private static int CreateIndexWithSideEffect()
    {
        SideEffects += 1;
        return 0;
    }

    public static int AssignChild(bool createNode)
    {
        Node? node = createNode ? new Node() : null;
        var assigned = node?.Child = new Node();
        return assigned is null ? 0 : 1;
    }

    public static int AssignSibling(bool createNode)
    {
        Node? node = createNode ? new Node() : null;
        node?.Sibling = new Node();
        return node?.Sibling is null ? 0 : 1;
    }

    public static int AssignStatic(bool createNode)
    {
        Node.StaticChild = createNode ? new Node() : null;
        Node.StaticChild?.Child = new Node();
        return Node.StaticChild?.Child is null ? 0 : 1;
    }

    public static int AssignGrandChild(bool createRoot, bool createChild)
    {
        Node? root = createRoot
            ? new Node { Child = createChild ? new Node() : null }
            : null;
        var assigned = root?.Child?.Child = new Node();
        return assigned is null ? 0 : 1;
    }

    public static int AssignSiblingFromOther(bool seedLeft, bool seedRight)
    {
        Node? left = seedLeft ? new Node { Sibling = new Node() } : null;
        Node? right = seedRight ? new Node() : null;
        var assigned = left?.Child = right?.Sibling = new Node();
        return assigned is null ? 0 : 1;
    }

    public static int AssignChildSideEffects(bool createNode)
    {
        SideEffects = 0;
        Node? node = createNode ? new Node() : null;
        node?.Child = CreateNodeWithSideEffect();
        return SideEffects;
    }

    public static int AssignElement(bool createArray)
    {
        int[]? values = createArray ? new[] { 1, 2 } : null;
        var assigned = values?[1] = 7;
        return assigned is null ? 0 : values![1];
    }

    public static int AssignElementSideEffects(bool createArray)
    {
        SideEffects = 0;
        int[]? values = createArray ? new[] { 1 } : null;
        values?[0] = CreateValueWithSideEffect();
        return SideEffects;
    }

    public static int AssignElementIndexAndValueSideEffects(bool createArray)
    {
        SideEffects = 0;
        int[]? values = createArray ? new[] { 1 } : null;
        var assigned = values?[CreateIndexWithSideEffect()] = CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }

    public static int AssignNestedElement(bool createRoot, bool createValues)
    {
        Node? root = createRoot
            ? new Node { Values = createValues ? new[] { 1 } : null }
            : null;
        var assigned = root?.Values?[0] = 9;
        return assigned is null ? 0 : root!.Values![0];
    }

    public static int AssignMatrixElement(bool createArray)
    {
        int[,]? values = createArray ? new int[1, 1] : null;
        var assigned = values?[0, 0] = 5;
        return assigned is null ? 0 : values![0, 0];
    }

    public static int AddChildValue(bool createNode)
    {
        Node? node = createNode ? new Node { Value = 3 } : null;
        var assigned = node?.Value += 4;
        return assigned is null ? 0 : node!.Value;
    }

    public static int AddChildCount(bool createNode)
    {
        Node? node = createNode ? new Node { Count = 3 } : null;
        var assigned = node?.Count += 4;
        return assigned is null ? 0 : node!.Count;
    }

    public static int AddElement(bool createArray)
    {
        int[]? values = createArray ? new[] { 3 } : null;
        var assigned = values?[0] += 4;
        return assigned is null ? 0 : values![0];
    }

    public static int AddElementSideEffects(bool createArray)
    {
        SideEffects = 0;
        int[]? values = createArray ? new[] { 3 } : null;
        values?[0] += CreateValueWithSideEffect();
        return SideEffects;
    }

    public static int AddElementIndexAndValueSideEffects(bool createArray)
    {
        SideEffects = 0;
        int[]? values = createArray ? new[] { 3 } : null;
        var assigned = values?[CreateIndexWithSideEffect()] += CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }

    public static int CoalesceChild(bool createNode, bool seedChild)
    {
        SideEffects = 0;
        Node? node = createNode
            ? new Node { MaybeChild = seedChild ? new Node() : null }
            : null;
        var assigned = node?.MaybeChild ??= CreateNodeWithSideEffect();
        return (assigned is null ? 0 : 1) + SideEffects * 10;
    }

    public static int CoalesceSibling(bool createNode, bool seedSibling)
    {
        SideEffects = 0;
        Node? node = createNode
            ? new Node { MaybeSibling = seedSibling ? new Node() : null }
            : null;
        var assigned = node?.MaybeSibling ??= CreateNodeWithSideEffect();
        return (assigned is null ? 0 : 1) + SideEffects * 10;
    }

    public static int CoalesceCount(bool createNode, bool seedCount)
    {
        SideEffects = 0;
        Node? node = createNode
            ? new Node { MaybeCount = seedCount ? 3 : null }
            : null;
        var assigned = node?.MaybeCount ??= CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }

    public static int CoalesceValue(bool createNode, bool seedValue)
    {
        SideEffects = 0;
        Node? node = createNode
            ? new Node { MaybeValue = seedValue ? 3 : null }
            : null;
        var assigned = node?.MaybeValue ??= CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }

    public static int CoalesceElement(bool createArray, bool seedElement)
    {
        SideEffects = 0;
        int?[]? values = createArray ? new int?[] { seedElement ? 3 : null } : null;
        var assigned = values?[0] ??= CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }

    public static int CoalesceElementIndexAndValueSideEffects(bool createArray, bool seedElement)
    {
        SideEffects = 0;
        int?[]? values = createArray ? new int?[] { seedElement ? 3 : null } : null;
        var assigned = values?[CreateIndexWithSideEffect()] ??= CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }

    public static int CoalesceNestedElement(bool createRoot, bool createValues, bool seedElement)
    {
        SideEffects = 0;
        Node? root = createRoot
            ? new Node { MaybeValues = createValues ? new int?[] { seedElement ? 3 : null } : null }
            : null;
        var assigned = root?.MaybeValues?[0] ??= CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }

    public static int AssignIndexer(bool createBox)
    {
        IndexerBox? box = createBox ? new IndexerBox() : null;
        var assigned = box?[0] = 7;
        return assigned ?? 0;
    }

    public static int AssignIndexerIndexSideEffects(bool createBox)
    {
        SideEffects = 0;
        IndexerBox? box = createBox ? new IndexerBox() : null;
        box?[CreateIndexWithSideEffect()] = 7;
        return SideEffects;
    }

    public static int AddIndexer(bool createBox)
    {
        IndexerBox? box = createBox ? new IndexerBox() : null;
        var assigned = box?[0] += 4;
        return assigned ?? 0;
    }

    public static int CoalesceIndexer(bool createBox, bool seedElement)
    {
        SideEffects = 0;
        NullableIndexerBox? box = createBox ? new NullableIndexerBox(seedElement) : null;
        var assigned = box?[0] ??= CreateValueWithSideEffect();
        return (assigned ?? 0) + SideEffects * 10;
    }
}
