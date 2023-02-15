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
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}