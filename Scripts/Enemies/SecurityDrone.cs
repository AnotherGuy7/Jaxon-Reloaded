using Godot;
using System;

public class SecurityDrone : RigidBody2D
{
	[Export]
	public bool turretDrone = false;

	private int floatTimer = 0;
	private int health = MaxHealth;
	private int shootTimer = 120;

	private const float MoveSpeed = 4.7f;
	private const int MaxHealth = 8;

	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	private Sprite turretSprite;
	private AnimatedSprite droneAnim;
	private AudioStreamPlayer2D laserShootSound;
	
	private bool playerDetected = false;

	public override void _Ready()
	{
		turretSprite = GetNode<Sprite>("DroneTurret");
		droneAnim = GetNode<AnimatedSprite>("DroneAnim");
		laserShootSound = GetNode<AudioStreamPlayer2D>("LaserShootSound");
		floatTimer = EffectsManager.random.Next(0, 360 + 1);
		turretSprite.Visible = turretDrone;
	}
	
	public override void _Process(float delta)
	{
		if (shootTimer > 0)
			shootTimer--;

		if (turretSprite.Visible)
		{
			Vector2 directionToPlayer = Player.position - GlobalPosition;
			turretSprite.GlobalRotation = directionToPlayer.Angle();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (playerDetected)
		{
			Vector2 directionToPlayer = Player.position - GlobalPosition;
			velocity.x = directionToPlayer.x;
			velocity = velocity.Normalized();
			velocity *= MoveSpeed;

			float distanceToPlayer = directionToPlayer.Length();
			if (distanceToPlayer <= 240f && turretSprite.Visible)
			{
				if (shootTimer <= 0)
				{
					shootTimer += 90;
					Vector2 shootPosition = GlobalPosition + new Vector2(0f, 2.5f);
					Vector2 shootVelocity = directionToPlayer;
					Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_EnemyLaser, 1, shootPosition, shootVelocity.Normalized() * 8f, HelperMethods.CollisionType.Player);
					ParticlesManager.AttachParticles(projectile, ParticlesManager.LaserParticles, 3);
					laserShootSound.Play();
				}
				velocity *= 0.5f;
			}
			droneAnim.FlipH = directionToPlayer.x < 0;
		}

		MoveLocalX(velocity.x * delta * 14f);
		MoveLocalY(velocity.y * delta * 14f);
	}

	private void OnExplosionAreaEntered(object body)
	{
		if (body == Player.player && !turretDrone)
		{
			Player.player.Call("Hurt", 1);
			for (int i = 0; i < EffectsManager.random.Next(1, 2 + 1); i++)
			{
				int goreIndex = EffectsManager.random.Next(GoreManager.Gore_Drone1, GoreManager.Gore_Drone2 + 1);
				Vector2 goreVelocity = new Vector2(EffectsManager.random.Next(-8, 8 + 1) / 5f, -EffectsManager.random.Next(25, 50 + 1));
				GoreManager.SpawnGore(goreIndex, GlobalPosition, goreVelocity);
			}
			for (int i = 0; i < EffectsManager.random.Next(2, 4 + 1); i++)
			{
				Vector2 explosionPosition = GlobalPosition + new Vector2(EffectsManager.random.Next(-6, 6 + 1), EffectsManager.random.Next(-6, 6 + 1));
				ParticlesManager.CreateExplosion(explosionPosition, 1f);
				if (EffectsManager.random.Next(0, 1 + 1) == 0)
					SoundManager.PlaySound(EffectsManager.random.Next(SoundManager.Sounds_Explosion, SoundManager.Sounds_BigExplosion + 1));
			}

			ParticlesManager.SpawnUnattatchedParticles(ParticlesManager.SmokeParticles, GlobalPosition, 5f, oneshot: true);
			SoundManager.PlaySound(SoundManager.Sounds_Explosion, 6f, 30);
			QueueFree();
		}
	}

	private void OnDetectionAreaEntered(object body)
	{
		if (body == Player.player)
		{
			playerDetected = true;
			shootTimer = 120;
		}
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
			for (int i = 0; i < EffectsManager.random.Next(1, 2 + 1); i++)
			{
				int goreIndex = EffectsManager.random.Next(GoreManager.Gore_Drone1, GoreManager.Gore_Drone2 + 1);
				Vector2 goreVelocity = new Vector2(EffectsManager.random.Next(-8, 8 + 1) / 5f, -EffectsManager.random.Next(25, 50 + 1));
				GoreManager.SpawnGore(goreIndex, GlobalPosition, goreVelocity);
			}
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
			SoundManager.PlaySound(SoundManager.Sounds_MetalImpact, 6f, 30);
		}
	}
}
