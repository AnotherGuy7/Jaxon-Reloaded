using Godot;
using System;

public class Player : KinematicBody2D
{
	private const float MoveSpeed = 6.5f;
	private const float GravityStrength = 0.9f;
	private const float JumpStrength = 21f;
	private const float FloorGravity = 0.3f;
	private const float FinalForceMultiplier = 15f;
	private const float SwordDamage = 12;
	public const int MaxHealth = 7;
	private readonly Vector2 GravityDefinition = new Vector2(0, -1);

	private Vector2 velocity;
	private float aerialVelocity = 0f;

	private AnimatedSprite playerAnim;
	private Sprite playerGun;
	private Area2D swordArea;
	private AudioStreamPlayer stepSound;
	private AudioStreamPlayer shootSound;
	private AudioStreamPlayer hurtSound;
	public static Camera2D playerCam;

	private float gunRotationOffset;
	private bool doubleJumped = false;
	private int shootTimer = 0;
	private string animationNameExtension = "";
	private FightingStyle fightingStyle;
	private Direction direction;
	private bool playerSwinging = false;
	private int lastSwingFrame = 0;
	private int immunityTimer = 0;
	private int flashTimer = 0;

	[Export]
	public AudioStream[] stepSounds;

	public static int playerHealth = MaxHealth;
	public static Player player;
	public static Vector2 position;
	public static int playerMoney;

	private AnimationState animationState;
	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Player;

	private enum AnimationState
	{
		Idle,
		Walking,
		Jumping,
		Falling,
		Swinging
	}

	private enum FightingStyle
	{
		Ranged,
		Melee
	}

	private enum Direction
	{
		Left,
		Right
	}


	public override void _Ready()
	{
		player = this;

		playerAnim = GetNode<AnimatedSprite>("PlayerAnim");
		playerCam = GetNode<Camera2D>("PlayerCam");
		playerGun = GetNode<Sprite>("PlayerGun");
		swordArea = GetNode<Area2D>("SwordArea");
		stepSound = GetNode<AudioStreamPlayer>("StepSound");
		shootSound = GetNode<AudioStreamPlayer>("PistolShootSound");
		hurtSound = GetNode<AudioStreamPlayer>("HurtSound");
	}

	public override void _Process(float delta)
	{
		if (shootTimer > 0)
			shootTimer--;

		if (shootTimer <= 0 && Input.IsMouseButtonPressed((int)ButtonList.Right))
		{
			shootTimer += 10;
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
			direction = Direction.Left;
			SetArmAnchors();
		}
		if (InputDefinitions.IsRightPressed())
		{
			velocity.x = MoveSpeed;
			playerAnim.FlipH = false;
			direction = Direction.Right;
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

				Vector2 particlePos = GlobalPosition;
				ParticlesManager.SpawnUnattatchedParticles(ParticlesManager.JumpParticles, particlePos, 2);
			}
		}
		else
		{
			aerialVelocity = FloorGravity;
			if (InputDefinitions.IsJumpPressed())
			{
				doubleJumped = false;
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

		if (immunityTimer > 0)
		{
			immunityTimer--;
			flashTimer++;
			if (flashTimer >= 3)
				flashTimer = -3;

			Modulate = new Color(1f, 1f, 1f, Math.Abs(flashTimer) / 3f);
			if (immunityTimer <= 0)
				Modulate = new Color(1f, 1f, 1f, 1f);
		}
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
		ControlGun();
		ControlMelee();
		AnimatePlayer();
		position = GlobalPosition;
	}

	private void AnimatePlayer()
	{
		animationNameExtension = "_NA";
		if (fightingStyle == FightingStyle.Melee)
			animationNameExtension = "";

		playerAnim.Offset = Vector2.Zero;
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

			case AnimationState.Swinging:
				playerAnim.Play("SwordSwing");

				int swordAnimOffset= 17;
				if (direction == Direction.Left)
					swordAnimOffset *= -1;

				playerAnim.Offset = new Vector2(swordAnimOffset, 0f);
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
		if (direction == Direction.Right)
			angleToMouse = Mathf.Clamp(angleToMouse, (float)-Math.PI / 2f, (float)Math.PI / 2f);
		else
		{
			//angleToMouse = Mathf.Clamp(angleToMouse, (float)Math.PI / 2f, (float)(Math.PI + (Math.PI / 2f)));
			if (angleToMouse <= 0 && angleToMouse > -Math.PI / 2f)
				angleToMouse = (float)-Math.PI / 2f;
			if (angleToMouse >= 0 && angleToMouse < Math.PI / 2f)
				angleToMouse = (float)Math.PI / 2f;
		}

		playerGun.Rotation = angleToMouse + gunRotationOffset;

		if (shootTimer <= 0 && Input.IsMouseButtonPressed((int)ButtonList.Left))
		{
			shootTimer += 15;
			shootSound.Play();
			Vector2 shootPosition = GlobalPosition + playerGun.Position + (normalizedMouseVector * 36f);
			Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_PlayerBullet, 6, shootPosition, normalizedMouseVector * 8f, HelperMethods.CollisionType.Enemies);
			ParticlesManager.AttachParticles(projectile, ParticlesManager.BulletParticles, 3);

			ParticlesManager.SpawnUnattatchedParticles(ParticlesManager.ShootParticles, shootPosition, 5, oneshot: true);
		}
	}

	private void ControlMelee()
	{
		if (fightingStyle != FightingStyle.Melee)
			return;

		if (shootTimer <= 0 && Input.IsMouseButtonPressed((int)ButtonList.Left) && !playerSwinging)
		{
			shootTimer += 20;

			playerSwinging = true;
			foreach (object body in swordArea.GetOverlappingBodies())     //because if you stand on top of a spike and not move while the spike is active, you're counted as not entering
			{
				if (!(body is PhysicsBody2D))
					continue;

				PhysicsBody2D kBody = body as PhysicsBody2D;
				if (kBody.HasMethod("Hurt") && HelperMethods.CollisionTypeMatch(kBody, HelperMethods.CollisionType.Enemies))
				{
					kBody.Call("Hurt", SwordDamage);
				}
			}
		}

		if (playerSwinging)
		{
			animationState = AnimationState.Swinging;
		}
	}

	private void SetArmAnchors()
	{
		if (direction == Direction.Left)
		{
			playerGun.FlipH = true;
			playerGun.Position = new Vector2(-2.5f, -7f);
			playerGun.Offset = new Vector2(-30f, -5f);
			gunRotationOffset = (float)Math.PI;
			swordArea.Scale = new Vector2(-1, 1f);
		}
		else
		{
			playerGun.FlipH = false;
			playerGun.Position = new Vector2(2.5f, -7f);
			playerGun.Offset = new Vector2(0f, -5f);
			gunRotationOffset = 0f;
			swordArea.Scale = new Vector2(1, 1f);
		}
	}

	private void OnAnimationFinished()
	{
		if (animationState == AnimationState.Swinging)
		{
			playerSwinging = false;
			animationState = AnimationState.Idle;
		}
	}

	private void OnAnimFrameChanged()
	{
		if (animationState == AnimationState.Walking && playerAnim.Frame == 1 || playerAnim.Frame == 3)
		{
			stepSound.Stream = stepSounds[EffectsManager.random.Next(0, stepSounds.Length)];
			stepSound.Play();
		}
	}

	private void Hurt(int damage)
	{
		if (immunityTimer > 0)
			return;

		playerHealth -= 1;
		immunityTimer = 35;
		hurtSound.Play();
		EffectsManager.ShakeCamera(3, 10);
	}
}
