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
		
		[QName("初始化UI")]
		[ViewEnum(nameof(QUIPanelPrefab) +"."+nameof(QUIPanelPrefab.GetEditorList))]
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
			List<Task<QUIPanel>> uiTaskList = new List<Task<QUIPanel>>(); 
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
