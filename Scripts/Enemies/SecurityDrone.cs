using Godot;
using System;

public class SecurityDrone : RigidBody2D
{
	private int floatTimer = 0;
	private int health = MaxHealth;

	private const float MoveSpeed = 4.7f;
	private const int MaxHealth = 8;

	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	private Sprite laserSprite;
	private AnimatedSprite droneAnim;
	private AudioStreamPlayer2D laserShootSound;

	public override void _Ready()
	{
		laserSprite = GetNode<Sprite>("DroneLaser");
		droneAnim = GetNode<AnimatedSprite>("DroneAnim");
		laserShootSound = GetNode<AudioStreamPlayer2D>("LaserShootSound");
		floatTimer = EffectsManager.random.Next(0, 360 + 1);
	}
	
	public override void _Process(float delta)
	{
		Vector2 directionToPlayer = Player.position - GlobalPosition;
		laserSprite.GlobalRotation = directionToPlayer.Angle();
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		Vector2 directionToPlayer = Player.position - GlobalPosition;
		if (Math.Abs(directionToPlayer.x) >= 60f)
		{
			velocity.x = directionToPlayer.x;
			velocity = velocity.Normalized();
			velocity *= MoveSpeed;
		}

		floatTimer++;
		if (floatTimer >= 360)
			floatTimer = 0;

		velocity.y = (float)Math.Sin(floatTimer / 16f) * 1.2f;

		float distanceToPlayer = directionToPlayer.Length();
		if (distanceToPlayer <= 180f)
		{
			if (EffectsManager.random.Next(0, 400 + 1) <= 0)
			{
				Vector2 shootPosition = GlobalPosition + new Vector2(0f, 2.5f);
				Vector2 shootVelocity = directionToPlayer;
				Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_EnemyLaser, 1, shootPosition, shootVelocity.Normalized() * 8f, HelperMethods.CollisionType.Player);
				ParticlesManager.AttachParticles(projectile, ParticlesManager.LaserParticles, 3);
				laserShootSound.Play();
			}
		}
		droneAnim.FlipH = directionToPlayer.x < 0;

		MoveLocalX(velocity.x * delta * 14f);
		MoveLocalY(velocity.y * delta * 14f);
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
			EffectsManager.ShakeCamera(3, 5);

			ParticlesManager.SpawnUnattatchedParticles(ParticlesManager.SmokeParticles, GlobalPosition, 5f, oneshot: true);
			SoundManager.PlaySound(SoundManager.Sounds_Exposion, 6f, 30);
			LootManager.SpawnCoins(3, GlobalPosition);
			QueueFree();
		}
		else
		{
			SoundManager.PlaySound(SoundManager.Sounds_MetalImpact, 6f, 30);
		}
	}
}
