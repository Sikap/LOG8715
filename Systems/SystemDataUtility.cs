using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SystemDataUtility
{
    public static uint id = 1;
    static global::ECSManager ecsManager = global::ECSManager.Instance;
    static Camera mainCamera = Camera.main;

    public static uint GetReservedId() {
        return id++;
    }

    //------------------------------------------------//
    // Abstraction of ECSmanager public functions     //
    // to allow verifications and other modifications //
    //------------------------------------------------//
    public static bool IsCircleInLeftScreen(Vector2 center, int diameter){
        float radius = diameter / 2f;
        var offsetLeftX = mainCamera.WorldToScreenPoint(new Vector2(center.x - radius, center.y)).x;
        if (offsetLeftX < Screen.width/2.0f)
        {            
            return true;
        }
        return false;
    }  
    public static void CreateCircles() {
        foreach (KeyValuePair<uint, CreationComponent> shape in worldData.WorldData.toCreate)
        {            
            CreateCircle(shape.Key);  
        }
        worldData.WorldData.toCreate.Clear();
    }

    public static void CreateCircle(uint id) {
        if (worldData.WorldData.circlesSize.ContainsKey(id) && worldData.WorldData.circlesPosition.ContainsKey(id))
        {
            var size = worldData.WorldData.circlesSize[id].size;
            var position = worldData.WorldData.circlesPosition[id].position;
            ecsManager.CreateShape(id, size);
            ecsManager.UpdateShapePosition(id, position);
        }
    }
    public static void DestroyCircles() {
        foreach (KeyValuePair<uint,DestroyComponent> shape in worldData.WorldData.toDestroy)
        {  
            RemoveShapeDataFromSystems(shape.Key);
            DestroyShape(shape.Key);
        }
        worldData.WorldData.toDestroy.Clear();
    }
    public static void DestroyShape(uint id) {
        ecsManager.DestroyShape(id);
    }

    public static void UpdateShape(uint id, Vector2 position, int size) {
        ecsManager.UpdateShapeSize(id, size);
        ecsManager.UpdateShapePosition(id, position);
    }

    public static void UpdateShapeSize(uint id, int size) {
        ecsManager.UpdateShapeSize(id, size);
    }

    public static void UpdateShapePosition(uint id, Vector2 position) {
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
    public static bool IsProtectionOver (uint id) {
        return Time.time > worldData.WorldData.circleProtectionStartTime[id].startProtectionTime + ecsManager.Config.protectionDuration;
    }
    public static bool IsCooldownOver (uint id) {
        return Time.time > worldData.WorldData.circleProtectionStartTime[id].startProtectionTime + ecsManager.Config.protectionDuration + ecsManager.Config.protectionCooldown;
    } 
    public static void HandleCirclesProtection(bool checkLeftScreen) {
        foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
        {
            var size = worldData.WorldData.circlesSize[shape.Key].size;
            if (!checkLeftScreen || IsCircleInLeftScreen(shape.Value.position,size)){
                HandleProtectionState(shape.Key);
            }
        }
    }
    public static void HandleProtectionState(uint id) {
        if(worldData.WorldData.circlesProtection[id].isProtected && IsProtectionOver(id)){
            worldData.WorldData.circlesProtection[id] = new ProtectedComponent { isProtected = false };
        }
        if(worldData.WorldData.circlesIsDynamic[id].isDynamic &&  worldData.WorldData.circlesSize[id].size <= ecsManager.Config.protectionSize){
            var isProtected = RandomProbability(ecsManager.Config.protectionProbability);
            if(isProtected && worldData.WorldData.circlesProtection[id].isProtected == false)
            {
                if(!worldData.WorldData.circleProtectionStartTime.ContainsKey(id)){
                    worldData.WorldData.circlesProtection[id] = new ProtectedComponent { isProtected = true };
                    worldData.WorldData.circleProtectionStartTime.Add(id, new StartProtectionTimeComponent { startProtectionTime = Time.time });
                }else if(IsCooldownOver(id)){
                    worldData.WorldData.circleProtectionStartTime.Remove(id);
                }
            }
        }
    }
    public static void MoveCircles(bool checkLeftScreen) {
        foreach (KeyValuePair<uint,DynamicComponent> shape in worldData.WorldData.circlesIsDynamic)
        {    
            var position = worldData.WorldData.circlesPosition[shape.Key].position;
            var size = worldData.WorldData.circlesSize[shape.Key].size;
            if (!checkLeftScreen || IsCircleInLeftScreen(position,size))
            {
                MoveCircle(shape.Key);
            }
        }
    }
    public static void MoveCircle(uint id) {
        if (IsProcessable(id))
        {
            var xPosition = worldData.WorldData.circlesPosition[id].position.x + worldData.WorldData.circlesSpeed[id].speed.x * Time.deltaTime;
            var yPosition = worldData.WorldData.circlesPosition[id].position.y + worldData.WorldData.circlesSpeed[id].speed.y * Time.deltaTime;
            worldData.WorldData.circlesPosition[id] = new PositionComponent { position = new Vector2(xPosition, yPosition)};
            UpdateShapePosition(id, worldData.WorldData.circlesPosition[id].position);
        }
    }
    public static bool IsCircleOffScreen(Vector2 center, int diameter)
    {
        float radius = diameter / 2f;
        var offsetRigthX = mainCamera.WorldToScreenPoint(new Vector2(center.x + radius, center.y)).x;
        var offsetTopY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y + radius)).y;
        var offsetLeftX = mainCamera.WorldToScreenPoint(new Vector2(center.x - radius, center.y)).x;
        var offsetBottomY = mainCamera.WorldToScreenPoint(new Vector2(center.x, center.y - radius)).y;
        if ( offsetLeftX < 0 || offsetRigthX > Screen.width || offsetBottomY < 0 || offsetTopY > Screen.height)
        {            
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

    public static Vector2 BounceBackSpeed(Vector2 center, int diameter, Vector2 velocity)
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
    public static void HandelCameraScreenBounds(bool checkLeftScreen) {
        worldData.WorldData.circleBorderBouncePosition.Clear();
        worldData.WorldData.circleBorderBounceSpeed.Clear();        
        foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
        {   
            var size = worldData.WorldData.circlesSize[shape.Key].size;
            if (!checkLeftScreen || IsCircleInLeftScreen(shape.Value.position,size))
            {
                var speed = worldData.WorldData.circlesSpeed[shape.Key].speed;
                Vector2 shapePosition = new Vector2(shape.Value.position.x , shape.Value.position.y);
                
                if(IsCircleOffScreen(shapePosition, size)){ 
                    worldData.WorldData.circleBorderBouncePosition[shape.Key] = new PositionComponent { position = BounceBackPosition(shape.Value.position,size) };
                    worldData.WorldData.circleBorderBounceSpeed[shape.Key] = new SpeedComponent { speed = BounceBackSpeed(shape.Value.position,size,speed) };
                }
            }
        }
        foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circleBorderBouncePosition)
        {
            worldData.WorldData.circlesPosition[shape.Key] = shape.Value;
            worldData.WorldData.circlesSpeed[shape.Key] =  worldData.WorldData.circleBorderBounceSpeed[shape.Key];
        }
    }
    public static void HandelCirclesCollision(bool checkLeftScreen) {
        worldData.WorldData.circlesCollision.Clear();
        var newPositionCollision = new Dictionary<uint, PositionComponent>();
        var newSpeedCollision = new Dictionary<uint, SpeedComponent>();
        foreach (KeyValuePair<uint, PositionComponent> shapeOne in worldData.WorldData.circlesPosition)
        {   
            var positionOne = worldData.WorldData.circlesPosition[shapeOne.Key].position;
            var sizeOne = worldData.WorldData.circlesSize[shapeOne.Key].size;         
            if (!checkLeftScreen || IsCircleInLeftScreen(positionOne,sizeOne)){

                if (SystemDataUtility.IsProcessable(shapeOne.Key)) {
                    var shapeOnePosition = shapeOne.Value.position;
                    var shapeOneSpeed = worldData.WorldData.circlesSpeed[shapeOne.Key].speed;
                    var shapeOneSize = worldData.WorldData.circlesSize[shapeOne.Key].size;
                    var shapeOneIsDynamic = worldData.WorldData.circlesIsDynamic[shapeOne.Key].isDynamic;

                    foreach (KeyValuePair<uint, PositionComponent> shapeTwo in worldData.WorldData.circlesPosition)
                    {
                        var positionTwo = worldData.WorldData.circlesPosition[shapeTwo.Key].position;
                        var sizeTwo = worldData.WorldData.circlesSize[shapeTwo.Key].size;         
                        if (!checkLeftScreen || IsCircleInLeftScreen(positionTwo,sizeTwo)){
                            if (SystemDataUtility.IsProcessable(shapeTwo.Key)) {  
                                if (shapeOne.Key != shapeTwo.Key) {
                                    var shapeTwoPosition = shapeTwo.Value.position;
                                    var shapeTwoSpeed = worldData.WorldData.circlesSpeed[shapeTwo.Key].speed;
                                    var shapeTwoSize = worldData.WorldData.circlesSize[shapeTwo.Key].size;
                                    var shapeTwoIsDynamic = worldData.WorldData.circlesIsDynamic[shapeTwo.Key].isDynamic;
                                    var collisionResult = CollisionUtility.CalculateCollision(shapeOnePosition, shapeOneSpeed, shapeOneSize, shapeTwoPosition, shapeTwoSpeed, shapeTwoSize);
                                    if (collisionResult != null)
                                    {   
                                        var newPosition1 = collisionResult.position1;
                                        var newSpeed1 = collisionResult.velocity1 ;
                                        var newPosition2 = collisionResult.position2;
                                        var newSpeed2 = collisionResult.velocity2 ;

                                        // Keep circle in same spot if it's static.
                                        if (!shapeTwoIsDynamic){
                                            newPosition2 = shapeTwoPosition;
                                            newSpeed2 = shapeTwoSpeed;
                                        } else if(!shapeOneIsDynamic){
                                            newPosition1 = shapeOnePosition;
                                            newSpeed1 = shapeOneSpeed;
                                        }
                                        
                                        newPositionCollision[shapeOne.Key] = new PositionComponent { position = newPosition1 };
                                        newSpeedCollision[shapeOne.Key] = new SpeedComponent { speed = newSpeed1 };

                                        newPositionCollision[shapeTwo.Key] = new PositionComponent { position = newPosition2 };
                                        newSpeedCollision[shapeTwo.Key] = new SpeedComponent { speed = newSpeed2 };

                                        // Only add one collision for the given collision between two circles
                                        worldData.WorldData.circlesCollision[shapeOne.Key] = new CollisionComponent { circleMain = shapeOne.Key, circleColliding = shapeTwo.Key };
                                        worldData.WorldData.circlesCollision[shapeTwo.Key] = new CollisionComponent { circleMain = shapeTwo.Key, circleColliding = shapeOne.Key };
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        foreach (KeyValuePair<uint, PositionComponent> shape in newPositionCollision)
        {
            worldData.WorldData.circlesPosition[shape.Key] = shape.Value;
            worldData.WorldData.circlesSpeed[shape.Key] = newSpeedCollision[shape.Key];
        }
    }
    public static void HandelExplodingCircles(bool checkLeftScreen) {

        foreach (KeyValuePair<uint, ExplosionComponent> shape in worldData.WorldData.toExplode)
        {
            var position = worldData.WorldData.circlesPosition[shape.Key].position;
            var size = worldData.WorldData.circlesSize[shape.Key].size;
            if (!checkLeftScreen || IsCircleInLeftScreen(position,size)){

                var newSpeedCollision = worldData.WorldData.circlesSpeed[shape.Key].speed;
                var newPositionCollision = worldData.WorldData.circlesPosition[shape.Key].position;
                var newSizeCollision = worldData.WorldData.circlesSize[shape.Key].size;

                var zeroRadVector = new Vector2(1,0);
                var motionAngle = Vector2.Angle(zeroRadVector, newSpeedCollision) * Mathf.Deg2Rad;

                var smallerCircleOnePositionX = newPositionCollision.x + ((newSizeCollision/4) * Mathf.Cos(motionAngle));
                var smallerCircleOnePositionY = newPositionCollision.y + ((newSizeCollision/4) * Mathf.Sin(motionAngle));

                var smallerCircleOnePosition = new Vector2(smallerCircleOnePositionX, smallerCircleOnePositionY);
                var smallerCircleOneSize = newSizeCollision/2;
                var smallerCircleOneSpeed = newSpeedCollision;
                
                var smallerCircleTwoPositionX = newPositionCollision.x + ((newSizeCollision/4) * Mathf.Cos(motionAngle + Mathf.PI));
                var smallerCircleTwoPositionY = newPositionCollision.y + ((newSizeCollision/4) * Mathf.Sin(motionAngle + Mathf.PI));                 

                var smallerCircleTwoPosition = new Vector2(smallerCircleTwoPositionX, smallerCircleTwoPositionY);
                var smallerCircleTwoSize = newSizeCollision/2;
                var smallerCircleTwoSpeed = newSpeedCollision * -1;
                
                var idOne = AddShapeDataToSystems(smallerCircleOneSize, smallerCircleOnePosition, smallerCircleOneSpeed, true, false);
                var idTwo = AddShapeDataToSystems(smallerCircleTwoSize, smallerCircleTwoPosition, smallerCircleTwoSpeed, true, false);
                worldData.WorldData.toCreate.Add(idOne, new CreationComponent{ toCreate = true });
                worldData.WorldData.toCreate.Add(idTwo, new CreationComponent{ toCreate = true });
            }
        }
        worldData.WorldData.toExplode.Clear();
    }
    public static void ResizeCircles(bool checkLeftScreen) {
        var alreadyProcessed = new Dictionary<uint, bool>();
        foreach (KeyValuePair<uint, CollisionComponent> shape in worldData.WorldData.circlesCollision)
        {
            var position = worldData.WorldData.circlesPosition[shape.Key].position;
            var size = worldData.WorldData.circlesSize[shape.Key].size;
            if (!checkLeftScreen || IsCircleInLeftScreen(position,size)){

                if (!alreadyProcessed.ContainsKey(shape.Key))
                {
                    var idMain = shape.Value.circleMain;
                    var idColliding = shape.Value.circleColliding;
                    var shapeOneIsDynamic = worldData.WorldData.circlesIsDynamic[idMain].isDynamic;
                    var shapeTwoIsDynamic = worldData.WorldData.circlesIsDynamic[idColliding].isDynamic;
                    var shapeOneSize = worldData.WorldData.circlesSize[idMain].size;
                    var shapeTwoSize = worldData.WorldData.circlesSize[idColliding].size;
                    if (SystemDataUtility.IsProcessable(shape.Key) && worldData.WorldData.circlesSize.ContainsKey(shape.Key))
                    {
                        if(shapeOneIsDynamic && shapeTwoIsDynamic){
                            if (shapeOneSize > shapeTwoSize)
                            {
                                if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                        shapeOneSize++;
                                    }else{
                                        shapeOneSize--;
                                    }
                                }
                                if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                        shapeTwoSize--;
                                    }
                                }
                            } else if (shapeOneSize < shapeTwoSize)
                            {
                                if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                        shapeOneSize--;
                                    }
                                }
                                if(!worldData.WorldData.circlesProtection[idColliding].isProtected){
                                    if(!worldData.WorldData.circlesProtection[idMain].isProtected){
                                        shapeTwoSize++;
                                    }else{
                                        shapeTwoSize--;
                                    }
                                }
                            }
                        }
                        ResizeCircle(idMain, shapeOneSize);
                        ResizeCircle(idColliding, shapeTwoSize);
                        if (!alreadyProcessed.ContainsKey(idMain))
                        {
                            alreadyProcessed.Add(idMain, true);
                        }
                        if (!alreadyProcessed.ContainsKey(idColliding))
                        {
                            alreadyProcessed.Add(idColliding, true);
                        }
                    }
                }
            }
        }
    }
    public static void ResizeCircle(uint id, int size) {
        if (size <= 0) {
            if (!worldData.WorldData.toDestroy.ContainsKey(id)) {
                worldData.WorldData.toDestroy.Add(id, new DestroyComponent{ toDestroy = true });
            }
        } else if (size >= ecsManager.Config.explosionSize) {
            if (!worldData.WorldData.toDestroy.ContainsKey(id)) {
                worldData.WorldData.toDestroy.Add(id, new DestroyComponent{ toDestroy = true });
                worldData.WorldData.toExplode.Add(id, new ExplosionComponent{ isExploding = true });
            }
        } else {
            if (worldData.WorldData.circlesSize.ContainsKey(id)) {
                worldData.WorldData.circlesSize[id] = new SizeComponent { size = size };
                SystemDataUtility.UpdateShapeSize(id, size);
            }
        }
    }
    public static void ColorCircles(bool checkLeftScreen) {
        foreach (KeyValuePair<uint, PositionComponent> shape in worldData.WorldData.circlesPosition)
        {   
            var size = worldData.WorldData.circlesSize[shape.Key].size;   
            if (!checkLeftScreen || IsCircleInLeftScreen(shape.Value.position,size))
            {
                if (IsProcessable(shape.Key))
                {    
                    if(!worldData.WorldData.circlesIsDynamic[shape.Key].isDynamic){
                        ecsManager.UpdateShapeColor(shape.Key, Color.red);
                    } else {
                        if(worldData.WorldData.circlesCollision.ContainsKey(shape.Key)){
                            ecsManager.UpdateShapeColor(shape.Key, Color.green);
                        } else {
                            if (size == (ecsManager.Config.explosionSize - 1)){
                                ecsManager.UpdateShapeColor(shape.Key, new Color(0xEE / 255f, 0x76 / 255f, 0x00 / 255f));
                            } else {
                                if(worldData.WorldData.circlesProtection[shape.Key].isProtected){
                                    ecsManager.UpdateShapeColor(shape.Key, Color.yellow);
                                }else{
                                    ecsManager.UpdateShapeColor(shape.Key, Color.blue);
                                }
                            }
                            
                        }
                    }
                }
            }
        }
    }

}