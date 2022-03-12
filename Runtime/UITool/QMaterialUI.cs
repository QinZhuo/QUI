using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QMaterialUI : MonoBehaviour
{
    public RectTransform Rect => transform as RectTransform;
    public string rectInfoKey;
    Material _mat;
    public bool freshOnUpdate = false;
    public Material Mat
    {
        get
        {
            if (_mat == null)
            {
                var ui = GetComponent<MaskableGraphic>();

                if (ui.material == null) return null;
                if (!ui.material.name.Contains("temp_"))
                {
                    if (Application.isPlaying)
                    {
                        var temp = new Material(ui.material);
                        temp.name = "temp_" + temp.GetHashCode();
                        ui.material = temp;

                    }
                }
                _mat = ui.material;
            }
            return _mat;
        }
    }
    private void Awake()
    {
        Fresh();
    }
    public void Fresh()
    {
        if (Mat == null) return;
        if (!string.IsNullOrWhiteSpace(rectInfoKey))
        {
            Mat.SetVector(rectInfoKey, new Vector4(Rect.sizeDelta.x, Rect.sizeDelta.y, Rect.position.x, Rect.position.y));
        }
    }
 
    void Update()
    {
        if (freshOnUpdate)
        {
            Fresh();
        }
    }
}
