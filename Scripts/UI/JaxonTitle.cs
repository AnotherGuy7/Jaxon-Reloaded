using Godot;
using System;

public class JaxonTitle : TextureRect
{
	private float originalY = 0f;
	private int sinTimer = 0;
	
	public override void _Ready()
	{
		originalY = RectGlobalPosition.y;
	}
	
	public override void _Process(float delta)
	{
		sinTimer++;
		RectGlobalPosition = new Vector2(RectGlobalPosition.x, originalY + (float)(Math.Sin(sinTimer / 12f) * 4f));
	}
}
