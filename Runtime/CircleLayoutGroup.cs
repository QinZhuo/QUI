using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    public class CircleLayoutGroup : LayoutGroup
    {
        private Vector2 Center
        {
            get
            {
                return rectTransform.rect.size / 2;
            }
        }
        private Vector2 Size
        {
            get
            {
                return rectTransform.rect.size;
            }
        }
        public float startAngle = 0;
        public bool reverse = false;
        public float distance = 20;
        private Vector2[] childPositions = new Vector2[0];
        public override void CalculateLayoutInputVertical()
        {
            FreshCirclePos();
        }
        public override void SetLayoutHorizontal()
        {
        }
        public override void SetLayoutVertical()
        {
            SetChildren();
        }
        private void SetChildren()
        {
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                SetChildAlongAxis(child, 0, childPositions[i].x);
                SetChildAlongAxis(child, 1, childPositions[i].y);
            }
        }
        private void FreshCirclePos()
        {
            childPositions = new Vector2[rectChildren.Count];
            var start = (-90 + startAngle) / 360 * Mathf.PI * 2;
            var dis = distance == 0 ? 0 : distance / 360 * Mathf.PI * 2;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var angle = start + (reverse ? 1 : -1) * i * dis;
                childPositions[i] = Center + new Vector2(Mathf.Cos(angle) * Size.x, Mathf.Sin(angle) * Size.y) / 2 - rectChildren[i].sizeDelta / 2;
            }
        }
    }
}