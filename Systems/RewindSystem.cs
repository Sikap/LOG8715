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
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
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
                // Debug.Log("REMOVING");
                // Debug.Log(worldData.WorldData.rewind.Count);
                // Debug.Log(rewindOld);
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
            Debug.Log("REWIND");
            Debug.Log(SystemDataUtility.id);
            var oldestKey = FindOldest();
            RemovePresentShapes();
            if (worldData.WorldData.rewind.ContainsKey(oldestKey))
            {
                SystemDataUtility.id = worldData.WorldData.rewind[oldestKey].id;
                worldData.WorldData.toCreate = new Dictionary<uint, CreationComponent>(worldData.WorldData.rewind[oldestKey].toCreateComponentList);
                worldData.WorldData.toDestroy = new Dictionary<uint, DestroyComponent>(worldData.WorldData.rewind[oldestKey].toDestroyComponentList);
                worldData.WorldData.toExplode = new Dictionary<uint, ExplosionComponent>(worldData.WorldData.rewind[oldestKey].toExplodeComponentList);
                worldData.WorldData.circlesProtection = new Dictionary<uint, ProtectedComponent>(worldData.WorldData.rewind[oldestKey].protectionComponentList);
                worldData.WorldData.circleProtectionStartTime = new Dictionary<uint, StartProtectionTimeComponent>(worldData.WorldData.rewind[oldestKey].startProtectionComponentList);
                worldData.WorldData.circlesPosition = new Dictionary<uint, PositionComponent>(worldData.WorldData.rewind[oldestKey].positionComponentList);
                worldData.WorldData.circlesSpeed = new Dictionary<uint, SpeedComponent>(worldData.WorldData.rewind[oldestKey].speedComponentList);
                worldData.WorldData.circlesIsDynamic = new Dictionary<uint, DynamicComponent>(worldData.WorldData.rewind[oldestKey].dynamicComponentList);
                worldData.WorldData.circleBorderBouncePosition = new Dictionary<uint, PositionComponent>(worldData.WorldData.rewind[oldestKey].circleBorderBouncePositionList);
                worldData.WorldData.circleBorderBounceSpeed = new Dictionary<uint, SpeedComponent>(worldData.WorldData.rewind[oldestKey].circleBorderBounceSpeedList);
                worldData.WorldData.circlesCollision = new Dictionary<uint, CollisionComponent>(worldData.WorldData.rewind[oldestKey].collisionComponentList);
                worldData.WorldData.circlesSize = new Dictionary<uint, SizeComponent>(worldData.WorldData.rewind[oldestKey].sizeComponentList);
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

        public static float FindNewest() {
            bool firstIteration = true;
            float newest = 0f;
            foreach (KeyValuePair<float, RewindComponent> rewindInst in worldData.WorldData.rewind)
            {
                if (firstIteration)
                {
                    newest = rewindInst.Key;
                    firstIteration = false;
                }
                if (newest < rewindInst.Key)
                {
                   newest = rewindInst.Key; 
                }
            }
            return newest;
        }
    }
}