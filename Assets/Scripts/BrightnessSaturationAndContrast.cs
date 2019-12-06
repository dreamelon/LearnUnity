using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightnessSaturationAndContrast : PostProcess
{
    [Range(0.0f, 3.0f)]//亮度
    public float brightness = 1.0f;
    [Range(0.0f, 3.0f)]//饱和度
    public float saturation = 1.0f;
    [Range(0.0f, 3.0f)]//对比度
    public float contrast = 1.0f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(_Material != null)
        {
            _Material.SetFloat("_Brightness", brightness);
            _Material.SetFloat("_Saturation", saturation);
            _Material.SetFloat("_Contrast", contrast);
            //source的结果传给_material中的_maintex纹理属性
            Graphics.Blit(source, destination, _Material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
