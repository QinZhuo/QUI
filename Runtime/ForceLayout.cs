using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace QTool.UI
{
    public static class RectTransformExtend
    {
        public static Vector2 UpRightRectOffset(this RectTransform rectTransform)
        {
            return new Vector2(rectTransform.sizeDelta.x * (1 - rectTransform.pivot.x), rectTransform.sizeDelta.y * (1 - rectTransform.pivot.y));
        }
        public static Vector2 DownLeftRectOffset(this RectTransform rectTransform)
        {
            return new Vector2(rectTransform.sizeDelta.x * (rectTransform.pivot.x), rectTransform.sizeDelta.y * (rectTransform.pivot.y));
        }

        public static float Height(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.y;
        }
        public static float Width(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.x;
        }
        public static RectTransform RectTransform(this Transform transform)
        {
            return transform as RectTransform;
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
        public static float Right(this RectTransform rectTransform)
        {
            return rectTransform.transform.position.x + rectTransform.UpRightRectOffset().x;
        }
    }
    public class ForceLayout : MonoBehaviour
    {
        [System.Serializable]
        public class ActionEvent : UnityEvent
        {
        }
        public ActionEvent OnLayout;
        [ContextMenu("布局")]
        public void Layout()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(WaitLayout());
            }
        }
        public int DelayLayout = 1;
        IEnumerator WaitLayout()
        {
            for (int i = 0; i < DelayLayout; i++)
            {
                yield return null;
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
                OnLayout?.Invoke();

            }

        }

    }
}

