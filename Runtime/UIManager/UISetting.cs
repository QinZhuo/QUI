using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTool.UI
{
    public class UISetting:InstanceBehaviour<UISetting>
    {
        [System.Serializable]
        public class UIInitSetting
        {
            public string uiKey;
            public bool show = false;
        }
        public List<UIInitSetting> UIList;
        protected override void Awake()
        {
            base.Awake();
            UIPanel.LoadOverRun(() =>
            {
                foreach (var item in UIList)
                {
                    var ui = UIManager.GetUI(item.uiKey);
                    if (ui != null && item.show)
                    {
                        ui.Show();
                    }
                }
            });
        }
    }
}