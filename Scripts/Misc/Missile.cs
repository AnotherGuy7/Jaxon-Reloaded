using Godot;
using System;

public class Missile : Area2D
{
	private bool missileActive = false;
	private bool setVelocity = false;

	private Vector2 velocity;
	private AnimatedSprite missileAnim;
	private int immunityTimer = 20;

	public override void _Ready()
	{
		missileAnim = GetNode<AnimatedSprite>("MissileAnim");
	}

    public override void _Process(float delta)
    {
		if (missileActive && immunityTimer > 0)
			immunityTimer--;
	}

    public override void _PhysicsProcess(float delta)
	{
		if (missileActive && !setVelocity)
		{
			setVelocity = true;
			velocity = Player.player.Position - GlobalPosition;
			velocity = velocity.Normalized();
			velocity *= 7f;
			missileAnim.GlobalRotationDegrees = Mathf.Rad2Deg(velocity.Angle()) + 90f;
		}

		MoveLocalX(velocity.x);
		MoveLocalY(velocity.y);
	}

	private void OnMissileBodyEntered(object body)
	{
		if (!missileActive || immunityTimer > 0)
			return;

		if (body == Player.player)
		{
			Player.player.Call("Hurt", 1);
			SoundManager.PlaySound(SoundManager.Sounds_Explosion, 0.6f);
			QueueFree();
		}
		else if (!(body is RobotTank))
		{
			SoundManager.PlaySound(SoundManager.Sounds_Explosion, 0.6f);
			QueueFree();
			EffectsManager.ShakeCamera(2, 8);
		}
	}


	private void OnMissileAreaEntered(object area)
	{
		if (area is RobotTank || !missileActive || immunityTimer > 0)
			return;

		if (area is Projectile)
		{
			Projectile projectile = area as Projectile;
			if (projectile.collisionType == HelperMethods.CollisionType.Enemies)
			{
				projectile.QueueFree();
				QueueFree();
				SoundManager.PlaySound(SoundManager.Sounds_Explosion, 0.6f);
				EffectsManager.ShakeCamera(2, 8);
			}
		}
		else if (area is StaticBody2D)
		{
			SoundManager.PlaySound(SoundManager.Sounds_Explosion, 0.6f);
			QueueFree();
			EffectsManager.ShakeCamera(2, 8);
		}
	}

	private void OnIdleTimerOut()
	{
		missileActive = true;
	}
}
