using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProtectionSystems
{

    public class ProtectionSystem : ISystem
    {
        Config config;
        public string Name { get; private set; }
        public ProtectionSystem(string name)
        {
            Name = name;
        }
        public bool ProtectionRandomProbability (float probability) {
            return UnityEngine.Random.value < probability;
        }
        public void UpdateSystem()
        {
            // processing logic here
           /* global::ECSManager ecsManager = global::ECSManager.Instance;
            foreach (KeyValuePair<uint, PositionComponent> shape in MovementSystem.MovementSystem.circlesPosition)
            {
                var isProtected = ProtectionRandomProbability(0.01f);
                if(isProtected && !circlesProtection.ContainsKey(shape.Key)){
                    circlesProtection.Add(shape.Key, new DynamicComponent { isProtected = true });
                }
            
            }*/

        }
    }
}