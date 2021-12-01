using Godot;
using System;

public class HurtArea : Area2D
{
	private void OnBodyEntered(object body)
	{
		if (body == Player.player)
			Player.player.Call("Hurt", 1);
	}
}
