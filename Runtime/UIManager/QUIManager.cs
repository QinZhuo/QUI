using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QTool;
using UnityEngine.Serialization;
using QTool.Asset;
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
        static QDictionary<string, RectTransform> PanelList = new QDictionary<string, RectTransform>();
        /// <summary>
        /// 注册UI到管理器
        /// </summary>
        /// <param name="key">关键名</param>
        /// <param name="panel">UI页面</param>
        /// <param name="parentKey">父页面</param>
        public static void ResisterPanel(string key, RectTransform panel, string parentKey = "")
        {
            panel.name = key;
            PanelList[key] = panel;
            InitPanret(panel, parentKey);
        }
        static async void InitPanret(RectTransform panel, string parentKey)
        {
            if (string.IsNullOrWhiteSpace(parentKey)) return;
            var parent = await Get(parentKey);
            if (parent != null && parent != panel)
            {
                var scale = panel.localScale;
                panel.SetParent(parent,false);
                panel.localScale = scale;
                if (panel.anchorMin == Vector2.zero && panel.anchorMax == Vector2.one)
                {
                    panel.offsetMin = Vector2.zero;
                    panel.offsetMax = Vector2.zero;
                }
            }
        }
		public static async Task<T> GetUI<T>(string key) where T:UIPanel
		{
			return (await Get(key)).GetComponent<T>();
		}
		/// <summary>
		/// 异步获取UI
		/// </summary>
		/// <param name="key">UI关键名</param>
		/// <returns></returns>
		public static async Task<UIPanel> GetUI(string key)
        {
            return (await Get(key)).GetComponent<UIPanel>();
        }
        /// <summary>
        /// UI是否正在显示
        /// </summary>
        /// <param name="key">UI关键名</param>
        /// <returns></returns>
        public static bool IsShow(string key)
        {
            if (PanelList.ContainsKey(key)) return PanelList[key].GetComponent<UIPanel>().IsShow;
            return false;
        }

        public static async Task Show(string key,bool show)
        {
            if (show == IsShow(key))
            {
                return;
            }
            (await GetUI(key)).Switch(show);
        }
        static async Task<RectTransform> Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (PanelList[key]!=null) return PanelList[key];
            if (key.SplitTowString(".",out var start,out var end))
			{
				Transform parent = await Get(start);
				PanelList[key] = parent.GetChild(end,true) as RectTransform;
			}
            else if (PanelList[key] == null)
			{
				await QTask.RunOnlyOne(nameof(QUIManager)+"_"+key, async() =>
				{
					var prefab = await UIPanelPrefabs.LoadAsync(key);
					var obj = GameObject.Instantiate(prefab);
					if (obj.transform.parent == null)
					{
						GameObject.DontDestroyOnLoad(obj);
					}
					ResisterPanel(key, obj.GetComponent<RectTransform>());
				});
			}
            return PanelList[key];
        }
		public static void Destory(string key)
		{
			if (!key .IsNullOrEmpty()&& PanelList.ContainsKey(key)){
				var ui= PanelList[key];
				PanelList.RemoveKey(key);
				GameObject.Destroy(ui.gameObject);
			}
		}
		public static int Count
        {
            get
            {
                return windowStack.Count;
            }
        }
        public static List<UIPanel> windowStack = new List<UIPanel>();
        public static void WindowPush(UIPanel window)
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
			QEventManager.Trigger("UI页面", windowStack.ToOneString());
#endif
		}
        public static event System.Action<UIPanel> WindowChange;
		
        public static void WindowRemove(UIPanel window)
        {
            if (windowStack.Count == 0||!windowStack.Contains(window)) return;
            windowStack.Remove(window);
            WindowChange?.Invoke(windowStack.StackPeek());
#if QDebug
			QEventManager.Trigger("UI页面", windowStack.ToOneString());
#endif
        }
    }

	
}
