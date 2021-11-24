using Godot;
using System;

public class SoundManager : Node2D
{
	private AudioStreamPlayer randomSound;

	public static SoundManager soundManager;

	[Export]
	public AudioStream[] randomSounds;

	public AudioStreamPlayer[] sounds;

	public const int Sounds_MetalImpact = 0;
	public const int Sounds_Explosion = 1;
	public const int Sounds_BigExplosion = 2;
	public const int Sound_BasicImpact = 3;

	public override void _Ready()
	{
		soundManager = this;

		randomSound = GetNode<AudioStreamPlayer>("RandomSoundPlayer");

		Node soundsContainer = GetNode<Node>("Sounds");
		sounds = new AudioStreamPlayer[soundsContainer.GetChildren().Count];
		for (int i = 0; i < soundsContainer.GetChildren().Count; i++)
			sounds[i] = soundsContainer.GetChild(i) as AudioStreamPlayer;
	}

	public override void _Process(float delta)
	{
		if (!randomSound.Playing && EffectsManager.random.Next(0, 500 + 1) == 0)
		{
			randomSound.Stream = randomSounds[EffectsManager.random.Next(0, randomSounds.Length)];
			randomSound.Play();
		}
	}

	/// <summary>
	/// Plays the specified sound with the specified modifications
	/// </summary>
	/// <param name="soundType">The type of sound to play. Sound types can be accessed using SoundManager.</param>
	/// <param name="pitchMinimum">The minimum pitch the sound will use.</param>
	/// <param name="pitchRandom">The amount of random the pitch can have. The random will be a range of -pitchRandom and random divided by 100.</param>
	public static void PlaySound(int soundType, float pitchMinimum = 1f, int pitchRandom = 0)
	{
		soundManager.sounds[soundType].PitchScale = pitchMinimum;
		if (pitchRandom != 0f)
			soundManager.sounds[soundType].PitchScale += (EffectsManager.random.Next(-pitchRandom, pitchRandom + 1) / 100f);

		soundManager.sounds[soundType].Play();
	}
}
