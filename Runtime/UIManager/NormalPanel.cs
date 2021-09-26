using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
    public class NormalPanel : UIPanel<NormalPanel>
    {
        protected override void OnShow()
        {
            base.OnShow();
            Debug.Log("ÏÔÊ¾Panel:¡¾" + this + "¡¿");
        }
        protected override void OnHide()
        {
            base.OnHide();
            Debug.Log("Òþ²ØPanel:¡¾" + this + "¡¿");
        }
    }
}

