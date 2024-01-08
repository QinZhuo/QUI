using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.Inspector;
using System.Threading.Tasks;

namespace QTool.UI
{
	/// <summary>
	/// 场景预加载UI配置
	/// </summary>
	public class QSceneUISetting : QInstanceBehaviour<QSceneUISetting>
	{
		[QName("预加载UI")]
		[QPopup(nameof(QUIPanelPrefab) +"."+nameof(QUIPanelPrefab.LoadAll))]
		public List<string> PanelList = new List<string>();
		protected override void Awake()
		{
			base.Awake();
			if (QSceneTool.IsLoading)
			{
				QSceneTool.PreLoadList.Add(PreLoad().Run());
			}
			else
			{
				QDebug.Begin("加载场景UI");
				foreach (var uiKey in PanelList)
				{
					if (!QUIManager.PanelList.ContainsKey(uiKey))
					{
						QUIManager.GetUI(uiKey)?.ResetUI();
					}
				}
				QDebug.End("加载场景UI");
			}
		}
		private async Task PreLoad()
		{
			QDebug.Begin("预加载场景UI");
			foreach (var uiKey in PanelList)
			{
				if (!QUIManager.PanelList.ContainsKey(uiKey))
				{
					await QUIPanelPrefab.LoadAsync(uiKey);
					QUIManager.GetUI(uiKey)?.ResetUI();
					await QTask.Step();
				}
			}
			QDebug.End("预加载场景UI");
		}
	}
}
