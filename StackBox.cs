using Godot;
using System;
using System.Linq;

public partial class StackBox : GridContainer
{
	int numberOfCharBoxes = 80;
	float[] boxColour = {0.5f, 0.25f, 0.3f};
	float[] highlightColour = {0.2f, 0.7f, 0.2f};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetText(string textToSet, int curStackLineNumber)
	{
		var boxSize = 30;
		int[] highlightedBoxes = new int[16];
		float[] curColour = highlightColour;

		for (int i = 0; i < highlightedBoxes.Length; i++)
		{
			highlightedBoxes[i] = -1;
		}

		if (curStackLineNumber >= 0)
		{
			Array.Clear(highlightedBoxes, 0, highlightedBoxes.Length);
			for (int i = 0; i < 16; i++)
			{
				highlightedBoxes[i] = curStackLineNumber * 16 + i;
				// GD.Print("Added to highlighted boxes: ", highlightedBoxes[i].ToString());
			}
		}

		foreach(Node child in this.GetChildren())
		{
			RemoveChild(child);
			// GD.Print("Child Removed");
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

			if (highlightedBoxes.Contains(i))
			{
				curColour = highlightColour;
			}
			else
			{
				curColour = boxColour;
			}
			
			container.AddChild(new ColorRect()
			{
  				Size = new Vector2(boxSize, boxSize),
  				Color = new Color(curColour[0], curColour[1], curColour[2])
  			});

			container.AddChild(label);
  
  			AddChild(container);
			//RemoveChild(container);
 		}
	}
}
