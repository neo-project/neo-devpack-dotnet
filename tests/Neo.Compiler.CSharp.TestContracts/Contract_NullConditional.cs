using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_NullConditional : SmartContract.Framework.SmartContract
{
    private class Node
    {
        public Node? Child { get; set; }
        public Node? Sibling;
        public static Node? StaticChild { get; set; }
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
}
