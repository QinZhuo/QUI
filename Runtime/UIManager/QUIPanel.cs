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
	public class QUIPanelPrefab : QPrefabLoader<QUIPanelPrefab>
	{
	}

	public abstract class QUIPanel : MonoBehaviour
	{
		internal GameObject Prefab;
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
	public abstract class QUIPanel<T> : QUIPanel where T : QUIPanel<T>
	{
		#region 单例逻辑
		static T _instance;
		protected static T Instance
		{
			get
			{
				if (_instance == null)
				{
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
				await HidePanel();
			}
		}


		public async Task ShowWaitHide()
		{
			await ShowAsync();
			await WaitHide();
		}
		public static async Task WaitHide()
		{
			while (Application.isPlaying&& Instance.IsShow)
			{
				await Task.Yield();
			}
#if QTween
			if (Instance.showAnim != null)
			{
				await Instance.showAnim.Anim.WaitOverAsync();
				await Task.Yield(); await Task.Yield();
			}
#endif
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
		[QName("初始显示")]
		public bool showOnStart = false;
		[QName("遮挡点击")]
		public bool blockInput = false;
		[QName("控制TimeScale")]
		public float timeScale = -1;
#if QTween
		[QName("显示动画")]
		public QTweenBehavior showAnim;
#endif
		[QName("父页面")]
		
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
			if (_instance != null) return _instance;
			_instance = await QUIManager.GetUI(typeof(T).Name) as T;
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

			SceneManager.activeSceneChanged += OnSceneChanged;
			QUIManager.WindowChange += Fresh;
			QUIManager.ResisterPanel(typeof(T).Name, GetComponent<RectTransform>(), ParentPanel);

		}
		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
			SceneManager.activeSceneChanged -= OnSceneChanged;
			QUIManager.WindowChange -= Fresh;
			QUIManager.Remove(GetType().Name, RectTransform);
			if (Prefab != null)
			{
				QUIPanelPrefab.Release(ref Prefab);
			}
		}

		/// <summary>
		/// 切换场景时调用默认隐藏拥有 ParentPanel 的页面
		/// </summary>
		protected virtual void OnSceneChanged(Scene scene,Scene next)
		{
			if (!string.IsNullOrEmpty(ParentPanel))
			{
				HideAndComplete();
				Destroy(gameObject);
			}
		}

		private void Fresh(QUIPanel window)
		{
			if (this == null) return;
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
		public virtual void OnFresh()
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
		/// 显示或隐藏页面时 最开始调用
		/// </summary>
		protected virtual async Task StartShow(bool IsShow)
		{
			QDebug.Log("QUI[" + this + "] 开关 " + IsShow);
			this.IsShow = IsShow;
			if (!gameObject.activeSelf)
			{
				if (IsShow)
				{
					gameObject.SetActive(true);
				}
			}
			else
			{
				OnShow();
			}
#if QTween
			if (showAnim != null)
			{
				var animTask=showAnim.PlayAsync(IsShow);
				await animTask;
				if (animTask.Exception != null)
				{
					Debug.LogError("播放页面" + this + "动画出错 " + animTask.Exception);
				}

			}
			else
#endif
			{
				Group.interactable =IsShow;
				Group.alpha = IsShow ? 1 : 0;
			}
			if (IsShow == base.IsShow)
			{
				gameObject.SetActive(IsShow);
			}
		}
		protected virtual void OnEnable()
		{
			OnShow();
		}
		protected virtual void OnDisable()
		{
			OnHide();
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
			try
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
			catch (System.Exception e)
			{
				Debug.LogError(this + " " + nameof(OnShow) + " 出错：" + e);
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
