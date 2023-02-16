using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace movementSystem
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
        }
        public void MoveObject() 
        {
            var toModify = new Dictionary<uint, PositionComponent>();
            foreach (KeyValuePair<uint,DynamicComponent> entry in worldData.WorldData.circlesIsDynamic)
            {            
                if (SystemDataUtility.IsProcessable(entry.Key))
                {
                    var xPosition = worldData.WorldData.circlesPosition[entry.Key].position.x + worldData.WorldData.circlesSpeed[entry.Key].speed.x * Time.deltaTime;
                    var yPosition = worldData.WorldData.circlesPosition[entry.Key].position.y + worldData.WorldData.circlesSpeed[entry.Key].speed.y * Time.deltaTime;
                    toModify.Add(entry.Key,new PositionComponent { position = new Vector2(xPosition, yPosition)});
                }
            }
            foreach (KeyValuePair<uint,PositionComponent> entry in toModify)
            {
                worldData.WorldData.circlesPosition[entry.Key] = entry.Value;
                SystemDataUtility.UpdateShapePosition(entry.Key, entry.Value.position);
            }
        }

        public void UpdateSystem()
        {
            MoveObject();
        }
    }
}