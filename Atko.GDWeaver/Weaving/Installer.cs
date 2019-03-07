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

        struct SearchParameters
        {
            public string SearchName { get; }
            public TypeImage SearchType { get; }
            public bool IsAncestor { get; }
            public bool IsOptional { get; }
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
                    throw new GDWeaverException(GenerateNotFoundMessage(root, parameters));
                }

                return null;
            }

            return result;
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