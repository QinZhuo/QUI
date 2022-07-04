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
		[ViewName("初始化UI")]
		public List<string> PanelList;
		[ViewName("摧毁UI")]
		public List<string> DestoryList;
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
			foreach (var uiKey in DestoryList)
			{
				QUIManager.Destory(uiKey);
			}
			foreach (var uiKey in PanelList)
            {
                var ui = await QUIManager.GetUI(uiKey);
                ui?.ResetUI();
            }
            curList.Clear();
            curList.AddRange(PanelList);
		
            InitOver = true;
        }
    }
}
