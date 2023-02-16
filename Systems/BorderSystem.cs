using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace borderSystem
{

    public class BorderSystem : ISystem
    {
        public static Camera mainCamera;

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
            worldData.WorldData.circleBorderBouncePosition.Clear();
            worldData.WorldData.circleBorderBounceSpeed.Clear();
        }
        public void UpdateInput() 
        {
        }
        public void UpdateSystem()
        {
            ClearDictionnaries();
            foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
            {            
                var size = worldData.WorldData.circlesSize[shape.Key].size;
                var speed =worldData.WorldData.circlesSpeed[shape.Key].speed;
                Vector2 shapePosition = new Vector2(shape.Value.position.x , shape.Value.position.y);
                
                if(IsCircleOffScreen(shapePosition, size)){ 
                    //Debug.Log("Circle hit camera bounds");
                    worldData.WorldData.circleBorderBouncePosition[shape.Key] = new PositionComponent { position = BounceBackPosition(shape.Value.position,size) };
                    worldData.WorldData.circleBorderBounceSpeed[shape.Key] = new SpeedComponent { speed = BounceBackSpeed(shape.Value.position,size,speed) };
                }
            }
            foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circleBorderBouncePosition)
            {
                //Debug.Log("Position was "+ MovementSystem.MovementSystem.circlesPosition[shape.Key].position );
                //Debug.Log("Speed was "+ MovementSystem.MovementSystem.circlesSpeed[shape.Key].speed );
                // Debug.Log("newPostion is "+ shape.Value.position );
                //Debug.Log("circleBorderBounceSpeed is "+ circleBorderBounceSpeed[shape.Key].speed );

                worldData.WorldData.circlesPosition[shape.Key] = shape.Value;
                worldData.WorldData.circlesSpeed[shape.Key] =  worldData.WorldData.circleBorderBounceSpeed[shape.Key];
            }

        }
    }
}