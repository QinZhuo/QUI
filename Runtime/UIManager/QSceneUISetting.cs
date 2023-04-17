using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.Inspector;
using System.Threading.Tasks;

namespace QTool.UI
{
	public class QSceneUISetting : InstanceBehaviour<QSceneUISetting>
	{
		[QName("预加载UI")]
		[QEnum(nameof(QUIPanelPrefab) +"."+nameof(QUIPanelPrefab.LoadAll))]
		public List<string> PanelList = new List<string>();
		protected override void Awake()
		{
			base.Awake();
			QSceneTool.PreLoadList.Add(PreLoad().Run());
		}
		private async Task PreLoad()
		{
			QDebug.BeginMarker("预加载场景UI");
			foreach (var uiKey in PanelList)
			{
				if (!QUIManager.PanelList.ContainsKey(uiKey))
				{
					await QUIPanelPrefab.LoadAsync(uiKey);
					QUIManager.GetUI(uiKey)?.ResetUI();
				}
			}
			QDebug.EndMarker("预加载场景UI");
		}
	}
}
