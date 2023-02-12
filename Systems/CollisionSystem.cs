using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementSystem = movementSystems;
using ResizeSystem = resizeSystems;
using CirclesSystem = circlesSystem;

namespace collisionSystems
{

    public class CollisionSystem : ISystem
    {
        public static Dictionary<uint, CollisionComponent> circlesCollision = new Dictionary<uint, CollisionComponent>();
        public static Dictionary<uint, PositionComponent> newPositionCollision = new Dictionary<uint, PositionComponent>();
        public static Dictionary<uint, SpeedComponent> newSpeedCollision = new Dictionary<uint, SpeedComponent>();
        public static Dictionary<uint, SizeComponent> newSizeCollision = new Dictionary<uint, SizeComponent>();

        public string Name { get; private set; }
        public CollisionSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            ClearDictionnaries();
            foreach (KeyValuePair<uint, PositionComponent> shapeOne in MovementSystem.MovementSystem.circlesPosition)
            {
                var shapeOnePosition = shapeOne.Value.position;
                var shapeOneSpeed = MovementSystem.MovementSystem.circlesSpeed[shapeOne.Key].speed;
                var shapeOneSize = ResizeSystem.ResizeSystem.circlesSize[shapeOne.Key].size;
                var shapeOneIsDynamic = MovementSystem.MovementSystem.circlesIsDynamic[shapeOne.Key].isDynamic;

                foreach (KeyValuePair<uint, PositionComponent> shapeTwo in MovementSystem.MovementSystem.circlesPosition)
                {
                    if (shapeOne.Key != shapeTwo.Key) {
                        var shapeTwoPosition = shapeTwo.Value.position;
                        var shapeTwoSpeed = MovementSystem.MovementSystem.circlesSpeed[shapeTwo.Key].speed;
                        var shapeTwoSize = ResizeSystem.ResizeSystem.circlesSize[shapeTwo.Key].size;
                        var shapeTwoIsDynamic = MovementSystem.MovementSystem.circlesIsDynamic[shapeTwo.Key].isDynamic;
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
                            
                            newPositionCollision[shapeOne.Key] = new PositionComponent { position = newPosition1 };
                            newSpeedCollision[shapeOne.Key] = new SpeedComponent { speed = newSpeed1 };
                            newSizeCollision[shapeOne.Key] = new SizeComponent { size = shapeOneSize };
                            circlesCollision[shapeOne.Key] = new CollisionComponent { isCollision = true };
                            newPositionCollision[shapeTwo.Key] = new PositionComponent { position = newPosition2 };
                            newSpeedCollision[shapeTwo.Key] = new SpeedComponent { speed = newSpeed2 };
                            newSizeCollision[shapeTwo.Key] = new SizeComponent { size = shapeTwoSize };
                            circlesCollision[shapeTwo.Key] = new CollisionComponent { isCollision = true };
                        }
                    }
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in newPositionCollision)
            {
                if(newSizeCollision.ContainsKey(shape.Key) && newSizeCollision[shape.Key].size <= 0) {
                    CirclesSystem.SpawnCirclesSystem.toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };
                } else if (newSizeCollision.ContainsKey(shape.Key) && newSizeCollision[shape.Key].size >= 10) {
                    CirclesSystem.SpawnCirclesSystem.toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };                    
                    var zeroRadVector = new Vector2(1,0);
                    var motionAngle = Vector2.Angle(zeroRadVector, newSpeedCollision[shape.Key].speed) * Mathf.Deg2Rad;

                    var smallerCircleOnePositionX = newPositionCollision[shape.Key].position.x + ((newSizeCollision[shape.Key].size/4) * Mathf.Cos(motionAngle));
                    var smallerCircleOnePositionY = newPositionCollision[shape.Key].position.y + ((newSizeCollision[shape.Key].size/4) * Mathf.Sin(motionAngle));

                    var smallerCircleOnePosition = new Vector2(smallerCircleOnePositionX, smallerCircleOnePositionY);
                    var smallerCircleOneSize = newSizeCollision[shape.Key].size/2;
                    var smallerCircleOneSpeed = newSpeedCollision[shape.Key].speed;
                    
                    var smallerCircleTwoPositionX = newPositionCollision[shape.Key].position.x + ((newSizeCollision[shape.Key].size/4) * Mathf.Cos(motionAngle + Mathf.PI));
                    var smallerCircleTwoPositionY = newPositionCollision[shape.Key].position.y + ((newSizeCollision[shape.Key].size/4) * Mathf.Sin(motionAngle + Mathf.PI));                 

                    var smallerCircleTwoPosition = new Vector2(smallerCircleTwoPositionX, smallerCircleTwoPositionY);
                    var smallerCircleTwoSize = newSizeCollision[shape.Key].size/2;
                    var smallerCircleTwoSpeed = newSpeedCollision[shape.Key].speed * -1;

                    CirclesSystem.SpawnCirclesSystem.toCreate.Add(new CreationComponent{ size = smallerCircleOneSize, position = smallerCircleOnePosition, speed = smallerCircleOneSpeed });
                    CirclesSystem.SpawnCirclesSystem.toCreate.Add(new CreationComponent{ size = smallerCircleTwoSize, position = smallerCircleTwoPosition, speed = smallerCircleTwoSpeed });
                } else {
                    MovementSystem.MovementSystem.circlesPosition[shape.Key] = shape.Value;
                    MovementSystem.MovementSystem.circlesSpeed[shape.Key] = newSpeedCollision[shape.Key];
                    ResizeSystem.ResizeSystem.circlesSize[shape.Key] = newSizeCollision[shape.Key];
                }
            }
        }

        public Vector2 RotateVec(Vector2 vector, float radRotate) {
            var vecX = vector.x * Mathf.Cos(radRotate) - vector.y * Mathf.Sin(radRotate);
            var vecY = vector.y * Mathf.Sin(radRotate) + vector.y * Mathf.Cos(radRotate);
            return new Vector2(vecX, vecY);
        }

        public void ClearDictionnaries() {
            circlesCollision.Clear();
            newPositionCollision.Clear();
            newSpeedCollision.Clear();
            newSizeCollision.Clear();
        }
    }
}