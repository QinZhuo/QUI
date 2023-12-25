using QTool.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    [RequireComponent(typeof(Image))]
    public class QCircle : QImageEffect
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
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	public abstract class QImageEffect : BaseMeshEffect
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
		#region Editor
#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			OnFresh();
		}
		protected override void OnValidate()
		{
			base.OnValidate();
			OnFresh();
		}
#endif
		#endregion
		[Range(3, 360)]
		[QName("圆角复杂度")]
		public int smooth = 36;
		protected override void OnEnable()
		{
			base.OnEnable();
			Canvas.willRenderCanvases += OnFresh;
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			Canvas.willRenderCanvases -= OnFresh;
		}
		public virtual void OnFresh() { }
		protected abstract List<UIVertex> Draw();
		public override void ModifyMesh(VertexHelper vh)
		{
			vh.Clear();
			vh.AddUIVertexTriangleStream(Draw());
		}
	}
	public static class QUIVertexTool
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
		public static void DrawRectangle(this List<UIVertex> vertexs, Graphic graphic, Rect graphicRect, Rect drawRect)
		{
			var v1 = new Vector2(drawRect.xMin, drawRect.yMax);
			var v2 = new Vector2(drawRect.xMax, drawRect.yMax);
			var v3 = new Vector2(drawRect.xMax, drawRect.yMin);
			var v4 = new Vector2(drawRect.xMin, drawRect.yMin);
			vertexs.AddUIPoints(graphicRect, v1, v2, v3, graphic.color);
			vertexs.AddUIPoints(graphicRect, v3, v4, v1, graphic.color);
		}
		public static void DrawCircle(this List<UIVertex> vertexs, Graphic graphic, Rect graphicRect, Rect drawRect, int smooth, float lineWidth, float angle = 360, float start = 0)
		{
			var startAngle = start / 360 * Mathf.PI * 2;
			var endAngle = (start + angle) / 360 * Mathf.PI * 2;
			float unit = Mathf.PI * 2 / smooth;
			float lineRate = lineWidth / drawRect.width;
			for (float unitStartAngle = startAngle; unitStartAngle < endAngle; unitStartAngle += unit)
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

}
