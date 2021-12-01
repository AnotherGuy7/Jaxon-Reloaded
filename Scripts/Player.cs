using Godot;
using System;

public class Player : KinematicBody2D
{
	private const float DefaultMoveSpeed = 6.5f;
	private const float GravityStrength = 0.9f;
	private const float JumpStrength = 21f;
	private const float FloorGravity = 0.3f;
	private const float FinalForceMultiplier = 15f;
	private const float SwordDamage = 12;
	private const int BlasterConeSpread = 7;		//Spread in degrees
	private const int BoomerConeSpread = 18;
	public static readonly int[] MaxHealth = new int[4] { 3, 5, 7, 9 };
	private readonly Vector2 GravityDefinition = new Vector2(0, -1);

	private Vector2 velocity;
	private float aerialVelocity = 0f;

	private AnimatedSprite playerAnim;
	private Sprite playerGun;
	private Area2D swordArea;
	private RayCast2D doomCannonRay;
	private Sprite doomCannonLaser1;
	private Sprite doomCannonLaser2;
	private AudioStreamPlayer stepSound;
	private AudioStreamPlayer shootSound;
	private AudioStreamPlayer hurtSound1;
	private AudioStreamPlayer hurtSound2;
	public static Camera2D playerCam;

	private float gunRotationOffset;
	private bool doubleJumped = false;
	private int shootTimer = 0;
	private string animationNameExtension = "";
	private FightingStyle fightingStyle;
	private Direction direction;
	private bool playerSwinging = false;
	private int immunityTimer = 0;
	private int flashTimer = 0;
	private bool escapePressed = false;

	[Export]
	public AudioStream[] stepSounds;

	[Export]
	public Texture[] armTextures;

	[Export]
	public AudioStream[] shootSounds;
	private float[] shootSoundPitches = new float[AmountOfGuns] { 1.8f, 1.2f, 1f, 1f, 1f };
	private float[] shootSoundVolumes = new float[AmountOfGuns] { 0f, -5f, -3f, -9f, 0f };

	public static int playerHealth;
	public static Player player;
	public static Vector2 position;
	public static int playerMoney = 0;
	public static bool[] gunUnlocked = new bool[AmountOfGuns] { true, false, false, false, false };
	public static int activeGun = Gun_Phaser;
	public static int healthLevel = 1;
	public static int strengthLevel = 1;
	public static int speedLevel = 1;
	public static Vector2 maxZoom;
	public static Vector2 cameraTopLeftClampPos;
	public static Vector2 cameraBottomRightClampPos;
	public static bool playerDead = false;

	public const int AmountOfGuns = 5;
	public const int Gun_Phaser = 0;
	public const int Gun_Blaster = 1;
	public const int Gun_Boomer = 2;
	public const int Gun_PhaseRifle = 3;
	public const int Gun_DoomCannon = 4;

	private readonly Color yellowBulletColor = new Color(0.95f, 0.9f, 0.59f);
	private readonly Color orangeBulletColor = new Color(1f, 0.62f, 0.18f);
	private readonly Color blueBulletColor = new Color(0.18f, 0.53f, 1f);
	private readonly Color greenBulletColor = new Color(0.18f, 1f, 0.24f);

	private AnimationState animationState;
	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Player;

	private enum AnimationState
	{
		Idle,
		Walking,
		Jumping,
		Falling,
		Swinging,
		Dead
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
		doomCannonRay = GetNode<RayCast2D>("PlayerGun/DoomCannonCast");
		doomCannonLaser1 = GetNode<Sprite>("PlayerGun/DoomCannonLaser1");
		doomCannonLaser2 = GetNode<Sprite>("PlayerGun/DoomCannonLaser2");
		stepSound = GetNode<AudioStreamPlayer>("StepSound");
		shootSound = GetNode<AudioStreamPlayer>("ShootSound");
		hurtSound1 = GetNode<AudioStreamPlayer>("HurtSound_1");
		hurtSound2 = GetNode<AudioStreamPlayer>("HurtSound_2");

		playerGun.Texture = armTextures[activeGun];
		shootSound.Stream = shootSounds[activeGun];
		shootSound.PitchScale = shootSoundPitches[activeGun];
		shootSound.VolumeDb = shootSoundVolumes[activeGun];
		playerHealth = MaxHealth[healthLevel - 1];
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

		if (Input.IsKeyPressed((int)KeyList.Escape))
        {
			escapePressed = true;
			Transitions.FadeIn();
        }
		if (escapePressed && Transitions.fadeInCompleted)
        {
			Transitions.FadeOut();
			ScenesHolder.SwitchScenesTo(ScenesHolder.UI_TitleScreen);
        }
	}

	public override void _PhysicsProcess(float delta)
	{
		velocity = Vector2.Zero;
		float moveSpeed = DefaultMoveSpeed + (0.8f * (speedLevel - 1));

		PreUpdateSteps();
		if (InputDefinitions.IsLeftPressed())
		{
			velocity.x = -moveSpeed;
			playerAnim.FlipH = true;
			direction = Direction.Left;
			SetArmAnchors();
		}
		if (InputDefinitions.IsRightPressed())
		{
			velocity.x = moveSpeed;
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

		if (playerDead)
			velocity.x = 0f;

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

		if (Transitions.fadeInCompleted && playerDead)
		{
			playerDead = false;
			playerHealth = MaxHealth[healthLevel - 1];
			playerMoney -= 50;
			if (playerMoney < 0)
				playerMoney = 0;
			ScenesHolder.SwitchScenesTo(ScenesHolder.UI_TitleScreen);
			Transitions.FadeOut();

		}
	}

	private void ControlCamera()
	{
		if (Input.IsKeyPressed((int)KeyList.Right))
		{
			Vector2 cameraZoom = playerCam.Zoom;
			cameraZoom -= Vector2.One * 0.008f;
			cameraZoom.x = Mathf.Clamp(cameraZoom.x, 0.5f, maxZoom.x);
			cameraZoom.y = Mathf.Clamp(cameraZoom.y, 0.5f, maxZoom.y);

			playerCam.Zoom = cameraZoom;
		}
		if (Input.IsKeyPressed((int)KeyList.Left))
		{
			Vector2 cameraZoom = playerCam.Zoom;
			cameraZoom += Vector2.One * 0.008f;
			cameraZoom.x = Mathf.Clamp(cameraZoom.x, 0.25f, maxZoom.x);
			cameraZoom.y = Mathf.Clamp(cameraZoom.y, 0.25f, maxZoom.y);

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

		if (playerDead)
			animationState = AnimationState.Dead;

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

			case AnimationState.Dead:
				playerAnim.Play("Dying");
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
		doomCannonLaser1.Visible = false;
		doomCannonLaser2.Visible = false;

		if (shootTimer <= 0 && Input.IsMouseButtonPressed((int)ButtonList.Left))
		{
			shootSound.Play();
			shootSound.PitchScale = shootSoundPitches[activeGun] + (EffectsManager.random.Next(-2, 2 + 1) / 10f);
			Vector2 angleVector = new Vector2((float)Math.Cos(angleToMouse), (float)Math.Sin(angleToMouse));
			Vector2 shootPosition = GlobalPosition + playerGun.Position + (angleVector * 36f);
			Vector2 shootVector = angleVector * 8f; 
			switch (activeGun)
			{
				case Gun_Phaser:
					shootTimer += 25;
					Node2D phaserProjectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_PlayerBullet_Yellow, 3, shootPosition, shootVector, HelperMethods.CollisionType.Enemies);
					ParticlesManager.AttachParticles(phaserProjectile, ParticlesManager.BulletParticles, yellowBulletColor, 3);
					break;

				case Gun_Blaster:
					shootTimer += 8;
					float blasterShootVectorX = (float)Math.Cos(angleVector.Angle() + Mathf.Deg2Rad(EffectsManager.random.Next(-BlasterConeSpread, BlasterConeSpread + 1)));
					float blasterShootVectorY = (float)Math.Sin(angleVector.Angle() + Mathf.Deg2Rad(EffectsManager.random.Next(-BlasterConeSpread, BlasterConeSpread + 1)));
					shootVector = new Vector2(blasterShootVectorX, blasterShootVectorY) * 8f;

					Node2D blasterProjectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_PlayerBullet_Orange, 5, shootPosition, shootVector, HelperMethods.CollisionType.Enemies);
					ParticlesManager.AttachParticles(blasterProjectile, ParticlesManager.BulletParticles, orangeBulletColor, 3);
					break;

				case Gun_Boomer:
					shootTimer += 40;
					int amountOfBoomerBullets = 5;
					for (int i = 0; i < amountOfBoomerBullets; i++)
					{
						float angleSpread = (((i - 2) / 2.5f) * BoomerConeSpread);
						float shootVectorX = (float)Math.Cos(angleVector.Angle() + Mathf.Deg2Rad(angleSpread));
						float shootVectorY = (float)Math.Sin(angleVector.Angle() + Mathf.Deg2Rad(angleSpread));
						shootVector = new Vector2(shootVectorX, shootVectorY).Normalized() * 8f;

						Node2D boomerProjectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_PlayerBullet_Blue, 13, shootPosition, shootVector, HelperMethods.CollisionType.Enemies);
						ParticlesManager.AttachParticles(boomerProjectile, ParticlesManager.BulletParticles, blueBulletColor, 3);
					}
					break;

				case Gun_PhaseRifle:
					shootTimer += 18;
					Node2D rifleProjectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_PlayerBullet_Green, 7, shootPosition, shootVector, HelperMethods.CollisionType.Enemies);
					ParticlesManager.AttachParticles(rifleProjectile, ParticlesManager.BulletParticles, greenBulletColor, 3);
					break;

				case Gun_DoomCannon:
					/*doomCannonRay.Enabled = true;
					if (doomCannonRay.IsColliding())
					{
						Node2D collider = doomCannonRay.GetCollider() as Node2D;
						float scale = (GlobalPosition - collider.GlobalPosition).Length();

						doomCannonLaser1.Visible = true;
						doomCannonLaser2.Visible = true;
						doomCannonLaser1.Scale = new Vector2(scale, EffectsManager.random.Next(6, 8 + 1));
						doomCannonLaser2.Scale = new Vector2(scale, EffectsManager.random.Next(4, 6 + 1));

						if (collider is PhysicsBody2D)
						{
							if (collider.HasMethod("Hurt"))
							{
								if (HelperMethods.CollisionTypeMatch(collider, collisionType))
								{
									collider.Call("Hurt", 1);
									QueueFree();
								}
							}
							return;
						}
					}*/

					shootTimer += 80;
					Node2D cannonProjectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_DoomCannonLaser, 18, shootPosition, shootVector, HelperMethods.CollisionType.Enemies);
					ParticlesManager.AttachParticles(cannonProjectile, ParticlesManager.LaserParticles, new Color(1f, 0f, 0f, 1f), 3);
					break;

		}
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
					kBody.Call("Hurt", SwordDamage + (5 * (strengthLevel - 1)));
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
			playerGun.Offset = new Vector2(-39f, -9f);
			gunRotationOffset = (float)Math.PI;
			swordArea.Scale = new Vector2(-1, 1f);
		}
		else
		{
			playerGun.FlipH = false;
			playerGun.Position = new Vector2(2.5f, -7f);
			playerGun.Offset = new Vector2(0f, -9f);
			gunRotationOffset = 0f;
			swordArea.Scale = new Vector2(1, 1f);
		}
	}

	public static void SetCameraLimits(int top, int bottom, int left, int right)
	{
		/*float maxZoomX = Math.Abs((left - right) / 1024f);
		float maxZoomY = Math.Abs((top - bottom) / 600f);
		if (maxZoomY < 600)
			maxZoomY = 600;
		maxZoom = new Vector2(maxZoomX, maxZoomY);
		GD.Print(maxZoom);

		Vector2 cameraZoom = playerCam.Zoom;
		cameraZoom.x = Mathf.Clamp(cameraZoom.x, 0.5f, maxZoom.x);
		cameraZoom.y = Mathf.Clamp(cameraZoom.y, 0.5f, maxZoom.y);

		playerCam.Zoom = cameraZoom;*/

		playerCam.LimitTop = top;
		playerCam.LimitBottom = bottom;
		playerCam.LimitLeft = left;
		playerCam.LimitRight = right;
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
		EffectsManager.ShakeCamera(3, 10);

		if (EffectsManager.random.Next(0, 1 + 1) == 0)
			hurtSound1.Play();
		else
			hurtSound2.Play();

		if (playerHealth <= 0)
		{
			playerDead = true;
			Transitions.FadeIn();
		}
	}
}
