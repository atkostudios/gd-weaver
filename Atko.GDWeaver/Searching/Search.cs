using System;
using Godot;
using NullGuard;

namespace Atko.GDWeaver.Searching
{
    /// <summary>
    /// Utility for fast search, query and iteration of nodes in the scene tree.
    /// </summary>
    public static class Search
    {
        /// <summary>
        /// Returns true if a given node is not null, has not been freed and is not queued to be freed.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node exists.</returns>
        public static bool Exists([AllowNull] this Node node)
        {
            return node != null && node.NativeInstance != IntPtr.Zero && !node.IsQueuedForDeletion();
        }

        /// <summary>
        /// Returns the opposite of <see cref="Exists"/>.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node does not exist.</returns>
        public static bool Missing([AllowNull] this Node node)
        {
            return !Exists(node);
        }

        /// <summary>
        /// Return true if a given node is the direct parent of the current node.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="parent">The node that may be the parent of the current node.</param>
        /// <returns>True if the tested node is the parent of the current node.</returns>
        public static bool HasParent(this Node node, Node parent)
        {
            return node.GetParent() == parent;
        }

        /// <summary>
        /// Return true if a given node is the direct child of the current node.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="parent">The node that may be a child of the current node.</param>
        /// <returns>True if the tested node is a child of the current node.</returns>
        public static bool HasChild(this Node node, Node child)
        {
            return child.GetParent() == node;
        }

        /// <summary>
        /// Return true if the given node is an ancestor of the current node. An ancestor is a node located somewhere
        /// above the current node in the tree.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="ancestor">The node that may be an ancestor of the current node.</param>
        /// <returns>True if the tested node is an ancestor of the current node.</returns>
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

        /// <summary>
        /// Return true if the given node is a descendant of the current node. A descendant is a node located somewhere
        /// below the current node in the tree.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="ancestor">The node that may be a descendant of the current node.</param>
        /// <returns>True if the tested node is a descendant of the current node.</returns>
        public static bool HasDescendant(this Node node, Node descendant)
        {
            return descendant.HasAncestor(node);
        }

        /// <summary>
        /// Return the root of the scene tree.
        /// </summary>
        /// <param name="node">A node somewhere in a scene tree.</param>
        /// <returns>The root of the scene tree.</returns>
        public static Viewport Root(this Node node)
        {
            return node.GetTree().Root;
        }

        /// <summary>
        /// Iterate over the children of the current node.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>An enumerable that iterates over the node's children.</returns>
        public static TreeEnumerable<Node, TreeEnumerator, Node> Children(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.Children);
        }

        /// <summary>
        /// Iterate over all nodes descending from the current node in breadth-first order.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>An enumerable that iterates over the node's descendants.</returns>
        public static TreeEnumerable<Node, TreeEnumerator, Node> Descend(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendBF, 1);
        }

        /// <summary>
        /// Iterate over all nodes descending from the current node in depth-first order.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>An enumerable that iterates over the node's descendants.</returns>
        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendDF(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendDF, 1);
        }

        /// <summary>
        /// Iterate over all nodes above the current node. This starts with the parent and goes to the root of the scene
        /// tree.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>An enumerable that iterates over the node's ancestors.</returns>
        public static TreeEnumerable<Node, TreeEnumerator, Node> Ascend(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.Ascend, 1);
        }

        /// <summary>
        /// Iterate over the current node and all nodes descending from it in breadth-first order.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>An enumerable that iterates over the current node followed by its descendants.</returns>
        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendInclusive(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendBF);
        }

        /// <summary>
        /// Iterate over the current node and all nodes descending from it in depth-first order.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>An enumerable that iterates over the current node followed by its descendants.</returns>
        public static TreeEnumerable<Node, TreeEnumerator, Node> DescendDFInclusive(this Node node)
        {
            return Enumerate(node, TreeEnumerationMode.DescendDF);
        }

        /// <summary>
        /// Iterate over the current node and all nodes above it. This starts with the current node and goes to the root
        /// of the scene tree.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns>An enumerable that iterates over the current node followed by the node's ancestors.</returns>
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