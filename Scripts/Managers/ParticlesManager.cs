using Godot;
using System;
using System.Collections.Generic;

public class ParticlesManager : Node2D
{
	[Export]
	public PackedScene emptyParticleEmmitter;

	public Particles2D[] particleNodes;
	public static ParticlesManager particlesManager;
	public static List<Particles2D> activeUnattatchedParticles = new List<Particles2D>();

	public static List<Particles2D> activeAttatchedParticles = new List<Particles2D>();
	public static Dictionary<string, Node2D> attatchedNodes = new Dictionary<string, Node2D>();

	public const int AmountOfParticles = 2;
	public const int BulletParticles = 0;
	public const int SmokeParticles = 1;

	public override void _Ready()
	{
		particlesManager = this;

		particleNodes = new Particles2D[AmountOfParticles];
		particleNodes[BulletParticles] = GetNode<Particles2D>("BulletParticles");
		particleNodes[SmokeParticles] = GetNode<Particles2D>("SmokeParticles");
	}

	public override void _Process(float delta)
	{
		CallDeferred(nameof(UpdateParticles));
	}

	private void UpdateParticles()
    {
		Particles2D[] activeAttatchedParticlesClone = activeAttatchedParticles.ToArray();
		Particles2D[] activeUnattatchedParticlesClone = activeUnattatchedParticles.ToArray();

		for (int i = 0; i < activeAttatchedParticlesClone.Length; i++)
		{
			Particles2D particle = activeAttatchedParticlesClone[i];

			Timer timer = particle.GetNode<Timer>("ParticleTimer");
			if (timer.TimeLeft <= 0)
			{
				/*if (i >= activeAttatchedParticles.Count)
				{
					activeAttatchedParticles.Clear();
					attatchedNodes.Clear();
					particle.QueueFree();
					ClearParticlesNode();
					GD.Print("Attatched Particles Clear");
					continue;
				}*/

				particle.QueueFree();
				activeAttatchedParticles.RemoveAt(i);
				attatchedNodes.Remove(particle.Name);
				continue;
			}

			//if (!attatchedNodes.ContainsKey(particle.Name))
				//continue;

			bool targetNodeExists = IsInstanceValid(ProjectileManager.projectileManager.projectileNode.GetNodeOrNull<Node2D>(attatchedNodes[particle.Name].Name));
			if (!targetNodeExists)
			{
				particle.Emitting = false;
				continue;
			}

			if (particle == null)
				continue;

			Node2D targetNode = ProjectileManager.projectileManager.projectileNode.GetNodeOrNull<Node2D>(attatchedNodes[particle.Name].Name);
			particle.GlobalRotation = targetNode.GlobalRotation;
			particle.GlobalPosition = targetNode.GlobalPosition;
		}

		for (int i = 0; i < activeUnattatchedParticlesClone.Length; i++)
		{
			Particles2D particle = activeUnattatchedParticlesClone[i];
			Timer timer = particle.GetNode<Timer>("ParticleTimer");

			if (timer.TimeLeft <= 0)
			{
				/*if (i >= activeUnattatchedParticles.Count)
				{
					activeUnattatchedParticles.Clear();
					particle.QueueFree();
					ClearParticlesNode();
					GD.Print("Unattatched Particles Clear");
					continue;
				}*/

				particle.QueueFree();
				if (i < activeUnattatchedParticles.Count)
					activeUnattatchedParticles.RemoveAt(i);
			}
		}
	}

	private void ClearParticlesNode()
    {
		foreach (Node2D node in particlesManager.GetChildren())
		{
			if (node.GetIndex() > AmountOfParticles)
				node.QueueFree();
		}
	}

	public static void RemoveFromNodes(Node2D targetNode)
    {
		if (attatchedNodes.ContainsValue(targetNode))
        {
			foreach (string key in attatchedNodes.Keys)
            {
				if (attatchedNodes[key] == targetNode)
                {
					attatchedNodes.Remove(key);
					return;
                }
            }
        }
    }

	/*public static void AttachParticles(Node2D node, int particleType, Vector2 position, float rotation)
	{
		Particles2D particles = (Particles2D)particlesManager.emptyParticleEmmitter.Instance();
		particles.Amount = particlesManager.particleNodes[particleType].Amount;
		particles.ProcessMaterial = particlesManager.particleNodes[particleType].ProcessMaterial;
		particlesManager.AddChild(particles);
		activeParticles.Add(particles);
		attatchedNodes.Add();
	}*/

	public static void AttachParticles(Node2D node, int particleType, float particleTime)
	{
		Particles2D particles = (Particles2D)particlesManager.emptyParticleEmmitter.Instance();
		particles.Amount = particlesManager.particleNodes[particleType].Amount;
		particles.Lifetime = particlesManager.particleNodes[particleType].Lifetime;
		particles.OneShot = particlesManager.particleNodes[particleType].OneShot;
		particles.ProcessMaterial = particlesManager.particleNodes[particleType].ProcessMaterial;
		particles.LocalCoords = particlesManager.particleNodes[particleType].LocalCoords;
		particles.Emitting = true;

		particlesManager.AddChild(particles);
		activeAttatchedParticles.Add(particles);
		attatchedNodes.Add(particles.Name, node);

		Timer timer = new Timer();
		timer.Name = "ParticleTimer";
		particles.AddChild(timer);
		timer.OneShot = true;
		timer.Start(particleTime);
	}

	public static void SpawnUnattatchedParticles(int particleType, Vector2 position, float particleTime)
	{
		Particles2D particles = (Particles2D)particlesManager.emptyParticleEmmitter.Instance();
		particles.Amount = particlesManager.particleNodes[particleType].Amount;
		particles.Lifetime = particlesManager.particleNodes[particleType].Lifetime;
		particles.OneShot = particlesManager.particleNodes[particleType].OneShot;
		particles.ProcessMaterial = particlesManager.particleNodes[particleType].ProcessMaterial;
		particles.LocalCoords = particlesManager.particleNodes[particleType].LocalCoords;
		particles.GlobalPosition = position;
		particles.Emitting = true;


		particlesManager.AddChild(particles);
		activeUnattatchedParticles.Add(particles);

		Timer timer = new Timer();
		timer.Name = "ParticleTimer";
		particles.AddChild(timer);
		timer.OneShot = true;
		timer.Start(particleTime);
	}
}
