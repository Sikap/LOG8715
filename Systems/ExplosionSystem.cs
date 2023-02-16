using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace explosionSystem 
{

    public class ExplosionSystem : ISystem
    {

        public string Name { get; private set; }
        public ExplosionSystem(string name)
        {
            Name = name;
        }
        public void UpdateInput() 
        {
        }

        public void UpdateSystem()
        {
            foreach (KeyValuePair<uint, ExplosionComponent> shape in worldData.WorldData.toExplode)
            {
                var newSpeedCollision = worldData.WorldData.circlesSpeed[shape.Key].speed;
                var newPositionCollision = worldData.WorldData.circlesPosition[shape.Key].position;
                var newSizeCollision = worldData.WorldData.circlesSize[shape.Key].size;

                var zeroRadVector = new Vector2(1,0);
                var motionAngle = Vector2.Angle(zeroRadVector, newSpeedCollision) * Mathf.Deg2Rad;

                var smallerCircleOnePositionX = newPositionCollision.x + ((newSizeCollision/4) * Mathf.Cos(motionAngle));
                var smallerCircleOnePositionY = newPositionCollision.y + ((newSizeCollision/4) * Mathf.Sin(motionAngle));

                var smallerCircleOnePosition = new Vector2(smallerCircleOnePositionX, smallerCircleOnePositionY);
                var smallerCircleOneSize = newSizeCollision/2;
                var smallerCircleOneSpeed = newSpeedCollision;
                
                var smallerCircleTwoPositionX = newPositionCollision.x + ((newSizeCollision/4) * Mathf.Cos(motionAngle + Mathf.PI));
                var smallerCircleTwoPositionY = newPositionCollision.y + ((newSizeCollision/4) * Mathf.Sin(motionAngle + Mathf.PI));                 

                var smallerCircleTwoPosition = new Vector2(smallerCircleTwoPositionX, smallerCircleTwoPositionY);
                var smallerCircleTwoSize = newSizeCollision/2;
                var smallerCircleTwoSpeed = newSpeedCollision * -1;

                var idOne = SystemDataUtility.AddShapeDataToSystems(smallerCircleOneSize, smallerCircleOnePosition, smallerCircleOneSpeed, true, false);
                var idTwo = SystemDataUtility.AddShapeDataToSystems(smallerCircleTwoSize, smallerCircleTwoPosition, smallerCircleTwoSpeed, true, false);
                worldData.WorldData.toCreate.Add(idOne, new CreationComponent{ toCreate = true });
                worldData.WorldData.toCreate.Add(idTwo, new CreationComponent{ toCreate = true });
            }
            worldData.WorldData.toExplode.Clear();
        }
    }
}