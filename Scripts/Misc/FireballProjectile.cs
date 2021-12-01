using Godot;
using System;

public class FireballProjectile : Area2D
{
	[Export]
	public int damage;

	[Export]
	public PackedScene smallerFireball;


	public Vector2 velocity;
	public Sprite fireballTexture;
	public HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Player;
	public bool spawnMoreOnDeath = true;
	public bool queueSmallerBallSpawn = false;
	public int amountOfTimesToReflect = -1;

	private int amountOfReflections = 0;

	public override void _Ready()
	{
		fireballTexture = GetNode<Sprite>("FireballSprite");
	}

	public override void _Process(float delta)
	{
		if (queueSmallerBallSpawn)
		{
			CallDeferred(nameof(SpawnFireballs));
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		MoveLocalX(velocity.x);
		MoveLocalY(velocity.y);
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
		}
		else
		{
			if (amountOfReflections <= amountOfTimesToReflect)
				amountOfReflections++;
			else
			{
				if (spawnMoreOnDeath)
				{
					queueSmallerBallSpawn = true;
					fireballTexture.Visible = false;
				}
				else
					QueueFree();
			}
			SoundManager.PlaySound(SoundManager.Sound_BasicImpact, 0.4f);
		}
	}

	private void SpawnFireballs()
	{
		int amountOfFireballs = 4;
		for (int i = 0; i < amountOfFireballs; i++)
		{
			float angle = (i + 1) * (360f / (float)amountOfFireballs);
			Node2D fireball = (Node2D)smallerFireball.Instance();
			fireball.GlobalPosition = GlobalPosition;
			Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
			velocity = velocity.Normalized() * 6f;
			fireball.Set("velocity", velocity);
			fireball.Set("spawnMoreOnDeath", false);
			fireball.Set("amountOfTimesToReflect", amountOfTimesToReflect);
			fireball.Scale = Vector2.One * 0.5f;

			EffectsManager.environmentNode.AddChild(fireball);
		}
		QueueFree();
	}

	private void OnLeftSideEntered(object body)
	{
		if (!(body is PhysicsBody2D))
			velocity.x *= -1f;
	}


	private void OnTopSideEntered(object body)
	{
		if (!(body is PhysicsBody2D))
			velocity.y *= -1f;
	}



	private void OnRightSideEntered(object body)
	{
		if (!(body is PhysicsBody2D))
			velocity.x *= -1f;
	}


	private void OnBottomSideEntered(object body)
	{
		if (!(body is PhysicsBody2D))
			velocity.y *= -1f;
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
