using Godot;

namespace Atko.GDWeaver.Weaving
{
    /// <summary>
    /// Simple node that, when present in the scene tree, will automatically call <see cref="Weaver.Weave"/> on added
    /// nodes as soon as they enter the tree.
    /// </summary>
    public class WeaveListener : Node
    {
        internal WeaveListener()
        {
            Name = nameof(WeaveListener);
        }

        public override void _Ready()
        {
            GetTree().Connect("node_added", this, nameof(Weave));
        }

        void Weave(Node node)
        {
            Weaver.Weave(node);
        }
    }
}