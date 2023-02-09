using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace circlesSystem
{
    public class SpawnCirclesSystem : ISystem
    {
        private uint id = 1;
        public string Name { get; private set; }
       
        public SpawnCirclesSystem(string name)
        {
            Name = name;
            UnityEngine.Random.InitState(1);
            global::ECSManager ecsManager = global::ECSManager.Instance;
            for (int i = 0; i < 3;i++)
            {
                ecsManager.CreateShape(id, UnityEngine.Random.Range(0, 10));
                ecsManager.UpdateShapePosition(id, new Vector2(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f)));
                id++;
            }
        }

        public void UpdateSystem()
        {
            // processing logic here
            
        }
    }
}