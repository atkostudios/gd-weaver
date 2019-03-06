using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Atko.GDWeaver.Utility;
using Atko.GDWeaver.Searching;
using Atko.Mirra.Images;
using Godot;

using static System.Reflection.BindingFlags;
using static Atko.GDWeaver.Weaving.Install;

namespace Atko.GDWeaver.Weaving
{
    static class Installer
    {
        struct Target
        {
            public AccessorImage Accessor { get; }
            public InstallAttribute Attribute { get; }

            public Target(AccessorImage accessor, InstallAttribute attribute)
            {
                Accessor = accessor;
                Attribute = attribute;
            }
        }

        static class Targets
        {
            static Dictionary<Type, Target[]> Cache { get; } = new Dictionary<Type, Target[]>();

            static List<Target> Buffer { get; } = new List<Target>();
            static HashSet<AccessorImage> Seen { get; } = new HashSet<AccessorImage>();

            public static Target[] GetTargets(TypeImage type)
            {
                if (Cache.TryGetValue(type, out var cached))
                {
                    return cached;
                }

                lock (Buffer)
                {
                    var accessors = type
                        .Accessors()
                        .Where((current) => !current.IsStatic)
                        .Where((current) => current.CanSet);

                    foreach (var accessor in accessors)
                    {
                        Add(accessor);
                    }

                    var targets = Cache[type] = Buffer.ToArray();

                    Buffer.Clear();
                    Seen.Clear();

                    return targets;
                }
            }

            static void Add(AccessorImage accessor)
            {
                var attribute = accessor.Attribute<InstallAttribute>();
                if (attribute != null)
                {
                    if (Seen.Add(accessor))
                    {
                        Buffer.Add(new Target(accessor, attribute));
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
            public TypeImage SearchType { get; }

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
            public TypeImage FromType { get; }

            public SearchParameters(InstallAttribute attribute, AccessorImage accessor)
            {
                SearchName = attribute.HasFlag(TypeOnly) ? null : attribute.Name ?? accessor.ShortName;
                SearchType = accessor.DeclaredType;
                IsAncestor = attribute.HasFlag(Ancestor);
                IsOptional = attribute.HasFlag(Optional);
                FromType = attribute.From;
            }
        }

        /// <summary>
        /// Search for and install all nodes specified by <see cref="InstallAttribute"/> on fields and properties.
        /// </summary>
        public static void Run(Node node)
        {
            var type = node.GetType();

            foreach (var target in Targets.GetTargets(type))
            {
                var installed = Search(node, new SearchParameters(target.Attribute, target.Accessor));
                if (installed == null)
                {
                    continue;
                }

                target.Accessor.Set(node, installed);
            }
        }

        static Node Search(Node node, SearchParameters parameters)
        {
            var root = node;

            if (parameters.FromType != null)
            {
                root = node.Ascend().At((current) =>
                {
                    var type = current.GetType().Image();
                    return type.IsAssignableTo(parameters.FromType);
                });
            }

            var nodes = parameters.IsAncestor ? root.Ascend() : root.Descend();
            var result = nodes.At((current) =>
            {
                var type = current.GetType().Image();
                return (parameters.SearchType == null || type.IsAssignableTo(parameters.SearchType)) &&
                       (parameters.SearchName == null || parameters.SearchName == current.Name);
            });

            if (!result.Exists())
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
            var nodeDescription = $"[{node.Name}] of type [{node.GetType().Name}]";
            var searchDescription = parameters.SearchName != null ? $"with name [{parameters.SearchName}] " : "";
            var term = parameters.IsAncestor ? "ancestor" : "descendant";

            return
                $"Node {nodeDescription} does not have any installed {term} {searchDescription}of type " +
                $"[{parameters.SearchType.Name}].";
        }
    }
}