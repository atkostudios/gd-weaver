using Atko.GDLyra.Installation;
using Godot;

namespace Atko.GDLyra
{
    public class Gusto : Node
    {
        public override void _Ready()
        {
            GetTree().Connect("node_added", this, nameof(InstallNodes));
        }

        void InstallNodes(Node node)
        {
            Installer.InstallNodes(node);
        }
    }
}