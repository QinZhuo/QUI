using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
    public static class QRectTransformUtility
    {
        public static Vector2 UpRightRectOffset(this RectTransform rectTransform)
        {
            return new Vector2(rectTransform.Width() * (1 - rectTransform.pivot.x), rectTransform.Height() * (1 - rectTransform.pivot.y));
        }
        public static Vector2 DownLeftRectOffset(this RectTransform rectTransform)
        {
            return new Vector2(rectTransform.Width() * (rectTransform.pivot.x), rectTransform.Height() * (rectTransform.pivot.y));
        }

        public static float Height(this RectTransform rectTransform)
        {
            return rectTransform.rect.size.y;
        }
        public static float Width(this RectTransform rectTransform)
        {
            return rectTransform.rect.size.x;
        }
        public static Vector2 Size(this RectTransform rectTransform)
        {
            return rectTransform.rect.size;
        }
        public static float ScaleHeight(this RectTransform rectTransform)
        {
            return rectTransform.rect.size.y*rectTransform.lossyScale.y;
        }
        public static float ScaleWidth(this RectTransform rectTransform)
        {
            return rectTransform.rect.size.x * rectTransform.lossyScale.x;
        }
        public static Vector2 ScaleSize(this RectTransform rectTransform)
        {
            return rectTransform.rect.size.Mult(rectTransform.lossyScale);
        }
        public static bool IsOutRange(this RectTransform transform, RectTransform mask)
        {
            if (transform.Left() > mask.Right() || transform.Right() < mask.Left())
            {
                return true;
            }
            else if (transform.Up() < mask.Down() || transform.Down() > mask.Up())
            {
                return true;
            }
            return false;
        }

        public static bool IsXOutRange(this Vector2 leftRight, RectTransform mask)
        {
            if (leftRight.x > mask.Right() || leftRight.y < mask.Left())
            {
                return true;
            }
            return false;
        }
        public static bool IsYOutRange(this Vector2 DonwUp, RectTransform mask)
        {
            if (DonwUp.x > mask.Up() || DonwUp.y < mask.Down())
            {
                return true;
            }
            return false;
        }

        public static RectTransform RectTransform(this Transform transform)
        {
            return transform as RectTransform;
        }
        public static Vector2 Mult(this Vector2 a,Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        public static Vector2 Div(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }
        public static Vector2 Center(this RectTransform rectTransform)
        {
            return new Vector2( rectTransform.position.x,rectTransform.position.y)+ Mult(rectTransform.ScaleSize(),Vector2.one*0.5f- rectTransform.pivot);
        }
        public static float Up(this RectTransform rectTransform)
        {
            return rectTransform.transform.position.y + rectTransform.UpRightRectOffset().y;
        }
        public static float Down(this RectTransform rectTransform)
        {
            return rectTransform.transform.position.y - rectTransform.DownLeftRectOffset().y;
        }
        public static float Left(this RectTransform rectTransform)
        {
            return rectTransform.transform.position.x - rectTransform.DownLeftRectOffset().x;
        }
        public static bool HasParentIs(this Transform transform,Transform target)
        {
            if (transform.parent == null)
            {
                return false;
            }
            else if (transform.parent == target)
            {
                return true;
            }
            else
            {
                return transform.parent.HasParentIs(target);
            }
        }
        public static float Right(this RectTransform rectTransform)
        {
            return rectTransform.transform.position.x + rectTransform.UpRightRectOffset().x;
        }
    }
}
