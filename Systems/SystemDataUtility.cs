using UnityEngine;
using MovementSystem = movementSystems;
using ResizeSystem = resizeSystems;
using CollisionSystem = collisionSystems;

public class SystemDataUtility
{
    public static uint id = 1;

    public static uint GetReservedId() {
        return id++;
    }

    public static void CreateCircle(uint id, Vector2 position, int size) {
        global::ECSManager ecsManager = global::ECSManager.Instance;
        ecsManager.CreateShape(id, size);
        ecsManager.UpdateShapePosition(id, position);
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

}