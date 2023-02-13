using System.Collections;
using System.Collections.Generic;
using System;
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
            float radius = diameter / 2f;
            var offsetRigthX = mainCamera.WorldToScreenPoint(new Vector2(center.x + radius, center.y)).x;
            var offsetTopY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y + radius)).y;
            var offsetLeftX = mainCamera.WorldToScreenPoint(new Vector2(center.x - radius, center.y)).x;
            var offsetBottomY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y - radius)).y;
            if ( offsetLeftX < 0 || offsetRigthX > Screen.width || offsetBottomY < 0 || offsetTopY > Screen.height)
            {            
                //Debug.Log("Screen height is " + Screen.height);
                //Debug.Log("hitPoint is " + offsetTopY);
                return true;
            }
            return false;
        }  
        public static Vector2 BounceBackPosition(Vector2 center, int diameter)
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(center);
            float radius = diameter / 2f;
            var offsetRigthX = mainCamera.WorldToScreenPoint(new Vector2(center.x + radius, center.y)).x;
            var offsetTopY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y + radius)).y;
            var offsetLeftX = mainCamera.WorldToScreenPoint(new Vector2(center.x - radius, center.y)).x;
            var offsetBottomY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y - radius)).y;
            if (offsetRigthX > Screen.width)
            {
                center.x = mainCamera.ScreenToWorldPoint(new Vector2(screenPoint.x - (offsetRigthX - Screen.width), screenPoint.y)).x;
            }
            else if (offsetLeftX < 0)
            {
                center.x = mainCamera.ScreenToWorldPoint(new Vector2(screenPoint.x + Math.Abs(offsetLeftX), screenPoint.y)).x;
            }

            if (offsetTopY > Screen.height)
            {
                center.y = mainCamera.ScreenToWorldPoint(new Vector2(screenPoint.x, screenPoint.y - (offsetTopY - Screen.height))).y;
            }
            else if (offsetBottomY < 0)
            {
                center.y = mainCamera.ScreenToWorldPoint(new Vector2(screenPoint.x, screenPoint.y +  Math.Abs(offsetBottomY))).y;
            }

            return center;
        }

        public Vector2 BounceBackSpeed(Vector2 center, int diameter, Vector2 velocity)
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(center);
            float radius = diameter / 2f;
            var offsetRigthX = mainCamera.WorldToScreenPoint(new Vector2(center.x + radius, center.y)).x;
            var offsetTopY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y + radius)).y;
            var offsetLeftX = mainCamera.WorldToScreenPoint(new Vector2(center.x - radius, center.y)).x;
            var offsetBottomY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y - radius)).y;

            if (offsetRigthX > Screen.width ||offsetLeftX < 0)
            {
                velocity.x = -velocity.x;
            }

            if (offsetTopY > Screen.height || offsetBottomY < 0)
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
                Vector2 shapePosition = new Vector2(shape.Value.position.x , shape.Value.position.y);
                
                if(IsCircleOffScreen(shapePosition, size)){ 
                    Debug.Log("Circle hit camera bounds");
                    newPosition[shape.Key] = new PositionComponent { position = BounceBackPosition(shape.Value.position,size) };
                    newSpeed[shape.Key] = new SpeedComponent { speed = BounceBackSpeed(shape.Value.position,size,speed) };
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in newPosition)
            {
                //Debug.Log("Position was "+ MovementSystem.MovementSystem.circlesPosition[shape.Key].position );
                //Debug.Log("Speed was "+ MovementSystem.MovementSystem.circlesSpeed[shape.Key].speed );
                // Debug.Log("newPostion is "+ shape.Value.position );
                //Debug.Log("newSpeed is "+ newSpeed[shape.Key].speed );

                MovementSystem.MovementSystem.circlesPosition[shape.Key] = shape.Value;
                MovementSystem.MovementSystem.circlesSpeed[shape.Key] =  newSpeed[shape.Key];
            }

        }
    }
}