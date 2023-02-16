using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace leftScreenSystem
{

    public class LeftScreenSystem : ISystem
    {

        public string Name { get; private set; }
        public LeftScreenSystem(string name)
        {
            Name = name;
        }
        public void UpdateInput() 
        {
        }

        public void UpdateSystem()
        {
            // DO NOT FORGET TO CHECK IF CIRCLE IS PROCESSABLE SystemDataUtility.IsProcessable(id)

        }
    }
}