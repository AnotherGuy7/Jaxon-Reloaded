using Godot;
using System;

public class FinalBoss : RigidBody2D
{
	private const int MaxHealth = 25;

	private int health = MaxHealth;

	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	public override void _Ready()
	{
		PlayerUI.BossHealthBarVisibilty(false);
	}

	public void Hurt(int damage)
	{
		health -= damage;
		PlayerUI.UpdateBossHealthScale(health / (float)MaxHealth);
		if (health <= 0)
		{
			QueueFree();
			PlayerUI.BossHealthBarVisibilty(true);
		}
	}
}
