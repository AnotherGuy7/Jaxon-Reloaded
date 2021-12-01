using Godot;
using System;

public class HelperMethods : Resource
{
    public enum CollisionType
    {
        Player,
        Enemies
    }

    public const string TravelLimitName = "TravelLimit";

    public static bool CollisionTypeMatch(Node2D node, CollisionType collisionType)
    {
        if (node.Get("collisionType") == null)
            return false;

        bool collisionTypeMatch = (CollisionType)node.Get("collisionType") == collisionType;
        return collisionTypeMatch;
    }
}
