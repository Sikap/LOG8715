using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace resizeSystem
{
    public class ResizeSystem : ISystem
    {
        public string Name { get; private set; }
        public ResizeSystem(string name)
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
            var alreadyProcessed = new Dictionary<uint, bool>();
            foreach (KeyValuePair<uint, CollisionComponent> shape in worldData.WorldData.circlesCollision)
            {
                if (!alreadyProcessed.ContainsKey(shape.Key))
                {
                    var idMain = shape.Value.circleMain;
                    var idColliding = shape.Value.circleColliding;
                    var shapeOneIsDynamic = worldData.WorldData.circlesIsDynamic[idMain].isDynamic;
                    var shapeTwoIsDynamic = worldData.WorldData.circlesIsDynamic[idColliding].isDynamic;
                    var shapeOneSize = worldData.WorldData.circlesSize[idMain].size;
                    var shapeTwoSize = worldData.WorldData.circlesSize[idColliding].size;
                    if (SystemDataUtility.IsProcessable(shape.Key) && worldData.WorldData.circlesSize.ContainsKey(shape.Key))
                    {
                        if(shapeOneIsDynamic && shapeTwoIsDynamic){
                            if (shapeOneSize > shapeTwoSize)
                            {
                                if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                        shapeOneSize++;
                                    }else{
                                        shapeOneSize--;
                                    }
                                }
                                if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                        shapeTwoSize--;
                                    }
                                }
                            } else if (shapeOneSize < shapeTwoSize)
                            {
                                if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                        shapeOneSize--;
                                    }
                                }
                                if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                        shapeTwoSize++;
                                    }else{
                                        shapeTwoSize--;
                                    }
                                }
                            }
                        }
                        ResizeCircle(idMain, shapeOneSize);
                        ResizeCircle(idColliding, shapeTwoSize);
                        if (!alreadyProcessed.ContainsKey(idMain))
                        {
                            alreadyProcessed.Add(idMain, true);
                        }
                        if (!alreadyProcessed.ContainsKey(idColliding))
                        {
                            alreadyProcessed.Add(idColliding, true);
                        }
                    }
                }
            }
        }

        public static void ResizeCircle(uint id, int size) {
            global::ECSManager ecsManager = global::ECSManager.Instance;
            if (size <= 0) {
                if (!worldData.WorldData.toDestroy.ContainsKey(id)) {
                    worldData.WorldData.toDestroy.Add(id, new DestroyComponent{ toDestroy = true });
                }
            } else if (size >= ecsManager.Config.explosionSize) {
                if (!worldData.WorldData.toDestroy.ContainsKey(id)) {
                    worldData.WorldData.toDestroy.Add(id, new DestroyComponent{ toDestroy = true });
                    worldData.WorldData.toExplode.Add(id, new ExplosionComponent{ isExploding = true });
                }
            } else {
                if (worldData.WorldData.circlesSize.ContainsKey(id)) {
                    worldData.WorldData.circlesSize[id] = new SizeComponent { size = size };
                    SystemDataUtility.UpdateShapeSize(id, size);
                }
            }
        }
    }
}