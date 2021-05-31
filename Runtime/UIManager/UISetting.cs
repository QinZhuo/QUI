using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTool.UI
{
    public class UISetting:InstanceBehaviour<UISetting>
    {
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