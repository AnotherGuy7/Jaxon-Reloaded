using Godot;
using System;

public class SecuritySensor : Sprite
{
	private Area2D detectionArea;
	private Position2D targetPoint;
	private Vector2 shootVelocity;
	private bool shootLaser = false;

	public override void _Ready()
	{
		targetPoint = GetNode<Position2D>("TargetPoint");

		detectionArea = GetNode<Area2D>("DetectionArea");
		int laserLength = (int)Math.Abs(targetPoint.GlobalPosition.x - GlobalPosition.x);
		detectionArea.Scale = new Vector2(1f, laserLength);

		shootVelocity = targetPoint.GlobalPosition - GlobalPosition;
		shootVelocity = shootVelocity.Normalized() * 8f;
	}

    public override void _Process(float delta)
    {
        if (shootLaser)
        {
			shootLaser = false;
			CallDeferred(nameof(ShootLaser));
		}
    }

    private void OnBodyEntered(object body)
	{
		if (body == Player.player)
			shootLaser = true;		//CallDeferred has to be called in a method that runs continuously.
	}

	private void ShootLaser()
    {
		Vector2 shootPosition = GlobalPosition + new Vector2(0f, -2f);

		Node2D projectile = ProjectileManager.NewProjectile(ProjectileManager.Projectile_EnemyLaser, 1, shootPosition, shootVelocity, HelperMethods.CollisionType.Player);
		ParticlesManager.AttachParticles(projectile, ParticlesManager.LaserParticles, 3);
	}
}
