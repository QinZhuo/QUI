﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            return rectTransform.rect.height;
        }
        public static float Width(this RectTransform rectTransform)
        {
            return rectTransform.rect.width;
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
    [ExecuteInEditMode]
    public class PosControlHide : MonoBehaviour
    {
        public QLerpHideUI.HideDir hideDir = QLerpHideUI.HideDir.UpDonw;
        public QLerpHideUI[] hideUI;
        public float Max;
        public float Min;
        public void Reset()
        {
            hideUI = GetComponentsInChildren<QLerpHideUI>();
        }
        public RectTransform Rect
        {
            get
            {
                return transform as RectTransform;
            }
        }
        private void Update()
        {
            foreach (var ui in hideUI)
            {
                if (ui == null) continue;
                Debug.LogError(ui.Rect.Up() + "  : " + Rect.Up());
                ui.MaxOffset = (hideDir == QLerpHideUI.HideDir.UpDonw ? (ui.Rect.Up() - Rect.Up()) : (ui.Rect.Right() - Rect.Right())) - Max;
                ui.MinOffset = (hideDir == QLerpHideUI.HideDir.UpDonw ? (Rect.Down() - ui.Rect.Down()) : (Rect.Left() - ui.Rect.Left())) - Min;
                ui.Fresh();
            }
        }
    }
}

