using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderObject : MonoBehaviour
{
    //public Mesh targetMesh;
    public Material targetMaterial;

    [System.NonSerialized]
    public Matrix4x4 localToWorldMatrix;

    void Init()
    {
        localToWorldMatrix = transform.localToWorldMatrix;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
