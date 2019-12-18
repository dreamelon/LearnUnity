using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运动模糊通常有两种方式累积缓存和速度缓存
/// 本节依靠将当前渲染结果叠加到之前存储的渲染结果中，达到运动模糊的效果
/// </summary>
public class MotionBlur : PostProcess
{
    [Range(0.0f, 0.9f)]
    public float blurAmount = 0.5f;

    private RenderTexture accumulationTexture;

    private void OnDisable()
    {
        Destroy(accumulationTexture);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material != null)
        {
            if (!accumulationTexture || accumulationTexture.width != source.width || accumulationTexture.height != source.height)
            {
                Destroy(accumulationTexture);
                accumulationTexture = new RenderTexture(source.width, source.height, 0);
                //意味着该变量不会显示在hierarchy中，也不会保存在scene中，就无法被使用者修改
                accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source, accumulationTexture);

            }
            //恢复操作
            accumulationTexture.MarkRestoreExpected();

            _Material.SetFloat("_BlurAmount", 1.0f - blurAmount);
            Graphics.Blit(source, accumulationTexture, _Material);
            Graphics.Blit(accumulationTexture, destination);

        }
        else
            Graphics.Blit(source, destination);
    }


}
