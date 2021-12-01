using Godot;
using System;

public class WelderBot : RigidBody2D
{
	private const int MaxHealth = 250;
	private const float MoveSpeed = 3.2f;

	private bool playerDetected;
	private int health = MaxHealth;
	private int areaScanTimer = 0;
	private int direction = 1;

	private AnimatedSprite welderBotAnim;
	private Area2D flamethrowerArea;
	private Particles2D flamethrowerParticles;
	private Timer attackTimer;
	private Timer attackRestTimer;
	private AudioStreamPlayer flamethrowerSound;
	private Area2D fireballArea;

	[Export]
	public PackedScene flamethrowerFireball;

	[Export]
	public PackedScene groundFireScene;

	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	private AnimationState animationState;
	private Attack attackType;

	private enum AnimationState
	{
		Idle,
		Walking,
		Flamethrower
	}

	private enum Attack
	{
		None,
		Flamethrower,
		Fireball,
		BurningGround,
		FireballBarrage
	}

	public override void _Ready()
	{
		welderBotAnim = GetNode<AnimatedSprite>("WelderBotAnim");
		flamethrowerArea = GetNode<Area2D>("FlamethrowerArea");
		flamethrowerParticles = GetNode<Particles2D>("FlamethrowerArea/FlamethrowerParticles");
		attackTimer = GetNode<Timer>("AttackTimer");
		attackRestTimer = GetNode<Timer>("AttackRestTimer");
		flamethrowerSound = GetNode<AudioStreamPlayer>("FlamethrowerSound");
		PlayerUI.BossHealthBarVisibilty(false);
	}

	public override void _Process(float delta)
	{
		if (areaScanTimer > 0)
			areaScanTimer--;

		if (attackType == Attack.Flamethrower)
		{
			if (areaScanTimer <= 0)
			{
				areaScanTimer += 7;
				foreach (object body in flamethrowerArea.GetOverlappingBodies())
				{
					if (body == Player.player)
					{
						Node2D playerNode = body as Node2D;
						playerNode.Call("Hurt", 1);
					}
				}
			}
		}

		if (DefaultMapScript.mapReference != null && fireballArea == null)
		{
			fireballArea = DefaultMapScript.mapReference.GetNodeOrNull<Area2D>("FireballArea");     //mapReference isn't quite ready yet in Ready()
		}
		//GD.Print(attackRestTimer.TimeLeft);
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (attackType == Attack.None)
		{
			if (Player.position.x > GlobalPosition.x)
			{
				velocity.x = MoveSpeed;
				FlipDirection(1);
			}
			else
			{
				velocity.x = -MoveSpeed;
				FlipDirection(-1);
			}
			animationState = AnimationState.Walking;
		}
		else
		{
			if (attackType == Attack.Flamethrower || attackType == Attack.BurningGround)
			{
				animationState = AnimationState.Flamethrower;
			}
		}

		MoveLocalX(velocity.x * delta * 14f);
		AnimateWelderBot();
	}

	private void AnimateWelderBot()
	{
		switch (animationState)
		{
			case AnimationState.Idle:
				welderBotAnim.Play("Idle");
				break;

			case AnimationState.Walking:
				welderBotAnim.Play("Walking");
				break;

			case AnimationState.Flamethrower:
				welderBotAnim.Play("Flamethrower");
				break;

			default:
				welderBotAnim.Play("Idle");
				break;
		}
	}

	private void OnWelderBodyEntered(object body)
	{
		if (body == Player.player)
		{
			Node2D playerNode = body as Node2D;
			playerNode.Call("Hurt", 1);
		}
	}

	private void OnAttackTimerOut()
	{
		if (attackType == Attack.FireballBarrage)
		{
			attackType = Attack.None;
			attackRestTimer.Start(2);
			return;
		}

		attackType = Attack.None;
		animationState = AnimationState.Idle;
		attackRestTimer.Start(EffectsManager.random.Next(3, 9 + 1));
		if (flamethrowerSound.Playing)
		{
			flamethrowerParticles.Emitting = false;
			flamethrowerSound.Stop();
		}
	}

	private void OnAttackRestTimerOut()
	{
		bool playerInArea = false;
		foreach (object body in flamethrowerArea.GetOverlappingBodies())
		{
			if (body == Player.player)
			{
				playerInArea = true;
				break;
			}
		}

		if (playerInArea)
		{
			Node2D fireball = (Node2D)flamethrowerFireball.Instance();
			fireball.GlobalPosition = GlobalPosition;
			Vector2 vectorToPlayer = Player.position - GlobalPosition + new Vector2(EffectsManager.random.Next(-10, 10 + 1), EffectsManager.random.Next(-10, 10 + 1));
			vectorToPlayer = vectorToPlayer.Normalized() * 5f;
			fireball.Set("velocity", vectorToPlayer);
			fireball.Set("amountOfTimesToReflect", 4);

			EffectsManager.environmentNode.AddChild(fireball);

			attackTimer.Start(1);
			animationState = AnimationState.Idle;
			attackType = Attack.FireballBarrage;
			return;
		}

		attackType = (Attack)EffectsManager.random.Next(1, 3 + 1);
		if (attackType == Attack.Flamethrower)
		{
			attackTimer.Start(6);
			flamethrowerSound.Play();
			flamethrowerParticles.Emitting = true;
		}
		else if (attackType == Attack.Fireball)
		{
			Node2D fireball = (Node2D)flamethrowerFireball.Instance();
			fireball.GlobalPosition = GlobalPosition;
			Vector2 vectorToPlayer = Player.position - GlobalPosition;
			vectorToPlayer = vectorToPlayer.Normalized() * 5f;
			fireball.Set("velocity", vectorToPlayer);

			EffectsManager.environmentNode.AddChild(fireball);

			attackTimer.Start(2);
			animationState = AnimationState.Idle;
		}
		else if (attackType == Attack.BurningGround)
		{
			for (int i = 0; i < 60; i++)
			{
				Node2D groundFire = (Node2D)groundFireScene.Instance();
				groundFire.GlobalPosition = GlobalPosition + new Vector2(14 * (i + 1) * direction, 40f) - new Vector2(14 * 30, 0f);
				EffectsManager.environmentNode.AddChild(groundFire);
			}
			attackTimer.Start(2);
			flamethrowerSound.Play();
			flamethrowerParticles.Emitting = true;
		}
	}

	private void FlipDirection(int side)
	{
		if (side == 1)
		{
			welderBotAnim.FlipH = false;
			welderBotAnim.Offset = new Vector2(50f, 0f);
			flamethrowerArea.Position = new Vector2(67f, 0f);
		}
		else
		{
			welderBotAnim.FlipH = true;
			welderBotAnim.Offset = new Vector2(-50f, 0f);
			flamethrowerArea.Position = new Vector2(-67f, 0f);
		}
		direction = side;
	}

	public void Hurt(int damage)
	{
		health -= damage;
		PlayerUI.UpdateBossHealthScale(health / (float)MaxHealth);
		if (health <= 0)
		{
			LootManager.SpawnCoins(30, GlobalPosition);
			QueueFree();
			PlayerUI.BossHealthBarVisibilty(true);
		}
	}
}
