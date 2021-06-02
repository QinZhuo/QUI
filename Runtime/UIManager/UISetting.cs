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
        protected async override void Awake()
        {
            base.Awake();
            foreach (var item in PanelList)
            {
                var ui = await UIManager.GetUI(item);
                ui?.ResetUI();
            }
        }
    }
}