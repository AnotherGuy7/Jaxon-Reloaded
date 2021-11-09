using Godot;
using System;

public class Gore : RigidBody2D
{
	public Sprite goreTexture;
	public Timer goreTimer;
	public int goreTextureIndex = 0;

	public override void _Ready()
	{
		goreTimer = GetNode<Timer>("TimeLeft");
		goreTexture = GetNode<Sprite>("GoreTexture");
		goreTexture.Texture = GoreManager.goreManager.textures[goreTextureIndex];
	}

	public override void _Process(float delta)
	{
		if (goreTimer.TimeLeft < 2f)
		{
			Color modulateColor = Modulate;
			modulateColor.a = Mathf.Clamp(goreTimer.TimeLeft / 2f, 0f, 1f);
			Modulate = modulateColor;
		}
	}

	private void OnTimeOut()
	{
		QueueFree();
	}
}
