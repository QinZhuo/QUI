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
    public class QMaskTarget : UIBehaviour, IMaterialModifier,ICanvasElement
    {
        public bool MaskActive
        {
            get
            {
                return mask != null && IsActive() ;
            }
        }
        RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
        public QMask mask;
        Material Mat
        {
            get
            {
                return _mat ?? (_mat = new Material(Shader.Find("QUI/QMaskUI")));
            }
        }
        Material _mat;
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            return MaskActive? Mat:baseMaterial;
        }
        Graphic _graphic;
        public Graphic graphic
        {
            get
            {
                return _graphic ?? (_graphic = GetComponent<Graphic>());
            }
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
            if (graphic != null)
            {
                graphic.SetMaterialDirty();
            }
            Fresh();
            Canvas.willRenderCanvases += Fresh;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            if (graphic != null)
            {
                graphic.SetMaterialDirty();
            }
            Canvas.willRenderCanvases -= Fresh;
        }
        private void Update()
        {
            Fresh();
        }
        private void OnTransformChildrenChanged()
        {
            if (MaskActive)
            {
                mask.CheckChildrenMaskTarget();
            }
        }
        private void Fresh()
        {
            if (!MaskActive) return;
            if (mask == null)
            {
                mask = GetComponentInParent<QMask>();
            }

            var tilling =  rectTransform.rect.size.Div(mask.rectTransform.rect.size);
            var offset = (rectTransform.Center() - mask.rectTransform.Center()).Mult(tilling);
            offset -= (rectTransform.rect.size).Mult((tilling-Vector2.one)/2);
            offset = offset.Div(rectTransform.rect.size);
            Mat.SetTexture("_Mask", mask.graphic.mainTexture);
            Mat.SetVector("_MaskTillingOffset", new Vector4(tilling.x, tilling.y, offset.x, offset.y));
            Mat.SetFloat("_Reverse", mask.Reverse?1:0);

        }
        public bool RootMaskIs(RectTransform rootMask)
        {
            var trans = transform;
            while (trans.parent!=null)
            {
                if (trans.parent == rootMask)
                {
                    return true;
                }
                trans = trans.parent;
            }
            return false;
        }
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            if (!RootMaskIs(mask.MaskRoot))
            {
                mask.Remove(this);
            }
            else
            {
                Fresh();
            }
           
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            Fresh();

        }

        public void Rebuild(CanvasUpdate executing)
        {
          
        }

        public void LayoutComplete()
        {
        }

        public void GraphicUpdateComplete()
        {
         
        }

       

     
      
    }
}