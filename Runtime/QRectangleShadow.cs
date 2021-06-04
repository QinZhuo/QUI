using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QTool.Inspector;
using UnityEngine.Serialization;
namespace QTool.UI
{

 
    public class QRectangleShadow : QImageEffect
    {

        [Range(1, 500)]
        [ViewName("阴影范围")]
        public float size=50;
        [Range(1,256)]
        [ViewName("圆角")]
        public float radius = 0;
        public override void Fresh()
        {
            image.sprite = Resources.Load<Sprite>("QCircleShadow");
            image.type = Image.Type.Sliced;
        
            rectTransform.localScale = new Vector3((1+(size/rectTransform.Width()*2)), 1 + (size / rectTransform.Height()*2));
            image.pixelsPerUnitMultiplier = 256 / (10+radius <= 1f ?  1f:10+  radius);
            image.SetVerticesDirty();
        }
    }
}
   
  
