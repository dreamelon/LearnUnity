using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

public struct CullMesh : IJobParallelFor
{
    public static RenderObject[] renderObjects;
    private static Plane[] frustumPlanes = new Plane[6];
    public static Vector3 cameraPos;
    public static float cameraFarClipDistance;

    public static void UpdateFrame(Camera cam, ref Matrix4x4 invVp, Vector3 cameraPosition)
    {
        GetCullingPlanes(ref invVp);
        cameraFarClipDistance = cam.farClipPlane;
        cameraPos = cameraPosition;
    }
    public static void GetCullingPlanes(ref Matrix4x4 invVp)
    {
        //reverse Z
        Vector3 nearLeftBottom = invVp.MultiplyPoint(new Vector3(-1, -1, 1));
        Vector3 nearLeftTop = invVp.MultiplyPoint(new Vector3(-1, 1, 1));
        Vector3 nearRightBottom = invVp.MultiplyPoint(new Vector3(1, -1, 1));
        Vector3 nearRightTop = invVp.MultiplyPoint(new Vector3(1, 1, 1));
        Vector3 farLeftBottom = invVp.MultiplyPoint(new Vector3(-1, -1, 0));
        Vector3 farLeftTop = invVp.MultiplyPoint(new Vector3(-1, 1, 0));
        Vector3 farRightBottom = invVp.MultiplyPoint(new Vector3(1, -1, 0));
        Vector3 farRightTop = invVp.MultiplyPoint(new Vector3(1, 1, 0));

        //Near
        frustumPlanes[0] = new Plane(nearRightTop, nearRightBottom, nearLeftBottom);
        //Up
        frustumPlanes[1] = new Plane(farLeftTop, farRightTop, nearRightTop);
        //Down
        frustumPlanes[2] = new Plane(nearRightBottom, farRightBottom, farLeftBottom);
        //Left
        frustumPlanes[3] = new Plane(farLeftBottom, farLeftTop, nearLeftTop);
        //Right
        frustumPlanes[4] = new Plane(farRightBottom, nearRightBottom, nearRightTop);
        //Far
        frustumPlanes[5] = new Plane(farLeftBottom, farRightBottom, farRightTop);
    }
    private static bool PlaneTest(ref Matrix4x4 objectToWorld, ref Vector3 extent, out Vector3 position)
    {
        Vector3 right = new Vector3(objectToWorld.m00, objectToWorld.m10, objectToWorld.m20);
        Vector3 up = new Vector3(objectToWorld.m01, objectToWorld.m11, objectToWorld.m21);
        Vector3 forward = new Vector3(objectToWorld.m02, objectToWorld.m12, objectToWorld.m22);
        position = new Vector3(objectToWorld.m03, objectToWorld.m13, objectToWorld.m23);
        for(int i = 0; i < 6; i++)
        {
            Plane plane = frustumPlanes[i];
            float r = Vector3.Dot(position, plane.normal);
            Vector3 absNormal = new Vector3(Mathf.Abs(Vector3.Dot(plane.normal, right)), 
                                            Mathf.Abs(Vector3.Dot(plane.normal, up)), 
                                            Mathf.Abs(Vector3.Dot(plane.normal, forward)));
            float f = Vector3.Dot(absNormal, extent);
            if ((r - f) >= -plane.distance)
                return false;

        }
        return true;
    }
    public void Execute(int i)
    {
        RenderObject obj = renderObjects[i];
        Vector3 position;
        if(PlaneTest(ref obj.localToWorldMatrix, ref obj.extent, out position))
        {
            float distance = Vector3.Distance(position, cameraPos);
            float layer = distance / cameraFarClipDistance;
            int layerValue = (int)Mathf.Clamp(Mathf.Lerp(0, SortMesh.LAYERCOUNT, layer), 0, SortMesh.LAYERCOUNT - 1);
            SortMesh.sorts[layerValue].Add(distance, obj);
        }
    }

    public static JobHandle Schedule()
    {
        return (new CullMesh()).Schedule(renderObjects.Length, 64);
    }

}
