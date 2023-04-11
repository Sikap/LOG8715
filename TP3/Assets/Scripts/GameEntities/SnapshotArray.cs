using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SnapshotArray
{
    private Vector2[,] data;
    private int maxTick = 120;
    private ulong maxNetworkObjectId;

    public int size { get => data.Length; }

    public SnapshotArray(ulong numNetworkObjectId)
    {
        // Create a new two-dimensional array to store the snapshot data
        data = new Vector2[numNetworkObjectId, maxTick];
        maxNetworkObjectId = numNetworkObjectId;
    }

    public void AddSnapshot(ulong networkObjectId, int tick, Vector2 value)
    {
        // Add the snapshot instance to the two-dimensional array at the appropriate index
        data[networkObjectId % maxNetworkObjectId, tick % maxTick] = value;
    }

    public Vector2 GetSnapshotValue(ulong networkObjectId, int tick)
    {
        // Get the snapshot instance from the two-dimensional array at the appropriate index
        Vector2 snapshot = data[networkObjectId % maxNetworkObjectId, tick % maxTick];

        // Return the Vector2 value from the snapshot instance
        return snapshot;
    }

}

public class SnapshotInputArray
{
    private Queue<Vector2>[,] data;
    private int maxTick = 120;
    private ulong maxNetworkObjectId;

    public int size { get => data.Length; }

    public SnapshotInputArray(ulong numNetworkObjectId)
    {
        // Create a new two-dimensional array of type Queue<Vector2> to store the snapshot data
        data = new Queue<Vector2>[numNetworkObjectId, maxTick];
        maxNetworkObjectId = numNetworkObjectId;
    }

    public void AddSnapshot(ulong networkObjectId, int tick, Queue<Vector2> value)
    {
        // If there is no Queue<Vector2> instance in the data array at this index, create a new one
        /*if (data[networkObjectId % maxNetworkObjectId, tick % maxTick] == null)
        {
            data[networkObjectId % maxNetworkObjectId, tick % maxTick] = new Queue<Vector2>();
        }*/

        // Add the snapshot value to the Queue<Vector2> instance at the appropriate index
        data[networkObjectId % maxNetworkObjectId, tick % maxTick] = value;
    }

    public Queue<Vector2> GetSnapshotValue(ulong networkObjectId, int tick)
    {
        // Get the Queue<Vector2> instance from the data array at the appropriate index
        Queue<Vector2> snapshotQueue = data[networkObjectId % maxNetworkObjectId, tick % maxTick];

        return snapshotQueue;
    }
}