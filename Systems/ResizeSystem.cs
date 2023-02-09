using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResizeSystems
{

    public class ResizeSystem : ISystem
    {
        public Dictionary<uint, SizeComponent> circlesSize;

        public string Name { get; private set; }
        public ResizeSystem(string name)
        {
            Name = name;
            circlesSize = new Dictionary<uint, SizeComponent>();
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}