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

	[Export]
	public PackedScene cityArea1;

	[Export]
	public PackedScene cityArea2;

	[Export]
	public PackedScene cityArea3;

	[Export]
	public PackedScene cityArea4;

	[Export]
	public PackedScene militaryArea1;

	[Export]
	public PackedScene militaryArea2;

	[Export]
	public PackedScene militaryArea3;

	[Export]
	public PackedScene labArea1;

	[Export]
	public PackedScene labArea2;

	[Export]
	public PackedScene labArea3;

	[Export]
	public PackedScene labArea4;

	public static ScenesHolder scenesHolder;

	public PackedScene[] containedPackedScenes;

	public static int nextSceneIndex = 0;

	public const int UI_TitleScreen = 0;
	public const int UI_ShopScreen = 1;
	public const int UI_LevelScreen = 2;
	public const int World_TestScene = 3;
	public const int World_CityArea_1 = 4;
	public const int World_CityArea_2 = 5;
	public const int World_CityArea_3 = 6;
	public const int World_CityArea_4 = 7;
	public const int World_MilitaryArea_1 = 8;
	public const int World_MilitaryArea_2 = 9;
	public const int World_MilitaryArea_3 = 10;
	public const int World_LabArea_1 = 11;
	public const int World_LabArea_2 = 12;
	public const int World_LabArea_3 = 13;
	public const int World_LabArea_4 = 14;

	public override void _Ready()
	{
		scenesHolder = this;

		containedPackedScenes = new PackedScene[15];
		containedPackedScenes[UI_TitleScreen] = titleScreen;
		containedPackedScenes[UI_ShopScreen] = shopScreen;
		containedPackedScenes[UI_LevelScreen] = levelSelectScreen;
		containedPackedScenes[World_TestScene] = testScene;
		containedPackedScenes[World_CityArea_1] = cityArea1;
		containedPackedScenes[World_CityArea_2] = cityArea2;
		containedPackedScenes[World_CityArea_3] = cityArea3;
		containedPackedScenes[World_CityArea_4] = cityArea4;
		containedPackedScenes[World_MilitaryArea_1] = militaryArea1;
		containedPackedScenes[World_MilitaryArea_2] = militaryArea2;
		containedPackedScenes[World_MilitaryArea_3] = militaryArea3;
		containedPackedScenes[World_LabArea_1] = labArea1;
		containedPackedScenes[World_LabArea_2] = labArea2;
		containedPackedScenes[World_LabArea_3] = labArea3;
		containedPackedScenes[World_LabArea_4] = labArea4;

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
