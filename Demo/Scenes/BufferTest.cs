using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ExecuteAlways]
public class BufferTest : MonoBehaviour, IMeshModifier
{
    protected  void OnEnable()
    {
        _mpb = new MaterialPropertyBlock();
        _cb = new CommandBuffer();
    }

    private MaterialPropertyBlock _mpb;
    private CommandBuffer _cb;
    Mesh _mesh;
    Mesh mesh
    {
        get { return _mesh ? _mesh : _mesh = new Mesh() { hideFlags = HideFlags.HideAndDontSave }; }
    }
    Material _material;
    public RenderTexture softMaskBuffer;

    Material material
    {
        get
        {
            return _material
                ? _material
                : _material =
                    new Material( Resources.Load<Shader>("SoftMask"))
                    { hideFlags = HideFlags.HideAndDontSave };
        }
    }
    void Update()
    {
       // _cb.Clear();
        _cb.SetRenderTarget(softMaskBuffer);
        _cb.ClearRenderTarget(false, true, Color.black);
        var c = GetComponent<Image>().canvas.rootCanvas;
        var cam = c.worldCamera ?? Camera.main;
        if (c && c.renderMode != RenderMode.ScreenSpaceOverlay && cam)
        {
            var p = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
            _cb.SetViewProjectionMatrices(cam.worldToCameraMatrix, p);
        }
        _cb.DrawMesh(mesh, transform.localToWorldMatrix, material, 0, 0, _mpb);
    }

    public void ModifyMesh(Mesh mesh)
    {
        
    }

    public void ModifyMesh(VertexHelper verts)
    {
        if (isActiveAndEnabled)
        {
            verts.FillMesh(mesh);
        }

    }
}
