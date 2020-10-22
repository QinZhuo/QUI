using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{

    public class QLerpHideUI : BaseMeshEffect
    {
        public enum HideDir
        {
            UpDonw,
            LeftRight,
        }
        public void Fresh()
        {
            graphic.SetVerticesDirty();
        }
        public HideDir hideDir = HideDir.UpDonw;
        public float MaxOffset = 0;
        public float MinOffset = 0;
        public float HideOffset =50;
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        public RectTransform Rect
        {
            get
            {
                return transform as RectTransform;
            }
        }
        public float Length
        {
            get
            {
                return (HideDir.UpDonw == hideDir ?Rect.Height() : Rect.Width());
            }
        }
        float HideRate
        {
            get
            {
                return HideOffset / Length;
            }
        }
        float MaxRate
        {
            get
            {
                return MaxOffset / Length;
            }
        }
        float MinRate
        {
            get
            {
                return MinOffset / Length;
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
        float GetValue(UIVertex v, int index)
        {
            switch (hideDir)
            {
                case HideDir.UpDonw:
                    return v.position.y;
                case HideDir.LeftRight:
                    return v.position.x;
                default:
                    return 0;
            }
         
        }
        public Color ChangeColor(float t, Color oldColor)
        {

            if (t < MinRate)
            {



                return new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.Lerp(1, 0, (MinRate - t / HideRate)));
            }
            else if (t > 1 - MaxRate)
            {
                return new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.Lerp(1, 0, ((t - (1 - MaxRate)) / HideRate)));
            }
            else
            {
                return oldColor;
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
           
                var newColor = ChangeColor(t, v.color);
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
