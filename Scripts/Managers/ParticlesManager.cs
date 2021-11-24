using Godot;
using System;
using System.Collections.Generic;

public class ParticlesManager : Node2D
{
	[Export]
	public PackedScene emptyParticleEmmitter;

	public Particles2D[] particleNodes;
	public static ParticlesManager particlesManager;
	public static CanvasLayer particleLayer;
	public static List<Particles2D> activeUnattatchedParticles = new List<Particles2D>();

	public static List<Particles2D> activeAttatchedParticles = new List<Particles2D>();
	public static Dictionary<string, Node2D> attatchedNodes = new Dictionary<string, Node2D>();
	public static Dictionary<string, bool> nodeActive = new Dictionary<string, bool>();

	public const int AmountOfParticles = 6;
	public const int BulletParticles = 0;
	public const int LaserParticles = 1;
	public const int SmokeParticles = 2;
	public const int FireSmokeParticles = 3;
	public const int ShootParticles = 4;
	public const int JumpParticles = 5;

	public bool stopParticleUpdate = false;

	public override void _Ready()
	{
		particlesManager = this;

		particleNodes = new Particles2D[AmountOfParticles];
		particleNodes[BulletParticles] = GetNode<Particles2D>("BulletParticles");
		particleNodes[LaserParticles] = GetNode<Particles2D>("LaserParticles");
		particleNodes[SmokeParticles] = GetNode<Particles2D>("SmokeParticles");
		particleNodes[FireSmokeParticles] = GetNode<Particles2D>("FireSmokeParticles");
		particleNodes[ShootParticles] = GetNode<Particles2D>("ShootParticles");
		particleNodes[JumpParticles] = GetNode<Particles2D>("JumpParticles");

		particleLayer = GetNode<CanvasLayer>("ParticleLayer");
	}

	public override void _Process(float delta)
	{
		if (!stopParticleUpdate)
			CallDeferred(nameof(UpdateParticles));

		stopParticleUpdate = false;
	}

	private void UpdateParticles()
	{

		int amountOfAttatchedParticlesRemoved = 0;
		Particles2D[] activeAttatchedParticlesClone = activeAttatchedParticles.ToArray();
		for (int i = 0; i < activeAttatchedParticlesClone.Length; i++)
		{
			if (stopParticleUpdate)
				return;

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
				activeAttatchedParticles.RemoveAt(i - amountOfAttatchedParticlesRemoved);
				nodeActive.Remove(particle.Name);
				attatchedNodes.Remove(particle.Name);
				amountOfAttatchedParticlesRemoved += 1;
				continue;
			}

			if (/*!attatchedNodes.ContainsValue(attatchedNodes[particle.Name]) || attatchedNodes[particle.Name].GetIndex() < 0 || */!nodeActive[particle.Name])
			{
				particle.Emitting = false;
				continue;
			}

			bool targetNodeExists = IsInstanceValid(ProjectileManager.projectileManager.projectileNode.GetNodeOrNull<Node2D>(attatchedNodes[particle.Name].Name));
			if (!targetNodeExists)
			{
				particle.Emitting = false;
				continue;
			}

			Node2D targetNode = ProjectileManager.projectileManager.projectileNode.GetNodeOrNull<Node2D>(attatchedNodes[particle.Name].Name);
			particle.GlobalRotation = targetNode.GlobalRotation;
			particle.GlobalPosition = targetNode.GlobalPosition;
		}

		int unattatchedParticlesDeleted = 0;
		Particles2D[] activeUnattatchedParticlesClone = activeUnattatchedParticles.ToArray();
		for (int i = 0; i < activeUnattatchedParticlesClone.Length; i++)
		{
			if (stopParticleUpdate)
				return;

			Particles2D particle = activeUnattatchedParticlesClone[i];
			Timer timer = particle.GetNode<Timer>("ParticleTimer");

			if (timer.TimeLeft <= 2f)
			{
				particle.Emitting = false;
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
					activeUnattatchedParticles.RemoveAt(i - unattatchedParticlesDeleted);
					unattatchedParticlesDeleted += 1;
				}
			}
		}
	}

	private void ClearParticlesNode()
	{
		foreach (Node2D node in particleLayer.GetChildren())
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
					nodeActive[key] = false;
					particlesManager.stopParticleUpdate = true;
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

		particleLayer.AddChild(particles);
		activeAttatchedParticles.Add(particles);
		attatchedNodes.Add(particles.Name, node);
		nodeActive.Add(particles.Name, true);

		Timer timer = new Timer();
		timer.Name = "ParticleTimer";
		particles.AddChild(timer);
		timer.OneShot = true;
		timer.Start(particleTime);
	}

	public static void AttachParticles(Node2D node, int particleType, Color particleColor, float particleTime)
	{
		Particles2D particles = (Particles2D)particlesManager.emptyParticleEmmitter.Instance();
		particles.Amount = particlesManager.particleNodes[particleType].Amount;
		particles.Lifetime = particlesManager.particleNodes[particleType].Lifetime;
		particles.OneShot = particlesManager.particleNodes[particleType].OneShot;
		particles.ProcessMaterial = particlesManager.particleNodes[particleType].ProcessMaterial;
		particles.LocalCoords = particlesManager.particleNodes[particleType].LocalCoords;
		particles.Modulate = particleColor;
		particles.Emitting = true;

		particleLayer.AddChild(particles);
		activeAttatchedParticles.Add(particles);
		attatchedNodes.Add(particles.Name, node);
		nodeActive.Add(particles.Name, true);

		Timer timer = new Timer();
		timer.Name = "ParticleTimer";
		particles.AddChild(timer);
		timer.OneShot = true;
		timer.Start(particleTime);
	}

	public static void SpawnUnattatchedParticles(int particleType, Vector2 position, float particleTime, float scale = 1f, bool oneshot = false)
	{
		Particles2D particles = (Particles2D)particlesManager.emptyParticleEmmitter.Instance();
		particles.Amount = particlesManager.particleNodes[particleType].Amount;
		particles.Lifetime = particlesManager.particleNodes[particleType].Lifetime;
		particles.OneShot = particlesManager.particleNodes[particleType].OneShot;
		particles.ProcessMaterial = particlesManager.particleNodes[particleType].ProcessMaterial;
		particles.GlobalPosition = position;
		particles.Scale = new Vector2(scale, scale);
		particles.OneShot = oneshot;
		particles.Emitting = true;


		particleLayer.AddChild(particles);
		activeUnattatchedParticles.Add(particles);

		Timer timer = new Timer();
		timer.Name = "ParticleTimer";
		particles.AddChild(timer);
		timer.OneShot = true;
		timer.Start(particleTime);
	}

	public static void SpawnUnattatchedParticles(int particleType, Vector2 position, Color particleColor, float particleTime, float scale = 1f, bool oneshot = false)
	{
		Particles2D particles = (Particles2D)particlesManager.emptyParticleEmmitter.Instance();
		particles.Amount = particlesManager.particleNodes[particleType].Amount;
		particles.Lifetime = particlesManager.particleNodes[particleType].Lifetime;
		particles.OneShot = particlesManager.particleNodes[particleType].OneShot;
		particles.ProcessMaterial = particlesManager.particleNodes[particleType].ProcessMaterial;
		particles.GlobalPosition = position;
		particles.Scale = new Vector2(scale, scale);
		particles.OneShot = oneshot;
		particles.Modulate = particleColor;
		particles.Emitting = true;


		particleLayer.AddChild(particles);
		activeUnattatchedParticles.Add(particles);

		Timer timer = new Timer();
		timer.Name = "ParticleTimer";
		particles.AddChild(timer);
		timer.OneShot = true;
		timer.Start(particleTime);
	}
}
