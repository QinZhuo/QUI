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
#if QTween
using QTool.Tween;
#endif
namespace QTool.UI
{
    public class UIPanelPrefab : PrefabAssetList<UIPanelPrefab>
    {
    }
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

        public static async Task Show(string key,bool show,object obj)
        {
            if (show == IsShow(key))
            {
                return;
            }
            (await GetUI(key)).Switch(show, obj);
        }
      
        static async Task<RectTransform> Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (PanelList.ContainsKey(key)) return PanelList[key];

            await UIPanelPrefab.LoadAllAsync();
            if (key.Contains("."))
            {
                var keys = key.Split('.');
                var index = 0;
                Transform parent = await Get(keys[index]);
                for (index = 1; index < keys.Length - 1; index++)
                {
                    parent = parent.Find(keys[index]) ;
                }
                PanelList[key] = parent.Find(keys[index]) as RectTransform;
            }
            else if (!PanelList.ContainsKey(key))
            {
                var obj = await UIPanelPrefab.GetInstance(key);
                if (obj.transform.parent == null)
                {
                    GameObject.DontDestroyOnLoad(obj);
                }
                ResisterPanel(key, obj.GetComponent<RectTransform>());
            }
            else
            {
                Debug.LogError("找不到【" + key + "】UI页面");
            }
            return PanelList[key];
        }

        public static int Count
        {
            get
            {
                return windowStack.Count;
            }
        }
        public static List<UIPanel> windowStack = new List<UIPanel>();
        public static void Push(UIPanel window)
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
            Debug.Log("当前页面 " + (windowStack.StackPeek() as MonoBehaviour));
        }
        public static event System.Action<UIPanel> WindowChange;
        public static void Remove(UIPanel window)
        {
            if (windowStack.Count == 0||!windowStack.Contains(window)) return;
            windowStack.Remove(window);
            WindowChange?.Invoke(windowStack.StackPeek());
            Debug.Log("当前页面 " + (windowStack.StackPeek() as MonoBehaviour) + " 移除：" + (window as MonoBehaviour));
        }
    }

    public abstract class UIPanel:MonoBehaviour
	{
		public abstract void Switch(bool show,object obj);
		public abstract void Show();
		public abstract Task ShowAsync();
		public abstract void Hide();
		public abstract Task HideAsync();
		public abstract void ResetUI();
		public abstract bool IsShow {protected set; get; }
		public abstract RectTransform RectTransform { get; }
    }
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel<T> : UIPanel where T : UIPanel<T>
    {
		static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					Debug.LogError("未初始化显示页面 " + typeof(T));
					return null;
				}
				else
				{
					return _instance;
				}

			}
		}
        public static bool PanelIsShow
        {
            get
            {
                if (Instance == null)
                {
                    return false;
                }
                else
                {
                    return Instance.IsShow;
                }
            }
        }
        [ViewName("初始显示")]
        public bool showOnStart=false;
        [ViewName("时间控制")]
        public float timeScale = -1;
        [ViewName("遮挡点击")]
        public bool blockInput = false;
        protected virtual void FreshWindow(UIPanel window)
        {
            if (group == null) return;
           
             var value =window==null ||this.Equals(window) || transform.HasParentIs(window.RectTransform);
            if (value)
            {
                if (IsShow)
                {
                    group.interactable = true;
                }
            }
            else
            {
                group.interactable = false;
            }
        }
        protected virtual void OnSceneChange(Scene scene, Scene nextScene)
        {
            if(!string.IsNullOrEmpty(ParentPanel))
            {
                FastHide();
            }
        }
        public void FastHide()
        {
            Hide();
#if QTween
            showAnim?.Anim.Complete();
#endif
        }
        public override void ResetUI()
        {
            if (showOnStart)
            {
                if (!IsShow)
                {
                    Show();
                }
            }
            else
            {
                if (IsShow)
                {
                    Hide();
                }
            }
#if QTween
            showAnim?.Anim.Complete();
#endif
        }
        protected virtual void OnDestroy()
        {
            QUIManager.WindowChange -= FreshWindow;
            SceneManager.activeSceneChanged -= OnSceneChange;

        }
        protected  void Awake()
        {
			
			if(this is T panel)
			{
				_instance = this as T;
				Debug.Log("初始化页面" +  _instance);
			}
			else
			{
				Debug.LogError("页面类型不匹配："+_instance+":"+typeof(T));
			}
		
            if (group==null)
            {
                group = GetComponent<CanvasGroup>();
            }
            IsShow = group.alpha >= 0.9f;

            SceneManager.activeSceneChanged += OnSceneChange;
            QUIManager.WindowChange += FreshWindow;
            QUIManager.ResisterPanel(name.Contains("(Clone)")?name.Substring(0,name.IndexOf("(Clone)")):name, GetComponent<RectTransform>(), ParentPanel);
#if QTween
            showAnim?.Anim.OnStart(() =>
            {
                if (IsShow)
                {
					InvokeOnShow(true);
				}
                else
                {
                    FreshGroup();
                }
            }).OnComplete(() =>
            {
                if (!IsShow)
                {
					InvokeOnShow(false);
				}
                else
                {
                    FreshGroup();
                }
            });
#endif
            // ResetUI();
        }

        protected virtual void Reset()
        {
            group = GetComponent<CanvasGroup>();
        }
        [ViewName("父页面")]
        public string ParentPanel = "";
        public CanvasGroup group;
#if QTween
        [ViewName("显示动画")]
        public QTweenBehavior showAnim;
#endif
		private void InvokeOnShow(bool show)
		{
			try
			{
				if (show)
				{
					OnShow();
				}
				else
				{
					OnHide();
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError((show ? "显示" : "隐藏") + typeof(T) + " 页面出错：\n" + e);
			}
		}
        
        public ActionEvent OnShowAction;
        public ActionEvent OnHideAction;
        public async Task RunAnim()
        {
         
            if (IsShow)
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

#if QTween
                if (showAnim != null)
                {
                    await showAnim.PlayAsync(true);
                }
                else
#endif
                {
                    FreshGroup();
					InvokeOnShow(true);
                }
            }
            else
            {
#if QTween
                if (showAnim != null)
                {
                      await showAnim.PlayAsync(false);
                }
                else     
#endif
                {
                    FreshGroup();
					InvokeOnShow(false);
				}
            }
        }
        public override RectTransform RectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }

        public override bool IsShow
        {
            protected set; get;
        }
        void FreshGroup()
        {
#if QTween
            if (showAnim == null)
#endif
            {
                gameObject.SetActive(IsShow);
                group.interactable=IsShow;
                group.alpha = IsShow ? 1 : 0;
                // group.blocksRaycasts = IsShow;
            }
        }
        protected virtual void OnShow()
        {
            transform.SetAsLastSibling();
            Fresh();
            //if (controlActive)
            //{
            //    gameObject.SetActive(IsShow);
            //}
            OnShowAction?.Invoke();
            if (timeScale >= 0)
            {
                QTime.ChangeScale(gameObject, timeScale);
            }
            if (blockInput)
            {
                QUIManager.Push(this);
            }
        }
        protected virtual void OnHide()
        {
            OnHideAction?.Invoke();
            QTime.RevertScale(gameObject);
            if (blockInput)
            {
                QUIManager.Remove(this);
            }
        }
     

        public static void SwitchWindow(bool switchBool)
        {
            if (switchBool)
            {
                ShowWindow();
            }
            else
            {
                HideWindow();
            }
        }

        public static async void ShowWindow()
        {
            if(await Tool.Wait(() => Instance != null))
			{
				Instance?.Show();
			}
        }
        public static void HideWindow()
        {
            Instance?.Hide();
        }
        [ViewButton("显示")]
        public override void Show()
        {
            _ = ShowAsync();
        }
        List<object> showObj = new List<object>();
        
        public override void Switch(bool show, object obj=null )
        {
            if (show)
            {
                Show();
                if (obj != null)
                {
                    showObj.AddCheckExist(obj);
                }
            }
            else
            {
                if (obj != null)
                {
                    Hide();
                }
                else
                {
                    showObj.Remove(obj);
                    if (showObj.Count == 0)
                    {
                        Hide();
                    }
                }
            }
           
        }

        public override async Task ShowAsync()
        {
            IsShow = true;
            await RunAnim();
        }
        [ViewButton("隐藏")]
        public override void Hide()
        {
            showObj.Clear(); 
            _ = HideAsync();
        }
        public override async Task HideAsync()
        {
            IsShow = false;
            await RunAnim();
        }
        public virtual void Fresh() { }

        public async Task ShowWaitHide()
        {
            await ShowAsync();
            await WaitHide();
        }
        public async Task WaitHide()
        {
            while (IsShow && Application.isPlaying)
            {
                await Task.Yield();
            }
        }

      
    }
}
