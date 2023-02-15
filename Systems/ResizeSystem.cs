using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace resizeSystem
{
    public class ResizeSystem : ISystem
    {
        public string Name { get; private set; }
        public ResizeSystem(string name)
        {
            Name = name;
        }
        public void UpdateInput() 
        {
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
        }

        public void UpdateSystem()
        {
            foreach (KeyValuePair<uint, CollisionComponent> shape in worldData.WorldData.circlesCollision)
            {
                if (SystemDataUtility.IsProcessable(shape.Key) && worldData.WorldData.circlesSize.ContainsKey(shape.Key))
                {
                    SystemDataUtility.UpdateShapeSize(shape.Key, worldData.WorldData.circlesSize[shape.Key].size);
                }
            }
        }
    }
}