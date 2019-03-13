using Godot;

namespace Atko.GDWeaver.Weaving
{
    /// <summary>
    /// Node which, when present in the scene tree, will automatically call <see cref="Weaver.Weave"/> on nodes as soon
    /// as they are added to the tree.
    /// </summary>
    public class WeaveListener : Node
    {
        public WeaveListener()
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