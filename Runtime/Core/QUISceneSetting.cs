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
	public class QUISceneSetting : QInstanceBehaviour<QUISceneSetting>
	{
		[QName("异步加载UI")]
		[QPopup(nameof(UI_Prefab) + "." + nameof(UI_Prefab.LoadAll))]
		public List<string> PanelList = new List<string>();
		protected override void Awake()
		{
			base.Awake();
			LoadAsync().PreLoadRun();
		}
		private async Task LoadAsync()
		{
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
