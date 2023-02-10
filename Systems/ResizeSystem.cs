using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionSystem = collisionSystems;

namespace resizeSystems
{
    public class ResizeSystem : ISystem
    {
        public static Dictionary<uint, SizeComponent> circlesSize = new Dictionary<uint,SizeComponent>();

        public string Name { get; private set; }
        public ResizeSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            foreach (KeyValuePair<uint, SizeComponent> shape in CollisionSystem.CollisionSystem.newSizeCollision)
            {
                if (circlesSize.ContainsKey(shape.Key))
                {
                    SystemDataUtility.UpdateShapeSize(shape.Key, shape.Value.size);
                }
            }
        }
    }
}