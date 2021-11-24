using Godot;
using System;

public class LevelSelectScreen : Control
{
	private bool pressedTestButton = false;

	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		if (pressedTestButton && Transitions.fadeInCompleted)
		{
			ScenesHolder.SwitchScenesTo(ScenesHolder.World_TestScene);
			Transitions.FadeOut();
		}
	}

	private void OnTempMapButtonPressed()
	{
		pressedTestButton = true;
		Transitions.FadeIn();
	}
}
