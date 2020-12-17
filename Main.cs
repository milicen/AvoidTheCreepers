using Godot;
using System;

public class Main : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	[Export] PackedScene mob;
	int score;
	Random random = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AudioStreamPlayer>("BGM").Play();
	}

	float RandRange(float min, float max){
		return (float)random.NextDouble() * (max - min) + min;
	}

	//signals
	async public void GameOver(){
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();

		GetNode<HUD>("HUD").ShowGameOver();
		
		GetTree().CallGroup("mobs", "queue_free");

		GetNode<AudioStreamPlayer>("DeathSound").Play();

		while(GetNode<AudioStreamPlayer>("BGM").VolumeDb > -10f){
			GetNode<AudioStreamPlayer>("BGM").VolumeDb -= 0.05f;
		}

		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		while(GetNode<AudioStreamPlayer>("BGM").VolumeDb < 0){
				GetNode<AudioStreamPlayer>("BGM").VolumeDb += 0.05f;
			}
	}

	public void NewGame(){
		score = 0;

		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Position2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();

		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(score);
		hud.ShowMessage("Get Ready");

		if(GetNode<AudioStreamPlayer>("BGM").Playing == false){
			GetNode<AudioStreamPlayer>("BGM").Play();
		}
	}

	public void _on_StartTimer_timeout(){
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	public void _on_ScoreTimer_timeout(){
		score++;
		GetNode<HUD>("HUD").UpdateScore(score);
	}

	public void _on_MobTimer_timeout(){
		//pick random spawn location
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.Offset = random.Next();

		//instanceing mob and put into scene
		var mobInstance = (RigidBody2D) mob.Instance();
		AddChild(mobInstance);

		//set mob direction perpendicular to path direction
		float direction = mobSpawnLocation.Rotation + Mathf.Pi/2;

		//set mob position in random location
		mobInstance.Position = mobSpawnLocation.Position;

		//add randomness in direction
		direction += RandRange(-Mathf.Pi/4, Mathf.Pi/4);
		mobInstance.Rotation = direction;

		//choose mob movement velocity
		mobInstance.LinearVelocity = new Vector2(RandRange(150, 250), 0).Rotated(direction);
	}
}
