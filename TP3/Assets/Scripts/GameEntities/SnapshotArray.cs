using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SnapshotLocation
{
    public Vector2[] data;
    private int maxNumberOfSnapshots;

    public SnapshotLocation(int max)
    {
        data = new Vector2[max];
        maxNumberOfSnapshots = max;
    }

    public void Add(Vector2 position, int tick) {
        data[tick % maxNumberOfSnapshots] = position;
    }

    public Vector2 Get(int tick) {
        return data[tick % maxNumberOfSnapshots];
    }

    public int Count() {
        return data.Length;
    }
}


public class SnapshotInput
{
    public Queue<Vector2>[] data;
    private int maxNumberOfSnapshots;

    public SnapshotInput(int max)
    {
        data = new Queue<Vector2>[max];
        maxNumberOfSnapshots = max;
    }

    public void Add(Queue<Vector2> inputs, int tick) {
        data[tick % maxNumberOfSnapshots] = inputs;
    }

    public Queue<Vector2> Get(int tick) {
        return data[tick % maxNumberOfSnapshots];
    }

    public int Count() {
        return data.Length;
    }
}