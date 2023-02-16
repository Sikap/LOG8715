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
            SystemDataUtility.DestroyCircles();
        }
    }
}