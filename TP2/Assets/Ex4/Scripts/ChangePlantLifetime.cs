using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using ParallelJobs;

public class ChangePlantLifetime : MonoBehaviour
{
    private Lifetime _lifetime;
    private NativeArray<float3> possitionArray;
    private NativeArray<float> lifeTimeDecreasingFactorArray;
    private TransformAccessArray transformAccessArray;
    public void Start()
    {
        _lifetime = GetComponent<Lifetime>();
    }

    public void Update()
    {
        /*_lifetime.decreasingFactor = 1.0f;
        foreach(var prey in Ex4Spawner.PreyTransforms)
        {
            if (Vector3.Distance(prey.position, transform.position) < Ex4Config.TouchingDistance)
            {
                _lifetime.decreasingFactor *= 2f;
                break;
            }
        }*/
        possitionArray = new NativeArray<float3>(Ex4Spawner.PreyTransforms.Length,Allocator.TempJob);        
        transformAccessArray = new TransformAccessArray(Ex4Spawner.PreyTransforms.Length);        
        lifeTimeDecreasingFactorArray =  new NativeArray<float>(Ex4Spawner.PlantLifetimes.Length,Allocator.TempJob);
        for(int i = 0 ;i<Ex4Spawner.PreyTransforms.Length;i++)
        {
            possitionArray[i] = Ex4Spawner.PreyTransforms[i].position;
            transformAccessArray.Add(transform);
        }
        for(int i = 0 ;i<Ex4Spawner.PlantLifetimes.Length;i++)
        {
            lifeTimeDecreasingFactorArray[i] = 1.0f;            
        }
        MultiplyLifetimeDeacreasingFactor changePlantLifetimeParallelJob = new MultiplyLifetimeDeacreasingFactor{
            touchingDistance =  Ex4Config.TouchingDistance,
            possitionArray = possitionArray,
            lifeTimeDecreasingFactorArray = lifeTimeDecreasingFactorArray,
        };
        JobHandle jobHandle = changePlantLifetimeParallelJob.Schedule(transformAccessArray);
        jobHandle.Complete();
        for(int i = 0 ;i<Ex4Spawner.PlantLifetimes.Length;i++)
        {
            Ex4Spawner.PlantLifetimes[i].decreasingFactor = lifeTimeDecreasingFactorArray[i];   
        }
        possitionArray.Dispose();
        transformAccessArray.Dispose();
        lifeTimeDecreasingFactorArray.Dispose();
    }
}