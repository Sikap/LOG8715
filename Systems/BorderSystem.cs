using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementSystem = movementSystems;
using ResizeSystem = resizeSystems;

namespace borderSystem
{

    public class BorderSystem : ISystem
    {
        public static Camera mainCamera;
        public static Dictionary<uint, PositionComponent> newPosition = new Dictionary<uint, PositionComponent>();
        public static Dictionary<uint, SpeedComponent> newSpeed = new Dictionary<uint, SpeedComponent>();

        public string Name { get; private set; }
        public BorderSystem(string name)
        {
            mainCamera = Camera.main;
            Name = name;
        }
        
        public bool IsCircleOffScreen(Vector2 center, int diameter)
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(center);
            float radius = diameter / 2f;
            if (screenPoint.x - radius < 0 || screenPoint.x + radius > Screen.width || screenPoint.y - radius < 0 || screenPoint.y + radius > Screen.height)
            {
                return true;
            }
            return false;
        }
        public static Vector2 BounceBackPosition(Vector2 center, int diameter)
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(center);
            float radius = diameter / 2f;

            if (screenPoint.x + radius > Screen.width)
            {
                center.x = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width - radius, screenPoint.y)).x;
            }
            else if (screenPoint.x - radius < 0)
            {
                center.x = mainCamera.ScreenToWorldPoint(new Vector2(radius, screenPoint.y)).x;
            }

            if (screenPoint.y + radius > Screen.height)
            {
                center.y = mainCamera.ScreenToWorldPoint(new Vector2(screenPoint.x, Screen.height - radius)).y;
            }
            else if (screenPoint.y - radius < 0)
            {
                center.y = mainCamera.ScreenToWorldPoint(new Vector2(screenPoint.x, radius)).y;
            }

            return center;
        }

        public Vector2 BounceBackSpeed(Vector2 center, int diameter, Vector2 velocity)
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(center);
            float radius = diameter / 2f;

            if (screenPoint.x + radius > Screen.width || screenPoint.x - radius < 0)
            {
                velocity.x = -velocity.x;
            }

            if (screenPoint.y + radius > Screen.height || screenPoint.y - radius < 0)
            {
                velocity.y = -velocity.y;
            }

            return velocity;
        }
        public void ClearDictionnaries() {
            newPosition.Clear();
            newSpeed.Clear();
        }
        public void UpdateSystem()
        {
            ClearDictionnaries();
            global::ECSManager ecsManager = global::ECSManager.Instance;
            foreach (KeyValuePair<uint, PositionComponent> shape in MovementSystem.MovementSystem.circlesPosition)
            {
                var size = ResizeSystem.ResizeSystem.circlesSize[shape.Key].size;
                var speed = MovementSystem.MovementSystem.circlesSpeed[shape.Key].speed;
                if(MovementSystem.MovementSystem.circlesIsDynamic[shape.Key].isDynamic && IsCircleOffScreen(shape.Value.position, size)){                         
                    newPosition[shape.Key] = new PositionComponent { position = BounceBackPosition(shape.Value.position,size) };
                    newSpeed[shape.Key] = new SpeedComponent { speed = BounceBackSpeed(shape.Value.position,size,speed) };
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in newPosition)
            {
                MovementSystem.MovementSystem.circlesPosition[shape.Key] = shape.Value;
                MovementSystem.MovementSystem.circlesSpeed[shape.Key] =  newSpeed[shape.Key];
            }

        }
    }
}