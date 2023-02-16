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
        }

        public void UpdateSystem()
        {
            SystemDataUtility.ResizeCircles(false);
        }
        
    }
}