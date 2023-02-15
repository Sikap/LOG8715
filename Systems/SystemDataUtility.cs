using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SystemDataUtility
{
    public static uint id = 1;

    public static uint GetReservedId() {
        return id++;
    }

    //------------------------------------------------//
    // Abstraction of ECSmanager public functions     //
    // to allow verifications and other modifications //
    //------------------------------------------------//

    public static void CreateCircle(uint id) {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        if (worldData.WorldData.circlesSize.ContainsKey(id) && worldData.WorldData.circlesPosition.ContainsKey(id))
        {
            var size = worldData.WorldData.circlesSize[id].size;
            var position = worldData.WorldData.circlesPosition[id].position;
            ecsManager.CreateShape(id, size);
            ecsManager.UpdateShapePosition(id, position);
        }
    }

    public static void DestroyShape(uint id) {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        ecsManager.DestroyShape(id);
    }

    public static void UpdateShape(uint id, Vector2 position, int size) {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        ecsManager.UpdateShapeSize(id, size);
        ecsManager.UpdateShapePosition(id, position);
    }

    public static void UpdateShapeSize(uint id, int size) {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        ecsManager.UpdateShapeSize(id, size);
    }

    public static void UpdateShapePosition(uint id, Vector2 position) {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        ecsManager.UpdateShapePosition(id, position);
    }

    //----------------------------------------------------//
    // Simplify data manipulation for adding and removing //
    // entities from the different dictionnaries          //
    //----------------------------------------------------//

    public static uint AddShapeDataToSystems(int shapeSize, Vector2 shapePosition, Vector2 shapeSpeed, bool shapeDynamic, bool shapeProtection) {
        id++;
        if (!worldData.WorldData.circlesSize.ContainsKey(id))
        {
            worldData.WorldData.circlesSize.Add(id, new SizeComponent { size = shapeSize });
        }
        if (!worldData.WorldData.circlesPosition.ContainsKey(id))
        {
            worldData.WorldData.circlesPosition.Add(id, new PositionComponent { position = shapePosition });
        }
        if (!worldData.WorldData.circlesSpeed.ContainsKey(id))
        {
            worldData.WorldData.circlesSpeed.Add(id, new SpeedComponent { speed = shapeSpeed });
        }
        if (!worldData.WorldData.circlesIsDynamic.ContainsKey(id))
        {
            worldData.WorldData.circlesIsDynamic.Add(id, new DynamicComponent { isDynamic = shapeDynamic });
        }
        if (!worldData.WorldData.circlesProtection.ContainsKey(id))
        {
            worldData.WorldData.circlesProtection.Add(id, new ProtectedComponent { isProtected = shapeProtection });
            if(shapeProtection && !worldData.WorldData.circleProtectionStartTime.ContainsKey(id) ){
                worldData.WorldData.circleProtectionStartTime.Add(id, new StartProtectionTimeComponent { startProtectionTime = Time.time });
            }
        }
        return id;
    }

    public static void RemoveShapeDataFromSystems(uint id) {
        if (worldData.WorldData.circlesSize.ContainsKey(id))
        {
            worldData.WorldData.circlesSize.Remove(id);
        }
        if (worldData.WorldData.circlesPosition.ContainsKey(id))
        {
            worldData.WorldData.circlesPosition.Remove(id);
        }
        if (worldData.WorldData.circlesSpeed.ContainsKey(id))
        {
            worldData.WorldData.circlesSpeed.Remove(id);
        }
        if (worldData.WorldData.circlesIsDynamic.ContainsKey(id))
        {
            worldData.WorldData.circlesIsDynamic.Remove(id);
        }
        if (worldData.WorldData.circlesProtection.ContainsKey(id))
        {
            worldData.WorldData.circlesProtection.Remove(id);
        }
    }

    public static bool IsProcessable(uint id) {
        if (!worldData.WorldData.toCreate.ContainsKey(id) && !worldData.WorldData.toDestroy.ContainsKey(id)) {
            return true;
        }
        return false;
    }

    public static bool RandomProbability (float probability) {
        return UnityEngine.Random.value < probability;
    }

    public static bool clickInCircle(Vector2 clickPosition, Vector2 circlePosition, float size) {
        return Vector2.Distance(clickPosition, circlePosition) < size;
    }

    public static void handleClickEvent() {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition){
            Vector2 clickScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 clickWorldPosition = Camera.main.ScreenToWorldPoint(clickScreenPosition);
            var size = worldData.WorldData.circlesSize[shape.Key].size/2.0f;
            if (worldData.WorldData.circlesIsDynamic[shape.Key].isDynamic && clickInCircle(clickWorldPosition,shape.Value.position,size)){
                ecsManager.UpdateShapeColor(shape.Key, new Color(1.0f, 0.41f, 0.71f));
                /*if(size>=2){

                }else{

                }*/
            }
        }
    }
}