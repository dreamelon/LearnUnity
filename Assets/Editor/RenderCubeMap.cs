using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenderCubeMapWizard : ScriptableWizard 
{
    public Transform renderFromPosition;
    public Cubemap cubemap;

    private void OnWizardCreate()
    {
        //创建临时相机
        GameObject go = new GameObject("CubemapCam");
        go.AddComponent<Camera>();
        //放到合适位置
        go.transform.position = renderFromPosition.position;
        //将相机渲染到cubemap
        go.GetComponent<Camera>().RenderToCubemap(cubemap);

        DestroyImmediate(go);
    }
    [MenuItem("GameObject/Render into Cubemap")]
    static void RenderCubemap()
    {
        ScriptableWizard.DisplayWizard<RenderCubeMapWizard>("Render CubeMap", "Render!");
    }

}
