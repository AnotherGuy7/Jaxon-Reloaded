using Godot;
using System;

public class Transitions : Control
{
	public AnimationPlayer transitionsPlayer;
	public static Transitions transitions;

	public static bool fadeInCompleted = false;
	public static bool fadeOutCompleted = false;

	public override void _Ready()
	{
		transitions = this;
		transitionsPlayer = GetNode<AnimationPlayer>("TransitionsPlayer");
	}

	public static void FadeIn()
	{
		transitions.transitionsPlayer.Play("FadeIn");
		fadeOutCompleted = false;
	}
	public static void FadeOut()
	{
		transitions.transitionsPlayer.Play("FadeOut");
		fadeInCompleted = false;
	}

	private void OnTransitionAnimationFinished(String anim_name)
	{
		string transitionName = anim_name;
		if (transitionName == "FadeIn")
		{
			fadeInCompleted = true;
		}
		if (transitionName == "FadeOut")
		{
			fadeOutCompleted = true;
		}
	}
}
