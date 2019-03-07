using System;
using Godot;

namespace Atko.GDWeaver.Weaving
{
    /// <summary>
    /// Utility for installation of node references and connection of signals.
    /// </summary>
    public static class Weaver
    {
        /// <summary>
        /// Install node references on a given node according to instances of <see cref="InstallAttribute"/>, then
        /// connect signals according to instances of <see cref="ConnectAttribute"/>.
        /// </summary>
        /// <param name="node">The node to weave node references and signals for.</param>
        public static void Weave(this Node node)
        {
            Installer.Run(node);
            Connector.Run(node);
        }

        /// <summary>
        /// Automatically apply <see cref="Weaver.Weave"/> to all nodes when they are added to the scene tree. Should
        /// only be called once or as the scene changes.
        /// </summary>
        /// <param name="tree">A reference to the active scene tree.</param>
        public static void Inject(SceneTree tree)
        {
            tree.Root.AddChild(new WeaveListener());
        }
    }
}