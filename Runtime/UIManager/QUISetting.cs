using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.Inspector;
using System.Threading.Tasks;

namespace QTool.UI
{
	public class QUISetting : InstanceBehaviour<QUISetting>
	{
		public static List<string> curList = new List<string>();
		[ViewName("UI配置")]
		public List<string> PanelList;
		public bool InitOver { private set; get; } = false;
		public static async Task<bool> WaitInitOver()
		{
			if (Instance == null)
			{
				return true;
			}
			else
			{
				return await Tool.Wait(() => Instance.InitOver);
			}
		}
        protected async  void Start()
        {
            InitOver = false;
          //  base.Awake();
            foreach (var item in PanelList)
            {
               // if (curList.Contains(item)) continue;
                var ui = await QUIManager.GetUI(item);
                ui?.ResetUI();
            }
            curList.Clear();
            curList.AddRange(PanelList);
            InitOver = true;
        }
    }
}
