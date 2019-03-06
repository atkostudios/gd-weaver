using Atko.GDWeaver.Weaving;
using Godot;
using System;

public class Tests : Button
{
    [Install] Button Child { get; }
    [Install(Install.Optional)] Node MissingChild { get; }

    public override void _Ready()
    {
        GD.Print("TESTING...");

        GD.Print("CHILD IS NULL BEFORE INSTALL: " + (Child == null));
        GD.Print("OPTIONAL CHILD IS NULL BEFORE INSTALL: " + (MissingChild == null));

        Weaver.Weave(this);

        GD.Print("CHILD IS NOT NULL AFTER INSTALL: " + (Child != null));
        GD.Print("OPTIONAL CHILD IS NULL AFTER INSTALL: " + (MissingChild == null));

        GD.Print("DONE");
        EmitSignal("pressed");
        Child.EmitSignal("pressed");
    }

    [Connect("pressed")]
    void OnPressed()
    {
        GD.Print("PRESSED");
    }

    [Connect(nameof(Child), "pressed")]
    void OnChildPressed()
    {
        GD.Print("CHILD PRESSED");
    }
}
