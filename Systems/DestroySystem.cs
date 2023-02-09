using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DestroySystems
{

    public class DestroySystem : ISystem
    {

        public string Name { get; private set; }
        public DestroySystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}