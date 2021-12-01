using Godot;
using System;

public class Medkit : Area2D
{
	private void OnBodyEntered(object body)
	{
		if (body == Player.player && Player.playerHealth < Player.MaxHealth[Player.healthLevel - 1])
		{
			Player.playerHealth = Player.MaxHealth[Player.healthLevel - 1];
			QueueFree();
		}
	}
}
