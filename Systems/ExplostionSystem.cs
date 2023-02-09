using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExplostionSystems 
{

    public class ExplostionSystem : ISystem
    {

        public string Name { get; private set; }
        public ExplostionSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}