using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace worldData{

    public class WorldData {
        // Spawn
        public static Dictionary<uint, CreationComponent> toCreate = new Dictionary<uint, CreationComponent>();
        public static Dictionary<uint, DestroyComponent> toDestroy = new Dictionary<uint, DestroyComponent>();
        public static Dictionary<uint, ExplosionComponent> toExplode = new Dictionary<uint, ExplosionComponent>();
        // Protection
        public static Dictionary<uint, ProtectedComponent> circlesProtection = new Dictionary<uint, ProtectedComponent>();
        public static Dictionary<uint, StartProtectionTimeComponent> circleProtectionStartTime = new Dictionary<uint, StartProtectionTimeComponent>();
        // Movement 
        public static Dictionary<uint, PositionComponent> circlesPosition = new Dictionary<uint,PositionComponent>();
        public static Dictionary<uint, SpeedComponent> circlesSpeed = new Dictionary<uint,SpeedComponent>();
        public static Dictionary<uint, DynamicComponent> circlesIsDynamic = new Dictionary<uint,DynamicComponent>();
        //Border
        public static Dictionary<uint, PositionComponent> circleBorderBouncePosition = new Dictionary<uint, PositionComponent>();
        public static Dictionary<uint, SpeedComponent> circleBorderBounceSpeed = new Dictionary<uint, SpeedComponent>();
        //CollisionSystem 
        public static Dictionary<uint, CollisionComponent> circlesCollision = new Dictionary<uint, CollisionComponent>();  
        //Resize 
        public static Dictionary<uint, SizeComponent> circlesSize = new Dictionary<uint,SizeComponent>();
        //Rewind
        public static Dictionary<float, RewindComponent> rewind = new Dictionary<float, RewindComponent>();
        //Click event
        public static Dictionary<uint, ClickComponent> circlesClicked = new Dictionary<uint, ClickComponent>();
        public static float rewindTimePressed = 0f;
        public static float timeClick = 0f;
    }

}