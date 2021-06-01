using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.Inspector;
namespace QTool.UI
{
    public class UISetting:InstanceBehaviour<UISetting>
    {
        [ViewName("UI≈‰÷√")]
        public List<string> PanelList;
        protected override void Awake()
        {
            base.Awake();
            UIPanel.LoadOverRun(() =>
            {
                foreach (var item in PanelList)
                {
                    var ui= UIManager.GetUI(item);
                    ui?.ResetUI();
                }
            });
        }
    }
}