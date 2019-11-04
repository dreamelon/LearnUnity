using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//非运行时也有效果
[ExecuteInEditMode]//添加脚本、启动、Stop的时候执行Awake()  Start()
public class OutlineEffect : MonoBehaviour
{
    public Camera outlineCamera;

    #region 纯色材质
    public Shader pureColorShader;
    private Material m_pureColor = null;
    private Material PureMat
    {
        get
        {
            if (m_pureColor == null)
                m_pureColor = new Material(pureColorShader);
            return m_pureColor;
        }
    }
    #endregion

    #region 合并材质
    public Shader compositeShader;
    private Material m_composite = null;
    private Material CompositeMat
    {
        get
        {
            if (m_composite == null)
                m_composite = new Material(compositeShader);
            return m_composite;
        }
    }
    #endregion

    #region 剔除材质
    public Shader cutoffShader;
    private Material m_cutoff = null;
    private Material CutoffMat
    {
        get
        {
            if (m_cutoff == null)
                m_cutoff = new Material(cutoffShader);
            return m_cutoff;
        }
    }
    #endregion

    #region 模糊材质
    public Shader blurShader;
    private Material m_blur = null;
    private Material BlurMat
    {
        get
        {
            if (m_blur == null)
                m_blur = new Material(blurShader);
            return m_blur;
        }
    }
    #endregion

    private RenderTexture outlineRT;

    [Range(0,4)]
    public int iterations = 2;

    private void Start()
    {
        outlineRT = new RenderTexture((int)outlineCamera.pixelWidth, (int)outlineCamera.pixelHeight, 16);
    }

    private void OnPreRender() 
    {
        outlineCamera.targetTexture = outlineRT;
        outlineCamera.RenderWithShader(pureColorShader, "");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture _renderTexture = RenderTexture.GetTemporary(outlineRT.width, outlineRT.height);
        RenderTexture temp = RenderTexture.GetTemporary(outlineRT.width, outlineRT.height);

        MixRender(outlineRT, ref _renderTexture);

        //GL.Clear(true, true, Color.grey);
        CompositeMat.SetTexture("_OutlineTex", _renderTexture);
        Graphics.Blit(_renderTexture, temp);
        
        Graphics.Blit(source, destination, CompositeMat);
        
        RenderTexture.ReleaseTemporary(_renderTexture);
    }

    void MixRender(RenderTexture in_outerTexture, ref RenderTexture _renderTexture)
    {
        RenderTexture temp1 = RenderTexture.GetTemporary(in_outerTexture.width, in_outerTexture.height, 0);
        RenderTexture temp2 = RenderTexture.GetTemporary(in_outerTexture.width, in_outerTexture.height, 0);
        //用buffer1接收纹理
        Graphics.Blit(in_outerTexture, temp1);

        //多次模糊
        for(int i=0; i<iterations; i++)
        {
            //用temp2接受temp1使用blurMat模糊过的纹理
            FourTapCone(temp1, temp2 ,i);
            //又传递给temp1
            Graphics.Blit(temp2, temp1);
        }
        //将buffer1与初始纹理比较，剔除，得到轮廓
        //cutoffMat.SetTexture("_BlurTex", temp1);
        Graphics.Blit(in_outerTexture, temp2, CutoffMat);
        Graphics.Blit(temp2, _renderTexture);
        

        RenderTexture.ReleaseTemporary(temp1);
        RenderTexture.ReleaseTemporary(temp2);
    }

    float Speed = 0.8f;
    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = 0.5f + iteration * Speed;
        BlurMat.SetVector("_BlurOffset", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
        Graphics.BlitMultiTap(source, dest, BlurMat,
            new Vector2(off, off),
            new Vector2(-off, off),
            new Vector2(off, -off),
            new Vector2(-off, -off));
    }

    private void OnDestroy()
    {

    }
}
