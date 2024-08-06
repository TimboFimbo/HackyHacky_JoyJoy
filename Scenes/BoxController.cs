using Godot;
using System;

public partial class BoxController : Node
{
	int numberOfVarsBoxes = 80;
	float[] varsBoxColour = {0.3f, 0.25f, 0.5f};
	int numberOfCodeBoxes = 176;
	float[] codeBoxColour = {0.5f, 0.25f, 0.5f};
	int numberOfStackBoxes = 80;
	float[] stackBoxColour = {0.5f, 0.25f, 0.3f};
	int boxSize = 30;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetText(string boxToSet, string textToSet)
	{
		GridContainer thisBox = null;
		float[] thisBoxColour = {0.0f, 0.0f, 0.0f};
		var thisTransform = (X: (500, 0), Y: (0, 176), O: (715, 60));
		
		switch (boxToSet)
		{
			case "Vars":
			thisBox = GetNode<GridContainer>("VarsBox");
			thisBoxColour = varsBoxColour;
			thisBox.SetPosition(new Vector2(715, 24));
			thisBox.SetSize(new Vector2(500, 176));
			GD.Print(thisBox.GetTransform());
			break;

			case "Code":
			thisBox = GetNode<GridContainer>("CodeBox");
			thisBoxColour = codeBoxColour;
			break;

			case "Stack":
			thisBox = GetNode<GridContainer>("StackBox");
			thisBoxColour = stackBoxColour;
			break;
		}

		foreach(Node child in thisBox.GetChildren())
		{
			RemoveChild(child);
			GD.Print("Child Removed");
		}

		for( var i = 0; i < textToSet.Length; ++i)
  		{
  			var container = new Control()
			{
  				Size = new Vector2(boxSize, boxSize),
				CustomMinimumSize = new Vector2(boxSize, boxSize)
  			};	

			var label = new RichTextLabel();
			label.BbcodeEnabled = true;
			string bbcode = "[center][font=res://Assets/Fonts/dogica/TTF/dogicapixel.ttf][font_size=32]" + 
				textToSet[i].ToString() + "[/font_size][/font][/center]";
			label.ParseBbcode(bbcode);
			label.CustomMinimumSize = new Vector2(boxSize, boxSize);
			label.Set("theme_override_font_sizes/normal_font_size", 16);

			container.AddChild(new ColorRect()
			{
  				Size = new Vector2(boxSize, boxSize),
  				Color = new Color(thisBoxColour[0], thisBoxColour[1], thisBoxColour[2])
  			});

			container.AddChild(label);
  
  			AddChild(container);
			//RemoveChild(container);
 		}
	}
}
