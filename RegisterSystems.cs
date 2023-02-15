using System.Collections.Generic;
using circlesSystem;
using movementSystem;
using collisionSystem;
using resizeSystem;
using colorSystem;
using borderSystem;
using protectionSystem;
using leftScreenSystem;
using creationSystem;
using destroySystem;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();

        // Add your systems here
        toRegister.Add(new SpawnCirclesSystem("SpawnCirclesSystem"));       
        toRegister.Add(new CreationSystem("CreationSystem"));       
        toRegister.Add(new DestroySystem("DestroySystem"));       
        toRegister.Add(new ProtectionSystem("ProtectionSystem"));
        toRegister.Add(new MovementSystem("MovementSystem"));
        toRegister.Add(new BorderSystem("BorderSystem"));
        toRegister.Add(new CollisionSystem("CollisionSystem"));
        toRegister.Add(new ResizeSystem("ResizeSystem"));
        toRegister.Add(new ColorSystem("ColorSystem"));
        toRegister.Add(new LeftScreenSystem("LeftScreenSystem"));

        return toRegister;
    }
}