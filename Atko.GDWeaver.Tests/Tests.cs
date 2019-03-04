using Atko.GDWeaver.Installation;
using Godot;
using System;

public class Tests : Node
{
    [Install] Node Child { get; }
    [Install(Flags = Install.Optional)] Node MissingChild { get; }

    public override void _Ready()
    {
        GD.Print("TESTING...");

        GD.Print("CHILD IS NULL BEFORE INSTALL: " + (Child == null));
        GD.Print("OPTIONAL CHILD IS NULL BEFORE INSTALL: " + (MissingChild == null));

        Installer.InstallNodes(this);

        GD.Print("CHILD IS NOT NULL AFTER INSTALL: " + (Child != null));
        GD.Print("OPTIONAL CHILD IS NULL AFTER INSTALL: " + (MissingChild == null));

        GD.Print("DONE");
    }
}
