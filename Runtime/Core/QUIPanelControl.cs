using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace QTool.UI
{
	public class QUIPanelControl : MonoBehaviour
	{
		[QPopup(nameof(QUIPanelPrefab) + "." + nameof(QUIPanelPrefab.LoadAll))]
		public string panelName;
		public void Show()
		{
			QUIManager.Load(panelName).Show();
		}
		public void Hide()
		{
			QUIManager.Load(panelName).Hide();
		}
	}
}
