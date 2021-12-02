using Godot;
using System;

public class RobotTurret : RigidBody2D
{
	private Area2D detectionArea;
	private Position2D shootPosition;

	private int health = 40;
	private bool playerDetected = false;
	private int shootTimer = 0;
	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	public override void _Ready()
	{
		detectionArea = GetNode<Area2D>("DetectionArea");
		shootPosition = GetNode<Position2D>("ShootPosition");

		Position2D detectionPoint = GetNodeOrNull<Position2D>("DetectionPoint");
		if (detectionPoint != null)
		{
			detectionArea.Scale = new Vector2(detectionPoint.GlobalPosition.x - GlobalPosition.x, 1f);
		}
	}

	public override void _Process(float delta)
	{
		if (shootTimer > 0)
			shootTimer--;

		if (playerDetected && shootTimer <= 0)
		{
			shootTimer += 40;
			Vector2 shootVelocity = shootPosition.GlobalPosition - GlobalPosition;
			shootVelocity = shootVelocity.Normalized();
			shootVelocity *= 9f;

			Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_EnemyLaser, 1, shootPosition.GlobalPosition + new Vector2(0f, 4f), shootVelocity.Normalized() * 8f, HelperMethods.CollisionType.Player);
			ParticlesManager.AttachParticles(projectile, ParticlesManager.LaserParticles, 3);
		}
	}

	private void OnDetectionAreaBodyEntered(object body)
	{
		if (body == Player.player)
			playerDetected = true;
	}


	private void OnDetectionAreaBodyExited(object body)
	{
		if (body == Player.player)
			playerDetected = false;
	}

	private void OnAreaEntered(object area)
	{
		Area2D areaNode = area as Area2D;
		if (HelperMethods.CollisionTypeMatch(areaNode, collisionType))
			Hurt((int)areaNode.Get("damage"));
	}

	public void Hurt(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			for (int i = 0; i < EffectsManager.random.Next(3, 5 + 1); i++)
			{
				Vector2 explosionPosition = GlobalPosition + new Vector2(EffectsManager.random.Next(-18, 18 + 1), EffectsManager.random.Next(-18, 18 + 1));
				ParticlesManager.CreateExplosion(explosionPosition, 0.5f);
				if (EffectsManager.random.Next(0, 1 + 1) == 0)
					SoundManager.PlaySound(EffectsManager.random.Next(SoundManager.Sounds_Explosion, SoundManager.Sounds_BigExplosion + 1));
			}

			EffectsManager.ShakeCamera(3, 5);
			SoundManager.PlaySound(SoundManager.Sounds_BigExplosion);
			LootManager.SpawnCoins(2, GlobalPosition);
			QueueFree();
		}
	}
}
