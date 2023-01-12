using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.Inspector;
using System.Threading.Tasks;

namespace QTool.UI
{
	public class QUISetting : InstanceBehaviour<QUISetting>
	{
		[QName("初始化UI")]
		[QEnum(nameof(QUIPanelPrefab) +"."+nameof(QUIPanelPrefab.LoadAll))]
		public List<string> PanelList = new List<string>();
		protected override void Awake()
		{
			base.Awake();
			foreach (var uiKey in PanelList)
			{
				QUIManager.GetUI(uiKey)?.ResetUI();
			}
		}
	}
}
