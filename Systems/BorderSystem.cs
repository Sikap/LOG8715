using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace borderSystem
{

    public class BorderSystem : ISystem
    {
        public static Camera mainCamera;

        public string Name { get; private set; }
        public BorderSystem(string name)
        {
            mainCamera = Camera.main;
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
            SystemDataUtility.HandelCameraScreenBounds(false);
        }
    }
}