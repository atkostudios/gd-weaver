using Godot;
using System;
using Atko.GDWeaver;

public class Tests : Button
{
    [Install] Button Child { get; }
    [Install(Install.Optional)] Node MissingChild { get; }

    public override void _Ready()
    {
        GD.Print("TESTING...");

        GD.Print("CHILD IS NOT NULL AFTER INSTALL: " + (Child != null));
        GD.Print("OPTIONAL CHILD IS NULL AFTER INSTALL: " + (MissingChild == null));

        EmitSignal("pressed");
        Child.EmitSignal("pressed");

        GD.Print("DONE");
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
