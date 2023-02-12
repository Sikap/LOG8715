using System.Collections.Generic;
using System;
using UnityEngine;
using MovementSystem = movementSystems;

namespace colorSystems
{

    public class ColorSystem : ISystem
    {

        public string Name { get; private set; }
        public ColorSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            foreach (KeyValuePair<uint,PositionComponent> shape in MovementSystem.MovementSystem.circlesPosition)
            {
                global::ECSManager ecsManager = global::ECSManager.Instance;
                ecsManager.UpdateShapeColor(shape.Key, Color.red);
            }
        }
    }
}