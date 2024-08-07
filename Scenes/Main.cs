using Godot;
using System;
using System.Linq;
using AsciiMaps;
using OLangCode;

public partial class Main : Node
{
	bool printed = false;
	int[] playerPos = {1, 1};
	int[] mapSize = {12, 8};
	char[] walkableBlocks = new char[4] {'.', '#', '#', 'E'};
	bool inputBoxOpen = false;
	// string curVarsCode = "";
	// string curStackCode = "";

	System.Text.StringBuilder varsCodeTest = new System.Text.StringBuilder(OLangCode.Code.varsLevel1);
	System.Text.StringBuilder stackCodeTest = new System.Text.StringBuilder(OLangCode.Code.emptyStack);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Game Started");

		var outputBox = GetNode<RichTextLabel>("output_box");
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");
		// outputBox.Text = interpreter.TextToCamel("Test Camel Output");
		// interpreter.ParseOLang(OLangCode.Code.oLangLevel1);

		// SetCurVarsAndStack(1);

		// test to ensure vars can be edited
		// EditVars("I Love You!;", 0);

		DisplayCode(1, -1);
		DisplayVars();
		DisplayStack();

		var sprite = GetNode<Player>("player").GetNode<AnimatedSprite2D>("SnakeSprite");
		// GD.Print("Found snake sprite: ", sprite.ToString());
		sprite.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
	}

	public void DirectionSignalReceived(string direction)
	{
		var outputBox = GetNode<RichTextLabel>("output_box");

		if (inputBoxOpen)
		{
			return;
		}

		// GD.Print("Signal received  - ", direction);
		bool moveOk = false;
		GD.Print(moveOk);

		var sprite = GetNode<Player>("player").GetNode<AnimatedSprite2D>("SnakeSprite");

		if (direction == "right")
		{
			if (playerPos[0] < mapSize[0] - 1)
			{
				sprite.FlipH = true;
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0] + 1, playerPos[1]);

				// GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos))
				{
					moveOk = true;
					// GD.Print("Moving player ", direction);
					playerPos[0] += 1;
					if (newPos == 'E') { ExitSteppedOn(); }
				}
			}
		}

		if (direction == "left")
		{
			if (playerPos[0] > 1)
			{
				sprite.FlipH = false;
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0] - 1, playerPos[1]);

				// GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos)) 
				{
					moveOk = true;
					// GD.Print("Moving player ", direction);
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

				// GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos)) 
				{
					moveOk = true;
					// GD.Print("Moving player ", direction);
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

				// GD.Print("newPos = ", newPos.ToString());

				if (walkableBlocks.Contains(newPos)) 
				{
					moveOk = true;
					// GD.Print("Moving player ", direction);
					playerPos[1] -= 1;
					if (newPos == 'E') { ExitSteppedOn(); }
				}
			}
		}

		// GD.Print("Move ok ", moveOk.ToString());

		if (moveOk == true)
		{
			GetNode<Player>("player").MovePlayer(direction);
		}
		else
		{
			// GD.Print("Cannot move there.");
			outputBox.Text = "Cannot Move There";
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
					// GD.Print("Terminal found at " + xPos.ToString() + ", " + yPos.ToString());
					ActivateTerminal();
				}

				xPos = playerPos[0] + i;
				yPos = playerPos[1] + j;

				if (AsciiMaps.Maps.CheckMap(xPos, yPos) == 'T')
				{
					// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
					ActivateTerminal();
				}
			}
		}
	}

	public void OpenDoorSignalReceived(int doorNumber)
	{
		if (inputBoxOpen)
		{
			return;
		}

		var outputBox = GetNode<RichTextLabel>("output_box");

		switch(doorNumber)
		{
			// open any door next to player
			case 0:
			// GD.Print("Any adjacent door to open");
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int xPos = playerPos[0] - i;
					int yPos = playerPos[1] - j;

					if (AsciiMaps.Maps.CheckMap(xPos, yPos) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos) == '2')
					{
						// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						OpenDoor(xPos, yPos);
						outputBox.Text = "Door Opened";
					}

					xPos = playerPos[0] + i;
					yPos = playerPos[1] + j;

					if (AsciiMaps.Maps.CheckMap(xPos, yPos) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos) == '2')
					{
						// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						OpenDoor(xPos, yPos);
						outputBox.Text = "Door Opened";
					}
				}
			}
			break;

			case 1:
			// GD.Print("Door 1 to open");
			for (int i = 0; i < mapSize[0]; i++)
			{
				for (int j = 0; j < mapSize[1]; j++)
				{
					if (AsciiMaps.Maps.CheckMap(i, j) == '1')
					{
						// GD.Print("Door found at " + i.ToString() + ", " + j.ToString());
						OpenDoor(i, j);
					}
				}
			}
			break;

			case 2:
			// GD.Print("Door 2 to open");
			for (int i = 0; i < mapSize[0]; i++)
			{
				for (int j = 0; j < mapSize[1]; j++)
				{
					if (AsciiMaps.Maps.CheckMap(i, j) == '2')
					{
						// GD.Print("Door found at " + i.ToString() + ", " + j.ToString());
						OpenDoor(i, j);
					}
				}
			}
			break;
		}

		for (int i = 0; i < walkableBlocks.Length; i++)
		{
			// GD.Print("Walkable block " + i.ToString() + " = " + walkableBlocks[i].ToString());
		}
	}

	public void InputBoxSignalReceived(string text)
	{
		var inputBox = GetNode<LineEdit>("input_box");
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");
		// GD.Print("Text from input box: ", inputBox.Text);
		inputBox.ReleaseFocus();
		inputBox.Hide();
		inputBoxOpen = false;
		interpreter.currentlyPaused = false;
	}

	public void PrintCommandSignalReceived(string thingToPrint)
	{
		// just printing entire string as debug - will get correct string from Vars
		// GD.Print(thingToPrint);
		var outputBox = GetNode<RichTextLabel>("output_box");

		if (thingToPrint[0] == '$') // for now, all print commands will start with this
		{
			string varToPrint = FindLineToPrint(thingToPrint);
			// GD.Print("Var to Print: ", varToPrint);
			outputBox.Text = varToPrint;
		}
	}

	public void InputCommandSignalReceived()
	{
		var inputBox = GetNode<LineEdit>("input_box");
		inputBox.Show();
		inputBox.GrabFocus();
		inputBoxOpen = true;
	}

	// this currently does the same as above, but wanted to keep errors separate
	public void ErrorCommandSignalReceived(string errorToPrint)
	{
		GD.Print(errorToPrint);
	}

	public void CurCommandChangeSignalReceived(int curCommandNum)
	{
		DisplayCode(1, curCommandNum);
		GD.Print("Cur Command in Main: ", curCommandNum.ToString());
	}

	private void OpenDoor(int xPos, int yPos)
	{
		var tileMap = GetNode<TileMap>("map_level1");
		char doorToOpen = AsciiMaps.Maps.CheckMap(xPos, yPos);

		tileMap.SetCell(0, new Vector2I(xPos, yPos), 0, new Vector2I(2, 0), 0);
		walkableBlocks[doorToOpen - '0'] = doorToOpen;

		// GD.Print("Opening door at " + xPos.ToString() + ", " + yPos.ToString());
	}

	private void ExitSteppedOn()
	{
		// GD.Print("You Win!");

		var outputBox = GetNode<RichTextLabel>("output_box");
		outputBox.Text = "You Win!";
	}

	private void ActivateTerminal()
	{
		// GD.Print("Interact Signal Received");
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");

		// input box should only open when Input command is received
		interpreter.ParseOLang(OLangCode.Code.oLangLevel1);
	}

	private void DisplayCode(int levelNumber, int curLineNumber)
	{
		var codeBox = GetNode<CodeBox>("CodeBox");

		switch (levelNumber)
		{
			case 1:
				codeBox.SetText(OLangCode.Code.oLangLevel1, curLineNumber);
				break;
		}
	}

	private void DisplayVars()
	{
		var varsBox = GetNode<VarsBox>("VarsBox");

		// switch (levelNumber)
		// {
		// 	case 1:
		// 		varsBox.SetText(OLangCode.Code.varsLevel1);
		// 		break;
		// }

		// varsBox.SetText(curVarsCode);
		varsBox.SetText(varsCodeTest.ToString());
	}

	private void DisplayStack()
	{
		var stackBox = GetNode<StackBox>("StackBox");

		// varsBox.SetText(OLangCode.Code.emptyStack);

		// stackBox.SetText(curStackCode);
		stackBox.SetText(stackCodeTest.ToString());
	}

	private string FindLineToPrint(string printCommandString)
	{
		int lineNumber = printCommandString.Remove(0, 1).ToInt() - OLangCode.Code.varsStart;
		// GD.Print("Line Number: ", lineNumber.ToString());

		int startChar = Convert.ToInt32(lineNumber * 1.6); // convert to hex - yes, i know this is stupid!
		// GD.Print("Start Char: ", startChar.ToString());

		string stringToPrint = "";

		// char thisChar = OLangCode.Code.varsLevel1[startChar];

		char thisChar = varsCodeTest[startChar];

		while (thisChar != ';')
		{
			stringToPrint += thisChar;
			startChar++;
			// thisChar = OLangCode.Code.varsLevel1[startChar];
			thisChar = varsCodeTest[startChar];
		}

		return stringToPrint;
	}

	// This function currently does nothing, so will probably be removed
	private void SetCurVarsAndStack(int levelNumber)
	{
		switch (levelNumber)
		{
			case 1:
				// curVarsCode = OLangCode.Code.varsLevel1;
				// curStackCode = OLangCode.Code.emptyStack;
				
				// just to test that the stack string can be changed
				var stackWordToAdd = "TestEdit";
				// curStackCode = stackWordToAdd + curStackCode.Remove(0, stackWordToAdd.Length);

				int offset = 0;

				// another way of testing that strings can be changed
				// for (int i = 0; i < stackWordToAdd.Length; i++)
				// {
				// 	varsCodeTest[i + offset] = stackWordToAdd[i];
				// }

				break;
		}
	}

	private void EditVars(string editedCode, int offset)
	{
		for (int i = 0; i < editedCode.Length; i++)
		{
			varsCodeTest[i + offset] = editedCode[i];
		}	
	}
}
