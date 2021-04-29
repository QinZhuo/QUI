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
        RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
        public QMask mask;
        Material _mat;
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            return _mat ?? (_mat = new Material(Shader.Find("QUI/QMask")));
        }

        protected override void Reset()
        {
            base.Reset();
            mask = GetComponentInParent<QMask>();
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
            if (mask == null)
            {
                mask = GetComponentInParent<QMask>();
            }
            // Debug.LogError(transform.RectTransform().Center());
          
             var tilling = rectTransform.rect.size.Div(mask.rectTransform.rect.size);
            var offset = (rectTransform.Center() - mask.rectTransform.Center()).Mult(tilling)-(rectTransform.rect.size+mask.rectTransform.rect.size)/2;
            //Debug.LogError(offset);
            //  Debug.LogError(GetComponent<Graphic>().materialForRendering.shader.name);
            _mat.SetTexture("_Mask", mask.graphic.mainTexture);
            _mat.SetVector("_MaskTillingOffset", new Vector4(tilling.x, tilling.y, offset.x/rectTransform.Width(), offset.y/rectTransform.Height()));
           
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