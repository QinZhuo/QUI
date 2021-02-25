using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class QRectangle :MonoBehaviour
{
    public Image image;
    public float circle=10;
    protected void Reset()
    {
        image = GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("QCircle512");
        image.type = Image.Type.Sliced;
        FreshCircle();
    }
    public void FreshCircle()
    {
        image.pixelsPerUnitMultiplier = circle == 0 ? 0.0001f : 256 / circle;
        image.SetVerticesDirty();
    }
    protected void OnValidate()
    {
        FreshCircle();
    }
}
   
  
