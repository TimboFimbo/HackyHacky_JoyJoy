using Godot;
using System;
using AsciiMaps;
using System.Linq;

public partial class Main : Node
{
	bool printed = false;
	int[] playerPos = {1, 1};
	int[] mapSize = {12, 8};
	char[] walkableBlocks = new char[2] {'.', '#'};

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
		GD.Print("Interact Signal Received");
		var tileMap = GetNode<TileMap>("map_level1");

		// GD.Print(tileMap.GetCellTileData(0, new Vector2I(3, 8)));
		// int abc = tileMap.GetCellSourceId(0, new Vector2I(3, 8));
		// GD.Print(abc.ToString());

		// GD.Print(tileMap.GetCellTileData(0, new Vector2I(8, 4)));
		// int abcd = tileMap.GetCellSourceId(0, new Vector2I(8, 4));
		// GD.Print(abcd.ToString());

		tileMap.SetCell(0, new Vector2I(8, 4), 0, new Vector2I(2, 0), 0);
		walkableBlocks[1] = 'D';

		foreach(char item in walkableBlocks)
		{
			GD.Print("Walkable Block: ", item.ToString());
		}
		// GD.Print("Walkable BLocks: ", walkableBlocks);
	}
}
