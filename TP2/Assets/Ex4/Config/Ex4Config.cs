using UnityEngine;

[CreateAssetMenu(fileName = "Ex4Config", menuName = "Config/Ex4", order = 1)]
public class Ex4Config : ScriptableObject
{
    [SerializeField] public int plantCount = 3000;
    [SerializeField] public int preyCount = 3000;
    [SerializeField] public int predatorCount = 2500;
    [SerializeField] public int gridSize = 12000;

    public static readonly float PreySpeed = 1;
    public static readonly float PredatorSpeed = 0.5f;
    public static readonly float TouchingDistance = 0.5f;
}

