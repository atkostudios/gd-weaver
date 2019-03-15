using System;
using NullGuard;

namespace Atko.GDWeaver
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
        /// Create a new <see cref="InstallAttribute"/> with the provided flags and name.
        /// </summary>
        /// <param name="flags"><see cref="Flags"/></param>
        /// <param name="name"><see cref="Name"/></param>
        public InstallAttribute(Install flags, string name)
        {
            Flags = flags;
            Name = name;
        }

        /// <summary>
        /// Create a new <see cref="InstallAttribute"/> with the provided flags.
        /// </summary>
        /// <param name="flags"><see cref="Flags"/></param>
        public InstallAttribute(Install flags)
        {
            Flags = flags;
        }

        /// <summary>
        /// Create a new <see cref="InstallAttribute"/> with the provided name.
        /// </summary>
        /// <param name="name"><see cref="Name"/></param>
        public InstallAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Create a new <see cref="InstallAttribute"/> with the default configuration.
        /// </summary>
        public InstallAttribute()
        { }

        internal bool HasFlag(Install flag)
        {
            return (Flags & flag) != 0;
        }
    }
}