using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRPCamera : MonoBehaviour
{
    public RenderTexture rt;
    public Transform[] cubeTrans;
    public Mesh cubeMesh;
    public Material pureColorMaterial;
    public SkyBoxDraw skybox;

    // Start is called before the first frame update
    void Start()
    {
        rt = new RenderTexture(Screen.width, Screen.height, 24);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPostRender()
    {
        Camera cam = Camera.current;
        Graphics.SetRenderTarget(rt);
        GL.Clear(true, true, Color.grey);
        //start dc
        pureColorMaterial.color = new Color(0, 0.5f, 0.8f);
        pureColorMaterial.SetPass(0);
        foreach (var i in cubeTrans)
        {
            Graphics.DrawMeshNow(cubeMesh, i.localToWorldMatrix);
        }
        skybox.DrawSkybox(cam, rt);
        //end dc
        Graphics.Blit(rt, cam.targetTexture);
    }
}

