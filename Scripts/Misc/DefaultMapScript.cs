using Godot;
using System;

public class DefaultMapScript : Node2D
{
	private Vector2 oldWindowSize;
	private ParallaxBackground parallaxBackground;

	public override void _Ready()
	{
		EffectsManager.AttemptSetEnvironmentNode();
		ProjectileManager.AttemptSetProjectileNode();
		oldWindowSize = OS.WindowSize;
		parallaxBackground = GetNode<ParallaxBackground>("Background/ParallaxBackground");
	}


	public override void _Process(float delta)
	{
		if (OS.WindowSize != oldWindowSize)
		{
			oldWindowSize = OS.WindowSize;
			//ReOffsetAllBackgrounds();
		}
	}


	private void ReOffsetAllBackgrounds()		//This was for when we wanted to have the game stay resizable, but there were just too many issues with that.
	{
		int childrenCount = parallaxBackground.GetChildren().Count;
		for (int i = 0; i < childrenCount; i++)
		{
			ParallaxLayer backgroundLayer = parallaxBackground.GetChild(i) as ParallaxLayer;

			/*
			 * 			float yChangeRatio = oldWindowSize.y / 600f;
			float lostWindowSpace = ((yChangeRatio * DefaultParallaxOffset) * ScreenSpaceMultiplier) * (1f - ScreenSpaceMultiplier - 0.1f);

			float newOffset = ((yChangeRatio * DefaultParallaxOffset) * ScreenSpaceMultiplier) + (lostWindowSpace * WindowSpaceMultiplier);
			backgroundLayer.MotionOffset = new Vector2(0f, newOffset);
			if (i == 1)
				GD.Print(backgroundLayer.MotionOffset);
			 */

			float windowScreenDivisionFactor = oldWindowSize.y / 280f;
			float lostWindowSpace = ((oldWindowSize.y / 600f) * -280f) * (0.98f - 0.1f);

			float yChangeRatio = (oldWindowSize.y / 600f);

			backgroundLayer.MotionOffset = new Vector2(0f, -190f * yChangeRatio);
		}
		//parallaxBackground.Scale = new Vector2(oldWindowSize.x / 1024f, oldWindowSize.y / 600f) * 3f;
	}
}
