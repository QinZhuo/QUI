﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QTool;
using UnityEngine.Serialization;
using QTool.Asset;
using QTool.Inspector;
using System.Threading.Tasks;
#if QTween
using QTool.Tween;
#endif
namespace QTool.UI
{
    public class UIPanel : PrefabAssetList<UIPanel>
    {
    }
    /// <summary>
    /// UI管理器
    /// </summary>
    public static class UIManager
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
        /// <summary>
        /// 异步获取UI
        /// </summary>
        /// <param name="key">UI关键名</param>
        /// <returns></returns>
        public static async Task<IUIPanel> GetUI(string key)
        {
            return (await Get(key)).GetComponent<IUIPanel>();
        }
        /// <summary>
        /// UI是否正在显示
        /// </summary>
        /// <param name="key">UI关键名</param>
        /// <returns></returns>
        public static bool IsShow(string key)
        {
            if (PanelList.ContainsKey(key)) return PanelList[key].GetComponent<IUIPanel>().IsShow;
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

            await UIPanel.LoadAllAsync();
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
                var obj = await UIPanel.GetInstance(key);
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
        public static List<IUIPanel> windowStack = new List<IUIPanel>();
        public static void Push(IUIPanel window)
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
            Debug.LogError("当前页面 " + (windowStack.StackPeek() as MonoBehaviour));
        }
        public static event System.Action<IUIPanel> WindowChange;
        public static void Remove(IUIPanel window)
        {
            if (windowStack.Count == 0||!windowStack.Contains(window)) return;
            windowStack.Remove(window);
            WindowChange?.Invoke(windowStack.StackPeek());
            Debug.LogError("当前页面 " + (windowStack.StackPeek() as MonoBehaviour) + " 移除：" + (window as MonoBehaviour));
        }
    }

    public interface IUIPanel
    {
        void Switch(bool show,object obj);
        void Show();
        Task ShowAsync();
        void Hide();
        Task HideAsync();
        void ResetUI();
        bool IsShow { get; }
        RectTransform rectTransform { get; }
    }
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel<T> : InstanceBehaviour<T, UIPanel>, IUIPanel where T : UIPanel<T>
    {
        public static bool PanelIsShow
        {
            get
            {
                if (_instance == null)
                {
                    return false;
                }
                else
                {
                    return Instance.IsShow;
                }
            }
        }
        public virtual IUIPanel BackUI => null;
        //[ViewName("控制Active")]
        //public bool controlActive = true;
        [ViewName("初始显示")]
        public bool showOnStart=false;
        [ViewName("时间控制")]
        public float timeScale = -1;
        [ViewName("遮挡点击")]
        public bool blockInput = false;
        protected virtual void FreshWindow(IUIPanel window)
        {
            if (group == null) return;
           
             var value =window==null ||this.Equals(window) || transform.HasParentIs(window.rectTransform);
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
        protected virtual void OnLevelWasLoaded(int level)
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
        public void ResetUI()
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
        private void OnDestroy()
        {
            UIManager.WindowChange -= FreshWindow;
        }
        protected override void Awake()
        {

            if (group==null)
            {
                group = GetComponent<CanvasGroup>();
            }
            IsShow = group.alpha >= 0.9f;
            UIManager.WindowChange += FreshWindow;
            base.Awake();
            UIManager.ResisterPanel(name.Contains("(Clone)")?name.Substring(0,name.IndexOf("(Clone)")):name, GetComponent<RectTransform>(), ParentPanel);
#if QTween
            showAnim?.Anim.OnStart(() =>
            {
                if (IsShow)
                {
                    OnShow();
                }
                else
                {
                    FreshGroup();
                }
            }).OnComplete(() =>
            {
                if (!IsShow)
                {
                    OnHide();
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
                    OnShow();
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
                    OnHide();
                }
            }
        }
        public RectTransform rectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }

        public bool IsShow
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
                TimeManager.ChangeScale(gameObject, timeScale);
            }
            if (blockInput)
            {
                UIManager.Push(this);
            }
        }
        protected virtual void OnHide()
        {
            OnHideAction?.Invoke();
            TimeManager.RevertScale(gameObject);
            if (blockInput)
            {
                UIManager.Remove(this);
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
            await Tool.Wait(() => Instance != null);
            Instance?.Show();
        }
        public static void HideWindow()
        {
            Instance?.Hide();
        }
        [ViewButton("显示")]
        public void Show()
        {
            ShowAsync();
        }
        List<object> showObj = new List<object>();
        
        public void Switch(bool show, object obj=null )
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

        public async Task ShowAsync()
        {
            IsShow = true;
            await RunAnim();
        }
        [ViewButton("隐藏")]
        public void Hide()
        {
            showObj.Clear();
            HideAsync();
        }
        public async Task HideAsync()
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