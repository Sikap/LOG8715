using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProtectionSystems
{

    public class ProtectionSystem : ISystem
    {
        public Dictionary<uint, ProtectedComponent> circlesProtection;

        public string Name { get; private set; }
        public ProtectionSystem(string name)
        {
            Name = name;
            circlesProtection = new Dictionary<uint, ProtectedComponent>();
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}