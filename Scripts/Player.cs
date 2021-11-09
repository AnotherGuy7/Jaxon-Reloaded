using Godot;
using System;

public class Player : KinematicBody2D
{
	private const float MoveSpeed = 6.5f;
	private const float GravityStrength = 0.9f;
	private const float JumpStrength = 21f;
	private const float FloorGravity = 0.3f;
	private const float FinalForceMultiplier = 15f;
	private readonly Vector2 GravityDefinition = new Vector2(0, -1);

	private Vector2 velocity;
	private float aerialVelocity = 0f;

	private AnimatedSprite playerAnim;
	private Camera2D playerCam;
	private Sprite playerGun;
	private Particles2D jumpParticles;
	private Particles2D shootParticles;

	private float gunRotationOffset;
	private bool doubleJumped = false;
	private int shootTimer = 0;
	private string animationNameExtension = "";
	private FightingStyle fightingStyle;


	private AnimationState animationState;
	private ProjectileManager.CollisionType collisionType = ProjectileManager.CollisionType.Player;

	private enum AnimationState
	{
		Idle,
		Walking,
		Jumping,
		Falling
	}

	private enum FightingStyle
	{
		Ranged,
		Melee
	}

	
	public override void _Ready()
	{
		playerAnim = GetNode<AnimatedSprite>("PlayerAnim");
		playerCam = GetNode<Camera2D>("PlayerCam");
		playerGun = GetNode<Sprite>("PlayerGun");
		jumpParticles = GetNode<Particles2D>("Jump Particles");
		shootParticles = GetNode<Particles2D>("PlayerGun/Shoot Particles");
	}

	public override void _Process(float delta)
	{
		if (shootTimer > 0)
			shootTimer--;

		if (shootTimer <= 0 && Input.IsMouseButtonPressed((int)ButtonList.Right))
		{
			shootTimer += 30;
			if (fightingStyle == FightingStyle.Ranged)
				fightingStyle = FightingStyle.Melee;
			else
				fightingStyle = FightingStyle.Ranged;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		velocity = Vector2.Zero;

		PreUpdateSteps();
		if (InputDefinitions.IsLeftPressed())
		{
			velocity.x = -MoveSpeed;
			playerAnim.FlipH = true;
			SetArmAnchors(true);
		}
		if (InputDefinitions.IsRightPressed())
		{
			velocity.x = MoveSpeed;
			playerAnim.FlipH = false;
			SetArmAnchors();
		}
		if (velocity.x == 0f)
			StandingStill();
		else
			animationState = AnimationState.Walking;

		if (!IsOnFloor())
		{
			aerialVelocity += GravityStrength;
			if (aerialVelocity > -3 && InputDefinitions.IsJumpPressed() && !doubleJumped)
			{
				doubleJumped = true;
				aerialVelocity = -JumpStrength;
				jumpParticles.Emitting = true;
			}
		}
		else
		{
			aerialVelocity = FloorGravity;
			if (InputDefinitions.IsJumpPressed())
			{
				doubleJumped = false;
				jumpParticles.Emitting = false;
				aerialVelocity = -JumpStrength;
			}
		}
		if (aerialVelocity > FloorGravity)
			animationState = AnimationState.Falling;
		else if (aerialVelocity < FloorGravity)
			animationState = AnimationState.Jumping;
		//GD.Print(aerialVelocity + ";" + (aerialVelocity > -JumpStrength + 6f));

		velocity.y = aerialVelocity;
		MoveAndSlide(velocity * FinalForceMultiplier, GravityDefinition);
		PostUpdateSteps();
	}

	private void StandingStill()
	{
		animationState = AnimationState.Idle;
	}

	private void PreUpdateSteps()
	{
		ControlCamera();
	}

	private void ControlCamera()
	{
		if (Input.IsKeyPressed((int)KeyList.Right))
		{
			Vector2 cameraZoom = playerCam.Zoom;
			cameraZoom -= Vector2.One * 0.008f;
			cameraZoom.x = Mathf.Clamp(cameraZoom.x, 0.5f, 1.5f);
			cameraZoom.y = Mathf.Clamp(cameraZoom.y, 0.5f, 1.5f);

			playerCam.Zoom = cameraZoom;
		}
		if (Input.IsKeyPressed((int)KeyList.Left))
		{
			Vector2 cameraZoom = playerCam.Zoom;
			cameraZoom += Vector2.One * 0.008f;
			cameraZoom.x = Mathf.Clamp(cameraZoom.x, 0.5f, 1.5f);
			cameraZoom.y = Mathf.Clamp(cameraZoom.y, 0.5f, 1.5f);

			playerCam.Zoom = cameraZoom;
		}
	}

	private void PostUpdateSteps()
	{
		AnimatePlayer();
		ControlGun();
	}

	private void AnimatePlayer()
	{
		animationNameExtension = "_NA";
		if (fightingStyle == FightingStyle.Melee)
			animationNameExtension = "";

		switch (animationState)		//The animations are listed in order of priority
		{
			case AnimationState.Jumping:
				playerAnim.Play("Jumping" + animationNameExtension);
				break;

			case AnimationState.Falling:
				playerAnim.Play("Falling" + animationNameExtension);
				break;

			case AnimationState.Walking:
				playerAnim.Play("Walking" + animationNameExtension);
				break;

			case AnimationState.Idle:
				playerAnim.Play("Idle" + animationNameExtension);
				break;
		}
	}

	private void ControlGun()
	{
		playerGun.Visible = fightingStyle == FightingStyle.Ranged;
		if (fightingStyle != FightingStyle.Ranged)
			return;

		Vector2 vectorToMouse = GetGlobalMousePosition() - playerGun.GlobalPosition;
		Vector2 normalizedMouseVector = vectorToMouse.Normalized();
		float angleToMouse = (float)Math.Atan2(normalizedMouseVector.y, normalizedMouseVector.x);
		playerGun.Rotation = angleToMouse + gunRotationOffset;
		shootParticles.Emitting = false;

		if (shootTimer <= 0 && Input.IsMouseButtonPressed((int)ButtonList.Left))
		{
			shootTimer += 15;
			Vector2 shootPosition = GlobalPosition + playerGun.Position + (normalizedMouseVector * 36f);
			Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_PlayerBullet, 6, shootPosition, normalizedMouseVector * 8f, ProjectileManager.CollisionType.Enemies);
			ParticlesManager.AttachParticles(projectile, ParticlesManager.BulletParticles, 3);

			shootParticles.Position = shootPosition;
			shootParticles.Rotation = playerGun.Rotation;
			shootParticles.Emitting = true;
		}
	}

	private void SetArmAnchors(bool left = false)
	{
		if (left)
		{
			playerGun.FlipH = true;
			playerGun.Position = new Vector2(-2.5f, -7f);
			playerGun.Offset = new Vector2(-30f, -5f);
			gunRotationOffset = (float)Math.PI;
		}
		else
		{
			playerGun.FlipH = false;
			playerGun.Position = new Vector2(2.5f, -7f);
			playerGun.Offset = new Vector2(0f, -5f);
			gunRotationOffset = 0f;
		}
	}

	private void Hurt(int damage)
	{

	}
}
