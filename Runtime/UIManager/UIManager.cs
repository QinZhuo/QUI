using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QTool;
using UnityEngine.Serialization;
using QTool.Resource;
#if QTween
using QTool.Tween;
#endif
namespace QTool.UI
{
    public class UIPanel : PrefabResourceList<UIPanel>
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
        static void InitPanret(RectTransform panel, string parentKey)
        {
            if (string.IsNullOrWhiteSpace(parentKey)) return;
            var parent = Get(parentKey);
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
        public static IUIPanel GetUI(string key)
        {
            return Get(key).GetComponent<IUIPanel>();
        }
        public static RectTransform Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (PanelList.ContainsKey(key)) return PanelList[key];

            if (!UIPanel.LoadOver())
            {
                Debug.LogError("UIPnael资源还未加载无法获取父页面[" + key + "]");
                return null;
            }
            if (key.Contains("."))
            {
                var keys = key.Split('.');
                var index = 0;
                Transform parent = Get(keys[index]);
                for (index = 1; index < keys.Length - 1; index++)
                {
                    parent = parent.Find(keys[index]) ;
                }
                PanelList[key] = parent.Find(keys[index]) as RectTransform;
            }
            else if (!PanelList.ContainsKey(key))
            {
                var obj = UIPanel.GetInstance(key);
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
        }
        public static void Remove(IUIPanel window)
        {
            if (windowStack.Count == 0) return;
            windowStack.Remove(window);
        }
    }

    public interface IUIPanel
    {
        void Show();
        void Hide();
        void ResetUI();
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
        public bool controlActive = true;
        public bool showOnStart=true;
        protected virtual void OnLevelWasLoaded(int level)
        {
            ResetUI();
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
        protected override void Awake()
        {
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
            ResetUI();
        }
        protected virtual void Reset()
        {
            group = GetComponent<CanvasGroup>();
        }
        public string ParentPanel = "";
        public CanvasGroup group;
#if QTween
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
        RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
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
                group.interactable = IsShow;
            }
        }
        public bool Focus
        {
            get
            {
                if (group != null)
                {
                    return group.interactable;
                }
                return true;
            }
            set
            {
                if (group != null)
                {
                    group.interactable = value;
                }
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
       
        public void Show()
        {
            IsShow = true;
            RunAnim();
        }
       
        public void Hide()
        {
            IsShow = false;
            RunAnim();
        }
        public virtual void Fresh() { }
    }
    public abstract class UIWindow<T> : UIPanel<T> where T : UIWindow<T>
    {
        protected override void Reset()
        {
            base.Reset();
            showOnStart = false;
        }
        public float timeScale = -1;
        [FormerlySerializedAs("backView")]
        public string backPanel = "";

        IUIPanel _backUI;
        IUIPanel BackUI
        {
            get
            {
                if (string.IsNullOrWhiteSpace(backPanel)) return null;
                if (_backUI == null)
                {
                    _backUI = UIManager.GetUI(backPanel);
                }
                return _backUI;
            }
        }
        public void ShowBack()
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
            BackUI?.Hide();
        }
    }

}