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
			foreach (var uiKey in PanelList)
            {
				var ui= await QUIManager.GetUI(uiKey);
				ui?.ResetUI();
            }
            curList.Clear();
            curList.AddRange(PanelList);
            InitOver = true;
        }
    }
}
