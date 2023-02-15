using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace destroySystem
{

    public class DestroySystem : ISystem
    {

        public string Name { get; private set; }
        public DestroySystem(string name)
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
            foreach (KeyValuePair<uint,DestroyComponent> shape in worldData.WorldData.toDestroy)
            {            
                SystemDataUtility.RemoveShapeDataFromSystems(shape.Key);
                SystemDataUtility.DestroyShape(shape.Key);
            }
            worldData.WorldData.toDestroy.Clear();
        }
    }
}