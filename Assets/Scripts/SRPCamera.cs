using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRPCamera : MonoBehaviour
{
    private RenderTexture cameraRT;

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

    public Material deferredMaterial;
    public Light directionLight;
    private static int _InvVP = Shader.PropertyToID("_InvVP");
    private static int _CurrentLightDir = Shader.PropertyToID("_CurrentLightDir");
    private static int _LightFinalColor = Shader.PropertyToID("_LightFinalColor");
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
        //深度贴图？
        depthTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
        GBuffers = new RenderBuffer[GBufferTextures.Length];
        for(int i = 0; i < GBuffers.Length; i++)
        {
            GBuffers[i] = GBufferTextures[i].colorBuffer;
        }
        //这就是纹理id把
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
        //渲染目标改为gbuffer
        Graphics.SetRenderTarget(GBuffers, depthTexture.depthBuffer);
        //Graphics.SetRenderTarget(cameraRT);
        GL.Clear(true, true, Color.grey);
        //start dc
        pureColorMaterial.color = new Color(0, 0.2f, 0.8f);
        pureColorMaterial.SetPass(0);
        foreach (var i in cubeTrans)
        {
            Graphics.DrawMeshNow(cubeMesh, i.localToWorldMatrix);
        }

        DrawLight(GBufferTextures, GBufferIDs, cameraRT, cam);

        skybox.DrawSkybox(cam, cameraRT);
        //end dc
        Graphics.Blit(cameraRT, cam.targetTexture);
    }
    public void DrawLight(RenderTexture[] gbuffers, int[] gbufferIDs, RenderTexture target, Camera cam)
    {
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        deferredMaterial.SetMatrix(_InvVP, vp.inverse);
        deferredMaterial.SetVector(_CurrentLightDir, directionLight.transform.forward);
        deferredMaterial.SetVector(_LightFinalColor, directionLight.color * directionLight.intensity);
        for(int i=0; i < gbufferIDs.Length; ++i)
        {
            deferredMaterial.SetTexture(gbufferIDs[i], gbuffers[i]);
        }
        Graphics.Blit(null, target, deferredMaterial, 0);
    }
}

