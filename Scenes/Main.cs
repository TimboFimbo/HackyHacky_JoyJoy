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
	Vector2 playerStartPos = new Vector2(88, 113);
	// Vector2 playerSpriteStartPos = new Vector2(80, 80);
	char[] walkableBlocks = new char[4] {'.', '#', '#', 'E'};
	bool inputBoxOpen = false;
	bool currentlyPlaying = false;
	bool helpMenuOpen = false;
	string curInpAddress;
	int curInpLength;
	int curLineNum;
	int curLevel = 1;
	int lineLength = 16;
	// string curVarsCode = "";
	// string curStackCode = "";

	System.Text.StringBuilder varsCodeTest = new System.Text.StringBuilder(OLangCode.Code.varsLevel1);
	System.Text.StringBuilder stackCodeTest = new System.Text.StringBuilder(OLangCode.Code.emptyStack);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResetLevel(curLevel);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
	}

	private void ResetLevel(int levelNumber)
	{
		curLevel = levelNumber;
		GD.Print("Game Started");

		var tileMap1 = GetNode<TileMap>("map_level1");
		var tileMap2 = GetNode<TileMap>("map_level2");
		var tileMap3 = GetNode<TileMap>("map_level3");
		var tileMap4 = GetNode<TileMap>("map_level4");
		var tileMap5 = GetNode<TileMap>("map_level5");
		var playerSprite = GetNode<Player>("player");
		var inputBox = GetNode<LineEdit>("input_box");
		var outputBox = GetNode<RichTextLabel>("output_box");
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");

		printed = false;
		playerPos[0] = 1;
		playerPos[1] = 1;
		// walkableBlocks = new char[4] {'.', '#', '#', 'E'};
		inputBoxOpen = false;
		playerSprite.Position = playerStartPos;
		playerSprite.ResetPlayer();
		stackCodeTest = new System.Text.StringBuilder(OLangCode.Code.emptyStack);
		inputBox.Text = "";
		outputBox.Text = "Output Box";
		TerminalLightOn(false);
		ExitDoorOpened(false);
		SetPlayPauseIcon("play", false);
		SetPlayPauseIcon("pause", false);
		currentlyPlaying = false;

		interpreter.ResetEverything();

		if (curLevel == 1)
		{
			varsCodeTest = new System.Text.StringBuilder(OLangCode.Code.varsLevel1);
			tileMap1.Show();
			tileMap2.Hide();
			tileMap3.Hide();
			tileMap4.Hide();
			tileMap5.Hide();
		}

		if (curLevel == 2)
		{
			varsCodeTest = new System.Text.StringBuilder(OLangCode.Code.varsLevel2);
			tileMap1.Hide();
			tileMap2.Show();
			tileMap3.Hide();
			tileMap4.Hide();
			tileMap5.Hide();
		}

		if (curLevel == 3)
		{
			varsCodeTest = new System.Text.StringBuilder(OLangCode.Code.varsLevel3);
			tileMap1.Hide();
			tileMap2.Hide();
			tileMap3.Show();
			tileMap4.Hide();
			tileMap5.Hide();
		}

		if (curLevel == 4)
		{
			varsCodeTest = new System.Text.StringBuilder(OLangCode.Code.varsLevel4);
			tileMap1.Hide();
			tileMap2.Hide();
			tileMap3.Hide();
			tileMap4.Show();
			tileMap5.Hide();
		}

		if (curLevel == 5)
		{
			varsCodeTest = new System.Text.StringBuilder(OLangCode.Code.varsLevel5);
			tileMap1.Hide();
			tileMap2.Hide();
			tileMap3.Hide();
			tileMap4.Hide();
			tileMap5.Show();
		}

		DisplayCode(curLevel, -1);
		DisplayVars();
		DisplayStack(-1);
		CloseDoorSignalReceived(1);
		CloseDoorSignalReceived(2);

		// var sprite = GetNode<Player>("player").GetNode<AnimatedSprite2D>("SnakeSprite");
		var sprite = GetNode<Player>("player").GetNode<AnimatedSprite2D>("CatSprite");
		sprite.Play("idle");
	}

	// *** Signal Received Methods ***

	public void StartGameSignalReceived(int levelNumber)
	{
		var hud = GetNode<Hud>("HUD");
		hud.ShowMessage("Level " + levelNumber.ToString());

		ResetLevel(levelNumber);
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
		// GD.Print(moveOk);

		// var sprite = GetNode<Player>("player").GetNode<AnimatedSprite2D>("SnakeSprite");
		var sprite = GetNode<Player>("player").GetNode<AnimatedSprite2D>("CatSprite");

		if (direction == "right")
		{
			if (playerPos[0] < mapSize[0] - 1)
			{
				sprite.FlipH = false;
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0] + 1, playerPos[1], curLevel);

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
				sprite.FlipH = true;
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0] - 1, playerPos[1], curLevel);

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
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0], playerPos[1] + 1, curLevel);

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
				char newPos = AsciiMaps.Maps.CheckMap(playerPos[0], playerPos[1] - 1, curLevel);

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
			// outputBox.Text = "Cannot Move There";
		}
	}

	public void InteractSignalReceived()
	{
		if (inputBoxOpen || currentlyPlaying)
		{
			return;
		}

		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				int xPos = playerPos[0] - i;
				int yPos = playerPos[1] - j;

				if (AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == 'T')
				{
					// GD.Print("Terminal found at " + xPos.ToString() + ", " + yPos.ToString());
					ActivateTerminal();
				}

				xPos = playerPos[0] + i;
				yPos = playerPos[1] + j;

				if (AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == 'T')
				{
					// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
					ActivateTerminal();
				}
			}
		}
	}

	public void PauseSignalReceived()
	{
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");
		var outputBox = GetNode<RichTextLabel>("output_box");

		// interpreter.currentlyPaused = !interpreter.currentlyPaused;

		if (inputBoxOpen || !currentlyPlaying)
		{
			return;
		}

		if (interpreter.currentlyPaused)
		{
			interpreter.currentlyPaused = false;
		}
		else
		{
			interpreter.currentlyPaused = true;
		}

		if (interpreter.stackCurrentlyPaused)
		{
			interpreter.stackCurrentlyPaused = false;
			SetPlayPauseIcon("play", true);
			SetPlayPauseIcon("pause", false);
			// outputBox.Text = "           -- PLAY --";
		}
		else
		{
			interpreter.stackCurrentlyPaused = true;
			SetPlayPauseIcon("play", false);
			SetPlayPauseIcon("pause", true);
			// outputBox.Text = "          -- PAUSE --";
		}
	}

	public void ResetSignalReceived()
	{
		if (inputBoxOpen)
		{
			return;
		}

		ResetLevel(curLevel);
	}

	public void HelpSignalReceived()
	{
		var helpMenu = GetNode<Hud>("HUD").GetNode<ColorRect>("HelpMenu");
		var hint = GetNode<Hud>("HUD").GetNode<ColorRect>("HelpMenu").GetNode<RichTextLabel>("HelpMenu_Hint");

		if (helpMenuOpen)
		{
			helpMenu.Hide();
			helpMenuOpen = false;
		}

		else
		{
			hint.Text = "Level Hint: " + Constants.Hints.hints[curLevel - 1];
			helpMenu.Show();
			helpMenuOpen = true;
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

					if (AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '2')
					{
						// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						OpenDoor(xPos, yPos);
						// outputBox.Text = "Door Opened";
					}

					xPos = playerPos[0] + i;
					yPos = playerPos[1] + j;

					if (AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '2')
					{
						// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						OpenDoor(xPos, yPos);
						// outputBox.Text = "Door Opened";
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
					if (AsciiMaps.Maps.CheckMap(i, j, curLevel) == '1')
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
					if (AsciiMaps.Maps.CheckMap(i, j, curLevel) == '2')
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

	public void CloseDoorSignalReceived(int doorNumber)
	{
		if (inputBoxOpen)
		{
			return;
		}

		var outputBox = GetNode<RichTextLabel>("output_box");

		switch(doorNumber)
		{
			// close any door next to player
			case 0:
			// GD.Print("Any adjacent door to close");
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int xPos = playerPos[0] - i;
					int yPos = playerPos[1] - j;

					if (AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '2')
					{
						// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						CloseDoor(xPos, yPos);
						// outputBox.Text = "Door Closed";
					}

					xPos = playerPos[0] + i;
					yPos = playerPos[1] + j;

					if (AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '1' ||
					AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel) == '2')
					{
						// GD.Print("Door found at " + xPos.ToString() + ", " + yPos.ToString());
						CloseDoor(xPos, yPos);
						// outputBox.Text = "Door Closed";
					}
				}
			}
			break;

			case 1:
			// GD.Print("Door 1 to close");
			for (int i = 0; i < mapSize[0]; i++)
			{
				for (int j = 0; j < mapSize[1]; j++)
				{
					if (AsciiMaps.Maps.CheckMap(i, j, curLevel) == '1')
					{
						// GD.Print("Door found at " + i.ToString() + ", " + j.ToString());
						CloseDoor(i, j);
					}
				}
			}
			break;

			case 2:
			// GD.Print("Door 2 to close");
			for (int i = 0; i < mapSize[0]; i++)
			{
				for (int j = 0; j < mapSize[1]; j++)
				{
					if (AsciiMaps.Maps.CheckMap(i, j, curLevel) == '2')
					{
						// GD.Print("Door found at " + i.ToString() + ", " + j.ToString());
						CloseDoor(i, j);
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

		EditStack(text);
		DisplayStack(-1);

		inputBox.ReleaseFocus();
		inputBox.Hide();
		inputBoxOpen = false;
		interpreter.currentlyPaused = true;
		interpreter.ParseStackInput(stackCodeTest.ToString());
		// interpreter.currentlyPaused = false;
	}

	public void PrintCommandSignalReceived(string thingToPrint)
	{
		// just printing entire string as debug - will get correct string from Vars
		// GD.Print(thingToPrint);
		var outputBox = GetNode<RichTextLabel>("output_box");

		if (thingToPrint.Contains(','))
		{
			string[] stringsToPrint = thingToPrint.Split(',');
			string stringToPrint = FindLineToPrint(stringsToPrint[0]) +
				" " + FindLineToPrint(stringsToPrint[1]);

			outputBox.Text = stringToPrint;
		}
		else 
		{
			string varToPrint = FindLineToPrint(thingToPrint);
			// GD.Print("Var to Print: ", varToPrint);
			outputBox.Text = varToPrint;
		}
	}

	public void InputCommandSignalReceived(string addressToInputTo, int lengthOfInput, int lineNum)
	{
		GD.Print("Address to input to: ", addressToInputTo.ToString());
		GD.Print("Length of input: ", lengthOfInput.ToString());
		GD.Print("Current Line: ", lineNum.ToString());

		curInpAddress = "$" + addressToInputTo;
		curInpLength = lengthOfInput;
		curLineNum = lineNum;

		var inputBox = GetNode<LineEdit>("input_box");
		inputBox.Show();
		inputBox.GrabFocus();
		inputBoxOpen = true;
	}

	public void StackInputCommandReceived(string input, int lineToUpdate, int offset)
	{
		int realOffset = lineToUpdate * 16 + offset;
		var finalInput = input.Substring(0, input.Length - offset);

		EditVars(finalInput, realOffset);
		DisplayVars();
		GetNode<OLangInterpreter>("OLangInterpreter").codePausedByStack = true;
	}

	// this currently does the same as above, but wanted to keep errors separate
	public void ErrorCommandSignalReceived(string errorToPrint)
	{
		var outputBox = GetNode<RichTextLabel>("output_box");
		GD.Print(errorToPrint);
		outputBox.Text = errorToPrint;
	}

	public void CurCommandChangeSignalReceived(int curCommandNum)
	{
		DisplayCode(curLevel, curCommandNum);
		GD.Print("Cur Command in Main: ", curCommandNum.ToString());
	}

	public void CurStackChangeSignalReceived(int curStackLineNum)
	{
		DisplayStack(curStackLineNum);
		GD.Print("Cur Stack Line in Main: ", curStackLineNum.ToString());
	}

	public void GenCommandSignalReceived(int codeLength)
	{
		string s = string.Empty;
		Random rnd = new Random();

		if (codeLength > 16) { codeLength = 16; }

		for (int i = 0; i < codeLength; i++)
		{
			s = String.Concat(s, rnd.Next(10).ToString());
		}

		EditVars(s, 0);
		DisplayVars();
	}

	public void CheckCommandSignalReceived(int inputCode)
	{
		if (inputCode == 0)
		{
			string randomCode = FindLineToPrint("$1000");
			string enteredCode = FindLineToPrint("$1010");

			if (randomCode == enteredCode)
			{
				OpenDoorSignalReceived(1);
			}
			else 
			{
				ErrorCommandSignalReceived("Code Incorrect");
			}
		}
	}

	public void RunStringNeededSignalReceived(string memAddress)
	{
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");
		interpreter.runString = FindLineToPrint(memAddress);
	}

	public void EndCommandSignalReceived()
	{
		var outputBox = GetNode<RichTextLabel>("output_box");
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");

		outputBox.Text = "Program Ended";

		DisplayCode(curLevel, -1); // removes highlighting from grid line
		interpreter.curCommandNum = 0;
		TerminalLightOn(false);

		SetPlayPauseIcon("play", false);
		SetPlayPauseIcon("pause", false);
		currentlyPlaying = false;
	}

	// *** Local Methods ***

	private void OpenDoor(int xPos, int yPos)
	{
		TileMap tileMap = GetNode<TileMap>("map_level1");

		if (curLevel == 2)
		{
			tileMap = GetNode<TileMap>("map_level2");
		}
		if (curLevel == 3)
		{
			tileMap = GetNode<TileMap>("map_level3");
		}
		if (curLevel == 4)
		{
			tileMap = GetNode<TileMap>("map_level4");
		}
		if (curLevel == 5)
		{
			tileMap = GetNode<TileMap>("map_level5");
		}

		char doorToOpen = AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel);

		tileMap.SetCell(0, new Vector2I(xPos, yPos), 1, new Vector2I(2, 0), 0);
		walkableBlocks[doorToOpen - '0'] = doorToOpen;

		// GD.Print("Opening door at " + xPos.ToString() + ", " + yPos.ToString());
	}

	private void CloseDoor(int xPos, int yPos)
	{
		TileMap tileMap = GetNode<TileMap>("map_level1");

		if (curLevel == 2)
		{
			tileMap = GetNode<TileMap>("map_level2");
		}
		if (curLevel == 3)
		{
			tileMap = GetNode<TileMap>("map_level3");
		}
		if (curLevel == 4)
		{
			tileMap = GetNode<TileMap>("map_level4");
		}
		if (curLevel == 5)
		{
			tileMap = GetNode<TileMap>("map_level5");
		}

		char doorToClose = AsciiMaps.Maps.CheckMap(xPos, yPos, curLevel);

		tileMap.SetCell(0, new Vector2I(xPos, yPos), 1, new Vector2I(7, 0), 0);
		walkableBlocks[doorToClose - '0'] = '#';

		// GD.Print("Opening door at " + xPos.ToString() + ", " + yPos.ToString());
	}

	private void TerminalLightOn(bool on=true)
	{
		TileMap tileMap = GetNode<TileMap>("map_level1");

		if (curLevel == 2)
		{
			tileMap = GetNode<TileMap>("map_level2");
		}
		if (curLevel == 3)
		{
			tileMap = GetNode<TileMap>("map_level3");
		}
		if (curLevel == 4)
		{
			tileMap = GetNode<TileMap>("map_level4");
		}
		if (curLevel == 5)
		{
			tileMap = GetNode<TileMap>("map_level5");
		}

		for (int i = 0; i < mapSize[0]; i++)
		{
			for (int j = 0; j < mapSize[1]; j++)
			{
				if (AsciiMaps.Maps.CheckMap(i, j, curLevel) == 'T')
				{
					if (on)
					{
						tileMap.SetCell(0, new Vector2I(i, j), 1, new Vector2I(12, 0), 0);
					}
					else
					{
						tileMap.SetCell(0, new Vector2I(i, j), 1, new Vector2I(11, 0), 0);
					}
				}
			}
		}
	}

	private void ExitDoorOpened(bool on=true)
	{
		TileMap tileMap = GetNode<TileMap>("map_level1");

		if (curLevel == 2)
		{
			tileMap = GetNode<TileMap>("map_level2");
		}
		if (curLevel == 3)
		{
			tileMap = GetNode<TileMap>("map_level3");
		}
		if (curLevel == 4)
		{
			tileMap = GetNode<TileMap>("map_level4");
		}
		if (curLevel == 5)
		{
			tileMap = GetNode<TileMap>("map_level5");
		}

		for (int i = 0; i < mapSize[0]; i++)
		{
			for (int j = 0; j < mapSize[1]; j++)
			{
				if (AsciiMaps.Maps.CheckMap(i, j, curLevel) == 'E')
				{
					if (on)
					{
						tileMap.SetCell(0, new Vector2I(i, j), 1, new Vector2I(18, 0), 0);
					}
					else
					{
						tileMap.SetCell(0, new Vector2I(i, j), 1, new Vector2I(10, 0), 0);
					}
				}
			}
		}
	}

	private void ExitSteppedOn()
	{
		// GD.Print("You Win!");

		var outputBox = GetNode<RichTextLabel>("output_box");
		outputBox.Text = "You Win!";
		ExitDoorOpened(true);

		GetNode<Hud>("HUD").ShowYouWin(curLevel);
	}

	private void ActivateTerminal()
	{
		// GD.Print("Interact Signal Received");
		var interpreter = GetNode<OLangInterpreter>("OLangInterpreter");

		// input box should only open when Input command is received
		if (curLevel == 1)
		{
			interpreter.ParseOLang(OLangCode.Code.oLangLevel1);
		}
		if (curLevel == 2)
		{
			interpreter.ParseOLang(OLangCode.Code.oLangLevel2);
		}
		if (curLevel == 3)
		{
			interpreter.ParseOLang(OLangCode.Code.oLangLevel3);
		}
		if (curLevel == 4)
		{
			interpreter.ParseOLang(OLangCode.Code.oLangLevel4);
		}
		if (curLevel == 5)
		{
			interpreter.ParseOLang(OLangCode.Code.oLangLevel5);
		}

		TerminalLightOn(true);
		SetPlayPauseIcon("play", true);
		SetPlayPauseIcon("pause", false);
		currentlyPlaying = true;
		interpreter.currentlyPaused = false;
		interpreter.codePausedByStack = false;
	}

	private void SetPlayPauseIcon(string playOrPause, bool on)
	{
		float[] iconGrey = {0.3f, 0.3f, 0.3f};
		float[] iconWhite = {1.0f, 1.0f, 1.0f};

		var playIcon = GetNode<Hud>("HUD").GetNode<TextureRect>("PlayIcon");
		var pauseIcon = GetNode<Hud>("HUD").GetNode<TextureRect>("PauseIcon");

		if (playOrPause == "play")
		{
			if (on) { playIcon.Modulate = new Color(iconWhite[0], iconWhite[1], iconWhite[2]); }
			else { playIcon.Modulate = new Color(iconGrey[0], iconGrey[1], iconGrey[2]); }
		}
		else
		{
			if (on) { pauseIcon.Modulate = new Color(iconWhite[0], iconWhite[1], iconWhite[2]); }
			else { pauseIcon.Modulate = new Color(iconGrey[0], iconGrey[1], iconGrey[2]); }
		}
	}

	private void DisplayCode(int levelNumber, int curLineNumber)
	{
		var codeBox = GetNode<CodeBox>("CodeBox");

		switch (levelNumber)
		{
			case 1:
				codeBox.SetText(OLangCode.Code.oLangLevel1, curLineNumber);
				break;
			case 2:
				codeBox.SetText(OLangCode.Code.oLangLevel2, curLineNumber);
				break;
			case 3:
				codeBox.SetText(OLangCode.Code.oLangLevel3, curLineNumber);
				break;
			case 4:
				codeBox.SetText(OLangCode.Code.oLangLevel4, curLineNumber);
				break;
			case 5:
				codeBox.SetText(OLangCode.Code.oLangLevel5, curLineNumber);
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

	private void DisplayStack(int curStackLineNum)
	{
		var stackBox = GetNode<StackBox>("StackBox");

		// varsBox.SetText(OLangCode.Code.emptyStack);

		// stackBox.SetText(curStackCode);
		stackBox.SetText(stackCodeTest.ToString(), curStackLineNum);
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

		for (int i = startChar; i < startChar + lineLength; i++)
		{
			stringToPrint += varsCodeTest[i];
		}

		// while (thisChar != ';')
		// {
		// 	stringToPrint += thisChar;
		// 	startChar++;
		// 	// thisChar = OLangCode.Code.varsLevel1[startChar];
		// 	thisChar = varsCodeTest[startChar];
		// }

		return stringToPrint.Trim();
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

	private void EditStack(string boxInput)
	{
		int rowSize = 16;
		string returnAddress = "$" + (2000 + curLineNum * 10).ToString();
		int numberOfInputChars;

		GD.Print("Box Input Received: ", boxInput);

		// first set the arguments
		for (int i = 0; i < curInpAddress.Length; i++)
		{
			stackCodeTest[i + rowSize * 2] = curInpAddress[i];
		}		

		// now set the return address
		for (int i = 0; i < returnAddress.Length; i++)
		{
			stackCodeTest[i + rowSize] = returnAddress[i];
		}

		// now set the input
		if (boxInput.Length > rowSize * 3 - 1) { numberOfInputChars = rowSize * 3; }
		else { numberOfInputChars = boxInput.Length; }

		for (int i = 0; i < numberOfInputChars; i++)
		{
			stackCodeTest[i] = boxInput[i];
		}

	}
}
