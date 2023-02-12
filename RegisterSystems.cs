using System.Collections.Generic;
using circlesSystem;
using movementSystems;
using collisionSystems;
using resizeSystems;
using colorSystems;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();

        // Add your systems here
        toRegister.Add(new SpawnCirclesSystem("SpawnCirclesSystem"));
        toRegister.Add(new MovementSystem("MovementSystem"));
        toRegister.Add(new CollisionSystem("CollisionSystem"));
        toRegister.Add(new ResizeSystem("ResizeSystem"));
        toRegister.Add(new ColorSystem("ColorSystem"));

        return toRegister;
    }
}