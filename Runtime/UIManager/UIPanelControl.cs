using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace QTool.UI
{
    public class UIPanelControl : MonoBehaviour
    {
        IUIPanel _panel;
        async Task< IUIPanel> Panel()
        {
                if (_panel == null)
                {
                    _panel = await UIManager.GetUI(panelName) ;
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
