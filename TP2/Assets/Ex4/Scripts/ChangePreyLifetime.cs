﻿using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using ParallelJobs;

public class ChangePreyLifetime : MonoBehaviour
{
    private Lifetime _lifetime;

    public void Start()
    {
        _lifetime = GetComponent<Lifetime>();
    }

    public void Update()
    {
        _lifetime.decreasingFactor = 1.0f;
        foreach(var plant in Ex4Spawner.PlantTransforms)
        {
            if (Vector3.Distance(plant.position, transform.position) < Ex4Config.TouchingDistance)
            {
                _lifetime.decreasingFactor /= 2;
                break;
            }
        }
        
        foreach(var predator in Ex4Spawner.PredatorTransforms)
        {
            if (Vector3.Distance(predator.position, transform.position) < Ex4Config.TouchingDistance)
            {
                _lifetime.decreasingFactor *= 2f;
                break;
            }
        }
        
        foreach(var prey in Ex4Spawner.PreyTransforms)
        {
            if (Vector3.Distance(prey.position, transform.position) < Ex4Config.TouchingDistance)
            {
                _lifetime.reproduced = true;
                break;
            }
        }
    }
}