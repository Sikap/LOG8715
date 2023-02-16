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
           SystemDataUtility.CreateCircles();
        }
    }
}