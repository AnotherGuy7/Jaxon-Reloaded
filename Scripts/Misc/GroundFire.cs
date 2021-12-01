using Godot;
using System;

public class GroundFire : Area2D
{
	public HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Player;

	private void OnBodyEntered(object body)
	{
		if (body == Player.player)
		{
			Node2D node = body as Node2D;
			node.Call("Hurt", 1);
		}
	}

	private void OnTimeLeftOut()
	{
		QueueFree();
	}
}
