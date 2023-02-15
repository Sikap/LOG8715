using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace resizeSystems
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
            foreach (KeyValuePair<uint, SizeComponent> shape in worldData.WorldData.newSizeCollision)
            {
                if (worldData.WorldData.circlesSize.ContainsKey(shape.Key))
                {
                    SystemDataUtility.UpdateShapeSize(shape.Key, shape.Value.size);
                }
            }
        }
    }
}