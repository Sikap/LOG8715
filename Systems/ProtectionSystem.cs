using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProtectionSystems
{

    public class ProtectionSystem : ISystem
    {
        global::ECSManager ecsManager = global::ECSManager.Instance;                        
        public string Name { get; private set; }
        public ProtectionSystem(string name)
        {
            Name = name;
        }
        public bool ProtectionRandomProbability (float probability) {
            return UnityEngine.Random.value < probability;
        }
        public bool CooldownIsOver (uint id,float curentTime) {
            return worldData.WorldData.circleProtectionStartTime[id].startProtectionTime + ecsManager.Config.protectionCooldown > curentTime;
        }
        public void UpdateInput() 
        {
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
        }
        public void UpdateSystem()
        {
            // processing logic here
           /* global::ECSManager ecsManager = global::ECSManager.Instance;                        
            float curentTime = Time.time;

            foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
            {
                if(worldData.WorldData.circlesProtection.ContainsKey(shape.Key) && worldData.WorldData.circlesProtection[shape.Key].isProtected){
                    if(worldData.WorldData.circleProtectionStartTime[shape.Key].startProtectionTime + ecsManager.Config.protectionDuration > curentTime){
                        worldData.WorldData.circlesProtection[shape.Key] = new ProtectedComponent { isProtected = false };
                        worldData.WorldData.circleProtectionStartTime[shape.Key] = new StartProtectionTimeComponent { startProtectionTime = 0 };
                    }
                }
                if(worldData.WorldData.circlesIsDynamic[shape.Key].isDynamic &&  worldData.WorldData.circlesSize[shape.Key].size <= ecsManager.Config.protectionSize){
                    var isProtected = ProtectionRandomProbability(ecsManager.Config.protectionProbability);
                    if(isProtected && worldData.WorldData.circlesProtection[shape.Key].isProtected == false && CooldownIsOver(shape.Key,curentTime)){
                        worldData.WorldData.circlesProtection.Add(shape.Key, new ProtectedComponent { isProtected = true });
                        worldData.WorldData.circleProtectionStartTime.Add(shape.Key, new StartProtectionTimeComponent { startProtectionTime = curentTime });
                    }
                }

            }*/

        }
    }
}