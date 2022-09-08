using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using QTool.Inspector;
namespace QTool.UI
{
    public enum ModifyType
    {
        图片,
        顶点,
    }
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public  abstract class QImageEffect : BaseMeshEffect
    {
        public RectTransform rectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }
        public Image image
        {
            get
            {
                return graphic as Image;
            }
        }
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            Fresh();
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            Fresh();
        }
#endif
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
        public abstract void Fresh();
        public override void ModifyMesh(VertexHelper vh)
        {
          
        }
    }
    [RequireComponent(typeof(Image))]
    public abstract class QImageShapeEffect: QImageEffect
    {
        public bool VertexMode
        {
            get => modifyType == ModifyType.顶点;
        }
        [QName("形状模式")]
        public ModifyType modifyType = ModifyType.图片;
        [Range(3, 360)]
        [QName("顶点复杂度", "VertexMode")]
        public int smooth = 36;
        protected abstract List<UIVertex> Draw();
        public override void ModifyMesh(VertexHelper vh)
        {
            if (modifyType == ModifyType.顶点)
            {
                vh.Clear();
                vh.AddUIVertexTriangleStream(Draw());
            }
        }
    }
 
    public class QRectangle : QImageShapeEffect
    {
        [FormerlySerializedAs("circle")]
        [Range(0, 256)]
        [QName("圆角")]
        public float radius = 10;
        [QName("线框", "VertexMode")]
        public float lineWidth = -1;
        public override void Fresh()
        {
            switch (modifyType)
            {
                case ModifyType.图片:
                    {
                        
                        image.sprite = Resources.Load<Sprite>("QCircle512");
                        image.type = Image.Type.Sliced;
                        image.pixelsPerUnitMultiplier = radius <=0.1f  ? 0.1f : 256 / radius;
                        image.SetVerticesDirty();
                    }
                    break;
                case ModifyType.顶点:
                    {
                       if(image.sprite!=null&&image.sprite.name== "QCircle512")
                        {
                            image.sprite = null;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        protected override List<UIVertex> Draw()
        {
            List<UIVertex> vertexs = new List<UIVertex>();
            if (smooth < 3)
            {
                return vertexs;
            }
            var Rect = graphic.GetPixelAdjustedRect();
            var leftUp = new Rect(Rect.xMin, Rect.yMax- radius*2, radius * 2, radius * 2);
            vertexs.DrawCircle(graphic, Rect, leftUp, smooth, lineWidth,90,90);
            var leftDown = new Rect(Rect.xMin, Rect.yMin, radius * 2, radius * 2);
            vertexs.DrawCircle(graphic, Rect, leftDown, smooth, lineWidth,90,180);
            var rightDonw = new Rect(Rect.xMax - radius*2, Rect.yMin , radius * 2, radius * 2);
            vertexs.DrawCircle(graphic, Rect, rightDonw, smooth, lineWidth,90,270);
            var rightUp = new Rect(Rect.xMax - radius*2, Rect.yMax - radius*2, radius * 2, radius * 2);
            vertexs.DrawCircle(graphic, Rect, rightUp, smooth, lineWidth, 90);
            vertexs.DrawRectangle(graphic,Rect,new Rect(Rect.xMin + radius, Rect.yMin + radius, Rect.width - radius * 2, Rect.height - radius * 2));
            vertexs.DrawRectangle(graphic,Rect,new Rect(Rect.xMin + radius, Rect.yMax - radius, Rect.width - radius * 2,radius ));
            vertexs.DrawRectangle(graphic, Rect, new Rect(Rect.xMin + radius, Rect.yMin, Rect.width - radius * 2, radius));
            vertexs.DrawRectangle(graphic, Rect, new Rect(Rect.xMin, Rect.yMin+radius, radius, Rect.height -radius*2));
            vertexs.DrawRectangle(graphic, Rect, new Rect(Rect.xMax-radius, Rect.yMin + radius, radius, Rect.height - radius * 2));
            return vertexs;
        }
        
    }
}
   
  
