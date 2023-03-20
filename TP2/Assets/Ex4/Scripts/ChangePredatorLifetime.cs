﻿using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using ParallelJobs;

public class ChangePredatorLifetime : MonoBehaviour
{
    private Lifetime _lifetime;

    public void Start()
    {
        _lifetime = GetComponent<Lifetime>();
    }

    public void Update()
    {
        _lifetime.decreasingFactor = 1.0f;
        foreach(var predator in Ex4Spawner.PredatorTransforms)
        {
            if (Vector3.Distance(predator.position, transform.position) < Ex4Config.TouchingDistance)
            {
                _lifetime.reproduced = true;
                break;
            }
        }
        
        foreach(var prey in Ex4Spawner.PreyTransforms)
        {
            if (Vector3.Distance(prey.position, transform.position) < Ex4Config.TouchingDistance)
            {
                _lifetime.decreasingFactor /= 2;
            }
        }
    }
}