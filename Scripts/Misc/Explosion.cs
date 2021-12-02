using Godot;
using System;

public class Explosion : AnimatedSprite
{
	public int soundType = -1;
	public float delayTime = 0;

	public override void _Ready()
	{
		GetNode<Timer>("DelayTimer").Start(delayTime);
	}

	private void OnDelayEnded()
	{
		Visible = true;
		Play("default");

		if (soundType != -1)
		{
			SoundManager.PlaySound(soundType);
			soundType = -1;
		}
	}

	private void OnAnimationFinished()
	{
		QueueFree();
	}
}
