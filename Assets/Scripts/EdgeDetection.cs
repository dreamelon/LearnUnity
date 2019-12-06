using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection : PostProcess
{
    [Range(0.0f, 1.0f)]
    public float edgesOnly = 0.0f;

    public Color edgeColor = Color.black;

    public Color backgroundColor = Color.white;
    //edgesOnly为0，边缘叠加在原图像上，为1时仅显示边缘
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material != null)
        {
            _Material.SetFloat("_EdgeOnly", edgesOnly);
            _Material.SetColor("_EdgeColor", edgeColor);
            _Material.SetColor("_BackgroundColor", backgroundColor);
            Graphics.Blit(source, destination, _Material);
        }
        else
            Graphics.Blit(source, destination);
    }
}
