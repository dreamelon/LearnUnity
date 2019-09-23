using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : PostProcess
{
    private Camera mainCam = null;
    private Camera additionalCam = null;
    private RenderTexture renderTexture = null;

    public Shader outlineShader = null;

    //采样率  
    public float samplerScale = 1;
    public int downSample = 1;
    public int iteration = 2;

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
                additionalCam.targetTexture = renderTexture;
                additionalCam.RenderWithShader(outlineShader, "");
            }
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
