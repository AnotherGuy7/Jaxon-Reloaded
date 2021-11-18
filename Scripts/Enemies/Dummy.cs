using Godot;
using System;

public class Dummy : KinematicBody2D
{
	[Export]
	public Texture normalFrame;

	[Export]
	public Texture hitFrame;

	private Sprite dummySprite;
	private AudioStreamPlayer2D screamSound;
	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	private int hurtTimer = 0;

	public override void _Ready()
	{
		dummySprite = GetNode<Sprite>("DummySprite");
		screamSound = GetNode<AudioStreamPlayer2D>("ScreamSound");
	}

	public override void _Process(float delta)
	{
		if (hurtTimer > 0)
			hurtTimer--;

		if (hurtTimer <= 0)
			dummySprite.Texture = normalFrame;
		else
			dummySprite.Texture = hitFrame;

	}

	public void Hurt(int damage)
	{
		hurtTimer += 8;

		screamSound.PitchScale = 1f + (EffectsManager.random.Next(-3, 3 + 1) / 15f);
		screamSound.Play();
	}
}
