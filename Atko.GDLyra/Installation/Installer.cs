using System;
using System.Collections.Generic;
using System.Reflection;
using Atko.GDLyra.Utility;
using Atko.GDLyra.Search;
using Godot;

using static System.Reflection.BindingFlags;
using static Atko.GDLyra.Installation.Install;

namespace Atko.GDLyra.Installation
{
    public static class Installer
    {
        struct Target
        {
            public MemberInfo Member { get; }
            public InstallAttribute Attribute { get; }

            public Target(MemberInfo member, InstallAttribute attribute)
            {
                Member = member;
                Attribute = attribute;
            }
        }

        static class Targets
        {
            const BindingFlags Flags = Instance | NonPublic | Public | DeclaredOnly;

            static Dictionary<Type, Target[]> Cache { get; } = new Dictionary<Type, Target[]>();

            static List<Target> Buffer { get; } = new List<Target>();
            static HashSet<MemberInfo> Seen { get; } = new HashSet<MemberInfo>();

            public static Target[] GetTargets(Type type)
            {
                if (Cache.TryGetValue(type, out var cached))
                {
                    return cached;
                }

                lock (Buffer)
                {
                    foreach (var ancestor in type.Inheritance())
                    {
                        foreach (var property in ancestor.GetProperties(Flags))
                        {
                            Add(property);
                        }

                        foreach (var field in ancestor.GetFields(Flags))
                        {
                            Add(field);
                        }
                    }

                    var targets = Cache[type] = Buffer.ToArray();

                    Buffer.Clear();
                    Seen.Clear();

                    return targets;
                }
            }

            static void Add(MemberInfo member)
            {
                var attribute = member.GetCustomAttribute<InstallAttribute>();
                if (attribute != null)
                {
                    if (Seen.Add(member))
                    {
                        Buffer.Add(new Target(member, attribute));
                    }
                }
            }
        }

        /// <summary>
        /// Event emitted when an install-related node could not be found. Provides the node the installation failed on
        /// and an error message specifying what went wrong.
        /// </summary>
        public static event Action<Node, string> FailedEvent;

        /// <summary>
        /// Parameters for <see cref="Search"/> specifying how to find the target node.
        /// </summary>
        struct SearchParameters
        {

            /// <summary>
            /// The name of the target node. This is null if a name option is not provided.
            /// </summary>
            public string SearchName { get; }

            /// <summary>
            /// The type of the target node to search for. Defaults to <see cref="Node"/>
            /// </summary>
            public Type SearchType { get; }

            /// <summary>
            /// True if the target node is an ancestor of the current node. Otherwise the target node is a descendant.
            /// </summary>
            public bool IsAncestor { get; }

            /// <summary>
            /// True if no error should be emitted when the target node is not found.
            /// </summary>
            public bool IsOptional { get; }

            /// <summary>
            /// The type of the root node to use. If this is not null during a search, we scan up the tree to find an
            /// ancestor of the provided type and search for installed nodes using that ancestor as the root rather than
            /// the current node. If the specified root node is not found the root will default to the current node.
            /// </summary>
            public Type FromType { get; }

            public SearchParameters(InstallAttribute attribute, MemberInfo member)
            {
                SearchName = attribute.HasFlag(TypeOnly) ? null : attribute.Name ?? member.Name;
                SearchType = member.GetReturnType();
                IsAncestor = attribute.HasFlag(Ancestor);
                IsOptional = attribute.HasFlag(Optional);
                FromType = attribute.From;
            }

        }

        /// <summary>
        /// Search for and install all nodes specified by <see cref="InstallAttribute"/> on fields and properties.
        /// </summary>
        public static void InstallNodes(this Node node)
        {
            var type = node.GetType();

            foreach (var target in Targets.GetTargets(type))
            {
                var installed = Search(node, new SearchParameters(target.Attribute, target.Member));
                if (installed == null)
                {
                    continue;
                }

                node.DynamicSet(target.Member, installed);
            }
        }

        /// <summary>
        /// Apply <see cref="InstallNodes"/> to the provided node and all of its descendants.
        /// </summary>
        public static void InstallNodesRecursive(this Node node)
        {
            InstallNodes(node);
            foreach (var descendant in node.Descend())
            {
                InstallNodes(descendant);
            }
        }

        static Node Search(Node node, SearchParameters parameters)
        {
            var root = parameters.FromType != null
                ? node.Ascend().At<Node>((current) => parameters.FromType.IsInstanceOfType(current)) ?? node
                : node;

            var nodes = parameters.IsAncestor ? root.Ascend() : root.Descend();
            var result = nodes.At<Node>((current) =>
            {
                return (parameters.SearchType == null || parameters.SearchType.IsInstanceOfType(current)) &&
                       (parameters.SearchName == null || parameters.SearchName == current.Name);
            });

            if (result.Missing())
            {
                if (!parameters.IsOptional)
                {
                    InstallError(root, GenerateNotFoundMessage(root, parameters));
                }

                return null;
            }

            return result;
        }

        static void InstallError(Node node, string message)
        {
            if (FailedEvent == null)
            {
                throw new ArgumentException(message);
            }

            FailedEvent(node, message);
        }

        static string GenerateNotFoundMessage(Node node, SearchParameters parameters)
        {
            var nodeDescription = $"[{node.Name}] of type [({node.GetType().Name})]";
            var searchDescription = parameters.SearchName != null ? $"with name [{parameters.SearchName}] " : "";
            var term = parameters.IsAncestor ? "ancestor" : "descendant";

            return
                $"Node {nodeDescription} does not have any installed {term} {searchDescription}of type " +
                $"[{parameters.SearchType.Name}].";
        }
    }
}