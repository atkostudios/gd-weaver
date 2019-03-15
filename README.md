# GDWeaver

GDWeaver is a library for Godot Engine designed to make using nodes and signals simpler.

## Requirements

* .NET Framework >= 4.7 
    
    *You can change this in your .csproj file.*

## Installation

* Install "Atko.GDWeaver" using Nuget.

## Documentation

---

```csharp
using Atko.GDWeaver;
```

### *Weaver*

The most central class of GDWeaver is `Weaver`. This static class contains extension methods for Godot's `Node`.

### *Weaving*

The most important method contained in `Weaver` is `Weaver.Weave(Node)`. The `Weave` method does two primary operations for the given node:

1. *Installs* node references according to `Install` attributes.
2. *Connects* signals according to `Connect` attributes.

In order to start weaving nodes, you have two options:

1. Call `Weave()` yourself for every node: 
    ```csharp
    public override void _EnterTree() 
    {
        Weaver.Weave(this);
    }
    ```
2. Add an `AutoWeaver` to your project's auto-loads. The `AutoWeaver` will automatically call `Weave()` on every node as it enters the tree. This is far easier than the first method. To add an `AutoWeaver`:

    1. Create a new, single-node scene called `AutoWeaver`.
    2. Create an empty script class called `AutoWeaver` that inherits `AutoWeaverBase`.
    ```csharp
    using Atko.GDWeaver;

    public class AutoWeaver : AutoWeaverBase { }
    ```
    3. Add the script to the root node in the scene. 
    4. Add the scene to your project's auto-loads.
    5. Enjoy.


##### Install Examples

```csharp
using Godot;
using Atko.GDWeaver;

class Example : Node
{
    /*
     * Install a node descending from this node that both:
     *
     *     1. Inherits or extends AnimationPlayer
     *     2. Is named "Animator"
     *      
     * If the node doesn't exist, an exception will be thrown.
     *
     * Weave() searches in breadth first order down the tree to 
     * find installed nodes.
     *
     * Weave() does not care about public or private visibility.
     *
     * Weave() can install nodes into: 
     *
     *     1. Any field (including readonly ones)
     *     2. Any settable property
     *     3. Any auto-property (including get-only ones)
     */
    [Install] 
    AnimationPlayer Animator { get; }

    /*
     * Install a node descending from this node that both:
     *
     *     1. Inherits or extends Sprite
     *     2. Is named "SomeSprite" 
     */
    [Install("SomeSprite")] 
    Sprite Sprite { get; }

    /*
     * Install a node descending from this node, if it 
     * exists, that both:
     *
     *     1. Inherits or extends Node 
     *     2. Is named "Optional" 
     *
     * The Install.Optional flag will tell Weave() to simply assign 
     * null to the field or property if the node is not found. 
     */
    [Install(Install.Optional)] 
    Node Optional; 

    /*
     * Install a node somewhere above this node that both:
     *
     *     1. Inherits or extends some type called Ancestor
     *     2. Is named "SomeAncestor" 
     *
     * The Install.Ancestor flag will tell Weave() to search up the 
     * tree to find the node rather than down it. Weave() will scan 
     * starting with the parent up all the way up to the root of the
     * tree.
     */
    [Install(Install.Ancestor, "SomeAncestor")] 
    Ancestor Ancestor { get; }

    /*
     * Install a node somewhere above this node that: 
     *
     *     1. Inherits or extends some type called Ancestor
     *
     * The Install.TypeOnly flag will tell Weave() to ignore the name
     * of the member and only find a node that inherits or extends the 
     * declared type. 
     */
    [Install(Install.Ancestor | Install.TypeOnly)] 
    Ancestor TypedAncestor { get; }

    /*
     * Install a node somewhere above this node, if it exists, that: 
     *
     *     1. Inherits or extends some type called Ancestor
     */
    [Install(Install.Ancestor | Install.TypeOnly | Install.Optional)] 
    Ancestor OptionalAncestor { get; }

    ...
}
```
