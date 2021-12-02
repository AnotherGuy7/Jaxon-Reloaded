using Godot;
using System;

public class FinalBoss : RigidBody2D
{
	private const int MaxHealth = 25;

	private int health = MaxHealth;
	private bool destroyed = false;
	private int explosionTimer = 0;
	private int soundTimer = 0;
	private Timer fadeOutTimer;

	private HelperMethods.CollisionType collisionType = HelperMethods.CollisionType.Enemies;

	public override void _Ready()
	{
		PlayerUI.BossHealthBarVisibilty(false);
		fadeOutTimer = GetNode<Timer>("FadeOutTimer");
	}

	public override void _Process(float delta)
	{
		if (destroyed)
		{
			explosionTimer++;
			soundTimer += EffectsManager.random.Next(1, 2 + 1);

			if (explosionTimer >= 8)
			{
				explosionTimer = 0;
				Vector2 explosionPosition = GlobalPosition + new Vector2(EffectsManager.random.Next(-52, 52 + 1), EffectsManager.random.Next(-52, 52 + 1));
				int soundType = 0;
				if (soundTimer >= 15)
				{
					soundType = SoundManager.Sounds_Explosion;
					soundTimer = 0;
				}

				ParticlesManager.CreateExplosion(explosionPosition, 0.1f, soundType);
			}

			if (Transitions.fadeInCompleted)
			{
				Transitions.FadeOut();
				ScenesHolder.SwitchScenesTo(ScenesHolder.UI_TitleScreen);
			}
		}
	}

	private void OnFadeOutTimerOut()
	{
		Transitions.FadeIn();
	}

	public void Hurt(int damage)
	{
		health -= damage;
		PlayerUI.UpdateBossHealthScale(health / (float)MaxHealth);
		if (health <= 0)
		{
			destroyed = true;
			fadeOutTimer.Start(5);
			PlayerUI.BossHealthBarVisibilty(true);
		}
	}
}
