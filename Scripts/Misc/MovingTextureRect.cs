using Godot;
using System;

public class MovingTextureRect : TextureRect
{
	[Export]
	public float moveSpeed;

	public override void _PhysicsProcess(float delta)
	{
		RectGlobalPosition += new Vector2(moveSpeed, 0f);
	}
}
