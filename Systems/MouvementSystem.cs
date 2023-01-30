using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mouvementSystem
{

    public class MouvementSystem : ISystem
    {
        private List<PositionComponent> components;

        public string Name { get; private set; }
        public MouvementSystem(string name)
        {
            Name = name;
            components = new List<PositionComponent>();
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}