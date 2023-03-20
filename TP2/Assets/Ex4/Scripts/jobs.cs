using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;


namespace ParallelJobs
{

    [BurstCompile]
    public struct MultiplyLifetimeDeacreasingFactor : IJobParallelForTransform
    {
        public NativeArray<float3> possitionArray;
        public NativeArray<float> lifeTimeDecreasingFactorArray;

        [ReadOnly] public float touchingDistance;
        
        public void Execute(int index, TransformAccess transform)
        {
            if (Vector3.Distance(possitionArray[index], transform.position) < touchingDistance)
            {
                lifeTimeDecreasingFactorArray[index] *= 2f;
                return;
            }
        }
    }

    [BurstCompile]
    public struct ChangeLifetimeReproduction : IJobParallelForTransform
    {
        public NativeArray<float3> possitionArray;
        public NativeArray<bool> lifeTimeReproducedArray;

        [ReadOnly] public float touchingDistance;
        
        public void Execute(int index, TransformAccess transform)
        {
            if (Vector3.Distance(possitionArray[index], transform.position) < touchingDistance)
            {
                lifeTimeReproducedArray[index] = true;
                return;
            }
        }
    }
    
    [BurstCompile]
    public struct DivideLifeTimeDecreasingFactor: IJobParallelForTransform
    {
        public NativeArray<float3> possitionArray;
        public NativeArray<float> lifeTimeDecreasingFactorArray;

        [ReadOnly] public float touchingDistance;
        
        public void Execute(int index, TransformAccess transform)
        {
            if (Vector3.Distance(possitionArray[index], transform.position) < touchingDistance)
            {
                lifeTimeDecreasingFactorArray[index] /= 2;
                return;
            }
        }
    }

}
