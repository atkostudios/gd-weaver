# GDWeaver

## Requirements
Requires using NetFramework4.7 or greater in your Godot solution.

## Install
```dotnet install Atko.GDWeaver```

## Usage

### AutoWeaverBase

In order to benefit the most from GDWeaver, you should add a Node with an attached script inheriting from ```AutoWeaverBase```.

This allows you to have the extension methods ```Install``` and ```Connect``` without having to call ```Weaver.Weave(this)``` in the ```_Ready``` method.

```csharp
using Atko.GDWeaver;
using Godot;

class Example : Node
{
    // Install the AnimationPlayer child named "AnimPlayer"
    [Install] private AnimationPlayer AnimPlayer;

    // Install the Sprite child named "MySprite"
    [Install("MySprite")] private AnimationPlayer PlayerSprite;
}
```

### Without AutoWeaverBase

```csharp
using Atko.GDWeaver;
using Godot;

class Example : Node
{
    // Install the AnimationPlayer child named "AnimPlayer"
    [Install] private AnimationPlayer AnimPlayer;

    // Install the Sprite child named "MySprite"
    [Install("MySprite")] private Sprite PlayerSprite;

    public override void _Ready()
    {
        Weaver.Weave(this);
    }
}
```
