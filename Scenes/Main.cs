using Godot;
using System;
using AsciiMaps;
using System.Linq;

public partial class Main : Node
{
	bool printed = false;
	int[] playerPos = {1, 1};
	int[] mapSize = {12, 8};
	char[] walkableBlocks = new char[4] {'.', '#', '#', 'E'};
	bool inputBoxOpen = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Game Started");

		// GD.Print("Coordinates (0, 0) contains a " + AsciiMaps.Maps.CheckMap(0, 0));
		// GD.Print("Coordinates (2, 4) contains a ", AsciiMaps.Maps.CheckMap(2, 4));
		// GD.Print("Coordinates (3, 7) contains a ", AsciiMaps.Maps.CheckMap(3, 7));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// while (!printed)
		// {
		// 	GD.Print("Coordinates (0, 0) contains a " + AsciiMaps.Maps.CheckMap(0, 0));
		// 	GD.Print("Coordinates (4, 2) contains a %s", AsciiMaps.Maps.CheckMap(4, 2));
		// 	GD.Print("Coordinates (9, 3) contains a %s", AsciiMaps.Maps.CheckMap(9, 3));
		// 	printed = true;
		// }

		// GD.Print("Something");
	}

	public void DirectionSignalReceived(string direction)
	{
		if (inputBoxOpen)
		{
			return;
		}

		GD.Print("Signal received  - ", direction);
		bool moveOk = false;
		GD.Print(moveOk);

		if (direction == "right")
		{
			if (playerPos[0] < mapSize[0] - 1)
			{
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0] + 1, playerPos[1]);

				GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos))
				{
					moveOk = true;
					GD.Print("Moving player ", direction);
					playerPos[0] += 1;
					if (newPos == 'E') { ExitSteppedOn(); }
				}
			}
		}

		if (direction == "left")
		{
			if (playerPos[0] > 1)
			{
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0] - 1, playerPos[1]);

				GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos)) 
				{
					moveOk = true;
					GD.Print("Moving player ", direction);
					playerPos[0] -= 1;
					if (newPos == 'E') { ExitSteppedOn(); }
				}
			}
		}

		if (direction == "down")
		{
			if (playerPos[1] < mapSize[1] - 1)
			{
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0], playerPos[1] + 1);

				GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos)) 
				{
					moveOk = true;
					GD.Print("Moving player ", direction);
					playerPos[1] += 1;
					if (newPos == 'E') { ExitSteppedOn(); }
				}
			}
		}

		if (direction == "up")
		{
			if (playerPos[1] > 1)
			{
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0], playerPos[1] - 1);

				GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos)) 
				{
					moveOk = true;
					GD.Print("Moving player ", direction);
					playerPos[1] -= 1;
					if (newPos == 'E') { ExitSteppedOn(); }
				}
			}
		}

		GD.Print("Move ok ", moveOk.ToString());

		if (moveOk == true)
		{
			GetNode<Player>("player").MovePlayer(direction);
		}
		else
		{
			GD.Print("Cannot move there.");
		}
	}

	public void InteractSignalReceived()
	{
		if (inputBoxOpen)
		{
			return;
		}

		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				int xPos = playerPos[0] - i;
				int yPos = playerPos[1] - j;

				if (AsciiMaps.Maps.CheckMap(xPos, yPos) == 'T')
				{
					GD.Print("Terminal found at " + xPos.ToString() + ", " + yPos.ToString());
					ActivateTerminal();
				}

				xPos = playerPos[0] + i;
				yPos = playerPos[1] + j;

				if (AsciiMaps.Maps.CheckMap(xPos, yPos) == 'T')
				{
					GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
					ActivateTerminal();
				}
			}
		}

		// This needs to open the terminal

		// var tileMap = GetNode<TileMap>("map_level1");
		// tileMap.SetCell(0, new Vector2I(8, 4), 0, new Vector2I(2, 0), 0);
		// walkableBlocks[1] = 'D';

		// foreach(char item in walkableBlocks)
		// {
		// 	GD.Print("Walkable Block: ", item.ToString());
		// }
	}

	public void OpenDoorSignalReceived(int doorNumber)
	{
		if (inputBoxOpen)
		{
			return;
		}

		switch(doorNumber)
		{
			// open any door next to player
			case 0:
			GD.Print("Any adjacent door to open");
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int xPos = playerPos[0] - i;
					int yPos = playerPos[1] - j;

					if (AsciiMaps.Maps.CheckMap(xPos, yPos) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos) == '2')
					{
						GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						OpenDoor(xPos, yPos);
					}

					xPos = playerPos[0] + i;
					yPos = playerPos[1] + j;

					if (AsciiMaps.Maps.CheckMap(xPos, yPos) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos) == '2')
					{
						GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						OpenDoor(xPos, yPos);
					}
				}
			}
			break;

			case 1:
			GD.Print("Door 1 to open");
			for (int i = 0; i < mapSize[0]; i++)
			{
				for (int j = 0; j < mapSize[1]; j++)
				{
					if (AsciiMaps.Maps.CheckMap(i, j) == '1')
					{
						GD.Print("Door found at " + i.ToString() + ", " + j.ToString());
						OpenDoor(i, j);
					}
				}
			}
			break;

			case 2:
			GD.Print("Door 2 to open");
			for (int i = 0; i < mapSize[0]; i++)
			{
				for (int j = 0; j < mapSize[1]; j++)
				{
					if (AsciiMaps.Maps.CheckMap(i, j) == '2')
					{
						GD.Print("Door found at " + i.ToString() + ", " + j.ToString());
						OpenDoor(i, j);
					}
				}
			}
			break;
		}

		for (int i = 0; i < walkableBlocks.Length; i++)
		{
			GD.Print("Walkable block " + i.ToString() + " = " + walkableBlocks[i].ToString());
		}
	}

	public void InputBoxSignalReceived(string text)
	{
		var inputBox = GetNode<LineEdit>("input_box");
		GD.Print("Text from input box: ", inputBox.Text);
		inputBox.ReleaseFocus();
		inputBox.Hide();
		inputBoxOpen = false;
	}

	private void OpenDoor(int xPos, int yPos)
	{
		var tileMap = GetNode<TileMap>("map_level1");
		char doorToOpen = AsciiMaps.Maps.CheckMap(xPos, yPos);

		tileMap.SetCell(0, new Vector2I(xPos, yPos), 0, new Vector2I(2, 0), 0);
		walkableBlocks[doorToOpen - '0'] = doorToOpen;

		GD.Print("Opening door at " + xPos.ToString() + ", " + yPos.ToString());
	}

	private void ExitSteppedOn()
	{
		GD.Print("You Win!");
	}

	private void ActivateTerminal()
	{
		GD.Print("Interact Signal Received");
		var inputBox = GetNode<LineEdit>("input_box");
		inputBox.Show();
		inputBox.GrabFocus();
		inputBoxOpen = true;
	}
}
