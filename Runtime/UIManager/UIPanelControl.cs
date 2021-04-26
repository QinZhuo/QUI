using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI.Manager
{
    public class UIPanelControl : MonoBehaviour
    {
        IUIPanel _panel;
        IUIPanel Panel
        {
            get
            {
                if (_panel == null)
                {
                    _panel = UIManager.GetUI(panelName);
                }
                return _panel;
            }
        }
        public string panelName;
        public void Show()
        {
            Panel?.Show();
        }
        public void Hide()
        {
            Panel?.Hide();
        }
    }
}
