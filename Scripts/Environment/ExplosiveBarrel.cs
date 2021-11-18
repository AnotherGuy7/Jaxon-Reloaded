using Godot;
using System;

public class ExplosiveBarrel : Node2D
{
	private const int StartingHealth = 8;
	private const int ExplosionDamage = 10;

	public int health = StartingHealth;
	public Area2D explosionRadius;
	public bool interactable = true;
	private int explosionTimer = 0;
	
	public override void _Ready()
	{
		explosionRadius = GetNode<Area2D>("ExplosionRadius");
	}

	public override void _Process(float delta)
	{
		if (health <= 0)
		{
			explosionTimer--;
			if (explosionTimer <= 0)
			{
				foreach (Node2D body in explosionRadius.GetOverlappingBodies())
					if (body.HasMethod("Hurt"))
						body.Call("Hurt", ExplosionDamage);

				for (int i = 0; i < 9; i++)
				{
					Random random = new Random();
					Vector2 spawnPosition = GlobalPosition;
					spawnPosition += new Vector2(random.Next(-15, 15 + 1), 7.25f);
					ParticlesManager.SpawnUnattatchedParticles(ParticlesManager.FireSmokeParticles, spawnPosition, 6, 3);

					int goreTextureIndex = random.Next(0, 2 + 1);
					Vector2 initialVelocity = new Vector2(random.Next(-2, 2), random.Next(-5, -2 + 1));
					GoreManager.SpawnGore(goreTextureIndex, spawnPosition, initialVelocity * 30f);
				}
				EffectsManager.ShakeCamera(3, 25);
				SoundManager.PlaySound(SoundManager.Sounds_Exposion, pitchRandom: 30);
				QueueFree();
			}
		}
	}

	public void Hurt(int damage)
	{
		health -= damage;
		if (health <= 0)
			explosionTimer += EffectsManager.random.Next(0, 4 + 1);

		SoundManager.PlaySound(SoundManager.Sounds_MetalImpact, pitchRandom: 30);
	}
}
