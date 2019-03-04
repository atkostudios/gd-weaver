using System;
using NullGuard;

namespace Atko.GDWeaver.Installation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InstallAttribute : Attribute
    {
        /// <summary>
        /// The name of the target node to install. Defaults to the name of the field or property if not specified.
        /// </summary>
        [AllowNull]
        public string Name { get; set; }

        /// <summary>
        /// Flags controlling the install operation.
        /// </summary>
        public Install Flags { get; set; }

        /// <summary>
        /// The type of the root node to search for nodes from. During installation, the tree will be scanned upward
        /// from the current node to find an ancestor of the provided type. If found, this ancestor will be used as the
        /// root node when searching for installed nodes rather than the current node. If this option is not provided,
        /// or the specified root node is not found, the current node will be used as the root.
        /// </summary>
        [AllowNull]
        public Type From { get; set; }

        public bool HasFlag(Install flag)
        {
            return (Flags & flag) != 0;
        }
    }
}