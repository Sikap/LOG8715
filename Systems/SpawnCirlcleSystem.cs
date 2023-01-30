using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace circlesSystem
{
    public class SpawnCirclesSystem : ISystem
    {
        private List<CircleComponent> circlecomponents = new List<CircleComponent>();

        public string Name { get; private set; }
       
        public SpawnCirclesSystem(string name)
        {
            Name = name;
            CircleComponent circle1 = new CircleComponent
            {
                id = 1,
                position = new Vector2(10, 0),
                velocity = new Vector2(0, 0),
                size = 3,
                isDynamic = false,
                isProtected = false,
                isCollision = false,
                color = Color.red
            };
            CircleComponent circle2 = new CircleComponent
            {
                id = 2,
                position = new Vector2(-10, 0),
                velocity = new Vector2(0, 0),
                size = 5,
                isDynamic = false,
                isProtected = false,
                isCollision = false,
                color = Color.green
            };
            CircleComponent circle3 = new CircleComponent
            {
                id = 3,
                position = new Vector2(0, 0),
                velocity = new Vector2(1, 0),
                size = 2,
                isDynamic = false,
                isProtected = false,
                isCollision = false,
                color = Color.blue
            };
           
            circlecomponents.AddRange(new CircleComponent[] { circle1, circle2, circle3 });       
            global::ECSManager ecsManager = global::ECSManager.Instance;

            foreach (CircleComponent circle in circlecomponents)
            {
                ecsManager.CreateShape(circle.id, circle.size);
                ecsManager.UpdateShapePosition(circle.id, circle.position);
            }

        }

        public void UpdateSystem()
        {
            // processing logic here
            
        }
    }
}