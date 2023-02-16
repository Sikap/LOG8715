using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace protectionSystem
{

    public class ProtectionSystem : ISystem
    {
        global::ECSManager ecsManager = global::ECSManager.Instance;                        
        public string Name { get; private set; }
        public ProtectionSystem(string name)
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
            SystemDataUtility.HandleCirclesProtection(false);
        }
    }
}