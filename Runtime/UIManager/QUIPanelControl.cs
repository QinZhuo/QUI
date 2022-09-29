using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace QTool.UI
{
    public class QUIPanelControl : MonoBehaviour
    {
		async Task<UIPanel> Panel()
		{
			return await QUIManager.GetUI(panelName);
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
