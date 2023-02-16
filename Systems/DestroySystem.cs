using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace destroySystem
{

    public class DestroySystem : ISystem
    {

        public string Name { get; private set; }
        public DestroySystem(string name)
        {
            worldData.WorldData.toDestroy.Clear();
            Name = name;
        }
        public void UpdateInput() 
        {
        }

        public void UpdateSystem()
        {
            SystemDataUtility.DestroyCircles();
        }
    }
}