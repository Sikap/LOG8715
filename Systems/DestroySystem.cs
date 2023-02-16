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
            worldData.WorldData.toDestroy.Clear();
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
                SystemDataUtility.DestroyShape(shape.Key);
                SystemDataUtility.RemoveShapeDataFromSystems(shape.Key);
            }
            worldData.WorldData.toDestroy.Clear();
        }
    }
}