using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
    public class UIDebug:DebugBase<UIDebug>
    {

    }
    public class NormalPanel : UIPanel<NormalPanel>
    {
        protected override void OnShow()
        {
            base.OnShow();
            UIDebug.Log("ÏÔÊ¾Panel:¡¾" + this + "¡¿");
        }
        protected override void OnHide()
        {
            base.OnHide();
            UIDebug.Log("Òþ²ØPanel:¡¾" + this + "¡¿");
        }
    }
}

