using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlinePostProcess : MonoBehaviour
{
    public Camera outlineCamera;

    #region 纯色材质
    public Shader pureColorShader;
    private Material m_pureColor = null;
    private Material pureMat
    {
        get
        {
            if (m_pureColor = null)
                m_pureColor = new Material(pureColorShader);
            return m_pureColor;
        }
    }
    #endregion

    #region 合并材质
    public Shader compositeShader;
    private Material m_composite = null;
    private Material compositeMat
    {
        get
        {
            if (m_composite = null)
                m_composite = new Material(compositeShader);
            return m_composite;
        }
    }
    #endregion
    private void OnPreRender()
    {
        
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
    }
    private void Start()
    {
        
    }
}
