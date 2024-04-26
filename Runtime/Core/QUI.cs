using QTool.Inspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

#if QTween
using QTool.Tween;
#endif
namespace QTool.UI
{
	public class QUI : MonoBehaviour
	{
		public virtual Task ShowAsync()
		{
			IsShow = true;
			gameObject.SetActive(true);
			return Task.CompletedTask;
		}

		public virtual Task HideAsync()
		{
			IsShow = false;
			gameObject.SetActive(false);
			return Task.CompletedTask;
		}
#if UNITY_EDITOR
		private void OnValidate()
		{
			gameObject.AutoAddPersistentListener(this);
		}
#endif
		protected virtual void Awake()
		{
			QUIManager.ResisterPanel(this);
		}
		[QName("隐藏")]
		public void Hide()
		{
			_ = HideAsync();
		}
		[QName("显示")]
		public void Show()
		{
			_ = ShowAsync();
		}
		public void Switch(bool show)
		{
			if (show)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
		[QName("显示")]
		public virtual void Show(IViewData viewData)
		{
			Show();
		}
		public bool IsShow { internal set; get; }
		public RectTransform RectTransform
		{
			get
			{
				return transform as RectTransform;
			}
		}

	}

	[RequireComponent(typeof(CanvasGroup))]
	public abstract class QUI<T> : QUI where T : QUI<T>
	{
		#region 静态逻辑
		private static T _instance;
		protected static T Instance
		{
			get
			{
				if (_instance != null || !Application.isPlaying) return _instance;
				_instance = QUIManager.Load(typeof(T).Name) as T;
				return _instance;
			}
		}
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
		public static async Task LoadAsync()
		{
			await QUIManager.LoadAsync(typeof(T).Name);
		}
		public static void ShowPanel()
		{
			_ = ShowPanelAsync();
		}
		public static void HidePanel()
		{
			_ = HidePanelAsync();
		}
		public static async Task ShowPanelAsync()
		{
			await Instance?.ShowAsync();
		}
		public static async Task HidePanelAsync()
		{
			if (PanelIsShow)
			{
				await Instance?.HideAsync();
			}
		}
		#endregion
		#region 基础属性
		[QName("初始显示")]
		public bool showOnStart = false;
		[QName("模态窗"), UnityEngine.Serialization.FormerlySerializedAs("isWindow")]
		public bool isModalWindow = false;
#if QTween
		[QName("显示动画")]
		public QTweenComponent showAnim;
#endif
		[QName("父页面"), QPopup(nameof(UI_Prefab) + "." + nameof(UI_Prefab.LoadAll))]

		public string ParentPanel = "";
		public CanvasGroup Group => _group ??= GetComponent<CanvasGroup>();
		CanvasGroup _group;

		#endregion
		#region 基本生命周期


		protected override void Awake()
		{
			if (this is T panel)
			{
				_instance = this as T;
			}
			else
			{
				Debug.LogError("页面类型不匹配：" + _instance + ":" + typeof(T));
			}
			IsShow = Group.alpha >= 0.9f;
			QUIManager.OnCurrentWindowChange += Fresh;
			QUIManager.ResisterPanel(this, transform.parent == null ? ParentPanel : "");

			if (showOnStart)
			{
				if (!IsShow)
				{
					ShowAndComplete();
				}
			}
			else
			{
				if (IsShow)
				{
					HideAndComplete();
				}
			}
			if (gameObject.activeSelf != IsShow)
			{
				gameObject.SetActive(IsShow);
			}
		}
		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
			QUIManager.OnCurrentWindowChange -= Fresh;
			QUIManager.Remove(this);
		}
		protected virtual void OnEnable()
		{
			try
			{
				transform.SetAsLastSibling();
				if (Application.isPlaying)
				{
					if (isModalWindow)
					{
						QUIManager.ModalWindowPush(this);
					}
					OnFresh();
				}
			}
			catch (System.Exception e)
			{
				QDebug.LogError(this + " " + nameof(OnEnable) + " 出错：" + e);
			}
		}
		protected virtual void OnDisable()
		{
			try
			{
				if (this != null)
				{
					QTime.RevertScale(gameObject);
				}
				if (isModalWindow)
				{
					QUIManager.ModalWindowRemove(this);
				}
			}
			catch (System.Exception e)
			{
				QDebug.LogError(this + " " + nameof(OnDisable) + " 出错：" + e); 
			}
		}


		private void Fresh(QUI window)
		{
			if (this == null) return;
			var value = window == null || this.Equals(window) || transform.ParentHas(window.RectTransform);
			if (value)
			{
				if (IsShow)
				{
					Group.interactable = true;
					if (isModalWindow)
					{
						Group.blocksRaycasts = true;
					}
					OnFresh();
				}
			}
			else
			{
				Group.interactable = false;
				if (isModalWindow)
				{
					Group.blocksRaycasts = false;
				}
			}
		}
		/// <summary>
		///  主页面切换时调用 刷新数据
		/// </summary>
		public virtual void OnFresh()
		{

		}
		/// <summary>
		/// 由UISettinng控制 是否初始化显示
		/// </summary>






		/// <summary>
		/// 显示或隐藏页面时 最开始调用
		/// </summary>
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
		protected virtual async Task SwitchAsync(bool IsShow)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
		{
			if (this == null) return;
			this.IsShow = IsShow;
			if (IsShow)
			{
				gameObject.SetActive(true);
			}
#if QTween
			if (showAnim != null)
			{
				var animTask = showAnim.PlayAsync(IsShow);
				if (await animTask.IsCancel())
				{
					return;
				}
				if (animTask.Exception != null)
				{
					Debug.LogError("播放页面" + this + "动画出错 " + animTask.Exception);
				}
				if (IsShow != base.IsShow)
				{
					return;
				}
				if (this != null && Application.IsPlaying(this))
				{
					gameObject.SetActive(IsShow);
				}
			}
			else
#endif
			{
				if (this == null) return;
				Group.interactable = IsShow;
				if (isModalWindow)
				{
					Group.blocksRaycasts = IsShow;
				}
				Group.alpha = IsShow ? 1 : 0;
			}
			if (!this.IsShow)
			{
				if (Application.IsPlaying(this))
				{
					gameObject.SetActive(true);
				}
			}
		}
		public override async Task ShowAsync()
		{
			await SwitchAsync(true).Run();
		}

		public override async Task HideAsync()
		{
			await SwitchAsync(false).Run();
		}
		protected virtual void Complete()
		{
#if QTween
			showAnim?.Complete();
#endif
		}
		public void ShowAndComplete()
		{
			Show();
			Complete();
		}
		public void HideAndComplete()
		{
			Hide();
			Complete();
		}
		#endregion
	}
	public interface IViewData
	{

	}
	public abstract class QUI<T, TViewData> : QUI<T> where T : QUI<T, TViewData> where TViewData : class, IViewData
	{
		private TViewData _ViewData = null;
		public TViewData ViewData
		{
			set
			{
				if (value != _ViewData)
				{
					if (_ViewData != null)
					{
						OnUnsetViewData();
					}
					_ViewData = value;
					if (_ViewData != null)
					{
						OnSetViewData();
					}
				}
			}
			get => _ViewData;
		}

		protected virtual void OnSetViewData()
		{
			//gameObject.RegisterEvent(ViewData);
		}

		protected virtual void OnUnsetViewData()
		{
			//gameObject.RegisterEvent(ViewData);
		}
		public override void Show(IViewData viewData)
		{
			ViewData = viewData as TViewData;
			base.Show(viewData);
		}
		public static void ShowPanel(TViewData viewData)
		{
			_ = ShowPanelAsync(viewData);
		}
		public static async Task ShowPanelAsync(TViewData viewData)
		{
			Instance.ViewData = viewData;
			await ShowPanelAsync();
		}
	}
}
