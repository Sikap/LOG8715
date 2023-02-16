using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace explosionSystem 
{

    public class ExplosionSystem : ISystem
    {

        public string Name { get; private set; }
        public ExplosionSystem(string name)
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
            SystemDataUtility.HandelExplodingCircles(false);
        }
    }
}