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
		[QName("摧毁UI")]
		public List<string> DestoryList = new List<string>();
		[QName("初始化UI")]
		public List<string> PanelList = new List<string>();
		public bool InitOver { private set; get; } = false;
		public static async Task<bool> WaitInitOver()
		{
			if (Instance == null)
			{
				return true;
			}
			else
			{
				await QTask.Wait(() => Instance.InitOver);
				return true;
			}
		} 
        protected async void Start()
        {
            InitOver = false;
			foreach (var uiKey in DestoryList)
			{
				QUIManager.Destory(uiKey);
			}
			List<Task<UIPanel>> uiTaskList = new List<Task<UIPanel>>(); 
			foreach (var uiKey in PanelList)
            {
				uiTaskList.Add(QUIManager.GetUI(uiKey));
            }
			foreach (var uiTask in uiTaskList)
			{
				var ui = (await uiTask);
				ui?.ResetUI();
			}
			uiTaskList.Clear();
            curList.Clear();
            curList.AddRange(PanelList);
            InitOver = true;
        }
    }
}
