using Godot;
using System;

public class ExplosiveBarrel : Node2D
{
	public int health = 12;
	public Area2D explosionRadius;
	public bool interactable = true;
	
	public override void _Ready()
	{
		explosionRadius = GetNode<Area2D>("ExplosionRadius");
	}
	
	public void Hurt(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			foreach (object body in explosionRadius.GetOverlappingBodies())     //because if you stand on top of a spike and not move while the spike is active, you're counted as not entering
			{
				if (!(body is PhysicsBody2D))
					continue;

				PhysicsBody2D kBody = body as PhysicsBody2D;
				if (kBody.HasMethod("Hurt"))
				{
					kBody.Call("Hurt", damage);
				}
			}

			for (int i = 0; i < 9; i++)
			{
				Random random = new Random();
				Vector2 spawnPosition = GlobalPosition;
				spawnPosition += new Vector2(random.Next(-15, 15 + 1), random.Next(-20, 20 + 1));
				ParticlesManager.SpawnUnattatchedParticles(ParticlesManager.SmokeParticles, spawnPosition, 6);

				int goreTextureIndex = random.Next(0, 2 + 1);
				Vector2 initialVelocity = new Vector2(random.Next(-2, 2), random.Next(-5, -2 + 1));
				GoreManager.SpawnGore(goreTextureIndex, spawnPosition, initialVelocity * 30f);
			}
			QueueFree();
		}
	}
}
