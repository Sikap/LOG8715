using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace clickSystem
{
    public class ClickSystem : ISystem
    {
        public string Name { get; private set; }
        public ClickSystem(string name)
        {
            Name = name;
        }
        public void UpdateInput() 
        {
            if (Input.GetMouseButtonDown(0)){
                HandleClickEvent();
            }
        }

        public void UpdateSystem()
        {
            worldData.WorldData.circlesClicked.Clear();
        }

        public static void HandleClickEvent() {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition){
            Vector2 clickScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 clickWorldPosition = Camera.main.ScreenToWorldPoint(clickScreenPosition);
            var size = worldData.WorldData.circlesSize[shape.Key].size/2.0f;
            var isDynamic = worldData.WorldData.circlesIsDynamic[shape.Key].isDynamic;
            var clickedInCircle = SystemDataUtility.clickInCircle(clickWorldPosition,shape.Value.position,size);
            var deltaTimeSufficient = ((Time.time - worldData.WorldData.timeClick) > 0.1f);
            
            if (isDynamic && clickedInCircle && deltaTimeSufficient){
                if (!worldData.WorldData.circlesClicked.ContainsKey(shape.Key))
                { 
                    worldData.WorldData.circlesClicked.Add(shape.Key, new ClickComponent{ timeColored = Time.time});
                }
                ecsManager.UpdateShapeColor(shape.Key, new Color(1.0f, 0.41f, 0.71f));
                if (worldData.WorldData.circlesSize[shape.Key].size >= 2){
                    if (!worldData.WorldData.toExplode.ContainsKey(shape.Key))
                    { 
                        worldData.WorldData.toExplode.Add(shape.Key, new ExplosionComponent{ isExploding = true });
                    }
                } else if (worldData.WorldData.circlesSize[shape.Key].size < 2) {
                    if (!worldData.WorldData.toExplode.ContainsKey(shape.Key))
                    {
                        worldData.WorldData.toDestroy.Add(shape.Key, new DestroyComponent{ toDestroy = true });
                    }
                }
            }
        }
    }
    }
}