using Godot;
using System;

public class Player : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal] delegate void Hit();
    [Export] int speed = 400;
    Vector2 screenSize;
    //touch input position
    Vector2 target;
    AnimatedSprite animatedSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Hide();
        screenSize = GetViewport().Size;
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public override void _Input(InputEvent @event){
        if(@event is InputEventScreenTouch eventMouseButton && eventMouseButton.Pressed){
            target = (@event as InputEventScreenTouch).Position;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2 velocity = new Vector2();

        // //check touch input
        // if(Position.DistanceTo(target) > 10){
        //     velocity = target - Position;
        // }

        //check keyboard input
        if(Input.IsActionPressed("ui_right")){
            velocity.x += 1; 
        }   
        else if(Input.IsActionPressed("ui_left")){
            velocity.x -= 1;
        }
        if(Input.IsActionPressed("ui_down")){
            velocity.y += 1;
        }
        else if(Input.IsActionPressed("ui_up")){
            velocity.y -= 1;
        }

        //play animation when is_moving and v.v
        if(velocity.Length() > 0){
            velocity = velocity.Normalized() * speed;
            animatedSprite.Play();
        }
        else{
            animatedSprite.Stop();
        }

        //update player position
        Position += velocity * delta;
        Position = new Vector2(
            Mathf.Clamp(Position.x, 0, screenSize.x),
            Mathf.Clamp(Position.y, 0, screenSize.y)
        );

        //flip animation according to movement
        if(velocity.x != 0){
            animatedSprite.Animation = "walk";
            animatedSprite.FlipV = false;
            animatedSprite.FlipH = velocity.x < 0;
        }
        else if (velocity.y != 0){
            animatedSprite.Animation = "up";
            animatedSprite.FlipV = velocity.y > 0;
        }

    }

    public void _on_Player_body_entered(PhysicsBody2D body){
        Hide();
        EmitSignal("Hit");
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }

    public void Start(Vector2 pos){
        Position = pos;
        //initial target is pos
        target = pos;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    
}
