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
        public bool IsProtectionOver (uint id) {
            return Time.time > worldData.WorldData.circleProtectionStartTime[id].startProtectionTime + ecsManager.Config.protectionDuration;
        }
        public bool IsCooldownOver (uint id) {
            return Time.time > worldData.WorldData.circleProtectionStartTime[id].startProtectionTime + ecsManager.Config.protectionCooldown;
        }
        public void UpdateInput() 
        {
            if (Input.GetMouseButtonDown(0)){
                SystemDataUtility.handleClickEvent();
            }
        }
        public void UpdateSystem()
        {
                    	
            foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
            {
                if(worldData.WorldData.circlesProtection[shape.Key].isProtected && IsProtectionOver(shape.Key)){
                    //Debug.Log("Circle " + shape.Key + " protection end time " + Time.time);
                    worldData.WorldData.circlesProtection[shape.Key] = new ProtectedComponent { isProtected = false };
                }
                if(worldData.WorldData.circlesIsDynamic[shape.Key].isDynamic &&  worldData.WorldData.circlesSize[shape.Key].size <= ecsManager.Config.protectionSize){
                    var isProtected = ProtectionRandomProbability(ecsManager.Config.protectionProbability);
                    if(isProtected && worldData.WorldData.circlesProtection[shape.Key].isProtected == false)
                    {
                        if(!worldData.WorldData.circleProtectionStartTime.ContainsKey(shape.Key)){
                            worldData.WorldData.circlesProtection[shape.Key] = new ProtectedComponent { isProtected = true };
                            //Debug.Log("Circle " + shape.Key + " protection start time " + Time.time);
                            worldData.WorldData.circleProtectionStartTime.Add(shape.Key, new StartProtectionTimeComponent { startProtectionTime = Time.time });
                        }else if(IsCooldownOver(shape.Key)){
                            worldData.WorldData.circleProtectionStartTime.Remove(shape.Key);
                        }
                    }
                }

            }

        }
    }
}