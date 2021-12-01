using Godot;
using System;

public class Coins : AnimatedSprite
{
	[Export]
	public int coinValue = 0;

	[Export]
	public Color uiColor;
	
	private int floatTimer = 0;
	private bool particlesAttatched = false;
	private bool falling = false;

	private AudioStreamPlayer coinPickupSound;

	public override void _Ready()
	{
		coinPickupSound = GetNode<AudioStreamPlayer>("CoinPickupSound");
	}

	public override void _PhysicsProcess(float delta)
	{
		floatTimer++;
		if (floatTimer >= 360)
			floatTimer = 0;
			
		if (IsInstanceValid(this) && !particlesAttatched)
		{
			particlesAttatched = true;
			//ParticlesManager.AttachParticles(this, ParticlesManager.LaserParticles, 5 * 60);
		}
			
		float yVelocity = (float)Math.Sin(Mathf.Deg2Rad(floatTimer) * 2) * 0.08f;
		if (falling)
			yVelocity = 1.5f;
		MoveLocalY(yVelocity);
	}
	
	private void OnBodyEntered(object body)
	{
		if (body == Player.player)
		{
			coinPickupSound.Play();
			Player.playerMoney += coinValue;
			PlayerUI.SetMoneyCounterModulate(uiColor);
			QueueFree();
		}
		if (body is StaticBody2D)
			falling = false;
	}
}
