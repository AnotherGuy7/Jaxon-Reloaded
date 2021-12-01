using Godot;
using System;

public class RobotTank : RigidBody2D
{
	private const int MaxHealth = 450;
	private const float MoveSpeed = 2.5f;
	private const float RamSpeed = 6f;

	private bool playerDetected;
	private int health = MaxHealth;
	private int areaScanTimer = 0;
	private int direction = -1;
	private int missileShootTimer = 0;
	private int laserShootTimer = 0;

	[Export]
	public PackedScene missileScene;

	private AnimatedSprite robotTankAnim;
	private Timer attackTimer;
	private Timer attackRestTimer;
	private Position2D missilePosition;
	private Sprite barrelSprite;
	private Position2D shootPosition;
	private Area2D ramHitbox;
	private AudioStreamPlayer missileShootSound;
	private AudioStreamPlayer laserShootSound;

	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	private AnimationState animationState;
	private Attack attackType;

	private enum AnimationState
	{
		Idle,
	}

	private enum Attack
	{
		None,
		Ram,
		Missile,
		Lasers
	}

	public override void _Ready()
	{
		robotTankAnim = GetNode<AnimatedSprite>("RobotTankAnim");
		attackTimer = GetNode<Timer>("AttackTimer");
		attackRestTimer = GetNode<Timer>("AttackRestTimer");
		missilePosition = GetNode<Position2D>("MissilePosition");
		barrelSprite = GetNode<Sprite>("BarrelSprite");
		shootPosition = GetNode<Position2D>("BarrelSprite/ShootPosition");
		ramHitbox = GetNode<Area2D>("RamHitbox");
		missileShootSound = GetNode<AudioStreamPlayer>("MissileShootSound");
		laserShootSound = GetNode<AudioStreamPlayer>("LaserShootSound");
		PlayerUI.BossHealthBarVisibilty(false);
		FlipDirection(-1);
	}

	public override void _Process(float delta)
	{
		if (areaScanTimer > 0)
			areaScanTimer--;

		if (attackType == Attack.Missile)
		{
			missileShootTimer++;
			if (missileShootTimer >= 2 * 60)
			{
				Node2D missile = (Node2D)missileScene.Instance();
				missile.GlobalPosition = missilePosition.GlobalPosition;
				missile.Set("velocity", Vector2.Up * 3f);
				//ParticlesManager.AttachParticles(missile, ParticlesManager.FireSmokeParticles, 3);
				ProjectileManager.projectileManager.projectileNode.AddChild(missile);
				missileShootTimer = 0;
				missileShootSound.Play();
			}
		}
		if (attackType == Attack.Lasers)
		{
			laserShootTimer++;
			Vector2 vectorToPlayer = Player.position - GlobalPosition;
			if (laserShootTimer >= 60)
			{
				Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_EnemyLaser, 1, shootPosition.GlobalPosition, vectorToPlayer.Normalized() * 8f, HelperMethods.CollisionType.Player);
				ParticlesManager.AttachParticles(projectile, ParticlesManager.LaserParticles, 3);
				laserShootSound.Play();
				laserShootTimer = 0;
			}
			barrelSprite.GlobalRotation = vectorToPlayer.Angle();
		}
		else
		{
			if (direction == 1)
				barrelSprite.GlobalRotation = 0f;
			else
				barrelSprite.GlobalRotation = (float)Math.PI;
		}
		//GD.Print(attackRestTimer.TimeLeft);
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (attackType == Attack.Ram)
		{
			velocity.x = direction * RamSpeed;
		}

		MoveLocalX(velocity.x * delta * 14f);
		AnimateRobotTank();
	}

	private void AnimateRobotTank()
	{
		if (attackType == Attack.Ram)
			robotTankAnim.SpeedScale = 2.5f;
		else
			robotTankAnim.SpeedScale = 1f;

		switch (animationState)
		{
			case AnimationState.Idle:
				robotTankAnim.Play("Idle");
				break;
		}
	}

	private void OnAttackTimerOut()
	{
		attackType = Attack.None;
		animationState = AnimationState.Idle;
		attackRestTimer.Start(EffectsManager.random.Next(1, 4 + 1));
	}

	private void OnAttackRestTimerOut()
	{
		attackType = (Attack)EffectsManager.random.Next(1, 3 + 1);
		if (attackType == Attack.Ram)
		{
			attackTimer.Start(8);
		}
		else if (attackType == Attack.Missile)
		{
			attackTimer.Start(7);
			missileShootTimer = 0;
		}
		else if (attackType == Attack.Lasers)
		{
			attackTimer.Start(9);
		}
	}

	private void FlipDirection(int side)
	{
		direction = side;
		if (side == 1)
		{
			robotTankAnim.FlipH = false;
			ramHitbox.Position = new Vector2(72f, 12f);
			missilePosition.Position = new Vector2(-60, -22);
		}
		else
		{
			robotTankAnim.FlipH = true;
			ramHitbox.Position = new Vector2(-72f, 12f);
			missilePosition.Position = new Vector2(60, -22);
		}
	}

	public void Hurt(int damage)
	{
		health -= damage;
		PlayerUI.UpdateBossHealthScale(health / (float)MaxHealth);
		if (health <= 0)
		{
			LootManager.SpawnCoins(50, GlobalPosition);
			QueueFree();
			PlayerUI.BossHealthBarVisibilty(true);
		}
	}

	private void OnBotHitboxAreaEntered(object area)
	{
		//	:l
	}

	private void OnRamHitboxEntered(object area)
	{
		Area2D areaNode = area as Area2D;
		if (areaNode.Name.Contains(HelperMethods.TravelLimitName))
		{
			attackType = Attack.None;
			FlipDirection(-direction);
		}
	}

	private void OnRamHitboxBodyEntered(object body)
	{
		if (attackType == Attack.Ram && body == Player.player)
		{
			Player.player.Call("Hurt", 1);
		}
	}
}
