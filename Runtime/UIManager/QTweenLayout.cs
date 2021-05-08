﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.UI;
#if QTween
namespace QTool.Tween.Component
{
    [RequireComponent(typeof( QElementList))]
    public class QTweenLayout: QTweenList
    {
        public Tween.QTweenList.TweenListType listType = Tween.QTweenList.TweenListType.顺序播放;
        public QElementList layout;

        private void Reset()
        {
            layout = GetComponent<QElementList>();
        }
  
        private void Start()
        {
            layout.OnCreate += (view) =>
            {
                if (view != null)
                {
                    var node = new QTweenlistNode(view.GetComponent<QTweenBehavior>()) { type = listType };
                    node.FreshName();
                    tweenList.Add(node);
                    ClearAnim();
                }
            };
            layout.OnPush += (view) =>
            {
               var tweenNode= tweenList.Get(view.GetComponent<QTweenBehavior>(), (obj) => obj.qTween);
                if (tweenNode != null)
                {
                    tweenList.Remove(tweenNode);
                    ClearAnim();
                }
            };
            //layout.OnClear += () =>
            //{
            //    ClearAnim();
            ////   lastTween.nextUIShow = null;
            //};
        }
    }
}
#endif