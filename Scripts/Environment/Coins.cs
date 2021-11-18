using Godot;
using System;

public class Coins : AnimatedSprite
{
	[Export]
	public int coinValue = 0;
	
	private int floatTimer = 0;
	private bool particlesAttatched = false;
	
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
			
		float yVelocity = (float)Math.Sin(floatTimer * 3f) * 0.5f;
		MoveLocalY(yVelocity);
	}
	
	private void OnBodyEntered(object body)
	{
		if (body == Player.player)
		{
			Player.playerMoney += coinValue;
			QueueFree();
		}
	}
}
