using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
delegate int functionPointer();

public class SRPCamera : MonoBehaviour
{
    private RenderTexture cameraRT;

    private static int _DepthTexture = Shader.PropertyToID("_DepthTexture");
    private static int _InvVP = Shader.PropertyToID("_InvVP");

    private RenderTexture[] GBufferTextures;
    private RenderBuffer[] GBuffers;
    private int[] GBufferIDs;


    public Transform[] cubeTrans;
    public Mesh cubeMesh;
    public Material DeferredMaterial;
    private RenderTexture depthTexture;
    public RenderObject[] renderObjects;
    public DeferredLighting lighting;

    [Range(0.5f, 4f)]
    public float superSample = 1;

    public SkyBoxDraw skybox;

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
        for (int i = 0; i < GBuffers.Length; i++)
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
        SortMesh.InitSortMesh(renderObjects.Length);
        CullMesh.renderObjects = renderObjects;
        foreach (var i in renderObjects)
        {
            i.Init();
        }
    }

    private void OnPostRender()
    {
        Camera cam = Camera.current;
        Shader.SetGlobalTexture(_DepthTexture, depthTexture);

        //Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        //Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        //Matrix4x4 inverseVp = vp.inverse;
        //Shader.SetGlobalMatrix(_InvVP, inverseVp);
        //CullMesh.UpdateFrame(cam, ref inverseVp, transform.position);
        //SortMesh.UpdateFrame();
        //JobHandle cullHandle = CullMesh.Schedule();
        //JobHandle sortHandle = SortMesh.Schedule(cullHandle);
        //JobHandle.ScheduleBatchedJobs();

        //渲染目标改为gbuffer
        Graphics.SetRenderTarget(GBuffers, depthTexture.depthBuffer);
        //Graphics.SetRenderTarget(cameraRT);
        GL.Clear(true, true, Color.grey);
        //start dc
        DeferredMaterial.color = new Color(0, 0.2f, 0.8f);
        DeferredMaterial.SetPass(0);
        //sortHandle.Complete();
        //for (int i = 0; i < SortMesh.sorts.Length; i++)
        //{
        //    DrawElements(ref SortMesh.sorts[i]);
        //}
        for(int i = 0; i < cubeTrans.Length; i++)
        {
            Graphics.DrawMeshNow(cubeMesh, cubeTrans[i].localToWorldMatrix);
        }

        lighting.DrawLight(GBufferTextures, GBufferIDs, cameraRT, cam);
        //Graphics.Blit(GBufferTextures[1], cameraRT);
        skybox.DrawSkybox(cam, cameraRT.colorBuffer, depthTexture.depthBuffer);
        //end dc
        Graphics.Blit(cameraRT, cam.targetTexture);
    }

    public void DrawElements(ref BinarySort<RenderObject> binarySort)
    {
        RenderObject[] objs = binarySort.meshes;
        for (int j = 0; j < binarySort.count; j++)
        {
            Graphics.DrawMeshNow(objs[j].targetMesh, objs[j].localToWorldMatrix);
        }

    }
}

