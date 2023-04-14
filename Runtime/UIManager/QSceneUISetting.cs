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
		private void Start()
		{
			QTool.PreLoadTaskList.Add(PreLoad());
		}
		private async Task PreLoad()
		{
			var startTime = QDebug.Timestamp;
			var taskList = new List<Task>();
			foreach (var uiKey in PanelList)
			{
				taskList.Add(QUIPanelPrefab.LoadAsync(uiKey));
			}
			await taskList.WaitAllOver();
			foreach (var uiKey in PanelList)
			{
				QUIManager.GetUI(uiKey)?.ResetUI();
			}
			QDebug.Log("预加载场景UI完成", startTime);
		}
	}
}
