using Godot;
using System;

public class ProjectileManager : Node2D
{
	public Node2D projectileNode;
	public static ProjectileManager projectileManager;

	public const int Projectile_PlayerBullet_Yellow = 0;
	public const int Projectile_PlayerBullet_Orange = 1;
	public const int Projectile_PlayerBullet_Blue = 2;
	public const int Projectile_PlayerBullet_Green = 3;
	public const int Projectile_EnemyLaser = 4;

	[Export]
	public PackedScene[] projectiles;

	public override void _Ready()
	{
		projectileManager = this;
	}

	public static void AttemptSetProjectileNode()
	{
		SceneTree sceneTree = projectileManager.GetTree();
		if (IsInstanceValid(sceneTree.CurrentScene.GetNodeOrNull<Node2D>("ProjectileNode")))
			projectileManager.projectileNode = projectileManager.GetTree().CurrentScene.GetNode<Node2D>("ProjectileNode");
	}

	public void SetProjectileNode()
	{
		projectileNode = GetTree().CurrentScene.GetNode<Node2D>("ProjectileNode");
	}

	public static Node2D NewProjectile(int projectileType, int damage, Vector2 position, Vector2 velocity, HelperMethods.CollisionType collisionType)
	{
		if (projectileManager.projectileNode == null)
			projectileManager.SetProjectileNode();

		Area2D projectile = (Area2D)projectileManager.projectiles[projectileType].Instance();
		projectile.GlobalPosition = position;
		projectileManager.projectileNode.AddChild(projectile);
		projectile.Set("velocity", velocity);
		projectile.Set("damage", damage);
		projectile.Set("collisionType", collisionType);

		return projectile;
	}
}
