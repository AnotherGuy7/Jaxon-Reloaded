using Godot;
using System;

public class EffectsManager : Node2D
{
	private int cameraShakeIntensity = 0;
	private bool cameraShaking = false;

	private Timer cameraShakeTimer;

	public static Random random;
	public static EffectsManager effectsManager;
	public static Node2D environmentNode;
	public static bool environmentNodeLoaded = false;

	public override void _Ready()
	{
		effectsManager = this;

		random = new Random();
		cameraShakeTimer = GetNode<Timer>("CameraShakeTimer");

		AttemptSetEnvironmentNode();
	}
	
	public static void AttemptSetEnvironmentNode()
    {
		SceneTree sceneTree = effectsManager.GetTree();
		if (IsInstanceValid(sceneTree.CurrentScene.GetNodeOrNull<Node2D>("EnvironmentNode")))
		{
			environmentNodeLoaded = true;
			environmentNode = effectsManager.GetTree().CurrentScene.GetNode<Node2D>("EnvironmentNode");
		}
	}

	/// <summary>
	/// Shakes the camera with the intensity and duration input in the method parameters.
	/// </summary>
	/// <param name="cameraShakeIntensity">The maximum offset the camera can move by</param>
	/// <param name="cameraShakeDuration">The duration (in frames) that the camrea will move for</param>
	public static void ShakeCamera(int cameraShakeIntensity, float cameraShakeDuration)
	{
		effectsManager.cameraShaking = true;
		effectsManager.cameraShakeIntensity = cameraShakeIntensity;
		effectsManager.cameraShakeTimer.Start(cameraShakeDuration / 60f);
	}

	public override void _Process(float delta)
	{
		if (cameraShaking)
		{
			if (Player.playerCam == null)
				return;

			Player.playerCam.Position = Vector2.Zero;

			int cameraShakeX = random.Next(-cameraShakeIntensity, cameraShakeIntensity + 1);
			int camreaShakeY = random.Next(-cameraShakeIntensity, cameraShakeIntensity + 1);
			Player.playerCam.Position += new Vector2(cameraShakeX, camreaShakeY);
		}
	}

	private void OnCameraShakeTimerOut()
	{
		cameraShaking = false;
		cameraShakeIntensity = 0;
		Player.playerCam.Position = Vector2.Zero;
	}
}
