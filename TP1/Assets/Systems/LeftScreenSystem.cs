using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace leftScreenSystem
{

    public class LeftScreenSystem : ISystem
    {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        public static Camera mainCamera;

        public string Name { get; private set; }
        public LeftScreenSystem(string name)
        {
            mainCamera = Camera.main;
            Name = name;
        }
        public void UpdateInput() 
        {
        }

        public void UpdateSystem()
        {   
            for(int i = 0; i<3; i++){
                SystemDataUtility.CreateCircles();
                SystemDataUtility.DestroyCircles();
                SystemDataUtility.HandleCirclesProtection(true);
                SystemDataUtility.MoveCircles(true);
                SystemDataUtility.HandelCameraScreenBounds(true);
                SystemDataUtility.HandelCirclesCollision(true);
                SystemDataUtility.ResizeCircles(true);
                SystemDataUtility.HandelExplodingCircles(true);
                SystemDataUtility.ColorCircles(true);
            }
        }
    }
}
