using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace collisionSystems
{

    public class CollisionSystem : ISystem
    {

        public string Name { get; private set; }
        public CollisionSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            ClearDictionnaries();
            foreach (KeyValuePair<uint, PositionComponent> shapeOne in worldData.WorldData.circlesPosition)
            {
                var shapeOnePosition = shapeOne.Value.position;
                var shapeOneSpeed = worldData.WorldData.circlesSpeed[shapeOne.Key].speed;
                var shapeOneSize = worldData.WorldData.circlesSize[shapeOne.Key].size;
                var shapeOneIsDynamic = worldData.WorldData.circlesIsDynamic[shapeOne.Key].isDynamic;

                foreach (KeyValuePair<uint, PositionComponent> shapeTwo in worldData.WorldData.circlesPosition)
                {
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
                            if(shapeOneIsDynamic && shapeTwoIsDynamic){
                                if (shapeOneSize > shapeTwoSize)
                                {
                                    shapeOneSize++;
                                    shapeTwoSize--;
                                } else if (shapeOneSize < shapeTwoSize)
                                {
                                    shapeOneSize--;
                                    shapeTwoSize++;
                                }
                            } else if (shapeOneIsDynamic){
                                newPosition2 = shapeTwoPosition;
                                newSpeed2 = shapeTwoSpeed;
                            } else if(shapeTwoIsDynamic){
                                newPosition1 = shapeOnePosition;
                                newSpeed1 = shapeOneSpeed;
                            }
                            
                            worldData.WorldData.newPositionCollision[shapeOne.Key] = new PositionComponent { position = newPosition1 };
                            worldData.WorldData.newSpeedCollision[shapeOne.Key] = new SpeedComponent { speed = newSpeed1 };
                            worldData.WorldData.newSizeCollision[shapeOne.Key] = new SizeComponent { size = shapeOneSize };
                            worldData.WorldData.circlesCollision[shapeOne.Key] = new CollisionComponent { isCollision = true };
                            worldData.WorldData.newPositionCollision[shapeTwo.Key] = new PositionComponent { position = newPosition2 };
                            worldData.WorldData.newSpeedCollision[shapeTwo.Key] = new SpeedComponent { speed = newSpeed2 };
                            worldData.WorldData.newSizeCollision[shapeTwo.Key] = new SizeComponent { size = shapeTwoSize };
                            worldData.WorldData.circlesCollision[shapeTwo.Key] = new CollisionComponent { isCollision = true };
                        }else {
                            worldData.WorldData.circlesCollision[shapeOne.Key] = new CollisionComponent { isCollision = false };
                            worldData.WorldData.circlesCollision[shapeTwo.Key] = new CollisionComponent { isCollision = false };
                        }
                    }
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.newPositionCollision)
            {
                if(worldData.WorldData.newSizeCollision.ContainsKey(shape.Key) && worldData.WorldData.newSizeCollision[shape.Key].size <= 0) {
                    worldData.WorldData.toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };
                } else if (worldData.WorldData.newSizeCollision.ContainsKey(shape.Key) && worldData.WorldData.newSizeCollision[shape.Key].size >= 10) {
                    worldData.WorldData.toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };                    
                    var zeroRadVector = new Vector2(1,0);
                    var motionAngle = Vector2.Angle(zeroRadVector, worldData.WorldData.newSpeedCollision[shape.Key].speed) * Mathf.Deg2Rad;

                    var smallerCircleOnePositionX = worldData.WorldData.newPositionCollision[shape.Key].position.x + ((worldData.WorldData.newSizeCollision[shape.Key].size/4) * Mathf.Cos(motionAngle));
                    var smallerCircleOnePositionY = worldData.WorldData.newPositionCollision[shape.Key].position.y + ((worldData.WorldData.newSizeCollision[shape.Key].size/4) * Mathf.Sin(motionAngle));

                    var smallerCircleOnePosition = new Vector2(smallerCircleOnePositionX, smallerCircleOnePositionY);
                    var smallerCircleOneSize = worldData.WorldData.newSizeCollision[shape.Key].size/2;
                    var smallerCircleOneSpeed = worldData.WorldData.newSpeedCollision[shape.Key].speed;
                    
                    var smallerCircleTwoPositionX = worldData.WorldData.newPositionCollision[shape.Key].position.x + ((worldData.WorldData.newSizeCollision[shape.Key].size/4) * Mathf.Cos(motionAngle + Mathf.PI));
                    var smallerCircleTwoPositionY = worldData.WorldData.newPositionCollision[shape.Key].position.y + ((worldData.WorldData.newSizeCollision[shape.Key].size/4) * Mathf.Sin(motionAngle + Mathf.PI));                 

                    var smallerCircleTwoPosition = new Vector2(smallerCircleTwoPositionX, smallerCircleTwoPositionY);
                    var smallerCircleTwoSize = worldData.WorldData.newSizeCollision[shape.Key].size/2;
                    var smallerCircleTwoSpeed = worldData.WorldData.newSpeedCollision[shape.Key].speed * -1;

                    worldData.WorldData.toCreate.Add(new CreationComponent{ size = smallerCircleOneSize, position = smallerCircleOnePosition, speed = smallerCircleOneSpeed });
                    worldData.WorldData.toCreate.Add(new CreationComponent{ size = smallerCircleTwoSize, position = smallerCircleTwoPosition, speed = smallerCircleTwoSpeed });
                } else {
                    worldData.WorldData.circlesPosition[shape.Key] = shape.Value;
                    worldData.WorldData.circlesSpeed[shape.Key] = worldData.WorldData.newSpeedCollision[shape.Key];
                    worldData.WorldData.circlesSize[shape.Key] = worldData.WorldData.newSizeCollision[shape.Key];
                }
            }
        }

        public Vector2 RotateVec(Vector2 vector, float radRotate) {
            var vecX = vector.x * Mathf.Cos(radRotate) - vector.y * Mathf.Sin(radRotate);
            var vecY = vector.y * Mathf.Sin(radRotate) + vector.y * Mathf.Cos(radRotate);
            return new Vector2(vecX, vecY);
        }

        public void ClearDictionnaries() {
            worldData.WorldData.circlesCollision.Clear();
            worldData.WorldData.newPositionCollision.Clear();
            worldData.WorldData.newSpeedCollision.Clear();
            worldData.WorldData.newSizeCollision.Clear();
        }
    }
}