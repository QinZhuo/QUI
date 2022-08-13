using QTool.Asset;
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
	public class UIPanelPrefabs : QPrefabLoader<UIPanelPrefabs>
	{
	}

	public abstract class UIPanel : MonoBehaviour
	{
		public abstract Task ShowAsync();
		public abstract Task HideAsync();
		public abstract void ResetUI();
		[ViewButton("隐藏")]
		public void Hide()
		{
			_ = HideAsync();
		}
		[ViewButton("显示")]
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
				Hide(); ;
			}
		}
		public bool IsShow { protected set; get; }
		public RectTransform RectTransform
		{
			get
			{
				return transform as RectTransform;
			}
		}
		

	}
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class UIPanel<T> : UIPanel where T : UIPanel<T>
	{
		#region 单例逻辑
		static T _instance;
		protected static T Instance
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
		#endregion
		#region 静态公开接口
		public static async Task SwitchPanel(bool switchBool)
		{
			if (switchBool)
			{
				await ShowPanel();
			}
			else
			{
				_ = HidePanel();
			}
		}


		public async Task ShowWaitHide()
		{
			await ShowAsync();
			await WaitHide();
		}
		public static async Task WaitHide()
		{
			while (Instance.IsShow && Application.isPlaying)
			{
				await Task.Yield();
			}
		}
		public static async Task ShowPanel()
		{
			await GetInstance();
			await Instance?.ShowAsync();
		}
		public static async Task HidePanel()
		{
			if (PanelIsShow)
			{
				await Instance?.HideAsync();
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
		public static void InvokeEvent(string eventName)
		{
			Instance.gameObject.InvokeEvent(eventName);
		}
		public static void InvokeEvent(string eventName, bool value)
		{
			Instance.gameObject.InvokeEvent(eventName, value);
		}
		#endregion
		#region 基础属性
		[Group(true)]
		[ViewName("初始显示")]
		public bool showOnStart = false;
		[ViewName("遮挡点击")]
		public bool blockInput = false;
		[ViewName("控制TimeScale")]
		public float timeScale = -1;
#if QTween
		[ViewName("显示动画")]
		public QTweenBehavior showAnim;
#endif
		[ViewName("父页面")]
		
		public string ParentPanel = "";
		public ActionEvent OnShowAction;
		[Group(false)]
		public ActionEvent OnHideAction;
		CanvasGroup _group;
		public CanvasGroup Group => _group ??= GetComponent<CanvasGroup>();

		#endregion
		#region 基本生命周期
		protected static async Task<T> GetInstance()
		{
			await QUIManager.GetUI(typeof(T).Name);
			return Instance;
		}
		protected virtual void Awake()
		{
			if (this is T panel)
			{
				_instance = this as T;
				QDebug.Log("初始化页面" + _instance);
			}
			else
			{
				Debug.LogError("页面类型不匹配：" + _instance + ":" + typeof(T));
			}
			IsShow = Group.alpha >= 0.9f;

			SceneManager.activeSceneChanged += OnSceneChange;
			QUIManager.WindowChange += Fresh;
			QUIManager.ResisterPanel(typeof(T).Name, GetComponent<RectTransform>(), ParentPanel);

		}
		protected virtual void OnDestroy()
		{
			QUIManager.WindowChange -= Fresh;
			SceneManager.activeSceneChanged -= OnSceneChange;

		}
	
		private void Fresh(UIPanel window)
		{
			var value = window == null || this.Equals(window) || transform.HasParentIs(window.RectTransform);
			if (value)
			{
				if (IsShow)
				{
					Group.interactable = true;
					OnFresh();
				}
			}
			else
			{
				Group.interactable = false;
			}
		}
		/// <summary>
		///  主页面切换时调用 刷新数据
		/// </summary>
		protected virtual void OnFresh()
		{

		}
		/// <summary>
		/// 由UISettinng控制 是否初始化显示
		/// </summary>
		public override void ResetUI()
		{
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
		}
		/// <summary>
		/// 切换场景时调用默认隐藏拥有 ParentPanel 的页面
		/// </summary>
		protected virtual void OnSceneChange(Scene scene, Scene nextScene)
		{
			if (!string.IsNullOrEmpty(ParentPanel))
			{
				HideAndComplete();
			}
		}



		private void InvokeOnShow()
		{
			try
			{
				if (IsShow)
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
				Debug.LogError((IsShow ? "显示" : "隐藏") + typeof(T) + " 页面出错：\n" + e);
			}
		}
		/// <summary>
		/// 显示或隐藏页面时 最开始调用
		/// </summary>
		protected virtual async Task StartShow(bool IsShow)
		{
			this.IsShow = IsShow;
#if QTween
			if (showAnim != null)
			{
				if (!gameObject.activeSelf)
				{
					if (base.IsShow)
					{
						gameObject.SetActive(true);
					}
				}
				await showAnim.PlayAsync(base.IsShow);
			}
			else
#endif
			{
				gameObject.SetActive(base.IsShow);
				Group.interactable = base.IsShow;
				Group.alpha = base.IsShow ? 1 : 0;
			}
			InvokeOnShow();
		}
		public override async Task ShowAsync()
		{
			await StartShow(true);
		}

		public override async Task HideAsync()
		{
			await StartShow(false);
		}

		public void ShowAndComplete()
		{
			Show();
#if QTween
			showAnim?.Complete();
#endif
		}
		public void HideAndComplete()
		{
			Hide();
#if QTween
			showAnim?.Complete();
#endif
		}

		protected virtual void OnShow()
		{
			transform.SetAsLastSibling();
			OnShowAction?.Invoke();
			if (Application.isPlaying)
			{
				if (timeScale >= 0)
				{
					QTime.ChangeScale(gameObject, timeScale);
				}
				if (blockInput)
				{
					QUIManager.WindowPush(this);
				}
				else
				{
					OnFresh();
				}
			}
			
		}
		protected virtual void OnHide()
		{
			OnHideAction?.Invoke();
			QTime.RevertScale(gameObject);
			if (blockInput)
			{
				QUIManager.WindowRemove(this);
			}
		}

		#endregion
		
	}
}
