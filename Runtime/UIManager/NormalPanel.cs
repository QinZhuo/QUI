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
            Debug.Log("��ʾPanel:��" + this + "��");
        }
        protected override void OnHide()
        {
            base.OnHide();
            Debug.Log("����Panel:��" + this + "��");
        }
    }
}

