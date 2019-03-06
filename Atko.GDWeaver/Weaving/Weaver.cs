using Godot;

namespace Atko.GDWeaver.Weaving
{
    public static class Weaver
    {
        /// <summary>
        /// Automatically apply <see cref="Weaver.Weave"/> to all nodes when they are added to the scene tree. Should
        /// only be called once or as the scene changes.
        /// </summary>
        /// <param name="tree"></param>
        public static void Inject(SceneTree tree)
        {
            tree.Root.AddChild(new WeaveListener());
        }

        public static void Weave(this Node node)
        {
            Installer.Run(node);
            Connector.Run(node);
        }

    }
}