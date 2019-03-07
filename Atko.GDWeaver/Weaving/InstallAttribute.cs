using System;
using NullGuard;

namespace Atko.GDWeaver.Weaving
{
    /// <summary>
    /// Attribute that marks a field or property as a reference to another node present somewhere in the scene tree
    /// relative to the current node.
    ///
    /// When <see cref="Weaver.Weave"/> is called on the current node, marked fields or properties will have their
    /// desired nodes searched for according to this attribute's configuration. If the desired node is found, a
    /// reference to it will be placed into the field or property. Otherwise, if the attribute is not marked as
    /// <see cref="Install.Optional"/>, an exception is thrown.
    ///
    /// Installation of nodes occurs before connection of signals via <see cref="ConnectAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InstallAttribute : Attribute
    {
        /// <summary>
        /// Flags to configure the install operation.
        /// </summary>
        public Install Flags { get; set; }

        /// <summary>
        /// The name of the target node to install. Defaults to the name of the field or property if not specified.
        /// </summary>
        [AllowNull]
        public string Name { get; set; }

        /// <summary>
        /// The type of the root node to search for nodes from. During installation, the tree will be scanned upward
        /// from the current node to find an ancestor of the provided type. If found, this ancestor will be used as the
        /// root node when searching for installed nodes rather than the current node. If this option is not provided,
        /// or the specified root node is not found, the current node will be used as the root.
        /// </summary>
        [AllowNull]
        public Type From { get; set; }

        /// <summary>
        /// Create a new <see cref="InstallAttribute"/> with the provided flags, name and from type.
        /// </summary>
        /// <param name="flags"><see cref="Flags"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="from"><see cref="From"/></param>
        public InstallAttribute(Install flags = default(Install), string name = null, Type from = null)
        {
            Flags = flags;
            Name = name;
            From = from;
        }

        /// <summary>
        /// Create a new <see cref="InstallAttribute"/> with the provided name and from type.
        /// </summary>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="from"><see cref="From"/></param>
        public InstallAttribute(string name, Type from)
        {
            Name = name;
            From = from;
        }

        /// <summary>
        /// Create a new <see cref="InstallAttribute"/> with the provided flags and from type.
        /// </summary>
        /// <param name="flags"><see cref="Flags"/></param>
        /// <param name="from"><see cref="From"/></param>
        public InstallAttribute(Install flags, Type from)
        {
            Flags = flags;
            From = from;
        }

        /// <summary>
        /// Create a new <see cref="InstallAttribute"/> with the provided from type.
        /// </summary>
        /// <param name="from"><see cref="From"/></param>
        public InstallAttribute(Type from)
        {
            From = from;
        }

        internal bool HasFlag(Install flag)
        {
            return (Flags & flag) != 0;
        }
    }
}