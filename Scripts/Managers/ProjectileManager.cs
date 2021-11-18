using Godot;
using System;

public class ProjectileManager : Node2D
{
	public Node2D projectileNode;
	public static ProjectileManager projectileManager;

	public const int Projectile_PlayerBullet = 0;
	public const int Projectile_EnemyLaser = 1;

	[Export]
	public PackedScene[] projectiles;

	public override void _Ready()
	{
		projectileManager = this;
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
