using Godot;
using System;

public class HelperMethods : Resource
{
    public enum CollisionType
    {
        Player,
        Enemies
    }

    public static bool CollisionTypeMatch(Node2D node, CollisionType collisionType)
    {
        bool collisionTypeMatch = (CollisionType)node.Get("collisionType") == collisionType;
        return collisionTypeMatch;
    }
}
