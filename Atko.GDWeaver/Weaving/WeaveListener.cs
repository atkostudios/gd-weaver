using Godot;

namespace Atko.GDWeaver.Weaving
{
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