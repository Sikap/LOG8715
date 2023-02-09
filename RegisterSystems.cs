using System.Collections.Generic;
using circlesSystem;
using movementSystems;
using collisionSystems;

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

        return toRegister;
    }
}