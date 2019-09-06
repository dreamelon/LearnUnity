﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRPCamera : MonoBehaviour
{
    public RenderTexture cameraRT;

    private static int _DepthTexture = Shader.PropertyToID("_DepthTexture");
    private RenderTexture[] GBufferTextures;
    private RenderBuffer[] GBuffers;
    private int[] GBufferIDs;
    

    public Transform[] cubeTrans;
    public Mesh cubeMesh;
    public Material pureColorMaterial;
    public SkyBoxDraw skybox;
    //public DeferredLighting lighting;
    public RenderTexture depthTexture;


    // Start is called before the first frame update
    void Start()
    {
        cameraRT = new RenderTexture(Screen.width, Screen.height, 24);
        GBufferTextures = new RenderTexture[]
        {
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
        };
        depthTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
        GBuffers = new RenderBuffer[GBufferTextures.Length];
        for(int i = 0; i < GBuffers.Length; i++)
        {
            GBuffers[i] = GBufferTextures[i].colorBuffer;
        }
        GBufferIDs = new int[]
        {
            Shader.PropertyToID("_GBuffer0"),
            Shader.PropertyToID("_GBuffer1"),
            Shader.PropertyToID("_GBuffer2"),
            Shader.PropertyToID("_GBuffer3")
        };
    }

    private void OnPostRender()
    {
        Camera cam = Camera.current;
        Shader.SetGlobalTexture(_DepthTexture, depthTexture);
        Graphics.SetRenderTarget(GBuffers, depthTexture.depthBuffer);
        GL.Clear(true, true, Color.grey);
        //start dc
        pureColorMaterial.color = new Color(0, 0.5f, 0.8f);
        pureColorMaterial.SetPass(0);
        foreach (var i in cubeTrans)
        {
            Graphics.DrawMeshNow(cubeMesh, i.localToWorldMatrix);
        }

        skybox.DrawSkybox(cam, cameraRT);
        //end dc
        Graphics.Blit(cameraRT, cam.targetTexture);
    }
}

