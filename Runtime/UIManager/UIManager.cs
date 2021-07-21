using System.Collections;
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
    public static class UIManager
    {
        static QDictionary<string, RectTransform> PanelList = new QDictionary<string, RectTransform>();

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
                panel.SetParent(parent);
                panel.localScale = scale;
                if (panel.anchorMin == Vector2.zero && panel.anchorMax == Vector2.one)
                {
                    panel.offsetMin = Vector2.zero;
                    panel.offsetMax = Vector2.zero;
                }
            }
        }
        public static async Task<IUIPanel> GetUI(string key)
        {
            return (await Get(key)).GetComponent<IUIPanel>();
        }
        public static async Task<RectTransform> Get(string key)
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
        }
        public static event System.Action<IUIPanel> WindowChange;
        public static void Remove(IUIPanel window)
        {
            if (windowStack.Count == 0||!windowStack.Contains(window)) return;
            windowStack.Remove(window);
            WindowChange?.Invoke(windowStack.StackPeek());
        }
    }

    public interface IUIPanel
    {
        void Show();
        Task ShowAsync();
        void Hide();
        void ResetUI();
        IUIPanel BackUI { get; }
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
        [ViewName("控制Active")]
        public bool controlActive = true;
        [ViewName("初始显示")]
        public bool showOnStart=false;
        protected virtual void FreshWindow(IUIPanel window)
        {
            if (window == null) return;
            group.interactable = this.Equals(window) || transform.HasParentIs(window.rectTransform)||window.BackUI?.Equals(this);
        }
        protected virtual void OnLevelWasLoaded(int level)
        {
            FastHide();
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
                Show();
            }
            else
            {
                Hide();
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
            UIManager.WindowChange += FreshWindow;
            base.Awake();
            UIManager.ResisterPanel(name, GetComponent<RectTransform>(), ParentPanel);
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
        [HideInInspector]
        public CanvasGroup group;
#if QTween
        [ViewName("显示动画")]
        public QTweenBehavior showAnim;
#endif
        
        public ActionEvent OnShowAction;
        public ActionEvent OnHideAction;
        public void RunAnim()
        {
            if (IsShow)
            {
#if QTween
                if (showAnim != null)
                {
                    showAnim.Show();
                }
                else
#else
                FreshGroup();
#endif
                {
                    OnShow();
                }
            }
            else
            {
#if QTween
                if (showAnim != null)
                {
                    showAnim.Hide();
                }
                else
#else
                FreshGroup();
#endif
                {
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
            if (group != null)
            {
                group.blocksRaycasts = IsShow;
            }
        }
        protected virtual void OnShow()
        {
            transform.SetAsLastSibling();
            Fresh();
            if (controlActive)
            {
                gameObject.SetActive(IsShow);
            }
            OnShowAction?.Invoke();

        }
        protected virtual void OnHide()
        {
            if (controlActive)
            {
                gameObject.SetActive(IsShow);
            }
            OnHideAction?.Invoke();
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

        public static void ShowWindow()
        {
            Instance?.Show();
        }
        public static void HideWindow()
        {
            Instance?.Hide();
        }
        [ViewButton("显示")]
        public void Show()
        {
            IsShow = true;
            RunAnim();
        }
        [ViewButton("隐藏")]
        public void Hide()
        {
            IsShow = false;
            RunAnim();
        }
        public virtual void Fresh() { }

        public async Task ShowAsync()
        {
            Show();
            await Task.Run(() =>
            {
                while (IsShow)
                {
                    Task.Delay(100);
                }
            });
        }
    }
    public abstract class UIWindow<T> : UIPanel<T> where T : UIWindow<T>
    {
        protected override void Reset()
        {
            base.Reset();
            showOnStart = false;
        }
        [ViewName("时间控制")]
        public float timeScale = -1;
        [ViewName("背景页面")]
        [FormerlySerializedAs("backView")]
        public string backPanel = "";

        IUIPanel _backUI;
        public override IUIPanel BackUI
        {
            get
            {
                if (string.IsNullOrWhiteSpace(backPanel)) return null;
                if (_backUI == null)
                {
                    InitBackUI();
                }
                return _backUI;
            }
        }
        async void InitBackUI()
        {
            _backUI =await UIManager.GetUI(backPanel);
        }
        protected virtual void ShowBack()
        {
            if (BackUI != null)
            {
                if (transform.parent != BackUI.rectTransform.parent)
                {
                    BackUI.rectTransform.SetParent(transform.parent);
                }
                BackUI.Show();
            }
        }
      
       
        protected virtual void HideBack()
        {
            BackUI?.Hide();
        }
        protected override void OnShow()
        {
          
            ShowBack();
            base.OnShow();
            if (timeScale >= 0)
            {
                TimeManager.ChangeScale(gameObject, timeScale);
            }
            UIManager.Push(this);
        }
        protected override void OnHide()
        {;
            base.OnHide();
            TimeManager.RevertScale(gameObject);
            UIManager.Remove(this);
            HideBack();
        }
    }

}