using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CircleComponent
{
    public uint id;
    public Vector2 position;
    public Vector2 velocity;
    public int size;
    public bool isDynamic;
    public bool isProtected;
    public bool isCollision;
    public Color color;
}
