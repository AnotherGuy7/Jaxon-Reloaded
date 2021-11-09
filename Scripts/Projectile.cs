using Godot;
using System;

public class Projectile : Area2D
{
	[Export]
	public int damage;

	[Export]
	public ProjectileManager.CollisionType collisionType;

	public Vector2 velocity;
	public Sprite projectileTexture;

	public override void _Ready()
	{
		projectileTexture = GetNode<Sprite>("ProjectileTexture");
	}

	public override void _PhysicsProcess(float delta)
	{
		MoveLocalX(velocity.x);
		MoveLocalY(velocity.y);
		projectileTexture.Rotation = velocity.Angle();
	}

	private void OnBodyEntered(object body)
	{
		Node2D node = body as Node2D;
		if (body is KinematicBody)
		{
			if (node.HasMethod("Hurt"))
			{
				if ((ProjectileManager.CollisionType)node.Get("collisionType") == collisionType)
				{
					node.Call("Hurt", damage);
					QueueFree();
				}
			}
			return;
		}
		else
		{
			if (node.Get("interactable") != null)
			{
				node.Call("Hurt", damage);
			}

			QueueFree();
			return;
		}
	}

	public override void _ExitTree()
	{
		ParticlesManager.RemoveFromNodes(this);
	}

	private void OnTimeLeftOut()
	{
		QueueFree();
	}
}
