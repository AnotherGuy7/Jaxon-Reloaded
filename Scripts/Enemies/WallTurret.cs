using Godot;
using System;

public class WallTurret : RigidBody2D
{
	private Sprite turretGunSprite;

	private int health = 30;
	private int shootTimer = 0;
	private bool playerDetected = false;
	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	public override void _Ready()
	{
		turretGunSprite = GetNode<Sprite>("TurretSprite");	
	}

	public override void _Process(float delta)
	{
		if (shootTimer > 0)
			shootTimer--;

		if (playerDetected)
		{
			Vector2 directionToPlayer = Player.position - GlobalPosition;
			turretGunSprite.GlobalRotation = directionToPlayer.Angle();

			if (shootTimer <= 0)
			{
				shootTimer += 60;
				Vector2 shootVelocity = Player.position - GlobalPosition;
				shootVelocity = shootVelocity.Normalized();
				shootVelocity *= 9f;

				Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_EnemyLaser, 1, GlobalPosition, shootVelocity.Normalized() * 8f, HelperMethods.CollisionType.Player);
				ParticlesManager.AttachParticles(projectile, ParticlesManager.LaserParticles, 3);
			}
		}
	}

	private void OnDetectionAreaEntered(object body)
	{
		if (body == Player.player)
			playerDetected = true;
	}

	private void OnDetectionAreaExited(object body)
	{
		if (body == Player.player)
			playerDetected = false;
	}

	public void Hurt(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			for (int i = 0; i < EffectsManager.random.Next(2, 4 + 1); i++)
			{
				Vector2 explosionPosition = GlobalPosition + new Vector2(EffectsManager.random.Next(-6, 6 + 1), EffectsManager.random.Next(-6, 6 + 1));
				ParticlesManager.CreateExplosion(explosionPosition, 1f);
				if (EffectsManager.random.Next(0, 1 + 1) == 0)
					SoundManager.PlaySound(EffectsManager.random.Next(SoundManager.Sounds_Explosion, SoundManager.Sounds_BigExplosion + 1));
			}
			EffectsManager.ShakeCamera(3, 5);

			ParticlesManager.SpawnUnattatchedParticles(ParticlesManager.SmokeParticles, GlobalPosition, 5f, oneshot: true);
			SoundManager.PlaySound(SoundManager.Sounds_Explosion, 6f, 30);
			LootManager.SpawnCoins(3, GlobalPosition);
			QueueFree();
		}
		else
		{
			SoundManager.PlaySound(SoundManager.Sounds_MetalImpact, 2f, 30);
		}
	}
}
