using Godot;
using System;

public class DefaultMapScript : Node2D
{
	[Export]
	public int nextLevelSceneID;

	[Export]
	public bool bossLevel = false;

	private Vector2 oldWindowSize;
	private ParallaxBackground parallaxBackground;
	private Position2D spawnPoint;
	private Node2D cameraLimitsNode;
	private bool spawnSet = false;
	private Node2D boss;
	private int nextLevelTimer = 0;
	
	public static PackedScene nextLevel;

	public static DefaultMapScript mapReference;

	public override void _Ready()
	{
		EffectsManager.AttemptSetEnvironmentNode();
		ProjectileManager.AttemptSetProjectileNode();
		oldWindowSize = OS.WindowSize;
		parallaxBackground = GetNode<ParallaxBackground>("Background/ParallaxBackground");
		mapReference = this;

		spawnPoint = GetNode<Position2D>("SpawnPoint");
		Player.player.GlobalPosition = spawnPoint.GlobalPosition;

		int top = 0;
		int bottom = 0;
		int left = 0;
		int right = 0;
		cameraLimitsNode = GetNode<Node2D>("CameraLimits");
		foreach (Position2D cameraLimit in cameraLimitsNode.GetChildren())
		{
			if (cameraLimit.Name.Contains("Top"))
				top = (int)cameraLimit.GlobalPosition.y;
			if (cameraLimit.Name.Contains("Bottom"))
				bottom = (int)cameraLimit.GlobalPosition.y;
			if (cameraLimit.Name.Contains("Left"))
				left = (int)cameraLimit.GlobalPosition.x;
			if (cameraLimit.Name.Contains("Right"))
				right = (int)cameraLimit.GlobalPosition.x;
		}
		Player.SetCameraLimits(top, bottom, left, right);

		if (Name.Contains("City"))
		{
			if (Name == "City4")
				MusicManager.PlayMusic(MusicManager.Music_CityBoss);
			else
				MusicManager.PlayMusic(MusicManager.Music_City);
		}
		if (Name.Contains("mb"))
		{
			if (Name == "mb3")
				MusicManager.PlayMusic(MusicManager.Music_MilitaryBaseBoss);
			else
				MusicManager.PlayMusic(MusicManager.Music_MilitaryBase);
		}
		if (Name.Contains("lab"))
		{
			if (Name != "lab4")
				MusicManager.PlayMusic(MusicManager.Music_Lab);
		}

		ScenesHolder.nextSceneIndex = nextLevelSceneID;

		if (bossLevel)
		{
			if (Name.Contains("City"))
				boss = GetNodeOrNull<Node2D>("WelderBot");

			if (Name.Contains("mb"))
				boss = GetNodeOrNull<Node2D>("RobotTank");

			if (Name.Contains("lab"))
				boss = GetNodeOrNull<Node2D>("FinalBoss");
		}
	}


	public override void _Process(float delta)
	{
		if (!spawnSet && Player.player != null && Player.player.GlobalPosition != spawnPoint.GlobalPosition)
		{
			spawnSet = true;
			Player.player.GlobalPosition = spawnPoint.GlobalPosition;
		}

		if (bossLevel && !IsInstanceValid(boss))
		{
			nextLevelTimer++;
			if (nextLevelTimer > 8 * 60)
			{
				Transitions.FadeIn();
			}
		}
		if (Transitions.fadeInCompleted && bossLevel)
		{
			Transitions.FadeOut();
			ScenesHolder.SwitchScenesTo(ScenesHolder.UI_ShopScreen);
		}

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
