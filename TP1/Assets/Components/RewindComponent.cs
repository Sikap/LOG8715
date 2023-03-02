using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RewindComponent
{
    public uint id;
    public Dictionary<uint, CreationComponent> toCreateComponentList;
    public Dictionary<uint, DestroyComponent> toDestroyComponentList;
    public Dictionary<uint, ExplosionComponent> toExplodeComponentList;
    public Dictionary<uint, ProtectedComponent> protectionComponentList;
    public Dictionary<uint, StartProtectionTimeComponent> startProtectionComponentList;
    public Dictionary<uint, PositionComponent> positionComponentList;
    public Dictionary<uint, SpeedComponent> speedComponentList;
    public Dictionary<uint, DynamicComponent> dynamicComponentList;
    public Dictionary<uint, PositionComponent> circleBorderBouncePositionList;
    public Dictionary<uint, SpeedComponent> circleBorderBounceSpeedList;
    public Dictionary<uint, CollisionComponent> collisionComponentList;
    public Dictionary<uint, SizeComponent> sizeComponentList;
}