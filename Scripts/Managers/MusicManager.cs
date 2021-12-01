using Godot;
using System;

public class MusicManager : Node2D
{
	[Export]
	public AudioStream[] music;

	public AudioStreamPlayer musicPlayer;
	public static int MusicVolume = 80;
	public static MusicManager musicManager;

	public const int Music_Title = 0;
	public const int Music_City = 1;
	public const int Music_CityBoss = 2;
	public const int Music_MilitaryBase = 3;
	public const int Music_MilitaryBaseBoss = 4;
	public const int Music_Lab = 5;
	public const int Music_LabBoss = 6;


	public override void _Ready()
	{
		musicManager = this;
		musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
	}

	public static void PlayMusic(int musicIndex)
	{
		musicManager.musicPlayer.Stream = musicManager.music[musicIndex];
		musicManager.musicPlayer.Play();
	}

	public static void ChangeMusicVolume(float percentage)
	{
		float newVolume = -60f + (60f * percentage);
		musicManager.musicPlayer.VolumeDb = newVolume;
	}
}
