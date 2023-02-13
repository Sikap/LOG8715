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
            //SpawnCircleCollisionPath();
            SpawnCircle(10);
        }

        public void SpawnCircle(int numOfCircle)
        {  
            Camera mainCamera = Camera.main;
            float halfCameraWidth = mainCamera.aspect * mainCamera.orthographicSize;
            float halfCameraHeight = mainCamera.orthographicSize;
            for (int i = 0; i < numOfCircle; i++)
            {
                var shapeSize = UnityEngine.Random.Range(1, 9);
                float halfCircle = shapeSize / 2f;
                var shapePosition = new Vector2(
                    UnityEngine.Random.Range(mainCamera.transform.position.x - halfCameraWidth + halfCircle, mainCamera.transform.position.x + halfCameraWidth - halfCircle), 
                    UnityEngine.Random.Range(mainCamera.transform.position.y - halfCameraHeight + halfCircle, mainCamera.transform.position.y + halfCameraHeight - halfCircle));
                var shapeSpeed = new Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
                var isDynamic = DynamicRandomProbability(0.75f);
                if(!isDynamic){
                     shapeSpeed = new Vector2(0.0f, 0.0f);
                }
                var id = SystemDataUtility.AddShapeDataToSystems(shapeSize, shapePosition, shapeSpeed, isDynamic);
                SystemDataUtility.CreateCircle(id, shapePosition, shapeSize);
            }
        }

        public bool DynamicRandomProbability (float probability) {
            return UnityEngine.Random.value < probability;
        }

        private void SpawnCircleCollisionPath()
        {
            var shapeOneSize = 1;
            var shapeOnePosition = new Vector2(5, 0);
            var shapeOneSpeed = new Vector2(-0.1f,0);
            var idOne = SystemDataUtility.AddShapeDataToSystems(shapeOneSize, shapeOnePosition, shapeOneSpeed, true);
            SystemDataUtility.CreateCircle(idOne, shapeOnePosition, shapeOneSize);
            
            var shapeTwoSize = 9;
            var shapeTwoPosition = new Vector2(-5, 0);
            var shapeTwoSpeed = new Vector2(0.1f,0);
            var idTwo = SystemDataUtility.AddShapeDataToSystems(shapeTwoSize, shapeTwoPosition, shapeTwoSpeed, true);
            SystemDataUtility.CreateCircle(idTwo, shapeTwoPosition, shapeTwoSize);
        }

        public void UpdateSystem()
        {
            foreach (KeyValuePair<uint,DestroyComponent> shape in worldData.WorldData.toDestroy)
            {
                SystemDataUtility.RemoveShapeDataFromSystems(shape.Key);
                SystemDataUtility.DestroyShape(shape.Key);
            }
            foreach (var shape in worldData.WorldData.toCreate)
            {
                var id = SystemDataUtility.AddShapeDataToSystems(shape.size, shape.position, shape.speed, true);
                SystemDataUtility.CreateCircle(id, shape.position, shape.size);
            }
            ClearDictionnaries();
        }

        public void ClearDictionnaries() {
            worldData.WorldData.toCreate.Clear();
            worldData.WorldData.toDestroy.Clear();
        }
    }
}