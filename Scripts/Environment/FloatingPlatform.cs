using Godot;
using System;

public class FloatingPlatform : AnimatedSprite
{
	[Export]
	public float moveSpeed = 3.5f;

	[Export]
	public bool stationary = true;

	public Vector2 startPoint;
	public Vector2 endPoint;

	private Vector2 currentPoint;

	public override void _Ready()
	{
		if (!stationary || GetNodeOrNull<Position2D>("StartPoint") != null)
		{
			startPoint = GetNodeOrNull<Position2D>("StartPoint").GlobalPosition;
			endPoint = GetNodeOrNull<Position2D>("EndPoint").GlobalPosition;

			currentPoint = startPoint;
		}
	}

	public override void _Process(float delta)
	{
		if (stationary)
			return;

		float distanceToPoint = (currentPoint - GlobalPosition).Length();
		if (distanceToPoint <= moveSpeed * 1.4f)
		{
			if (currentPoint == startPoint)
				currentPoint = endPoint;
			else
				currentPoint = startPoint;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!stationary)
		{
			Vector2 velocity = currentPoint - GlobalPosition;
			velocity = velocity.Normalized();
			velocity *= moveSpeed;

			MoveLocalX(velocity.x);
			MoveLocalY(velocity.y);
		}
	}
}
