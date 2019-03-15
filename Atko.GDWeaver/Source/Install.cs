using System;

namespace Atko.GDWeaver
{
    /// <summary>
    /// Configuration flags for <see cref="InstallAttribute"/>.
    /// </summary>
    [Flags]
    public enum Install
    {
        /// <summary>
        /// Normal installation. Install a non-optional descendant node matching the name and type of the field or
        /// property.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// If the desired node is not found, just assign null to the field or property and don't throw an exception.
        /// </summary>
        Optional = 1,

        /// <summary>
        /// Install a node that is an ancestor of the current node rather than a descendant.
        /// </summary>
        Ancestor = 2,

        /// <summary>
        /// Only search for the node that matches the field or property type. Ignore the name of the field or property
        /// and the provided <see cref="InstallAttribute.Name"/>.
        /// </summary>
        TypeOnly = 4
    }
}