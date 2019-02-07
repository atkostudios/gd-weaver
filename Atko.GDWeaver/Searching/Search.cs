using System;
using Godot;

namespace Atko.GDWeaver.Searching
{
    public static class Search
    {
        public static bool Exists(this Node node)
        {
            return node != null && node.NativeInstance != IntPtr.Zero && !node.IsQueuedForDeletion();
        }

        public static bool HasAncestor(this Node node, Node ancestor)
        {
            foreach (var current in Ascend(node))
            {
                if (current == ancestor)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasParent(this Node node, Node parent)
        {
            return node.GetParent() == parent;
        }

        public static bool HasChild(this Node node, Node child)
        {
            return child.GetParent() == node;
        }

        public static bool HasDescendant(this Node node, Node descendant)
        {
            return descendant.HasAncestor(node);
        }

        public static Node Root(this Node node)
        {
            return node.GetNode("/root");
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> Children(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.Children);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> Descend(this Node node)
        {
            return DescendBF(node);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendBF(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendBF, 1);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendDF(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendDF, 1);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> Ascend(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.Ascend, 1);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendInclusive(this Node node)
        {
            return DescendBFInclusive(node);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendBFInclusive(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendBF);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendDFInclusive(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendDF);
        }

        public static TreeEnumerable<Node, TreeEnumerator, Node> AscendInclusive(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.Ascend);
        }

        static TreeEnumerable<Node, TreeEnumerator, Node> Enumerate(Node root, TreeEnumerationMode mode,
            int skip = 0)
        {
            return Enumerate(root, mode, skip, new TreeQuery<Node>());
        }

        static TreeEnumerable<Node, TreeEnumerator, Node> Enumerate(Node root, TreeEnumerationMode mode,
            int skip, TreeQuery<Node> query)
        {
            return new TreeEnumerable<Node, TreeEnumerator, Node>(new TreeEnumerator(root, mode), skip, query);
        }
    }
}