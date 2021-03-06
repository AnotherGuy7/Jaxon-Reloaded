using Godot;
using System;

public class CityGuard : RigidBody2D
{
	private Timer walkTimer;
	private Timer idleTimer;
	private AnimatedSprite cityGuardAnim;
	private Position2D shootPosition;

	private const float MoveSpeed = 18.1f;

	private bool alerted = false;
	private int direction = 1;
	private AnimationStates animationState;
	private int shootTimer = 0;
	private int health = 20;
	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	private enum AnimationStates
	{
		Idle,
		Walking,
		Shooting
	}

	public override void _Ready()
	{
		walkTimer = GetNode<Timer>("WalkTimer");
		idleTimer = GetNode<Timer>("IdleTimer");
		cityGuardAnim = GetNode<AnimatedSprite>("CityGuardAnim");
		shootPosition = GetNode<Position2D>("ShootPosition");
		animationState = AnimationStates.Idle;
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (shootTimer > 0)
			shootTimer--;

		if (walkTimer.TimeLeft > 0)
		{
			animationState = AnimationStates.Walking;
			velocity.x = MoveSpeed * direction * delta;
		}
		else
			animationState = AnimationStates.Idle;

		if (alerted)
		{
			velocity = Vector2.Zero;
			animationState = AnimationStates.Shooting;
			if (Player.position.x - GlobalPosition.x > 0)
			{
				direction = 1;
				cityGuardAnim.FlipH = false;
			}
			else
			{
				direction = -1;
				cityGuardAnim.FlipH = true;
			}

			if (shootTimer <= 0)
			{
				shootTimer += 140;
				Vector2 shootVelocity = Player.position - GlobalPosition;
				shootVelocity.y = 0f;
				Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_EnemyLaser, 1, shootPosition.GlobalPosition, shootVelocity.Normalized() * 8f, HelperMethods.CollisionType.Player);
				ParticlesManager.AttachParticles(projectile, ParticlesManager.LaserParticles, 3);
			}
		}

		MoveLocalX(velocity.x);
		AnimateBot();
	}

	private void AnimateBot()
	{
		switch (animationState)
		{
			case AnimationStates.Idle:
				cityGuardAnim.Play("Idle");
				break;

			case AnimationStates.Walking:
				cityGuardAnim.Play("Walking");
				break;

			case AnimationStates.Shooting:
				cityGuardAnim.Play("Idle");
				break;
		}
	}

	private void SetRandomWalkValues(int specificDirection = 0, int specificTime = 0)
	{
		int walkTime = EffectsManager.random.Next(2, 6 + 1);
		if (specificTime != 0)
			walkTime = specificTime;

		walkTimer.Start(walkTime);
		direction = EffectsManager.random.Next(0, 1 + 1);
		if (direction == 0)
			direction = -1;

		if (specificDirection != 0)
			direction = specificDirection;

		if (direction == 1)
			cityGuardAnim.FlipH = false;
		else
			cityGuardAnim.FlipH = true;

	}

	private void SetIdleTime()
	{
		int idleTime = EffectsManager.random.Next(1, 4 + 1);

		idleTimer.Start(idleTime);
	}

	private void OnWalkTimerOut()
	{
		SetIdleTime();
	}


	private void OnIdleTimerOut()
	{
		SetRandomWalkValues();
	}

	private void OnLimitDetectorAreaEntered(object area)
	{
		Area2D areaNode = area as Area2D;
		if (areaNode.Name.Contains(HelperMethods.TravelLimitName))
		{
			SetRandomWalkValues(-direction, 1);
		}
	}

	private void OnDetectionAreaEntered(object body)
	{
		if (body == Player.player)
			alerted = true;
	}

	private void OnDetectionAreaExited(object body)
	{
		if (body == Player.player)
			alerted = false;
	}

	private void Hurt(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			bool headSpawned = false;
			for (int i = 0; i < EffectsManager.random.Next(1, 2 + 1); i++)
			{
				int goreIndex = 0;
				if (!headSpawned)
					goreIndex = EffectsManager.random.Next(GoreManager.Gore_CityGuard1, GoreManager.Gore_CityGuard3 + 1);
				else
					goreIndex = EffectsManager.random.Next(GoreManager.Gore_CityGuard1, GoreManager.Gore_CityGuard2 + 1);

				Vector2 goreVelocity = new Vector2(EffectsManager.random.Next(-36, 36 + 1), -EffectsManager.random.Next(45, 65 + 1) * 2.1f);
				GoreManager.SpawnGore(goreIndex, GlobalPosition, goreVelocity);
			}
			for (int i = 0; i < EffectsManager.random.Next(3, 5 + 1); i++)
			{
				Vector2 explosionPosition = GlobalPosition + new Vector2(EffectsManager.random.Next(-18, 18 + 1), EffectsManager.random.Next(-18, 18 + 1));
				ParticlesManager.CreateExplosion(explosionPosition, 0.5f);
				if (EffectsManager.random.Next(0, 1 + 1) == 0)
					SoundManager.PlaySound(EffectsManager.random.Next(SoundManager.Sounds_Explosion, SoundManager.Sounds_BigExplosion + 1));
			}

			EffectsManager.ShakeCamera(3, 5);
			SoundManager.PlaySound(SoundManager.Sounds_BigExplosion);
			LootManager.SpawnCoins(3, GlobalPosition);
			QueueFree();
		}
	}
}
