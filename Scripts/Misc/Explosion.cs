using Godot;
using System;

public class Explosion : AnimatedSprite
{
	public int soundType = 0;
	public float delayTime = 0;

	public override void _Ready()
	{
		GetNode<Timer>("DelayTimer").Start(delayTime);
	}

	private void OnDelayEnded()
	{
		Visible = true;
		Play("default");

		if (soundType != 0)
			SoundManager.PlaySound(soundType);
	}

	private void OnAnimationFinished()
	{
		QueueFree();
	}
}
