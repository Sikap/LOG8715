using UnityEngine;
using MovementSystem = movementSystems;
using ResizeSystem = resizeSystems;

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

    public static uint AddShapeDataToSystems(int shapeSize, Vector2 shapePosition, Vector2 shapeSpeed) {
        id++;
        if (!ResizeSystem.ResizeSystem.circlesSize.ContainsKey(id))
        {
            ResizeSystem.ResizeSystem.circlesSize.Add(id, new SizeComponent { size = shapeSize });
        }
        if (!MovementSystem.MovementSystem.circlesPosition.ContainsKey(id))
        {
            MovementSystem.MovementSystem.circlesPosition.Add(id, new PositionComponent { position = shapePosition });
        }
        if (!MovementSystem.MovementSystem.circlesSpeed.ContainsKey(id))
        {
            MovementSystem.MovementSystem.circlesSpeed.Add(id, new SpeedComponent { speed = shapeSpeed });
        }
        return id;
    }

    public static void RemoveShapeDataFromSystems(uint id) {
        if (ResizeSystem.ResizeSystem.circlesSize.ContainsKey(id))
        {
            ResizeSystem.ResizeSystem.circlesSize.Remove(id);
        }
        if (MovementSystem.MovementSystem.circlesPosition.ContainsKey(id))
        {
            MovementSystem.MovementSystem.circlesPosition.Remove(id);
        }
        if (MovementSystem.MovementSystem.circlesSpeed.ContainsKey(id))
        {
            MovementSystem.MovementSystem.circlesSpeed.Remove(id);
        }
    }

}