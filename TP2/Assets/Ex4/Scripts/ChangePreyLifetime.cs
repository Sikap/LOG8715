using UnityEngine;
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
        }*/
        possitionArray = new NativeArray<float3>(Ex4Spawner.PlantTransforms.Length,Allocator.TempJob);
        transformAccessArray = new TransformAccessArray(Ex4Spawner.PlantTransforms.Length);
        lifeTimeDecreasingFactorArray = new NativeArray<float>(Ex4Spawner.PlantTransforms.Length,Allocator.TempJob);
        for(int i = 0 ;i<Ex4Spawner.PlantTransforms.Length;i++)
        {
            possitionArray[i] = Ex4Spawner.PlantTransforms[i].position;
            transformAccessArray.Add(transform);
        }
        for(int i = 0 ;i<Ex4Spawner.PreyLifetimes.Length;i++)
        {
            lifeTimeDecreasingFactorArray[i] =  1.0f;
        }
        DivideLifeTimeDecreasingFactor dividePreyLifetimeParallelJob = new DivideLifeTimeDecreasingFactor
        {
            touchingDistance =  Ex4Config.TouchingDistance,
            possitionArray = possitionArray,
            lifeTimeDecreasingFactorArray = lifeTimeDecreasingFactorArray,
        };
        JobHandle jobPrayLifetimeHandle = dividePreyLifetimeParallelJob.Schedule(transformAccessArray);
        jobPrayLifetimeHandle.Complete();
        for(int i = 0 ;i<Ex4Spawner.PreyLifetimes.Length;i++)
        {
            Ex4Spawner.PreyLifetimes[i].decreasingFactor = lifeTimeDecreasingFactorArray[i];   
        }
        possitionArray.Dispose();
        transformAccessArray.Dispose();
        lifeTimeDecreasingFactorArray.Dispose();


        possitionArray = new NativeArray<float3>(Ex4Spawner.PredatorTransforms.Length,Allocator.TempJob);        
        transformAccessArray = new TransformAccessArray(Ex4Spawner.PredatorTransforms.Length);        
        lifeTimeDecreasingFactorArray =  new NativeArray<float>(Ex4Spawner.PreyLifetimes.Length,Allocator.TempJob);
        for(int i = 0 ;i<Ex4Spawner.PredatorTransforms.Length;i++)
        {
            possitionArray[i] = Ex4Spawner.PredatorTransforms[i].position;
            transformAccessArray.Add(transform);
        }
        for(int i = 0 ;i<Ex4Spawner.PreyLifetimes.Length;i++)
        {
            lifeTimeDecreasingFactorArray[i] = 1.0f;            
        }
        MultiplyLifetimeDeacreasingFactor multiplyPreyLifetimeParallelJob = new MultiplyLifetimeDeacreasingFactor{
            touchingDistance =  Ex4Config.TouchingDistance,
            possitionArray = possitionArray,
            lifeTimeDecreasingFactorArray = lifeTimeDecreasingFactorArray,
        };
        JobHandle jobHandle = multiplyPreyLifetimeParallelJob.Schedule(transformAccessArray);
        jobHandle.Complete();
        for(int i = 0 ;i<Ex4Spawner.PreyLifetimes.Length;i++)
        {
            Ex4Spawner.PreyLifetimes[i].decreasingFactor = lifeTimeDecreasingFactorArray[i];   
        }
        possitionArray.Dispose();
        transformAccessArray.Dispose();
        lifeTimeDecreasingFactorArray.Dispose();


        possitionArray = new NativeArray<float3>(Ex4Spawner.PreyTransforms.Length,Allocator.TempJob);
        transformAccessArray = new TransformAccessArray(Ex4Spawner.PreyTransforms.Length);
        lifeTimeReproducedArray = new NativeArray<bool>(Ex4Spawner.PreyLifetimes.Length,Allocator.TempJob);
        for(int i = 0 ;i<Ex4Spawner.PreyLifetimes.Length;i++)
        {
            possitionArray[i] = Ex4Spawner.PreyTransforms[i].position;
            lifeTimeReproducedArray[i] = Ex4Spawner.PreyLifetimes[i].reproduced;
            transformAccessArray.Add(transform);
        }
        ChangeLifetimeReproduction changeLifetimeReproduction = new ChangeLifetimeReproduction{
            touchingDistance =  Ex4Config.TouchingDistance,
            possitionArray = possitionArray,
            lifeTimeReproducedArray =  lifeTimeReproducedArray,
        };
        JobHandle jobPredatorLifetimeHandle = changeLifetimeReproduction.Schedule(transformAccessArray);
        jobPredatorLifetimeHandle.Complete();       
        for(int i = 0 ;i<Ex4Spawner.PreyLifetimes.Length;i++)
        {
            Ex4Spawner.PreyLifetimes[i].reproduced = lifeTimeReproducedArray[i];   
        }
        possitionArray.Dispose();
        transformAccessArray.Dispose();
        lifeTimeReproducedArray.Dispose();
    }
}