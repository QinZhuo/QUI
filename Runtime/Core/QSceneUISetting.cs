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
		[QName("异步加载UI")]
		[QPopup(nameof(QUIPanelPrefab) + "." + nameof(QUIPanelPrefab.LoadAll))]
		public List<string> PanelList = new List<string>();
		protected override void Awake()
		{
			base.Awake();
			if (QSceneTool.IsLoading)
			{
				QSceneTool.PreLoadList.Add(LoadAsync().Run());
			}
			else
			{
				_ = LoadAsync();
			}
		}
		private async Task LoadAsync()
		{
			await QDataList.PreLoadAsync();
			QDebug.Begin("异步加载场景UI");
			foreach (var uiKey in PanelList)
			{
				await QUIManager.LoadAsync(uiKey);
				await QTask.Step();
			}
			QDebug.End("异步加载场景UI");
		}
	}
}
