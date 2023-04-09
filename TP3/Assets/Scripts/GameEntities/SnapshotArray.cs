using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SnapshotArray
{
    private Vector2[,] data;
    private int maxTick = 120;
    public int size { get => data.Length; }

    public SnapshotArray(int maxNetworkObjectId)
    {
        // Create a new two-dimensional array to store the snapshot data
        data = new Vector2[maxNetworkObjectId + 2, maxTick];
    }

    public void AddSnapshot(ulong networkObjectId, int tick, Vector2 value)
    {
        int index = tick % maxTick;

        // Add the snapshot instance to the two-dimensional array at the appropriate index
        data[networkObjectId, index] = value;
    }

    public Vector2 GetSnapshotValue(ulong networkObjectId, int tick)
    {
        // Get the snapshot instance from the two-dimensional array at the appropriate index
        Vector2 snapshot = data[networkObjectId, tick % maxTick];

        // Return the Vector2 value from the snapshot instance
        return snapshot;
    }

}