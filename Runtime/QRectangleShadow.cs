using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
namespace QTool.UI
{

 
    public class QRectangleShadow : QImageEffect
    {
    
        public float lineWidth = -1;
        public float size=10;
        public float shadow = 20;
        public override void Fresh()
        {
            image.sprite = Resources.Load<Sprite>("QCircleShadow");
            image.type = Image.Type.Sliced;
        
            rectTransform.localScale = new Vector3((1+(size/rectTransform.Width())), 1 + (size / rectTransform.Height()));
            image.pixelsPerUnitMultiplier = 0.5f+(1 / size)*(512f/ shadow);
            image.SetVerticesDirty();
        }
    }
}
   
  
