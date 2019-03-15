using Godot;

namespace Atko.GDWeaver
{
    /// <summary>
    /// Node which, when present in the scene tree, will automatically call <see cref="Weaver.Weave"/> on nodes as soon
    /// as they are added to the tree.
    ///
    /// To use: Create a script that inherits <see cref="AutoWeaverBase"/>, add it to a single node scene, then add
    /// that scene to your auto-loads.
    ///
    public class AutoWeaverBase : Node
    {
        public override void _EnterTree()
        {
            if (GetTree().IsConnected("node_added", this, nameof(Weave)))
            {
                return;
            }

            GetTree().Connect("node_added", this, nameof(Weave));
        }

        void Weave(Node node)
        {
            Weaver.Weave(node);
        }
    }
}