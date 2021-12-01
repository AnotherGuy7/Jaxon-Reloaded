using Godot;
using System;

public class TitleScreen : Control
{
	private bool startPressed = false;
	private bool shopPressed = false;

	[Export]
	public PackedScene helicopterSilhouette;

	private Control backgroundNode;
	private TextureRect darkSmoke1Node;
	private TextureRect darkSmoke2Node;
	private Panel controlsPanel;

	public override void _Ready()
	{
		backgroundNode = GetNode<Control>("Background");
		darkSmoke1Node = GetNode<TextureRect>("Background/DarkSmoke_1");
		darkSmoke2Node = GetNode<TextureRect>("Background/DarkSmoke_2");
		controlsPanel = GetNode<Panel>("ControlsPanel");

		MusicManager.PlayMusic(MusicManager.Music_Title);
	}

	public override void _Process(float delta)
	{
		if (Transitions.fadeInCompleted)
		{
			if (startPressed)
				ScenesHolder.SwitchScenesTo(ScenesHolder.UI_LevelScreen);
			else if (shopPressed)
				ScenesHolder.SwitchScenesTo(ScenesHolder.UI_ShopScreen);

			Transitions.FadeOut();
		}

		if (EffectsManager.random.Next(0, 90 + 1) == 0 || Input.IsKeyPressed((int)KeyList.M))
		{
			TextureRect newHelicopter = (TextureRect)helicopterSilhouette.Instance();
			if (EffectsManager.random.Next(0, 1 + 1) == 0)
			{
				newHelicopter.RectGlobalPosition = new Vector2(-32, 30 + EffectsManager.random.Next(-12, 12 + 1));
			}
			else
			{
				newHelicopter.Set("moveSpeed", -1.7f);
				newHelicopter.RectGlobalPosition = new Vector2(1056, 30 + EffectsManager.random.Next(-12, 12 + 1));
				newHelicopter.FlipH = true;
			}
			//newHelicopter.RectScale *= 2f;
			backgroundNode.AddChild(newHelicopter);
		}

		if (darkSmoke1Node.RectGlobalPosition.x <= -80f)
		{
			darkSmoke1Node.RectGlobalPosition = Vector2.Zero;
		}
		if (darkSmoke2Node.RectGlobalPosition.x >= 320f)
		{
			darkSmoke2Node.RectGlobalPosition = Vector2.Zero;
		}

		if (Input.IsKeyPressed((int)KeyList.Escape))
		{
			controlsPanel.Visible = false;
		}
	}

	private void OnStartPressed()
	{
		startPressed = true;
		shopPressed = false;
		Transitions.FadeIn();
	}


	private void OnControlsButtonPressed()
	{
		controlsPanel.Visible = true;
	}


	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
