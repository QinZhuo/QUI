using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace QTool.UI
{
    public class QUIPanelControl : MonoBehaviour
    {
		QUIPanel Panel()
		{
			return QUIManager.GetUI(panelName);
		}
        public string panelName;
        public void Show()
        {
          Panel()?.Show();
        }
		public void Hide()
		{
			Panel()?.Hide();
		}
    }
}
