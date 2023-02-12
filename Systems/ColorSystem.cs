using System.Collections.Generic;
using System;
using UnityEngine;
using MovementSystem = movementSystems;
using CollisionSystem = collisionSystems;
using ResizeSystem = resizeSystems;

namespace colorSystems
{

    public class ColorSystem : ISystem
    {

        public string Name { get; private set; }
        public ColorSystem(string name)
        {
            Name = name;
        }

        public bool clickInCircle(Vector2 clickPosition, Vector2 circlePosition, float size) {
            return Vector2.Distance(clickPosition, circlePosition) < size;
        }
        public void UpdateSystem()        
        {            
            global::ECSManager ecsManager = global::ECSManager.Instance;
            foreach (KeyValuePair<uint, PositionComponent> shape in MovementSystem.MovementSystem.circlesPosition)
            {
                if(!MovementSystem.MovementSystem.circlesIsDynamic[shape.Key].isDynamic){
                    ecsManager.UpdateShapeColor(shape.Key, Color.red);
                } else {
                    Vector2 clickScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    Vector2 clickWorldPosition = Camera.main.ScreenToWorldPoint(clickScreenPosition);
                    var size = ResizeSystem.ResizeSystem.circlesSize[shape.Key].size;
                    if (Input.GetMouseButtonDown(0) && clickInCircle(clickWorldPosition,shape.Value.position,size)){
                        ecsManager.UpdateShapeColor(shape.Key, new Color(255f/255f, 192f/255f, 203f/255f));
                    }
                    if (CollisionSystem.CollisionSystem.circlesCollision[shape.Key].isCollision){
                        ecsManager.UpdateShapeColor(shape.Key, Color.green);
                    } else {
                        if (size == 9){
                            ecsManager.UpdateShapeColor(shape.Key, new Color(1.0f, 0.64f, 0.0f));
                        } else {
                            ecsManager.UpdateShapeColor(shape.Key, Color.blue);
                        }
                    }

                }
            }                        
        }
    }
}