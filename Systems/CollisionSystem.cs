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
            var newSizeCollision = new Dictionary<uint, SizeComponent>(); 
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
                                    if(!worldData.WorldData.circlesProtection[shapeOne.Key].isProtected){
                                        if(!worldData.WorldData.circlesProtection[shapeTwo.Key].isProtected){
                                            shapeOneSize++;
                                        }else{
                                            shapeOneSize--;
                                        }
                                    }
                                    if(!worldData.WorldData.circlesProtection[shapeTwo.Key].isProtected){
                                        if(!worldData.WorldData.circlesProtection[shapeOne.Key].isProtected){
                                            shapeTwoSize--;
                                        }
                                    }
                                } else if (shapeOneSize < shapeTwoSize)
                                {
                                    if(!worldData.WorldData.circlesProtection[shapeOne.Key].isProtected){
                                        if(!worldData.WorldData.circlesProtection[shapeTwo.Key].isProtected){
                                            shapeOneSize--;
                                        }
                                    }
                                    if(!worldData.WorldData.circlesProtection[shapeTwo.Key].isProtected){
                                        if(!worldData.WorldData.circlesProtection[shapeOne.Key].isProtected){
                                            shapeTwoSize++;
                                        }else{
                                            shapeTwoSize--;
                                        }
                                    }
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
                            worldData.WorldData.circlesCollision[shapeOne.Key] = new CollisionComponent { isCollision = true };
                            newPositionCollision[shapeTwo.Key] = new PositionComponent { position = newPosition2 };
                            newSpeedCollision[shapeTwo.Key] = new SpeedComponent { speed = newSpeed2 };
                            newSizeCollision[shapeTwo.Key] = new SizeComponent { size = shapeTwoSize };
                            worldData.WorldData.circlesCollision[shapeTwo.Key] = new CollisionComponent { isCollision = true };
                        }else {
                            worldData.WorldData.circlesCollision[shapeOne.Key] = new CollisionComponent { isCollision = false };
                            worldData.WorldData.circlesCollision[shapeTwo.Key] = new CollisionComponent { isCollision = false };
                        }
                    }
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in newPositionCollision)
            {
                if(newSizeCollision.ContainsKey(shape.Key) && newSizeCollision[shape.Key].size <= 0) {
                    worldData.WorldData.toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };
                } else if (newSizeCollision.ContainsKey(shape.Key) && newSizeCollision[shape.Key].size >= 10) {
                    worldData.WorldData.toDestroy[shape.Key] = new DestroyComponent{ toDestroy = true };                    
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

                    var idOne = SystemDataUtility.AddShapeDataToSystems(smallerCircleOneSize, smallerCircleOnePosition, smallerCircleOneSpeed, true, false);
                    var idTwo = SystemDataUtility.AddShapeDataToSystems(smallerCircleTwoSize, smallerCircleTwoPosition, smallerCircleTwoSpeed, true, false);
                    worldData.WorldData.toCreate.Add(idOne, new CreationComponent{ toCreate = true });
                    worldData.WorldData.toCreate.Add(idTwo, new CreationComponent{ toCreate = true });
                } else {
                    worldData.WorldData.circlesPosition[shape.Key] = shape.Value;
                    worldData.WorldData.circlesSpeed[shape.Key] = newSpeedCollision[shape.Key];
                    worldData.WorldData.circlesSize[shape.Key] = newSizeCollision[shape.Key];
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
        }
    }
}