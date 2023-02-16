using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rewindSystem 
{
    public class RewindSystem : ISystem
    {
        public string Name { get; private set; }
        public RewindSystem(string name)
        {
            Name = name;
        }
        public void UpdateInput() 
        {
            if (Input.GetKeyDown("space")) {
                if (Time.time - worldData.WorldData.rewindTimePressed >= 3f)
                {
                    Rewind();
                    worldData.WorldData.rewindTimePressed = Time.time;
                }
            }
        }

        public void UpdateSystem()
        {
            var rewind = new RewindComponent {
                id = SystemDataUtility.id,
                toCreateComponentList = new Dictionary<uint, CreationComponent>(worldData.WorldData.toCreate),
                toDestroyComponentList = new Dictionary<uint, DestroyComponent>(worldData.WorldData.toDestroy),
                toExplodeComponentList = new Dictionary<uint, ExplosionComponent>(worldData.WorldData.toExplode),
                protectionComponentList = new Dictionary<uint, ProtectedComponent>(worldData.WorldData.circlesProtection),
                startProtectionComponentList = new Dictionary<uint, StartProtectionTimeComponent>(worldData.WorldData.circleProtectionStartTime),
                positionComponentList = new Dictionary<uint, PositionComponent>(worldData.WorldData.circlesPosition),
                speedComponentList = new Dictionary<uint, SpeedComponent>(worldData.WorldData.circlesSpeed),
                dynamicComponentList = new Dictionary<uint, DynamicComponent>(worldData.WorldData.circlesIsDynamic),
                circleBorderBouncePositionList = new Dictionary<uint, PositionComponent>(worldData.WorldData.circleBorderBouncePosition),
                circleBorderBounceSpeedList = new Dictionary<uint, SpeedComponent>(worldData.WorldData.circleBorderBounceSpeed),
                collisionComponentList = new Dictionary<uint, CollisionComponent>(worldData.WorldData.circlesCollision),
                sizeComponentList = new Dictionary<uint, SizeComponent>(worldData.WorldData.circlesSize),
            };
            worldData.WorldData.rewind.Add(Time.time, rewind);
            RemoveOldSave();
        }

        public static void RemoveOldSave() {
            var toRemove = new List<float>();
            foreach (KeyValuePair<float, RewindComponent> rewindInst in worldData.WorldData.rewind)
            {     
                if (rewindInst.Key < Time.time - 3f)
                {
                    if (!toRemove.Contains(rewindInst.Key))
                    {
                        toRemove.Add(rewindInst.Key);
                    }
                }
            }
            foreach (var rewindOld in toRemove)
            {   
                if (worldData.WorldData.rewind.ContainsKey(rewindOld))
                {
                    worldData.WorldData.rewind.Remove(rewindOld);
                }
            }
        }

        public static void RemovePresentShapes() {
            foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
            {
                SystemDataUtility.DestroyShape(shape.Key);
            }
        }

        public static void PlaceShapes() {
            foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
            {
                if (SystemDataUtility.IsProcessable(shape.Key))
                {
                    SystemDataUtility.CreateCircle(shape.Key);
                }
            }
        }

        public static void Rewind() {
            var oldestKey = FindOldest();
            RemovePresentShapes();
            if (worldData.WorldData.rewind.ContainsKey(oldestKey))
            {
                SystemDataUtility.id = worldData.WorldData.rewind[oldestKey].id;
                worldData.WorldData.toCreate = worldData.WorldData.rewind[oldestKey].toCreateComponentList;
                worldData.WorldData.toDestroy = worldData.WorldData.rewind[oldestKey].toDestroyComponentList;
                worldData.WorldData.toExplode = worldData.WorldData.rewind[oldestKey].toExplodeComponentList;
                worldData.WorldData.circlesProtection = worldData.WorldData.rewind[oldestKey].protectionComponentList;
                worldData.WorldData.circleProtectionStartTime = worldData.WorldData.rewind[oldestKey].startProtectionComponentList;
                worldData.WorldData.circlesPosition = worldData.WorldData.rewind[oldestKey].positionComponentList;
                worldData.WorldData.circlesSpeed = worldData.WorldData.rewind[oldestKey].speedComponentList;
                worldData.WorldData.circlesIsDynamic = worldData.WorldData.rewind[oldestKey].dynamicComponentList;
                worldData.WorldData.circleBorderBouncePosition = worldData.WorldData.rewind[oldestKey].circleBorderBouncePositionList;
                worldData.WorldData.circleBorderBounceSpeed = worldData.WorldData.rewind[oldestKey].circleBorderBounceSpeedList;
                worldData.WorldData.circlesCollision = worldData.WorldData.rewind[oldestKey].collisionComponentList;
                worldData.WorldData.circlesSize = worldData.WorldData.rewind[oldestKey].sizeComponentList;
                worldData.WorldData.rewind.Clear();
            }
            PlaceShapes();
        }

        public static float FindOldest() {
            bool firstIteration = true;
            var oldest = 0f;
            foreach (KeyValuePair<float, RewindComponent> rewindInst in worldData.WorldData.rewind)
            {
                if (firstIteration)
                {
                    oldest = rewindInst.Key;
                    firstIteration = false;
                }
                if (oldest > rewindInst.Key)
                {
                   oldest = rewindInst.Key; 
                }
            }
            return oldest;
        }
    }
}