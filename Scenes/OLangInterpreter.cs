using Godot;
using System;
using System.Threading;
using Constants;

public partial class OLangInterpreter : Node
{
	[Signal]
	public delegate void PrintCommandEventHandler(string toPrint);
	[Signal]
	public delegate void ErrorCommandEventHandler(string errorToPrint);
	[Signal]
	public delegate void CurCommandChangeEventHandler(int curCommandNum);
	[Signal]
	public delegate void CurStackChangeEventHandler(int curStackCommandNum);
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
	public int curStackCommandNum = 0;
	public bool currentlyPaused = false;
	public bool codePausedByStack = false;
	public bool stackCurrentlyPaused = false;
	private float timeBetweenLines = 1.0f;
	private int stackLineLength = 16;
	public string runString = "";
	public bool currentlyResetting = false;

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

	// Reset everything
	public void ResetEverything()
	{
		curCommandNum = 0;
		curStackCommandNum = 0;
		currentlyPaused = false;
		codePausedByStack = false;
		stackCurrentlyPaused = false;
		currentlyResetting = true;
	}

	// Here it is - the fun part. Takes the oLang code, splits it, and runs each command
	// Cannot access children of Main, so sends signal when needed
	public async void ParseOLang(string oLangCode)
	{
		GD.Print("Parsing oLangCode...");
		string[] commandLines = oLangCode.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < commandLines.Length; i++)
		{
			while (currentlyPaused || codePausedByStack) 
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
						// EmitSignal(SignalName.ErrorCommand, "Nothing to print!");
						UponErrorFound(Constants.Errors.ERR_NO_PRINT);
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
						// EmitSignal(SignalName.ErrorCommand, "No address to input to!");
						UponErrorFound(Constants.Errors.ERR_NO_ADDRESS);
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

				// TODO: This needs to check if function can be parsed first
				case "RUN": //runs a prewritten function
					if (commandLines[i].Length < 4)
					{
						// EmitSignal(SignalName.ErrorCommand, "Nothing to run!");
						UponErrorFound(Constants.Errors.ERR_NO_RUN);
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
								// EmitSignal(SignalName.ErrorCommand, "No Valid Function");
								UponErrorFound(Constants.Errors.ERR_NO_FUNC);
							}
						}
						else
						{
							lines = arguments.Split(',');
						}

						// check if int can be parsed first
						try
						{
						  var testParse = lines[1].ToInt();
						}
						catch (Exception e)
						{
						  Console.WriteLine(e.Message);
						  UponErrorFound(Constants.Errors.ERR_NO_FUNC);
						  break;
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
							// EmitSignal(SignalName.ErrorCommand, "Not a function!");
							UponErrorFound(Constants.Errors.ERR_NO_FUNC);
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
		// TODO: have errors end program, and check if values can be parsed to int
		string blankLine = "                ";
		System.Text.StringBuilder inputLineParse = new System.Text.StringBuilder(blankLine);
		System.Text.StringBuilder retLineParse = new System.Text.StringBuilder(blankLine);
		System.Text.StringBuilder argsLineParse = new System.Text.StringBuilder(blankLine);
		bool errorFound = false;
		codePausedByStack = true;
		currentlyResetting = false;

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

		string finalRetLine = "";

		if (retLineParse[0] == '$')
		{
			finalRetLine = retLineParse.Remove(0,1).ToString();

			// check if int can be parsed first
			try
			{
				var testParse = finalRetLine.ToInt();
				if (testParse < 2000 || testParse > 2090)
				{
					errorFound = true;
					// EmitSignal(SignalName.ErrorCommand, "Return Line Error");
					UponErrorFound(Constants.Errors.ERR_RET_LINE);
				}
			}
			catch (Exception e)
			{
			  Console.WriteLine(e.Message);
			  UponErrorFound(Constants.Errors.ERR_RET_LINE);
			  errorFound = true;
			}
		}
		else
		{
			errorFound = true;
			// EmitSignal(SignalName.ErrorCommand, "Return Line Error");
			UponErrorFound(Constants.Errors.ERR_RET_LINE);
		}

		string finalArgsLine = "";

		if (argsLineParse[0] == '$')
		{
			finalArgsLine = argsLineParse.Remove(0,1).ToString();

			try
			{
				// TODO add way of parsing hex chars, in case offset includes them
				var testParse = finalArgsLine.ToInt();
				if (testParse < 1000 || testParse > 1040)
				{
					errorFound = true;
					// EmitSignal(SignalName.ErrorCommand, "Args Line Error");
					UponErrorFound(Constants.Errors.ERR_ARGS_LINE);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			  	UponErrorFound(Constants.Errors.ERR_ARGS_LINE);
			  	errorFound = true;
			}
		}
		else
		{
			errorFound = true;
			// EmitSignal(SignalName.ErrorCommand, "Args Line Error");
			UponErrorFound(Constants.Errors.ERR_ARGS_LINE);
		}

		if (!errorFound)
		{
			GD.Print("No stack parsing errors found");

			EmitSignal(SignalName.CurStackChange, 2);
			var lineToUpdate = finalArgsLine[2].ToString().ToInt();
			var offset = finalArgsLine[3].ToString().ToInt();
			await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);

			while (stackCurrentlyPaused) 
			{ 
				await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout); 
			}

			if (!currentlyResetting)
			{
				EmitSignal(SignalName.CurStackChange, 0);
				EmitSignal(SignalName.StackInputCommand, inputLineParse.ToString(), lineToUpdate, offset);
				await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);
			}

			while (stackCurrentlyPaused) 
			{ 
				await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout); 
			}

			if (!currentlyResetting)
			{
				EmitSignal(SignalName.CurStackChange, 1);
				curCommandNum = finalRetLine[2].ToString().ToInt();
				await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);
			}

			while (stackCurrentlyPaused) 
			{ 
				await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout); 
			}

			EmitSignal(SignalName.CurStackChange, -1);
			// await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);

			codePausedByStack = false;
			currentlyPaused = false;
			currentlyResetting = false;
		}
		else { GD.Print("Stack Parsing Error"); }

		GD.Print("Stack Input Parsed: ", inputLineParse.ToString());
		GD.Print("Stack Return Parsed: ", retLineParse.ToString());
		GD.Print("Stack Args Parsed: ", argsLineParse.ToString());
		// await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);
		// codePausedByStack = false;
		// currentlyPaused = false;
		// currentlyResetting = false;
	}
	
	private async void UponErrorFound(string errorMessage)
	{
		EmitSignal(SignalName.ErrorCommand, errorMessage);
		await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
		EmitSignal(SignalName.EndInputCommand);
		codePausedByStack = false;
		currentlyPaused = false;
		currentlyResetting = false;
	}
}
