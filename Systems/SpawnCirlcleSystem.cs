using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace circlesSystem
{
    public class SpawnCirclesSystem : ISystem
    {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        public string Name { get; private set; }
        public SpawnCirclesSystem(string name)
        {
            Name = name;
            UnityEngine.Random.InitState(1);
            SpawnCircle(10);
        }

        public void SpawnCircle(int numOfCircle)
        {  
            Camera mainCamera = Camera.main;
            float halfCameraWidth = mainCamera.aspect * mainCamera.orthographicSize;
            float halfCameraHeight = mainCamera.orthographicSize;
            var circlesToSpawn = ecsManager.Config.circleInstancesToSpawn;

            foreach(Config.ShapeConfig shape in circlesToSpawn)
            {
                var isDynamic = true ;               
                if(shape.initialVelocity == new Vector2(0.0f, 0.0f)){
                    isDynamic = false;
                }
                var isProtected = SystemDataUtility.RandomProbability(ecsManager.Config.protectionProbability);
                var id = SystemDataUtility.AddShapeDataToSystems(shape.initialSize, shape.initialPosition, shape.initialVelocity, isDynamic, isProtected);
                SystemDataUtility.CreateCircle(id); 
            } 
        }

        public void UpdateInput() 
        {
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
        }

        public void UpdateSystem()
        {
        }
    }
}