using Godot;
using System;

public class Projectile : Area2D
{
	[Export]
	public int damage;

	[Export]
	public HelperMethods.CollisionType collisionType;

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
		if (body is PhysicsBody2D)
		{
			if (node.HasMethod("Hurt"))
			{
				if (HelperMethods.CollisionTypeMatch(node, collisionType))
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

			SoundManager.PlaySound(SoundManager.Sound_BasicImpact, 0.8f, 80);
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
