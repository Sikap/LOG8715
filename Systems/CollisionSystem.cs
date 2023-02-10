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
        public static List<CreationComponent> toCreate = new List<CreationComponent>();
        public static Dictionary<uint, DestroyComponent> toDestroy = new Dictionary<uint, DestroyComponent>();

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
                foreach (KeyValuePair<uint, PositionComponent> shapeTwo in MovementSystem.MovementSystem.circlesPosition)
                {
                    if (shapeOne.Key != shapeTwo.Key) {
                        var shapeTwoPosition = shapeTwo.Value.position;
                        var shapeTwoSpeed = MovementSystem.MovementSystem.circlesSpeed[shapeTwo.Key].speed;
                        var shapeTwoSize = ResizeSystem.ResizeSystem.circlesSize[shapeTwo.Key].size;
                        var collisionResult = CollisionUtility.CalculateCollision(shapeOnePosition, shapeOneSpeed, shapeOneSize, shapeTwoPosition, shapeTwoSpeed, shapeTwoSize);
                        if (collisionResult != null)
                        {
                            if (shapeOneSize > shapeTwoSize)
                            {
                                shapeOneSize++;
                                shapeTwoSize--;
                            } else if (shapeOneSize < shapeTwoSize)
                            {
                                shapeOneSize--;
                                shapeTwoSize++;
                            }
                            newPositionCollision[shapeOne.Key] = new PositionComponent { position = collisionResult.position1 };
                            newSpeedCollision[shapeOne.Key] = new SpeedComponent { speed = collisionResult.velocity1 };
                            newSizeCollision[shapeOne.Key] = new SizeComponent { size = shapeOneSize };
                            circlesCollision[shapeOne.Key] = new CollisionComponent { isCollision = true };
                            newPositionCollision[shapeTwo.Key] = new PositionComponent { position = collisionResult.position2 };
                            newSpeedCollision[shapeTwo.Key] = new SpeedComponent { speed = collisionResult.velocity2 };
                            newSizeCollision[shapeTwo.Key] = new SizeComponent { size = shapeTwoSize };
                            circlesCollision[shapeOne.Key] = new CollisionComponent { isCollision = true };
                        }
                    }
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in newPositionCollision)
            {
                if(newSizeCollision.ContainsKey(shape.Key) && newSizeCollision[shape.Key].size <= 0) {
                    toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };
                } else if (newSizeCollision.ContainsKey(shape.Key) && newSizeCollision[shape.Key].size >= 10) {
                    toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };
                    
                    var idOne = SystemDataUtility.id++;
                    var idTwo = SystemDataUtility.id++;
                    
                    var zeroRadVector = new Vector2(1,0);
                    // Angle of the speed of the two circle +45deg and -45deg from the original new speed as if they broke apart.
                    var angledSpeed = RotateVec(newSpeedCollision[shape.Key].speed, Mathf.Deg2Rad * 45);
                    // To separate circle along the original direction of motion so they dont collide again together
                    var angleRadPerpendicularToMotion = Vector2.Angle(zeroRadVector, Vector2.Perpendicular(MovementSystem.MovementSystem.circlesSpeed[shape.Key].speed)) * Mathf.Deg2Rad;

                    // Find new center of circle x
                    var smallerCircleOnePositionX = newPositionCollision[shape.Key].position.x + ((newSizeCollision[shape.Key].size/2) * Mathf.Cos(angleRadPerpendicularToMotion));
                    // Find new center of circle y
                    var smallerCircleOnePositionY = newPositionCollision[shape.Key].position.y + ((newSizeCollision[shape.Key].size/2) * Mathf.Sin(angleRadPerpendicularToMotion));
                    var smallerCircleOnePosition = new Vector2(smallerCircleOnePositionX, smallerCircleOnePositionY);
                    var smallerCircleOneSize = newSizeCollision[shape.Key].size/2;
                    var smallerCircleOneSpeed = angledSpeed;
                    
                    // Find new center of circle x
                    var smallerCircleTwoPositionX = newPositionCollision[shape.Key].position.x + ((newSizeCollision[shape.Key].size/2) * Mathf.Cos(angleRadPerpendicularToMotion +1));
                    // Find new center of circle y
                    var smallerCircleTwoPositionY = newPositionCollision[shape.Key].position.y + ((newSizeCollision[shape.Key].size/2) * Mathf.Sin(angleRadPerpendicularToMotion +1));                 
                    var smallerCircleTwoPosition = new Vector2(smallerCircleTwoPositionX, smallerCircleTwoPositionY);
                    var smallerCircleTwoSize = newSizeCollision[shape.Key].size/2;
                    var smallerCircleTwoSpeed = Vector2.Perpendicular(angledSpeed);

                    toCreate.Add(new CreationComponent{ size = smallerCircleOneSize, position = smallerCircleOnePosition, speed = smallerCircleOneSpeed });
                    toCreate.Add(new CreationComponent{ size = smallerCircleTwoSize, position = smallerCircleTwoPosition, speed = smallerCircleTwoSpeed });

                    // ecsManager.DestroyShape(shape.Key);
                    // SystemDataUtility.RemoveShapeDataFromSystems(shape.Key);
                    // var idOne = SystemDataUtility.AddShapeDataToSystems(smallerCircleOneSize, smallerCircleOnePosition, smallerCircleOneSpeed);
                    // var idTwo = SystemDataUtility.AddShapeDataToSystems(smallerCircleTwoSize, smallerCircleTwoPosition, smallerCircleTwoSpeed);
                    // SystemDataUtility.CreateCircle(idOne, smallerCircleOnePosition, smallerCircleOneSize);
                    // SystemDataUtility.CreateCircle(idTwo, smallerCircleTwoPosition, smallerCircleTwoSize);
                } else {
                    MovementSystem.MovementSystem.circlesPosition[shape.Key] = shape.Value;
                    MovementSystem.MovementSystem.circlesSpeed[shape.Key] = newSpeedCollision[shape.Key];
                    ResizeSystem.ResizeSystem.circlesSize[shape.Key] = newSizeCollision[shape.Key];
                    // SystemDataUtility.UpdateShapePosition(shape.Key, shape.Value.position);
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
            toCreate.Clear();
            toDestroy.Clear();
        }
    }
}