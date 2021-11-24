using Godot;
using System;

public class LootManager : Node2D
{
	[Export]
	public PackedScene[] coins;

	public const int Loot_Unibit = 0;
	public const int Loot_Pentabit = 1;
	public const int Loot_Decibit = 2;
	public const int Loot_Health = 3;

	public static LootManager lootManager;

	public override void _Ready()
	{
		lootManager = this;
	}

	public static void SpawnLoot(int lootType, Vector2 position, int dropChance)
	{
		if (EffectsManager.environmentNodeLoaded && EffectsManager.random.Next(0, 100 + 1) > dropChance)
			return;

		Node2D loot = (Node2D)lootManager.coins[lootType].Instance();
		loot.GlobalPosition = position;
		EffectsManager.environmentNode.AddChild(loot);
	}

	/// <summary>
	/// Spawns coins at a max of the input amount.
	/// </summary>
	/// <param name="totalCoinSum">The amount of coins that could drop.</param>
	/// <param name="position">The poition the coins will spawn in</param>
	public static void SpawnCoins(int totalCoinCount, Vector2 position)
	{
		for (int i = 0; i < totalCoinCount; i++)
		{
			int coinType = EffectsManager.random.Next(Loot_Unibit, Loot_Decibit + 1);
			lootManager.CallDeferred(nameof(SpawnLoot), coinType, position, 30 / (coinType + 1));
		}
	}
}
