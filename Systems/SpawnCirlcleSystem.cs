using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionSystem = collisionSystems;

namespace circlesSystem
{
    public class SpawnCirclesSystem : ISystem
    {
        public string Name { get; private set; }
       
        public SpawnCirclesSystem(string name)
        {
            Name = name;
            UnityEngine.Random.InitState(1);
            SpawnCircleCollisionPath();
            // SpawnCircle(10);
        }

        public void SpawnCircle(int numOfCircle)
        {  
            for (int i = 0; i < numOfCircle;i++)
            {
                var shapeSize = UnityEngine.Random.Range(0, 10);
                var shapePosition = new Vector2(UnityEngine.Random.Range(-30f, 30f), UnityEngine.Random.Range(-30f, 30f));
                var shapeSpeed = new Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
                var id = SystemDataUtility.AddShapeDataToSystems(shapeSize, shapePosition, shapeSpeed);
                SystemDataUtility.CreateCircle(id, shapePosition, shapeSize);
            }
        }

        private void SpawnCircleCollisionPath()
        {
            var shapeOneSize = 1;
            var shapeOnePosition = new Vector2(10, 0);
            var shapeOneSpeed = new Vector2(-0.1f,0);
            var idOne = SystemDataUtility.AddShapeDataToSystems(shapeOneSize, shapeOnePosition, shapeOneSpeed);
            SystemDataUtility.CreateCircle(idOne, shapeOnePosition, shapeOneSize);
            
            var shapeTwoSize = 9;
            var shapeTwoPosition = new Vector2(-10, 0);
            var shapeTwoSpeed = new Vector2(0.1f,0);
            var idTwo = SystemDataUtility.AddShapeDataToSystems(shapeTwoSize, shapeTwoPosition, shapeTwoSpeed);
            SystemDataUtility.CreateCircle(idTwo, shapeTwoPosition, shapeTwoSize);
        }

        public void UpdateSystem()
        {
            foreach (KeyValuePair<uint,DestroyComponent> shape in CollisionSystem.CollisionSystem.toDestroy)
            {
                SystemDataUtility.RemoveShapeDataFromSystems(shape.Key);
                SystemDataUtility.DestroyShape(shape.Key);
            }
            foreach (var shape in CollisionSystem.CollisionSystem.toCreate)
            {
                var id = SystemDataUtility.AddShapeDataToSystems(shape.size, shape.position, shape.speed);
                SystemDataUtility.CreateCircle(id, shape.position, shape.size);
            }
        }
    }
}