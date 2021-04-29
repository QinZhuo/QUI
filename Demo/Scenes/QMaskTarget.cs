using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using QTool;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace QTool.UI
{
    [ExecuteAlways]
    public class QMaskTarget : UIBehaviour, IMaterialModifier,ICanvasElement, IMeshModifier
    {
        public RenderTexture renderTexture;
        CommandBuffer _buffer;
        public CommandBuffer buffer
        {
            get
            {
                if (_buffer == null)
                {
                    _buffer = new CommandBuffer { name = "MaskBuffer" };
                    var p = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
                    _buffer.SetViewProjectionMatrices(Camera.main.worldToCameraMatrix, p);
                }
                return _buffer ;
            }
        }
        RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
        public RectTransform mask;
        Material _mat;
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            return _mat ?? (_mat = new Material(Shader.Find("QUI/QMask")));
        }

      
        protected override void Awake()
        {
            base.Awake();
            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Canvas.willRenderCanvases += Fresh;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            Canvas.willRenderCanvases -= Fresh;
        }
        protected override void OnValidate()
        {
            base.OnValidate(); Debug.LogError("OnValidate");

        }
        private void Update()
        {
            Fresh();
        }
        private void Fresh()
        {
           // Debug.LogError(transform.RectTransform().Center());
            var offset = (transform.RectTransform().Center()-mask.RectTransform().Center()  );
            //Debug.LogError(offset);
            //  Debug.LogError(GetComponent<Graphic>().materialForRendering.shader.name);
            GetComponent<Graphic>().materialForRendering.SetVector("_MaskOffset", new Vector4(offset.x/rectTransform.Width(), offset.y/rectTransform.Height()));
            //   Debug.LogError("Fresh");
         
            buffer.Clear();
            buffer.SetRenderTarget(renderTexture);
            buffer.ClearRenderTarget(true, true, Color.clear);
            var image = GetComponent<Image>();
            //  buffer.ClearRenderTarget(true, true, clearColor);
          //  if (Mesh.vertices.Length > 0)
            {

                buffer.DrawMesh(Mesh, transform.localToWorldMatrix, image.materialForRendering);
            }
        }


        //protected override void OnRectTransformDimensionsChange()
        //{
        //    base.OnRectTransformDimensionsChange();
        //}

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            Fresh();
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            Fresh();

        }

        public void Rebuild(CanvasUpdate executing)
        {
            Fresh();
        }

        public void LayoutComplete()
        {
        }

        public void GraphicUpdateComplete()
        {
          //  throw new System.NotImplementedException();
        }

        public void RecalculateClipping()
        {
            Fresh();
        }

        public void Cull(Rect clipRect, bool validRect)
        {
            //throw new System.NotImplementedException();
        }

        public void SetClipRect(Rect value, bool validRect)
        {
            Fresh();
            //throw new System.NotImplementedException();
        }

        public void SetClipSoftness(Vector2 clipSoftness)
        {
            Fresh();
        }

        public void ModifyMesh(Mesh mesh)
        {
           
        }
        Mesh _mesh;
        public Mesh Mesh
        {
            get
            {
                return _mesh ?? (_mesh = new Mesh());
            }
        }
        public void ModifyMesh(VertexHelper verts)
        {
            verts.FillMesh(Mesh);
            Debug.LogError("mesh " + Mesh.vertices.Length);
        }
    }
}