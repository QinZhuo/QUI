using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace QTool.UI
{
	public class QUIControl : MonoBehaviour
	{
		[QPopup(nameof(UI_Prefab) + "." + nameof(UI_Prefab.LoadAll))]
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
