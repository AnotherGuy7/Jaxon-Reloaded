using Godot;
using System;

public class ExitSign : Sprite
{
	public bool exitSignTouched = false;

	public override void _Process(float delta)
	{
		if (exitSignTouched && Transitions.fadeInCompleted)
        {
			Transitions.FadeOut();
			ScenesHolder.SwitchScenesTo(ScenesHolder.UI_ShopScreen);
        }
	}

	private void OnExitAreaEntered(object body)
	{
		if (body == Player.player)
		{
			Transitions.FadeIn();
			exitSignTouched = true;
		}
	}
}
