using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QTool;
using QTool.Tween;
using UnityEngine.Serialization;
using QTool.Resource;
namespace QTool.UI.Manager
{
    public class UIPanel : PrefabResourceList<UIPanel>
    {
    }
    public static class UIManager
    {
        static QDcitionary<string, RectTransform> PanelList = new QDcitionary<string, RectTransform>();

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

            if (!UIPanel.LabelLoadOver)
            {
                Debug.LogError("UIPnael资源还未加载无法获取父页面[" + key + "]");
                return null;
            }
            if (!PanelList.ContainsKey(key))
            {
                var obj = UIPanel.GetInstance(key);
                if (obj.transform.parent == null)
                {
                    GameObject.DontDestroyOnLoad(obj);
                }
                ResisterPanel(key, obj.GetComponent<RectTransform>());
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
        RectTransform rectTransform { get; }
    }
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PanelView<T> : InstanceBehaviour<T, UIPanel>, IUIPanel where T : PanelView<T>
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
        protected override void Awake()
        {
            base.Awake();
            UIManager.ResisterPanel(name, GetComponent<RectTransform>(), ParentPanel);
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
            Init();
        }
        protected virtual void Init()
        {
            Show();
        }
        protected virtual void Reset()
        {
            group = GetComponent<CanvasGroup>();
        }
        public string ParentPanel = "";
        public CanvasGroup group;
        public QTweenBehavior showAnim;
        public ActionEvent OnShowAction;
        public ActionEvent OnHideAction;
        public void RunAnim()
        {
            if (IsShow)
            {
                if (showAnim != null)
                {
                    showAnim.Show();
                }
                else
                {
                    OnShow();
                }
            }
            else
            {
                if (showAnim != null)
                {
                    showAnim.Hide();
                }
                else
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
            gameObject.SetActive(IsShow);
            OnShowAction?.Invoke();

        }
        protected virtual void OnHide()
        {
            gameObject.SetActive(IsShow);
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
            if (IsShow)
            {
                IsShow = false;
                RunAnim();
            }
        }
        public abstract void Fresh();
    }
    public abstract class WindowPanelView<T> : PanelView<T> where T : WindowPanelView<T>
    {
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
            if (IsShow) return;
            ShowBack();
            base.OnShow();
            if (timeScale >= 0)
            {
                TimeManager.ChangeScale(gameObject, timeScale);
            }
            UIManager.Push(this);
        }
        protected override void OnHide()
        {
            base.OnHide();
            TimeManager.RevertScale(gameObject);
            UIManager.Remove(this);
            BackUI?.Hide();
        }
    }

}