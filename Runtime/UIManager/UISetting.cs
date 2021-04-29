using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTool.UI
{
    public class UISetting:MonoBehaviour
    {
        public List<string> PanelList;
        private void Awake()
        {
            UIPanel.LoadOverRun(() =>
            {
                foreach (var item in PanelList)
                {
                    var ui= UIManager.Get(item).GetComponent<IUIPanel>();
                    ui?.Show();
                }
            });
        }
    }
}