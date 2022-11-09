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
#if UNITY_EDITOR
		[QEnum(nameof(QUIPanelPrefab) +"."+nameof(QUIPanelPrefab.GetEditorList))]
#endif
		public List<string> PanelList = new List<string>();
		public bool InitOver { private set; get; } = false;
		public static async Task InitOverAsync()
		{
			if (Instance != null)
			{
				await QTask.Wait(() => Instance.InitOver);
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
