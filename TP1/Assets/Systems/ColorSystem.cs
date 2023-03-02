using System.Collections.Generic;
using System;
using UnityEngine;

namespace colorSystem
{

    public class ColorSystem : ISystem
    {
        global::ECSManager ecsManager = global::ECSManager.Instance;                        

        public string Name { get; private set; }
        public ColorSystem(string name)
        {
            Name = name;
        }

        public bool clickInCircle(Vector2 clickPosition, Vector2 circlePosition, float size) {
            return Vector2.Distance(clickPosition, circlePosition) < size;
        }
        public void UpdateInput() 
        {
        }
        public void UpdateSystem()        
        {            
           SystemDataUtility.ColorCircles(false);                        
        }
    }
}