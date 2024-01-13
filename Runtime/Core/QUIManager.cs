using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QTool;
using UnityEngine.Serialization;
using QTool.Inspector;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
namespace QTool.UI
{
  
    /// <summary>
    /// UI管理器
    /// </summary>
    public static class QUIManager
    {
        internal static QDictionary<string, QUIPanel> PanelList = new QDictionary<string, QUIPanel>();
		/// <summary>
		/// 注册UI到管理器
		/// </summary>
		/// <param name="key">关键名</param>
		/// <param name="panel">UI页面</param>
		/// <param name="parentKey">父页面</param>
		public static void ResisterPanel(QUIPanel panel, string parentKey = "")
		{
			PanelList[panel.QName()] = panel;
			if (!parentKey.IsNull())
			{
				var parent = Get(parentKey);
				if (parent != null && parent != panel)
				{
					var scale = panel.RectTransform.localScale;
					panel.RectTransform.SetParent(parent.RectTransform, false);
					panel.RectTransform.localScale = scale;
					if (panel.RectTransform.anchorMin == Vector2.zero && panel.RectTransform.anchorMax == Vector2.one)
					{
						panel.RectTransform.offsetMin = Vector2.zero;
						panel.RectTransform.offsetMax = Vector2.zero;
					}
				}
			}
		}
        /// <summary>
        /// UI是否正在显示
        /// </summary>
        /// <param name="key">UI关键名</param>
        /// <returns></returns>
        public static bool IsShow(this string key)
        {
            if (PanelList.ContainsKey(key)) return PanelList[key].GetComponent<QUIPanel>().IsShow;
            return false;
        }

		public static async Task Show(this string key, bool show)
		{
			if (show == IsShow(key))
			{
				return;
			}
			if (show)
			{
				await Get(key).ShowAsync();
			}
			else
			{
				await Get(key).HideAsync();
			}
		}
		public static QUIPanel Get(this string key)
		{
			if (key.IsNull()) return Get(nameof(Canvas));
			if (PanelList[key] == null)
			{
				QDebug.Begin("动态创建" + nameof(QUIPanel) + "<" + key + ">");
				var prefab = QUIPanelPrefab.Load(key);
				if (prefab == null)
				{
					QDebug.LogError("不存在UI预制体 [" + key + "]");
					return null;
				}
				var panel = Object.Instantiate(prefab).GetComponent<QUIPanel>(true);
				panel.name = key;
				ResisterPanel(panel);
				if (panel.transform.parent == null)
				{
					Object.DontDestroyOnLoad(panel.RectTransform.gameObject);
				}
				QDebug.End("动态创建" + nameof(QUIPanel) + "<" + key + ">");
			}
			return PanelList[key];
		}
		public static void Remove(QUIPanel panel)
		{
			var key = panel.name;
			if (!key.IsNull() && PanelList.ContainsKey(key))
			{
				var ui = PanelList[key];
				if (ui == panel)
				{
					PanelList.RemoveKey(key);
				}
			}
		}
		public static int Count
        {
            get
            {
                return windowStack.Count;
            }
        }
        public static List<QUIPanel> windowStack = new List<QUIPanel>();
        public static void WindowPush(QUIPanel window)
        {
            if (windowStack.StackPeek() == window)
            {
                return;
            }
            if (windowStack.Contains(window))
            {
                windowStack.Remove(window);
            }
            windowStack.Push(window);
            WindowChange?.Invoke(windowStack.StackPeek());
#if QDebug
			QEventManager.InvokeEvent("UI页面", windowStack.ToOneString());
#endif
		}
        public static event System.Action<QUIPanel> WindowChange;
		
        public static void WindowRemove(QUIPanel window)
        {
            if (windowStack.Count == 0||!windowStack.Contains(window)) return;
            windowStack.Remove(window);
            WindowChange?.Invoke(windowStack.StackPeek());
#if QDebug
			QEventManager.InvokeEvent("UI页面", windowStack.ToOneString());
#endif
        }
    }


	public class QUIPanelPrefab : QPrefabLoader<QUIPanelPrefab>
	{
	}
}
