using System.Collections.Generic;
using System;
using UnityEngine;

namespace colorSystems
{

    public class ColorSystem : ISystem
    {
        global::ECSManager ecsManager = global::ECSManager.Instance;                        

        public string Name { get; private set; }
        public ColorSystem(string name)
        {
            Name = name;
        }

        public bool clickInCircle(Vector2 clickPosition, Vector2 circlePosition, float size) {
            return Vector2.Distance(clickPosition, circlePosition) < size;
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
                var size = worldData.WorldData.circlesSize[shape.Key].size;
                if(!worldData.WorldData.circlesIsDynamic[shape.Key].isDynamic){
                    ecsManager.UpdateShapeColor(shape.Key, Color.red);
                } else {
                    if(worldData.WorldData.circlesCollision.ContainsKey(shape.Key) && worldData.WorldData.circlesCollision[shape.Key].isCollision){
                        ecsManager.UpdateShapeColor(shape.Key, Color.green);
                    } else {
                        if (size == (ecsManager.Config.explosionSize - 1)){
                            ecsManager.UpdateShapeColor(shape.Key, new Color(0xEE / 255f, 0x76 / 255f, 0x00 / 255f));
                        } else {
                            if(worldData.WorldData.circlesProtection[shape.Key].isProtected){
                                ecsManager.UpdateShapeColor(shape.Key, Color.yellow);
                            }else{
                                ecsManager.UpdateShapeColor(shape.Key, Color.blue);
                            }
                        }
                        
                    }
                }
            }                        
        }
    }
}