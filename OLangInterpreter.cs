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
	public delegate void InputCommandEventHandler();
	private int curCommandNum = 0;
	public bool currentlyPaused = false;
	private float timeBetweenLines = 1.0f;

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
					EmitSignal(SignalName.InputCommand);
					currentlyPaused = true;
					curCommandNum++;
					break;
			}

			// Thread.Sleep(500);
			await ToSignal(GetTree().CreateTimer(timeBetweenLines), SceneTreeTimer.SignalName.Timeout);
		}
	}
}
