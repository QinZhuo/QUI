using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.Inspector;
namespace QTool.UI
{
    public class UISetting:InstanceBehaviour<UISetting>
    {
        public static List<string> curList = new List<string>();
        [ViewName("UI≈‰÷√")]
        public List<string> PanelList;
        public bool InitOver=false;
        protected async  void Start()
        {
            InitOver = false;
          //  base.Awake();
            foreach (var item in PanelList)
            {
               // if (curList.Contains(item)) continue;
                var ui = await UIManager.GetUI(item);
                ui?.ResetUI();
            }
            curList.Clear();
            curList.AddRange(PanelList);
            InitOver = true;
        }
    }
}