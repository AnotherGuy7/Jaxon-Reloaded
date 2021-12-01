using Godot;
using System;

public class LevelButton : TextureButton
{
    [Export]
    public int sceneID;

    public bool buttonPressed = false;

    public override void _Process(float delta)
    {
        if (buttonPressed && Transitions.fadeInCompleted)
        {
            ScenesHolder.SwitchScenesTo(sceneID);
            Transitions.FadeOut();
        }
    }

    public override void _Pressed()
    {
        Transitions.FadeIn();
        buttonPressed = true;
    }
}
