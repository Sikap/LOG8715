using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace movementSystems
{
    public class MovementSystem : ISystem
    {

        public string Name { get; private set; }
        public MovementSystem(string name)
        {
            Name = name;
        }
        public void UpdateInput() 
        {
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
        }
        public void MoveObject() 
        {
            global::ECSManager ecsManager = global::ECSManager.Instance;
            var toModify = new Dictionary<uint, PositionComponent>();
            foreach (KeyValuePair<uint,DynamicComponent> entry in worldData.WorldData.circlesIsDynamic)
            {            
                if (!worldData.WorldData.circlesCollision.ContainsKey(entry.Key) || true)
                {
                    var xPosition = worldData.WorldData.circlesPosition[entry.Key].position.x + worldData.WorldData.circlesSpeed[entry.Key].speed.x;
                    var yPosition = worldData.WorldData.circlesPosition[entry.Key].position.y + worldData.WorldData.circlesSpeed[entry.Key].speed.y;
                    toModify.Add(entry.Key,new PositionComponent { position = new Vector2(xPosition, yPosition)});
                }
            }
            foreach (KeyValuePair<uint,PositionComponent> entry in toModify)
            {
                worldData.WorldData.circlesPosition[entry.Key] = entry.Value;
                ecsManager.UpdateShapePosition(entry.Key, entry.Value.position);
            }
        }

        public void UpdateSystem()
        {
            MoveObject();
        }
    }
}