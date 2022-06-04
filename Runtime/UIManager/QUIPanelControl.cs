using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace QTool.UI
{
    public class QUIPanelControl : MonoBehaviour
    {
        UIPanel _panel;
		async Task<UIPanel> Panel()
		{
			if (_panel == null)
			{
				_panel = await QUIManager.GetUI(panelName);
			}
			return _panel;
		}
        public string panelName;
        public async void Show()
        {
          (await Panel())? .Show();
        }
        public async void Hide()
        {
            (await Panel()) ?.Hide();
        }
    }
}
