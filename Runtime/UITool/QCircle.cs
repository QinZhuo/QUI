using QTool.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    public static class MeshExtends
    {
        public static void AddUIPoint(this List<UIVertex> vertices, Rect rect, Vector2 v, Color color)
        {
            vertices.Add(new UIVertex
            {
                position = v,
                uv0 = new Vector2((v.x - rect.xMin) / rect.width, (v.y - rect.yMin) / rect.height),
                color = color,
            });
        }
        public static void AddUIPoints(this List<UIVertex> vertices, Rect rect, Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            vertices.AddUIPoint(rect, v1, color);
            vertices.AddUIPoint(rect, v2, color);
            vertices.AddUIPoint(rect, v3, color);
        }
        public static void DrawRectangle(this List<UIVertex> vertexs, Graphic graphic,Rect graphicRect,Rect drawRect)
        {
            var v1 = new Vector2(drawRect.xMin, drawRect.yMax);
            var v2 = new Vector2(drawRect.xMax, drawRect.yMax);
            var v3 = new Vector2(drawRect.xMax, drawRect.yMin);
            var v4 = new Vector2(drawRect.xMin, drawRect.yMin);
            vertexs.AddUIPoints(graphicRect, v1, v2, v3,graphic.color);
            vertexs.AddUIPoints(graphicRect, v3, v4, v1, graphic.color);
        }
        public static void DrawCircle(this List<UIVertex> vertexs, Graphic graphic, Rect graphicRect, Rect drawRect, int smooth, float lineWidth, float angle = 360,float start = 0)
        {
            var startAngle = start / 360 * Mathf.PI * 2;
            var endAngle =(start+ angle) / 360 * Mathf.PI * 2;
            float unit = Mathf.PI * 2 / smooth;
            float lineRate = lineWidth / drawRect.width;
            for (float unitStartAngle = startAngle; unitStartAngle < endAngle; unitStartAngle+=unit)
            {

                var unitEndAngle = unitStartAngle + unit;
                if (unitEndAngle > endAngle)
                {
                    unitEndAngle = endAngle;
                }
                var v1 = new Vector2(Mathf.Cos(unitStartAngle) * drawRect.width, Mathf.Sin(unitStartAngle) * drawRect.height) / 2;
                var v2 = new Vector2(Mathf.Cos(unitEndAngle) * drawRect.width, Mathf.Sin(unitEndAngle) * drawRect.height) / 2;
                if (lineWidth >= 0 && lineRate < 1)
                {
                    var v3 = v1 * (1 - lineRate / 1);
                    var v4 = v2 * (1 - lineRate / 1);
                    vertexs.AddUIPoints(graphicRect, v3 + drawRect.center, v2 + drawRect.center, v1 + drawRect.center, graphic.color);
                    vertexs.AddUIPoints(graphicRect, v3 + drawRect.center, v4 + drawRect.center, v2 + drawRect.center, graphic.color);
                }
                else
                {
                    var v3 = new Vector2(0, 0);
                    vertexs.AddUIPoints(graphicRect, v3 + drawRect.center, v2 + drawRect.center, v1 + drawRect.center, graphic.color);
                }
            }
        }
    }
    [RequireComponent(typeof(Image))]
    public class QCircle : QImageShapeEffect
    {

        [QName("线框", "VertexMode")]
        public float lineWidth = -1;
        [Range(0,360)]
        [QName("角度")]
        public float angle = 360;
		public float Angle
		{
			get => angle;
			set
			{
				angle = value;
				graphic.SetVerticesDirty();
				Fresh();
			}
		}
        public override void Fresh()
        {
            switch (modifyType)
            {
                case ModifyType.图片:
                    {
                        image.sprite = Resources.Load<Sprite>("QCircle512");
                        image.type = Image.Type.Filled;
						image.fillMethod = Image.FillMethod.Radial360;
						image.fillAmount = angle/360;
						//image.SetVerticesDirty();
                    }
                    break;
                case ModifyType.顶点:
                    {
                        if (image.sprite != null && image.sprite.name == "QCircle512")
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
            vertexs.DrawCircle(graphic, Rect, Rect, smooth, lineWidth, angle);
            return vertexs;
        }
    }
}
