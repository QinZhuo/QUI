using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    [ExecuteAlways]
    public class QShaderUI : MonoBehaviour
    {
        public RectTransform rectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }
        private RectTransform _rectTransform;
        public Material material
        {
            get
            {
                if (_material == null)
                {
                    _material = GetComponent<Graphic>().material;
                }
                return _material;
            }
        }
        private Material _material;

        public bool UpdateSize = true;
        public string UISizeName = "_UISize";
        public bool RuntimeUpdate = false;
        private void Update()
        {
            if (!Application.isPlaying || RuntimeUpdate)
            {
                if (UpdateSize)
                {
                    material.SetVector(UISizeName, new Vector4(rectTransform.rect.size.x, rectTransform.rect.size.y));
                }
            }
        }
    }
}