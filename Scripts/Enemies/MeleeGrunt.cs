using Godot;
using System;

public class MeleeGrunt : RigidBody2D
{
	public Timer walkTimer;
	public Timer idleTimer;
	public AnimatedSprite gruntAnim;
	public Area2D slashArea;

	private const float MoveSpeed = 30f;

	private int stunTimer = 0;
	private bool playerDetected = false;
	private bool attacking = false;
	private AnimationState animationState;
	private int direction = 1;
	private int health = 30;
	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	public enum AnimationState
	{
		Idle,
		Walking,
		Slashing
	}

	
	public override void _Ready()
	{
		walkTimer = GetNode<Timer>("WalkTimer");
		idleTimer = GetNode<Timer>("IdleTimer");
		gruntAnim = GetNode<AnimatedSprite>("MeleeGruntAnim");
		slashArea = GetNode<Area2D>("SlashHitbox");
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;
		if (stunTimer > 0)
			stunTimer--;

		if (!attacking)
		{
			if (playerDetected)
			{
				velocity = Player.position - GlobalPosition;
				velocity = velocity.Normalized();
				velocity *= MoveSpeed;
				velocity.y = 0f;
			}
			else
			{
				if (walkTimer.TimeLeft > 0)
				{
					animationState = AnimationState.Walking;
					velocity.x = MoveSpeed * direction;
				}
				else
					animationState = AnimationState.Idle;
			}

			if (velocity != Vector2.Zero)
				animationState = AnimationState.Walking;
			else
				animationState = AnimationState.Idle;
		}
		else
		{
			animationState = AnimationState.Slashing;
			velocity = Vector2.Zero;
		}

		if (velocity.x > 0f)
			FlipDirection(1);
		else if (velocity.x < 0f)
			FlipDirection(-1);

		if (stunTimer > 0)
		{
			animationState = AnimationState.Idle;
			velocity = Vector2.Zero;
		}


		MoveLocalX(velocity.x * delta);
		AnimateMeleeGrunt();
	}

	private void AnimateMeleeGrunt()
	{
		switch (animationState)
		{
			case AnimationState.Idle:
				gruntAnim.Play("Idle");
				break;

			case AnimationState.Walking:
				gruntAnim.Play("Walking");
				break;

			case AnimationState.Slashing:
				gruntAnim.Play("Slash");
				break;
		}
	}

	private void FlipDirection(int side)
	{
		if (side == 1)
		{
			gruntAnim.FlipH = false;
			gruntAnim.Offset = new Vector2(8f, 0f);
			slashArea.Position = new Vector2(22f, 4f);
		}
		else
		{
			gruntAnim.FlipH = true;
			gruntAnim.Offset = new Vector2(-8f, 0f);
			slashArea.Position = new Vector2(-22f, 4f);
		}
	}

	private void OnSlashHitboxEntered(object body)
	{
		if (body == Player.player)
		{
			attacking = true;
		}
	}


	private void OnSlashHitboxExited(object body)
	{
		if (body == Player.player)
		{
			attacking = false;
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
		{
			playerDetected = false;
			walkTimer.Stop();
			idleTimer.Stop();
			SetIdleTime();
		}
	}

	private void OnGruntAnimFrameChanged()
	{
		if (animationState == AnimationState.Slashing && gruntAnim.Frame == 2)
		{
			foreach (object body in slashArea.GetOverlappingBodies())
			{
				if (body == Player.player)
					Player.player.Call("Hurt", 1);
			}
		}
	}

	private void OnLimitDetectorAreaEntered(object area)
	{
		Area2D areaNode = area as Area2D;
		if (areaNode.Name.Contains(HelperMethods.TravelLimitName))
		{
			attacking = false;
			playerDetected = false;
			walkTimer.Stop();
			idleTimer.Stop();
			SetRandomWalkValues(-direction, 1);
		}
	}

	private void OnWalkTimerOut()
	{
		SetIdleTime();
	}


	private void OnIdleTimerOut()
	{
		SetRandomWalkValues();
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

		FlipDirection(direction);
	}

	private void SetIdleTime()
	{
		if (!IsInstanceValid(idleTimer))
			return;

		int idleTime = EffectsManager.random.Next(1, 4 + 1);

		idleTimer.Start(idleTime);
	}

	private void Hurt(int damage)
	{
		health -= damage;
		stunTimer += 10;
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

				Vector2 goreVelocity = new Vector2(EffectsManager.random.Next(-6, 6 + 1), -EffectsManager.random.Next(45, 65 + 1) * 2.1f);
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
			LootManager.SpawnCoins(2, GlobalPosition);
			QueueFree();
		}
	}
}
