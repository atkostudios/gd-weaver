using Godot;

namespace Atko.GDWeaver.Weaving
{
    public static class Weaver
    {
        /// <summary>
        /// Install related nodes on a given node according to instances of <see cref="InstallAttribute"/>, then connect
        /// signals according to instances of <see cref="ConnectAttribute"/>.
        /// </summary>
        /// <param name="node"></param>
        public static void Weave(this Node node)
        {
            Installer.Run(node);
            Connector.Run(node);
        }

        /// <summary>
        /// Automatically apply <see cref="Weaver.Weave"/> to all nodes when they are added to the scene tree. Should
        /// only be called once or as the scene changes.
        /// </summary>
        /// <param name="tree"></param>
        public static void Inject(SceneTree tree)
        {
            tree.Root.AddChild(new WeaveListener());
        }
    }
}