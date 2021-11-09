using Godot;
using System;

public class InputDefinitions : Resource
{
    public const string Left = "Move_Left";
    public const string Right = "Move_Right";
    public const string Jump = "Jump";

    public static bool IsLeftPressed()
    {
        return Input.IsActionPressed(Left);
    }

    public static bool IsRightPressed()
    {
        return Input.IsActionPressed(Right);
    }

    public static bool IsJumpPressed()
    {
        return Input.IsActionPressed(Jump);
    }
}
