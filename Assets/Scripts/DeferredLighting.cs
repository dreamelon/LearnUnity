using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeferredLighting : MonoBehaviour
{
    public Material deferredMaterial;
    public Light directionLight;
    private static int _InvVP = Shader.PropertyToID("_InvVP");
    private static int _CurrentLightDir = Shader.PropertyToID("_CurrentLightDir");
    private static int _LightFinalColor = Shader.PropertyToID("_LightFinalColor");

    public void DrawLight(RenderTexture[] gbuffers, int[] gbufferIDs, RenderTexture target, Camera cam)
    {
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        deferredMaterial.SetMatrix(_InvVP, vp.inverse);
        deferredMaterial.SetVector(_CurrentLightDir, directionLight.transform.forward);
        deferredMaterial.SetVector(_LightFinalColor, directionLight.color * directionLight.intensity);
        for (int i = 0; i < gbufferIDs.Length; ++i)
        {
            deferredMaterial.SetTexture(gbufferIDs[i], gbuffers[i]);
        }
        Graphics.Blit(null, target, deferredMaterial, 0);
    }
}
