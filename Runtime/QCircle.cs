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
    }
    public class QCircle : BaseMeshEffect
    {
        [Range(0, 1)]
        public float radius = 1;
        [Range(3, 90)]
        public int smooth = 30;
        [Range(0, 1)]
        public float lineWidth = 1;
        private List<UIVertex> Draw()
        {
            List<UIVertex> vertexs = new List<UIVertex>();
            if (smooth <3)
            {
                return vertexs;
            }
            var Rect = graphic.GetPixelAdjustedRect();
            float unit = 3.14f * 2 / smooth;
            for (int i = 0; i < smooth; i++)
            {
                var angle = unit * i;
                var v1 = new Vector2(Mathf.Cos(angle) * Rect.width, Mathf.Sin(angle) * Rect.height) * radius / 2;
                var v2 = new Vector2(Mathf.Cos(angle + unit) * Rect.width, Mathf.Sin(angle + unit) * Rect.height) * radius / 2;
                if (lineWidth > 0 && lineWidth < radius)
                {
                    var v3 = v1 * (1 - lineWidth / radius);
                    var v4 = v2 * (1 - lineWidth / radius);
                    vertexs.AddUIPoints(Rect, v3, v2, v1, graphic.color);
                    vertexs.AddUIPoints(Rect, v3, v4, v2, graphic.color);
                }
                else
                {
                    var v3 = new Vector2(0, 0);
                    vertexs.AddUIPoints(Rect, v3, v2, v1, graphic.color);
                }
            }
            return vertexs;
        }
        public override void ModifyMesh(VertexHelper vh)
        {
            vh.Clear();
            vh.AddUIVertexTriangleStream(Draw());
        }
    }
}