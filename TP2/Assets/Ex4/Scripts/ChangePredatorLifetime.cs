using UnityEngine;
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
    private NativeArray<float3> possitionArray;
    private NativeArray<float> lifeTimeDecreasingFactorArray;
    private NativeArray<bool> lifeTimeReproducedArray;

    private TransformAccessArray transformAccessArray;

    public void Start()
    {
        _lifetime = GetComponent<Lifetime>();
    }

    public void Update()
    {
        /*_lifetime.decreasingFactor = 1.0f;
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
        }*/
        possitionArray = new NativeArray<float3>(Ex4Spawner.PredatorTransforms.Length,Allocator.TempJob);
        transformAccessArray = new TransformAccessArray(Ex4Spawner.PredatorTransforms.Length);
        lifeTimeReproducedArray = new NativeArray<bool>(Ex4Spawner.PredatorLifetimes.Length,Allocator.TempJob);
        for(int i = 0 ;i<Ex4Spawner.PredatorLifetimes.Length;i++)
        {
            possitionArray[i] = Ex4Spawner.PredatorTransforms[i].position;
            lifeTimeReproducedArray[i] = Ex4Spawner.PredatorLifetimes[i].reproduced;
            transformAccessArray.Add(transform);
        }
        ChangeLifetimeReproduction changeLifetimeReproduction = new ChangeLifetimeReproduction{
            touchingDistance =  Ex4Config.TouchingDistance,
            possitionArray = possitionArray,
            lifeTimeReproducedArray =  lifeTimeReproducedArray,
        };
        JobHandle jobPredatorLifetimeHandle = changeLifetimeReproduction.Schedule(transformAccessArray);
        jobPredatorLifetimeHandle.Complete();       
        for(int i = 0 ;i<Ex4Spawner.PredatorLifetimes.Length;i++)
        {
            Ex4Spawner.PredatorLifetimes[i].reproduced = lifeTimeReproducedArray[i];   
        }
        possitionArray.Dispose();
        transformAccessArray.Dispose();
        lifeTimeReproducedArray.Dispose();
        

        possitionArray = new NativeArray<float3>(Ex4Spawner.PreyTransforms.Length,Allocator.TempJob);
        transformAccessArray = new TransformAccessArray(Ex4Spawner.PreyTransforms.Length);
        lifeTimeDecreasingFactorArray = new NativeArray<float>(Ex4Spawner.PreyTransforms.Length,Allocator.TempJob);
        for(int i = 0 ;i<Ex4Spawner.PreyTransforms.Length;i++)
        {
            possitionArray[i] = Ex4Spawner.PreyTransforms[i].position;
            transformAccessArray.Add(transform);
        }
        for(int i = 0 ;i<Ex4Spawner.PredatorLifetimes.Length;i++)
        {
            lifeTimeDecreasingFactorArray[i] =  1.0f;
        }
        DivideLifeTimeDecreasingFactor changePreyLifetimeParallelJob = new DivideLifeTimeDecreasingFactor
        {
            touchingDistance =  Ex4Config.TouchingDistance,
            possitionArray = possitionArray,
            lifeTimeDecreasingFactorArray = lifeTimeDecreasingFactorArray,
        };
        JobHandle jobPrayLifetimeHandle = changePreyLifetimeParallelJob.Schedule(transformAccessArray);
        jobPrayLifetimeHandle.Complete();
        for(int i = 0 ;i<Ex4Spawner.PredatorLifetimes.Length;i++)
        {
            Ex4Spawner.PredatorLifetimes[i].decreasingFactor = lifeTimeDecreasingFactorArray[i];   
        }
        possitionArray.Dispose();
        transformAccessArray.Dispose();
        lifeTimeDecreasingFactorArray.Dispose();
    }
}