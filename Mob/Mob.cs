using Godot;
using System;

public class Mob : RigidBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export] public int minSpeed = 150;
    [Export] public int maxSpeed = 250;

    static private Random random = new Random();

    AnimatedSprite animatedSprite;
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        var mobTypes = animatedSprite.Frames.GetAnimationNames();    
        animatedSprite.Animation = mobTypes[random.Next(mobTypes.Length)];
    }

    public void _on_VisibilityNotifier2D_screen_exited()
    {
        QueueFree();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
