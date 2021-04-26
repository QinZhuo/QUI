using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTool.UI.Manager
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
                    UIManager.Get(item);
                }
            });
        }
    }
}