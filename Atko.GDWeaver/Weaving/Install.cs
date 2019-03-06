using System;

namespace Atko.GDWeaver.Weaving
{
    [Flags]
    public enum Install
    {
        /// <summary>
        /// Normal installation. Install a non-optional descendant according to the name and type of the field or
        /// property.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// If the target node is not found, assign null to the field or property and don't emit an error.
        /// </summary>
        Optional = 1,

        /// <summary>
        /// Install a target node that is an ancestor of the current node rather than a descendant.
        /// </summary>
        Ancestor = 2,

        /// <summary>
        /// Only find the target node according to the field or property type. Ignore the name of the field or
        /// property and the provided <see cref="InstallAttribute.Name"/>.
        /// </summary>
        TypeOnly = 4
    }
}