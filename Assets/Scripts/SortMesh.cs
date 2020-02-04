using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
public struct SortMesh : IJobParallelFor
{
    public const int LAYERCOUNT = 20;
    public static BinarySort<RenderObject>[] sorts = new BinarySort<RenderObject>[LAYERCOUNT];
    public static bool init = false;

    public static void InitSortMesh(int maximumDrawCall)
    {
        if (init) return;
        init = true;
        for (int i = 0; i < LAYERCOUNT; ++i)
        {
            sorts[i] = new BinarySort<RenderObject>(maximumDrawCall);

        }
    }
    public void Execute(int index)
    {
        sorts[index].Sort();
        sorts[index].GetSorted();
    }

    public static JobHandle Schedule(JobHandle cull)
    {
        SortMesh instance = new SortMesh();
        return instance.Schedule<SortMesh>(LAYERCOUNT, 1, cull);
    }

    public static void UpdateFrame()
    {
        for (int i = 0; i < sorts.Length; i++)
        {
            sorts[i].Clear();
        }
    }
}
