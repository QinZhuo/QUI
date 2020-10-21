using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{

    public class QGradientUI : BaseMeshEffect
    {
        public enum LerpDir
        {
            TopDown,
            LeftRight,
            TextCount,
        }
        public LerpDir lerpDir = LerpDir.TextCount;
        [SerializeField]
        private Gradient _gradientColor = new Gradient();
        public Gradient GradientColor
        {
            get
            {
                return _gradientColor;
            }
            set
            {
                if (graphic != null)
                {
                    Fresh();
                    _gradientColor = value;
                }
            }
        }
        public void SetAlphaPos(float t,float delay, params int[] index)
        {
            if (index.Length == 0) return;
            var alphaKeys = GradientColor.alphaKeys;

            for (int i = 0; i < index.Length; i++)
            {

                alphaKeys[index[i]].time += t+delay*i;
            }
            GradientColor.SetKeys(GradientColor.colorKeys, alphaKeys); ;
            Fresh();
        }
        public void Fresh()
        {
            graphic.SetVerticesDirty();
        }
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        float GetValue(UIVertex v, int index)
        {
            switch (lerpDir)
            {
                case LerpDir.TopDown:
                    return v.position.y;
                case LerpDir.LeftRight:
                    return v.position.x;
                case LerpDir.TextCount:
                    return index;
                default:
                    return 0;
            }
        }
        void Init(VertexHelper vh)
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                var value = GetValue(v, i);
                if (value > maxValue)
                {
                    maxValue = value;
                }
                if (value < minValue)
                {
                    minValue = value;
                }
            }
        }
        UIVertex v = new UIVertex();
        public void LerpColor(VertexHelper vh)
        {
            var length = maxValue - minValue;

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                var t = (GetValue(v, i) - minValue) / length;
                var newColor = GradientColor.Evaluate(t);
                if (v.color != newColor)
                {
                    v.color = newColor;
                    vh.SetUIVertex(v, i);
                }
            }

        }
        public override void ModifyMesh(VertexHelper vh)
        {

            if (!IsActive() || vh.currentVertCount == 0)
            {
                return;
            }
            Init(vh);
            LerpColor(vh);
        }


    }
}
