using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace collisionSystem
{

    public class CollisionSystem : ISystem
    {

        public string Name { get; private set; }
        public CollisionSystem(string name)
        {
            worldData.WorldData.circlesCollision.Clear();
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
            ClearDictionnaries();
            var newPositionCollision = new Dictionary<uint, PositionComponent>();
            var newSpeedCollision = new Dictionary<uint, SpeedComponent>();
            foreach (KeyValuePair<uint, PositionComponent> shapeOne in worldData.WorldData.circlesPosition)
            {
                if (SystemDataUtility.IsProcessable(shapeOne.Key)) {
                    var shapeOnePosition = shapeOne.Value.position;
                    var shapeOneSpeed = worldData.WorldData.circlesSpeed[shapeOne.Key].speed;
                    var shapeOneSize = worldData.WorldData.circlesSize[shapeOne.Key].size;
                    var shapeOneIsDynamic = worldData.WorldData.circlesIsDynamic[shapeOne.Key].isDynamic;

                    foreach (KeyValuePair<uint, PositionComponent> shapeTwo in worldData.WorldData.circlesPosition)
                    {
                        if (SystemDataUtility.IsProcessable(shapeTwo.Key)) {  
                            if (shapeOne.Key != shapeTwo.Key) {
                                var shapeTwoPosition = shapeTwo.Value.position;
                                var shapeTwoSpeed = worldData.WorldData.circlesSpeed[shapeTwo.Key].speed;
                                var shapeTwoSize = worldData.WorldData.circlesSize[shapeTwo.Key].size;
                                var shapeTwoIsDynamic = worldData.WorldData.circlesIsDynamic[shapeTwo.Key].isDynamic;
                                var collisionResult = CollisionUtility.CalculateCollision(shapeOnePosition, shapeOneSpeed, shapeOneSize, shapeTwoPosition, shapeTwoSpeed, shapeTwoSize);
                                if (collisionResult != null)
                                {   
                                    var newPosition1 = collisionResult.position1;
                                    var newSpeed1 = collisionResult.velocity1 ;
                                    var newPosition2 = collisionResult.position2;
                                    var newSpeed2 = collisionResult.velocity2 ;

                                    // Keep circle in same spot if it's static.
                                    if (!shapeTwoIsDynamic){
                                        newPosition2 = shapeTwoPosition;
                                        newSpeed2 = shapeTwoSpeed;
                                    } else if(!shapeOneIsDynamic){
                                        newPosition1 = shapeOnePosition;
                                        newSpeed1 = shapeOneSpeed;
                                    }
                                    
                                    newPositionCollision[shapeOne.Key] = new PositionComponent { position = newPosition1 };
                                    newSpeedCollision[shapeOne.Key] = new SpeedComponent { speed = newSpeed1 };

                                    newPositionCollision[shapeTwo.Key] = new PositionComponent { position = newPosition2 };
                                    newSpeedCollision[shapeTwo.Key] = new SpeedComponent { speed = newSpeed2 };

                                    // Only add one collision for the given collision between two circles
                                    worldData.WorldData.circlesCollision[shapeOne.Key] = new CollisionComponent { circleMain = shapeOne.Key, circleColliding = shapeTwo.Key };
                                    worldData.WorldData.circlesCollision[shapeTwo.Key] = new CollisionComponent { circleMain = shapeTwo.Key, circleColliding = shapeOne.Key };
                                }
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in newPositionCollision)
            {
                worldData.WorldData.circlesPosition[shape.Key] = shape.Value;
                worldData.WorldData.circlesSpeed[shape.Key] = newSpeedCollision[shape.Key];
            }
        }

        public Vector2 RotateVec(Vector2 vector, float radRotate) {
            var vecX = vector.x * Mathf.Cos(radRotate) - vector.y * Mathf.Sin(radRotate);
            var vecY = vector.y * Mathf.Sin(radRotate) + vector.y * Mathf.Cos(radRotate);
            return new Vector2(vecX, vecY);
        }

        public void ClearDictionnaries() {
            worldData.WorldData.circlesCollision.Clear();
        }
    }
}