using Godot;
using System;

public partial class CodeBox : GridContainer
{
	int numberOfCharBoxes = 160;
	float[] boxColour = {0.5f, 0.25f, 0.5f};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetText(string textToSet)
	{
		var boxSize = 30;

		foreach(Node child in this.GetChildren())
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
			label.SetText(textToSet[i].ToString());
			label.CustomMinimumSize = new Vector2(boxSize, boxSize);
			label.Set("theme_override_font_sizes/normal_font_size", 16);

			// container.AddChild(new RichTextLabel()
			// {
			// 	Text = "H",
			// 	// set(add_theme_font_size_override("normal_font_size", 50)),
			// 	CustomMinimumSize = new Vector2(100, 100)
			// });

			container.AddChild(new ColorRect()
			{
  				Size = new Vector2(boxSize, boxSize),
  				Color = new Color(boxColour[0], boxColour[1], boxColour[2])
  			});

			container.AddChild(label);
  
  			AddChild(container);
			//RemoveChild(container);
 		}
	}
}
