using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace circlesSystem
{
    public class SpawnCirclesSystem : ISystem
    {
        public string Name { get; private set; }
       
        public SpawnCirclesSystem(string name)
        {
            Name = name;
            UnityEngine.Random.InitState(1);
            // SpawnCircleCollisionPath();
            SpawnCircle(100);
        }

        public void SpawnCircle(int numOfCircle) {  
            global::ECSManager ecsManager = global::ECSManager.Instance;
            for (int i = 0; i < numOfCircle;i++)
            {
                var shapeSize = UnityEngine.Random.Range(0, 10);
                var shapePosition = new Vector2(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f));
                var shapeSpeed = new Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
                var id = SystemDataUtility.AddShapeDataToSystems(shapeSize, shapePosition, shapeSpeed);
                ecsManager.CreateShape(id, shapeSize);
                ecsManager.UpdateShapePosition(id, shapePosition);
            }
        }

        private void SpawnCircleCollisionPath() {
            global::ECSManager ecsManager = global::ECSManager.Instance;
            var shapeOneSize = UnityEngine.Random.Range(0, 10);
            var shapeOnePosition = new Vector2(10, 0);
            var shapeOneSpeed = new Vector2(-0.1f,0);
            var idOne = SystemDataUtility.AddShapeDataToSystems(shapeOneSize, shapeOnePosition, shapeOneSpeed);
            ecsManager.CreateShape(idOne, shapeOneSize);
            ecsManager.UpdateShapePosition(idOne, shapeOnePosition);
            
            var shapeTwoSize = UnityEngine.Random.Range(0, 10);
            var shapeTwoPosition = new Vector2(-10, 0);
            var shapeTwoSpeed = new Vector2(0.1f,0);
            var idTwo = SystemDataUtility.AddShapeDataToSystems(shapeTwoSize, shapeTwoPosition, shapeTwoSpeed);
            ecsManager.CreateShape(idTwo, shapeTwoSize);
            ecsManager.UpdateShapePosition(idTwo, shapeTwoPosition);
        }

        public void UpdateSystem()
        {
        }
    }
}