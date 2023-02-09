using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mouvementSystems
{

    public class MouvementSystem : ISystem
    {
        public Dictionary<uint, PositionComponent> circlesPosition;
        public Dictionary<uint, SpeedComponent> circlesSpeed;
        public Dictionary<uint, DynamicComponent> circlesIsDynamic;

        public string Name { get; private set; }
        public MouvementSystem(string name)
        {
            Name = name;
            circlesPosition = new Dictionary<uint, PositionComponent>();
            circlesSpeed = new Dictionary<uint, SpeedComponent>();
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}