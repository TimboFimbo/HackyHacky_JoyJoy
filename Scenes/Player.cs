using Godot;
using System;

public partial class Player : Area2D
{
	int tileSize = 32;
	int ScreenSize;
	int movementSpeed = 1;
	Vector2 pos = new Vector2(80, 80);
	[Signal]
	public delegate void DirectionPressedEventHandler(string pressed);
	[Signal]
	public delegate void InteractPressedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pos = Position;
		// GD.Print(pos);
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

		if (Input.IsActionJustPressed("interact"))
		{
			EmitSignal(SignalName.InteractPressed);
		}
	}

	async public void MovePlayer(string dir)
	{
		var num = movementSpeed;

		if (dir == "left")
		{
			while (num <= tileSize)
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
			while (num <= tileSize)
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
			while (num <= tileSize)
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
			while (num <= tileSize)
			{
				pos.Y += movementSpeed;
				// Position.X -= movementSpeed;
				Position = pos;
				num += movementSpeed;
				// GD.Print(pos);
				await ToSignal(GetTree().CreateTimer(0.01), SceneTreeTimer.SignalName.Timeout);
			}
		}
		// GD.Print("Square = " + CalculateSquare(Position)[0] + "," + CalculateSquare(Position)[1]);
	}

	private int[] CalculateSquare(Vector2 pos)
	{
		int[] position = new int[2];
		position[0] = (int)(pos.X - 79)/32;
		position[1] = (int)(pos.Y - 79)/32;

		return position;
	}
}
