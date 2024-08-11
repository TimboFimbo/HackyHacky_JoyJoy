using Godot;
using System;
using System.Threading;

public partial class OLangInterpreter : Node
{
	[Signal]
	public delegate void PrintCommandEventHandler(string toPrint);
	[Signal]
	public delegate void ErrorCommandEventHandler(string errorToPrint);
	[Signal]
	public delegate void CurCommandChangeEventHandler(int curCommandNum);
	[Signal]
	public delegate void InputCommandEventHandler(string addressToInputTo, int lengthOfInput, int curLineNum);
	[Signal]
	public delegate void StackInputCommandEventHandler(string input, int lineToUpdate, int offset);
	[Signal]
	public delegate void EndInputCommandEventHandler();
	// [Signal]
	// public delegate void RunCommandEventHandler(string functionToRun, string args);
	[Signal]
	public delegate void OpenDoorCommandEventHandler(int doorNumber);
	[Signal]
	public delegate void CloseDoorCommandEventHandler(int doorNumber);
	[Signal]
	public delegate void CheckCodeCommandEventHandler(int inputCode);
	[Signal]
	public delegate void GenCodeCommandEventHandler(int codeLength);
	[Signal]
	public delegate void RunStringNeededEventHandler(string memAddress);
	public int curCommandNum = 0;
	public bool currentlyPaused = false;
	private float timeBetweenLines = 1.0f;
	private int stackLineLength = 16;
	public string runString = "";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Just a test to ensure it's working
	public string TextToCamel(string textToSet)
	{
		return textToSet.ToCamelCase();
	}

	// Here it is - the fun part. Takes the oLang code, splits it, and runs each command
	// Cannot access children of Main, so sends signal when needed
	public async void ParseOLang(string oLangCode)
	{
		GD.Print("Parsing oLangCode...");
		string[] commandLines = oLangCode.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < commandLines.Length; i++)
		{
			while (currentlyPaused) 
			{ 
				await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout); 
			}

			if (i != curCommandNum)
			{
				continue;
			}

			// GD.Print(commandLines[i]);

			string command = commandLines[i][0].ToString() + commandLines[i][1] + commandLines[i][2];
			GD.Print(command);
			EmitSignal(SignalName.CurCommandChange, curCommandNum);

			switch (command)
			{
				case "PRT": // for printing
					if (commandLines[i].Length < 4)
					{
						EmitSignal(SignalName.ErrorCommand, "Nothing to print!");
					}
					else
					{
						string arguments = "";
						for (int j = 3; j < commandLines[i].Length; j++)
						{
							arguments += commandLines[i][j];
						}
						EmitSignal(SignalName.PrintCommand, arguments);
					}
					curCommandNum++;
					break;

				case "INP": // for player input
					string[] inpArguments = new string[2];
					if (commandLines[i].Length < 4)
					{
						EmitSignal(SignalName.ErrorCommand, "No address to input to!");
					}
					else
					{
						string arguments = "";
						for (int j = 4; j < commandLines[i].Length; j++)
						{
							if (commandLines[i][j] != ' ')
							{
								arguments += commandLines[i][j];
							}
							inpArguments = arguments.Split(',');
						}
					}
					EmitSignal(SignalName.InputCommand, inpArguments[0], inpArguments[1].ToInt(), curCommandNum + 1);
					currentlyPaused = true;
					curCommandNum++;
					break;

				case "END": // when program reaches end
					EmitSignal(SignalName.EndInputCommand);
					break;

				case "RUN": //runs a prewritten function
					if (commandLines[i].Length < 4)
					{
						EmitSignal(SignalName.ErrorCommand, "Nothing to run!");
					}
					else
					{
						string arguments = "";
						for (int j = 3; j < commandLines[i].Length; j++)
						{
							arguments += commandLines[i][j];
						}

						// first remove whitespace
						string commandNoWhite = arguments.Replace(" ", "");

						string[] lines = new string[2];

						if (arguments[0] == '$')
						{
							runString = "";
							EmitSignal(SignalName.RunStringNeeded, arguments);

							while(runString == "")
							{
								await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout); 
							}
							GD.Print("Run String: ", runString);
							// now split into function and argument
							if (runString.Contains(','))
							{
								lines = runString.Split(',');
							}
							else
							{
								EmitSignal(SignalName.ErrorCommand, "No Valid Function");
							}
						}
						else
						{
							lines = arguments.Split(',');
						}

						// check the function and send signal
						if (lines[0] == "open_door")
						{
							EmitSignal(SignalName.OpenDoorCommand, lines[1].ToInt());
						}
						else if (lines[0] == "close_door")
						{
							EmitSignal(SignalName.CloseDoorCommand, lines[1].ToInt());
						}
						else if (lines[0] == "null_func")
						{
							GD.Print("Null func run");
						}
						else if (lines[0] == "gen_code")
						{
							EmitSignal(SignalName.GenCodeCommand, lines[1].ToInt());
						}
						else if (lines[0] == "check_code")
						{
							EmitSignal(SignalName.CheckCodeCommand, lines[1].ToInt());
						}
						else
						{
							EmitSignal(SignalName.ErrorCommand, "Not a function!");
						}

						GD.Print("Run command: ", lines[0]);
						GD.Print("Run Args: ", lines[1]);
					}
					curCommandNum++;
					break;
			}

			// Thread.Sleep(500);
			await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);
		}
	}

	public async void ParseStackInput(string stackCode)
	{
		string blankLine = "                ";
		System.Text.StringBuilder inputLineParse = new System.Text.StringBuilder(blankLine);
		System.Text.StringBuilder retLineParse = new System.Text.StringBuilder(blankLine);
		System.Text.StringBuilder argsLineParse = new System.Text.StringBuilder(blankLine);
		bool errorFound = false;

		GD.Print("Stack Code to be Parsed: ", stackCode);

		// first parse the stack lines
		for (int i = 0; i < stackLineLength * 3; i++)
		{
			if (i < stackLineLength)
			{
				if (stackCode[i] >= ' ')
				{
					inputLineParse[i] = stackCode[i];
				}
			}

			if (i >= stackLineLength && i < stackLineLength * 2)
			{
				if (stackCode[i] != ' ')
				{
					retLineParse[i - stackLineLength] = stackCode[i];
				}
			}

			if (i >= stackLineLength * 2 && i < stackLineLength * 3)
			{
				if (stackCode[i] != ' ')
				{
					argsLineParse[i - stackLineLength * 2] = stackCode[i];
				}
				
			}
		}

		// add check to ensure value can be parsed to int, and don't hard code values
		if (retLineParse.ToString().ToInt() < 2000 || retLineParse.ToString().ToInt() > 2090)
		{
			errorFound = true;
			EmitSignal(SignalName.ErrorCommand, "Stack Return Line Error");
		}

		if (argsLineParse.ToString().ToInt() < 1000 || argsLineParse.ToString().ToInt() > 1040)
		{
			errorFound = true;
			EmitSignal(SignalName.ErrorCommand, "Stack Args Line Error");
		}

		if (!errorFound)
		{
			curCommandNum = retLineParse[2].ToString().ToInt();
			var lineToUpdate = argsLineParse[2].ToString().ToInt();
			var offset = argsLineParse[3].ToString().ToInt();
			EmitSignal(SignalName.StackInputCommand, inputLineParse.ToString(), lineToUpdate, offset);
		}
		else { GD.Print("Stack Parsing Error"); }

		GD.Print("Stack Input Parsed: ", inputLineParse.ToString());
		GD.Print("Stack Return Parsed: ", retLineParse.ToString());
		GD.Print("Stack Args Parsed: ", argsLineParse.ToString());
		await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);
	}
}
