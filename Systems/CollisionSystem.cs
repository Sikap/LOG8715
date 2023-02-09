using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionSystems
{

    public class CollisionSystem : ISystem
    {
        public Dictionary<uint, CollisionComponent> circlesCollision;

        public string Name { get; private set; }
        public CollisionSystem(string name)
        {
            Name = name;
            circlesCollision = new Dictionary<uint, CollisionComponent>();
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}