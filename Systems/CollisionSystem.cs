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
           SystemDataUtility.HandelCirclesCollision(false);
        }
    }
}