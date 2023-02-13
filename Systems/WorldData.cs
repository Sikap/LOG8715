using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace worldData{

    public class WorldData {
        // Spawn
        public static List<CreationComponent> toCreate = new List<CreationComponent>();
        public static Dictionary<uint, DestroyComponent> toDestroy = new Dictionary<uint, DestroyComponent>();
        // Protection
        public static Dictionary<uint, ProtectedComponent> circlesProtection = new Dictionary<uint, ProtectedComponent>();
        // Movement 
        public static Dictionary<uint, PositionComponent> circlesPosition = new Dictionary<uint,PositionComponent>();
        public static Dictionary<uint, SpeedComponent> circlesSpeed = new Dictionary<uint,SpeedComponent>();
        public static Dictionary<uint, DynamicComponent> circlesIsDynamic = new Dictionary<uint,DynamicComponent>();
        //Border
        public static Dictionary<uint, PositionComponent> circleBorderBouncePosition = new Dictionary<uint, PositionComponent>();
        public static Dictionary<uint, SpeedComponent> circleBorderBounceSpeed = new Dictionary<uint, SpeedComponent>();
        //CollisionSystem 
        public static Dictionary<uint, CollisionComponent> circlesCollision = new Dictionary<uint, CollisionComponent>();
        public static Dictionary<uint, PositionComponent> newPositionCollision = new Dictionary<uint, PositionComponent>();
        public static Dictionary<uint, SpeedComponent> newSpeedCollision = new Dictionary<uint, SpeedComponent>();
        public static Dictionary<uint, SizeComponent> newSizeCollision = new Dictionary<uint, SizeComponent>();   
        //Resize 
        public static Dictionary<uint, SizeComponent> circlesSize = new Dictionary<uint,SizeComponent>();
    }

}