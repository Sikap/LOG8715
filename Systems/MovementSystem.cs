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
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
        }

        public void UpdateSystem()
        {
            SystemDataUtility.MoveCircles(false);
        }
    }
}