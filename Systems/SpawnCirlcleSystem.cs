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
        bool firstSpawn = true;
        public string Name { get; private set; }
        public SpawnCirclesSystem(string name)
        {
            Name = name;
            UnityEngine.Random.InitState(ecsManager.Config.seed);
        }

        public void SpawnCircle()
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
        }

        public void UpdateSystem()
        {
            if (firstSpawn)
            {
                SpawnCircle();
                firstSpawn = false;
            }
        }
    }
}