using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
//一般绑定在相机上
[RequireComponent(typeof(Camera))]
public class PostProcess : MonoBehaviour
{
    public Shader shader = null;
    private Material _material = null;
    public Material _Material
    {
        get
        {
            if (_material == null)
                _material = GenerateMaterial(shader);
            return _material;
        }
    }

    protected Material GenerateMaterial(Shader shader)
    {
        if(shader == null)        
            return null;
        if (!shader.isSupported)
            return null;
        Material material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        if (material)
            return material;
        return material;  
    }
}
