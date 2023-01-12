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
		[QEnum(nameof(QUIPanelPrefab) +"."+nameof(QUIPanelPrefab.LoadAll))]
#endif
		public List<string> PanelList = new List<string>();
		private void Start()
		{
			foreach (var uiKey in PanelList)
			{
				var ui = QUIManager.GetUI(uiKey);
				ui?.ResetUI();
			}
			curList.Clear();
			curList.AddRange(PanelList);
		}
	}
}
