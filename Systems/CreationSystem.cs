using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace creationSystem
{

    public class CreationSystem : ISystem
    {

        public string Name { get; private set; }
        public CreationSystem(string name)
        {
            worldData.WorldData.toCreate.Clear();
            Name = name;
        }
        public void UpdateInput() 
        {
        }

        public void UpdateSystem()
        {
            foreach (KeyValuePair<uint, CreationComponent> shape in worldData.WorldData.toCreate)
            {
                SystemDataUtility.CreateCircle(shape.Key);
            }
            worldData.WorldData.toCreate.Clear();
        }
    }
}