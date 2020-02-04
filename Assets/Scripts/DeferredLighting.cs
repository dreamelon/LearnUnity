using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeferredLighting
{
    public Material lightingMaterial;
    public Light directionLight;
    private static int _InvVP = Shader.PropertyToID("_InvVP");
    private static int _CurrentLightDir = Shader.PropertyToID("_CurrentLightDir");
    private static int _LightFinalColor = Shader.PropertyToID("_LightFinalColor");
    

    public void DrawLight(RenderTexture[] gbuffers, int[] gbufferIDs, RenderTexture target, Camera cam)
    {
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        lightingMaterial.SetMatrix(_InvVP, vp.inverse);
        lightingMaterial.SetVector(_CurrentLightDir, -directionLight.transform.forward);
        lightingMaterial.SetVector(_LightFinalColor, directionLight.color * directionLight.intensity);
        for (int i = 0; i < gbufferIDs.Length; ++i)
        {
            lightingMaterial.SetTexture(gbufferIDs[i], gbuffers[i]);
        }
        Graphics.Blit(null, target, lightingMaterial, 0);
    }
}
