using Godot;
using System;

public class ScenesHolder : Node
{
	[Export]
	public PackedScene titleScreen;

	[Export]
	public PackedScene shopScreen;

	[Export]
	public PackedScene testScene;

	[Export]
	public PackedScene levelSelectScreen;

	public static ScenesHolder scenesHolder;

	public PackedScene[] containedPackedScenes;

	public const int UI_TitleScreen = 0;
	public const int UI_ShopScreen = 1;
	public const int UI_LevelScreen = 2;
	public const int World_TestScene = 3;

	public override void _Ready()
	{
		scenesHolder = this;

		containedPackedScenes = new PackedScene[4];
		containedPackedScenes[UI_TitleScreen] = titleScreen;
		containedPackedScenes[UI_ShopScreen] = shopScreen;
		containedPackedScenes[UI_LevelScreen] = levelSelectScreen;
		containedPackedScenes[World_TestScene] = testScene;
	}

	public static void SwitchScenesTo(int sceneID)
	{
		scenesHolder.CallDeferred(nameof(scenesHolder.DeferredSwitchScenes), sceneID);
	}

	private void DeferredSwitchScenes(int sceneID)
	{
		GetTree().ChangeSceneTo(containedPackedScenes[sceneID]);
	}
}
