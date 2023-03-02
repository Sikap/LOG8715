using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace collisionSystem
{

    public class CollisionSystem : ISystem
    {

        public string Name { get; private set; }
        public CollisionSystem(string name)
        {
            worldData.WorldData.circlesCollision.Clear();
            Name = name;
        }
        public void UpdateInput() 
        {
        }
        public void UpdateSystem()
        {
           SystemDataUtility.HandelCirclesCollision(false);
        }
    }
}