using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlinePostProcess : PostProcess
{
    private Camera mainCam = null;
    private Camera additionalCam = null;
    private RenderTexture renderTexture = null;

    public Shader outlineShader = null;

    //采样率  
    public float samplerScale = 1;
    public int downSample = 1;
    public int iterations = 3;
    public Color outlineColor = Color.green;

    private void Awake()
    {
        InitAdditionalCam();
    }

    private void InitAdditionalCam()
    {
        mainCam = this.GetComponent<Camera>();
        if (mainCam == null)
            return;

        Transform addCamTransform = transform.Find("additionalCam");
        if (addCamTransform != null)
            DestroyImmediate(addCamTransform.gameObject);

        GameObject additionalCamObj = new GameObject("additionalCam");
        additionalCam = additionalCamObj.AddComponent<Camera>();

        SetAdditionalCam();
    }

    private void SetAdditionalCam()
    {
        if (additionalCam)
        {
            additionalCam.transform.parent = mainCam.transform;
            additionalCam.transform.localPosition = Vector3.zero;
            additionalCam.transform.localRotation = Quaternion.identity;
            additionalCam.transform.localScale = Vector3.one;
            additionalCam.farClipPlane = mainCam.farClipPlane;
            additionalCam.nearClipPlane = mainCam.nearClipPlane;
            additionalCam.fieldOfView = mainCam.fieldOfView;
            additionalCam.backgroundColor = Color.clear;
            additionalCam.clearFlags = CameraClearFlags.Color;
            additionalCam.cullingMask = 1 << LayerMask.NameToLayer("dragon");
            additionalCam.depth = -999;
            if (renderTexture == null)
                renderTexture = RenderTexture.GetTemporary(additionalCam.pixelWidth >> downSample, additionalCam.pixelHeight >> downSample, 0);
        }
    }

    private void OnEnable()
    {
        SetAdditionalCam();
        additionalCam.enabled = true;
    }

    private void OnDisable()
    {
        additionalCam.enabled = false;
    }

    private void OnDestroy()
    {
        if (renderTexture)
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }
        DestroyImmediate(additionalCam.gameObject);
    }

    private void OnPreRender()
    {
        if (additionalCam.enabled)
        {
            if (renderTexture != null && (renderTexture.width != Screen.width >> downSample || renderTexture.height != Screen.height >> downSample))
            {
                RenderTexture.ReleaseTemporary(renderTexture);
                renderTexture = RenderTexture.GetTemporary(Screen.width >> downSample, Screen.height >> downSample, 0);
            }
            additionalCam.targetTexture = renderTexture;
            additionalCam.RenderWithShader(outlineShader, "");
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(_Material && renderTexture)
        {
            RenderTexture temp1 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0);
            RenderTexture temp2 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0);
            
            //_Material.SetTexture("_MainTex", renderTexture);
            Graphics.Blit(renderTexture, temp1);

            //for (int i = 0; i < iterations; i++)
            //{
                _Material.SetFloat("_BlurSize", 1.0f);
                Graphics.Blit(temp1, temp2, _Material, 0);

                Graphics.Blit(temp2, temp1, _Material, 1);
            //}

            _Material.SetTexture("_BlurTex", temp1);
            _Material.SetTexture("_SrcTex", renderTexture);
            _Material.SetColor("_OutlineColor", outlineColor);
            Graphics.Blit(source, destination, _Material, 2);
            //Graphics.Blit(temp1, destination);

            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);

        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
