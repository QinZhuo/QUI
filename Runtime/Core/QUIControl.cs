using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace QTool.UI {
	public class QUIControl : MonoBehaviour {
		public string panelName;
		public void Show() {
			QUIManager.Show(panelName);
		}
		public void Hide() {
			QUIManager.Hide(panelName);
		}
		public void Switch(bool show) {
			if (show) {
				Show();
			}
			else {
				Hide();
			}
		}
	}
}
