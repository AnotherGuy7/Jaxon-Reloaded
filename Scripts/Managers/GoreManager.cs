using Godot;
using System;

public class GoreManager : Node2D
{
	[Export]
	public PackedScene emptyGoreParticle;

	[Export]
	public Texture[] textures;

	public static GoreManager goreManager;

	public const int AmountOfGore = 3;
	public const int Barrel_1 = 0;
	public const int Barrel_2 = 1;
	public const int Barrel_3 = 2;

	public override void _Ready()
	{
		goreManager = this;
	}

	public static void SpawnGore(int textureIndex, Vector2 position, Vector2 initialVelocity)
	{
		goreManager.CallDeferred(nameof(DeferredSpawnGore), textureIndex, position, initialVelocity);
	}

	private static void DeferredSpawnGore(int textureIndex, Vector2 position, Vector2 initialVelocity)
	{
		RigidBody2D newGore = (RigidBody2D)goreManager.emptyGoreParticle.Instance();
		newGore.GlobalPosition = position;
		newGore.LinearVelocity = initialVelocity;
		newGore.Set("goreTextureIndex", textureIndex);

		goreManager.AddChild(newGore);
	}
}
