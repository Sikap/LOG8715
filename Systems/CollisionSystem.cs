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

        public string Name { get; private set; }
        public CollisionSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            circlesCollision.Clear();
            global::ECSManager ecsManager = global::ECSManager.Instance;
            var newPositionCollision = new Dictionary<uint, PositionComponent>();
            var newSpeedCollision = new Dictionary<uint, SpeedComponent>();
            var newSizeCollision = new Dictionary<uint, SizeComponent>();
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
                    ecsManager.DestroyShape(shape.Key);
                    SystemDataUtility.RemoveShapeDataFromSystems(shape.Key);
                } else if (newSizeCollision.ContainsKey(shape.Key) && newSizeCollision[shape.Key].size >= 10) {
                    ecsManager.DestroyShape(shape.Key);
                    SystemDataUtility.RemoveShapeDataFromSystems(shape.Key);
                    var idOne = SystemDataUtility.AddShapeDataToSystems(5, new Vector2(10,10), new Vector2(0.1f,0.1f));
                    var idTwo = SystemDataUtility.AddShapeDataToSystems(5, new Vector2(-10,10), new Vector2(-0.1f,-0.1f));
                    ecsManager.CreateShape(idOne, 5);
                    ecsManager.UpdateShapePosition(idOne, new Vector2(10,10));
                    ecsManager.CreateShape(idTwo, 5);
                    ecsManager.UpdateShapePosition(idTwo, new Vector2(-10,10));
                } else {
                    MovementSystem.MovementSystem.circlesPosition[shape.Key] = shape.Value;
                    MovementSystem.MovementSystem.circlesSpeed[shape.Key] = newSpeedCollision[shape.Key];
                    ResizeSystem.ResizeSystem.circlesSize[shape.Key] = newSizeCollision[shape.Key];
                    ecsManager.UpdateShapeSize(shape.Key, newSizeCollision[shape.Key].size);
                    ecsManager.UpdateShapePosition(shape.Key, shape.Value.position);
                }
            }
        }

        public void SplitCircle(Vector2 position, Vector2 speed) {

        }
    }
}