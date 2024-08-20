using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Signal]
    public delegate void StartGameEventHandler(int levelNumber);
	float[] colourRed = {1.0f, 0.0f, 0.0f};
	bool level1_complete = false, 
		level2_complete = false, 
		level3_complete = false, 
		level4_complete = false, 
		level5_complete = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ButtonPressed(int levelNumber)
	{
		// hide background
		GetNode<TextureRect>("Background").Hide();
		GetNode<Label>("WinMessage").Hide();

		// hide buttons - change to loop
		GetNode<Button>("Button_1").Hide();
		GetNode<Button>("Button_2").Hide();
		GetNode<Button>("Button_3").Hide();
		GetNode<Button>("Button_4").Hide();
		GetNode<Button>("Button_5").Hide();

		EmitSignal(SignalName.StartGame, levelNumber);
	}

	public void ShowMessage(string text)
	{
	    var message = GetNode<Label>("Message");
	    message.Text = text;
	    message.Show();

	    GetNode<Timer>("MessageTimer").Start();
	}

	async public void ShowYouWin(int curLevel)
	{
		SetLevelAsComplete(curLevel);

	    ShowMessage("Level Complete");

	    var messageTimer = GetNode<Timer>("MessageTimer");
	    await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode<Label>("Message");
    	message.Text = "Hacky Hacky Joy Joy";
    	message.Show();

	    var background = GetNode<TextureRect>("Background");
		background.Show();

	    await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		var button1 = GetNode<Button>("Button_1");
		var button2 = GetNode<Button>("Button_2");
		var button3 = GetNode<Button>("Button_3");
		var button4 = GetNode<Button>("Button_4");
		var button5 = GetNode<Button>("Button_5");

		// check if levels are complete, then show buttons
		if (level1_complete){ button1.Modulate = new Color(colourRed[0], colourRed[1], colourRed[2]); }
		if (level2_complete){ button2.Modulate = new Color(colourRed[0], colourRed[1], colourRed[2]); }
		if (level3_complete){ button3.Modulate = new Color(colourRed[0], colourRed[1], colourRed[2]); }
		if (level4_complete){ button4.Modulate = new Color(colourRed[0], colourRed[1], colourRed[2]); }
		if (level5_complete){ button5.Modulate = new Color(colourRed[0], colourRed[1], colourRed[2]); }

	    button1.Show();
		button2.Show();
		button3.Show();
		button4.Show();
		button5.Show();

		CheckForWinState();
	}

	public void SetLevelAsComplete(int levelNumber)
	{
		switch (levelNumber)
		{
			case 1:
				level1_complete = true;
				break;
			case 2:
				level2_complete = true;
				break;
			case 3:
				level3_complete = true;
				break;
			case 4:
				level4_complete = true;
				break;
			case 5:
				level5_complete = true;
				break;
		}
	}

	private void CheckForWinState()
	{
		if (level1_complete && 
			level2_complete &&
			level3_complete &&
			level4_complete &&
			level5_complete)
			{
				GetNode<Label>("WinMessage").Show();
			}
	}

	private void OnButton1Pressed()
	{
		ButtonPressed(1);
	}

	private void OnButton2Pressed()
	{
		ButtonPressed(2);
	}

	private void OnButton3Pressed()
	{
		ButtonPressed(3);
	}

	private void OnButton4Pressed()
	{
		ButtonPressed(4);
	}

	private void OnButton5Pressed()
	{
		ButtonPressed(5);
	}

	private void OnMessageTimerTimeout()
	{
	    GetNode<Label>("Message").Hide();
	}
}
