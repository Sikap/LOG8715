using System.Collections.Generic;
using System;

namespace ColorSystems
{

    public class ColorSystem : ISystem
    {

        public string Name { get; private set; }
        public ColorSystem(string name)
        {
            Name = name;
        }

        public void UpdateSystem()
        {
            // processing logic here

        }
    }
}