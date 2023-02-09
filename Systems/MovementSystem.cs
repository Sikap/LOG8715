using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionSystems = collisionSystems;

namespace movementSystems
{
    public class MovementSystem : ISystem
    {
        public static Dictionary<uint, PositionComponent> circlesPosition = new Dictionary<uint,PositionComponent>();
        public static Dictionary<uint, SpeedComponent> circlesSpeed = new Dictionary<uint,SpeedComponent>();
        public static Dictionary<uint, DynamicComponent> circlesIsDynamic = new Dictionary<uint,DynamicComponent>();

        public string Name { get; private set; }
        public MovementSystem(string name)
        {
            Name = name;
        }

        public void MoveObject() 
        {
            global::ECSManager ecsManager = global::ECSManager.Instance;
            var toModify = new Dictionary<uint, PositionComponent>();
            foreach (KeyValuePair<uint,PositionComponent> entry in circlesPosition)
            {
                if (!CollisionSystems.CollisionSystem.circlesCollision.ContainsKey(entry.Key) || true)
                {
                    var xPosition = circlesPosition[entry.Key].position.x + circlesSpeed[entry.Key].speed.x;
                    var yPosition = circlesPosition[entry.Key].position.y + circlesSpeed[entry.Key].speed.y;
                    toModify.Add(entry.Key,new PositionComponent { position = new Vector2(xPosition, yPosition)});
                }
            }
            foreach (KeyValuePair<uint,PositionComponent> entry in toModify)
            {
                circlesPosition[entry.Key] = entry.Value;
                ecsManager.UpdateShapePosition(entry.Key, entry.Value.position);
            }
        }

        public void UpdateSystem()
        {
            MoveObject();
        }
    }
}