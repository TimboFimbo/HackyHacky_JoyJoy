using Godot;
using System;

public partial class Player : Area2D
{
	int tileSize = 48;
	int ScreenSize;
	int movementSpeed = 1;
	Vector2 pos = new Vector2(88, 113);
	[Signal]
	public delegate void DirectionPressedEventHandler(string pressed);
	[Signal]
	public delegate void InteractPressedEventHandler();
	[Signal]
	public delegate void OpenDoorPressedEventHandler(int doorNumber);
	[Signal]
	public delegate void CloseDoorPressedEventHandler(int doorNumber);
	[Signal]
	public delegate void PausePressedEventHandler();
	[Signal]
	public delegate void ResetPressedEventHandler();
	[Signal]
	public delegate void HelpPressedEventHandler();
	public int playerMovingTime = 0;
	int timeToIdleAnim = 60;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// pos = Position;
		// GD.Print(pos);

		ResetPlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("walk_right"))
		{
			// MovePlayer("right");
			EmitSignal(SignalName.DirectionPressed, "right");
		}

		if (Input.IsActionJustPressed("walk_left"))
		{
			// MovePlayer("left");
			EmitSignal(SignalName.DirectionPressed, "left");
		}

		if (Input.IsActionJustPressed("walk_up"))
		{
			// MovePlayer("up");
			EmitSignal(SignalName.DirectionPressed, "up");
		}

		if (Input.IsActionJustPressed("walk_down"))
		{
			// MovePlayer("down");
			EmitSignal(SignalName.DirectionPressed, "down");
		}

		if (Input.IsActionJustPressed("interact")) // key 'i'
		{
			EmitSignal(SignalName.InteractPressed);
		}

		if (Input.IsActionJustPressed("pause")) // key 'SPACE'
		{
			EmitSignal(SignalName.PausePressed);
		}

		if (Input.IsActionJustPressed("reset")) // key 'r'
		{
			EmitSignal(SignalName.ResetPressed);
		}

		if (Input.IsActionJustPressed("help")) // key 'ESC'
		{
			EmitSignal(SignalName.HelpPressed);
		}

		// The following are debug keys and won't be used in-game

		if (Input.IsActionJustPressed("open_door")) // key 'o'
		{
			EmitSignal(SignalName.OpenDoorPressed, 0);
		}

		if (Input.IsActionJustPressed("open_door_1")) // key '1'
		{
			EmitSignal(SignalName.OpenDoorPressed, 1);
		}

		if (Input.IsActionJustPressed("open_door_2")) // key '2'
		{
			EmitSignal(SignalName.OpenDoorPressed, 2);
		}

		if (Input.IsActionJustPressed("close_door")) // key 'c'
		{
			EmitSignal(SignalName.CloseDoorPressed, 0);
		}

		if (Input.IsActionJustPressed("close_door_1")) // key '3'
		{
			EmitSignal(SignalName.CloseDoorPressed, 1);
		}

		if (Input.IsActionJustPressed("close_door_2")) // key '4'
		{
			EmitSignal(SignalName.CloseDoorPressed, 2);
		}

		// GD.Print("Player moving time: ", playerMovingTime);

		if (playerMovingTime >= 0) 
		{
			playerMovingTime --;
			
		}
		else
		{
			GetNode<AnimatedSprite2D>("CatSprite").Play("idle");
		}
	}

	public void ResetPlayer()
	{
		playerMovingTime = 0;
		pos = Position;
		GetNode<GpuParticles2D>("SpawnParticles").Emitting = true;
	}

	async public void MovePlayer(string dir)
	{
		var num = movementSpeed;
		var catSprite = GetNode<AnimatedSprite2D>("CatSprite");
		catSprite.Play("walk");
		playerMovingTime = timeToIdleAnim;

		if (dir == "left")
		{
			while (num <= tileSize && playerMovingTime > 0)
			{
				pos.X -= movementSpeed;
				// Position.X -= movementSpeed;
				Position = pos;
				num += movementSpeed;
				await ToSignal(GetTree().CreateTimer(0.01), SceneTreeTimer.SignalName.Timeout);
			}
		}
		if (dir == "right")
		{
			while (num <= tileSize && playerMovingTime > 0)
			{
				pos.X += movementSpeed;
				// Position.X -= movementSpeed;
				Position = pos;
				num += movementSpeed;
				// GD.Print(pos);
				await ToSignal(GetTree().CreateTimer(0.01), SceneTreeTimer.SignalName.Timeout);
			}
		}
		if (dir == "up")
		{
			while (num <= tileSize && playerMovingTime > 0)
			{
				pos.Y -= movementSpeed;
				// Position.X -= movementSpeed;
				Position = pos;
				num += movementSpeed;
				// GD.Print(pos);
				await ToSignal(GetTree().CreateTimer(0.01), SceneTreeTimer.SignalName.Timeout);
			}
		}
		if (dir == "down")
		{
			while (num <= tileSize && playerMovingTime > 0)
			{
				pos.Y += movementSpeed;
				// Position.X -= movementSpeed;
				Position = pos;
				num += movementSpeed;
				// GD.Print(pos);
				await ToSignal(GetTree().CreateTimer(0.01), SceneTreeTimer.SignalName.Timeout);
			}
		}
	}

	private int[] CalculateSquare(Vector2 pos)
	{
		int[] position = new int[2];
		position[0] = (int)(pos.X - 79)/32;
		position[1] = (int)(pos.Y - 79)/32;

		return position;
	}
}
