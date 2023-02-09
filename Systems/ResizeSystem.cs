using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace resizeSystems
{
    public class ResizeSystem : ISystem
    {
        public static Dictionary<uint, SizeComponent> circlesSize = new Dictionary<uint,SizeComponent>();

        public string Name { get; private set; }
        public ResizeSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            // processing logic here
        }
    }
}