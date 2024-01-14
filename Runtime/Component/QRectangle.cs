using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using QTool.Inspector;
namespace QTool.UI
{

    public class QRectangle : QImageEffect
	{
        [FormerlySerializedAs("circle")]
        [Range(0, 256)]
        [QName("圆角")]
        public float radius = 10;
        [QName("线框", "VertexMode")]
        public float lineWidth = -1;
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


