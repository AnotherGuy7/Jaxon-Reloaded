using Godot;

public class GoreManager : Node2D
{
	[Export]
	public PackedScene emptyGoreParticle;

	[Export]
	public Texture[] textures;

	public static CanvasLayer goreLayer;
	public static GoreManager goreManager;

	public const int AmountOfGore = 8;
	public const int Gore_Barrel1 = 0;
	public const int Gore_Barrel2 = 1;
	public const int Gore_Barrel3 = 2;
	public const int Gore_Drone1 = 3;
	public const int Gore_Drone2 = 4;
	public const int Gore_CityGuard1 = 5;
	public const int Gore_CityGuard2 = 6;
	public const int Gore_CityGuard3 = 7;

	public override void _Ready()
	{
		goreManager = this;
		goreLayer = GetNode<CanvasLayer>("GoreLayer");
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

		goreLayer.AddChild(newGore);
	}
}
