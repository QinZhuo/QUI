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
	public class QUIPanel : MonoBehaviour
	{
		public virtual void Set<TObj>(TObj obj)
		{
			throw new System.Exception("未实现" + GetType() + ".Set函数[" + typeof(TObj) + "]" + obj);
		}
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

		protected virtual void Awake()
		{
			gameObject.SetActive(true);
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
	public abstract class QUIPanel<T> : QUIPanel where T : QUIPanel<T>
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
		public static async Task ShowPanel()
		{
			await Instance?.ShowAsync();
		}
		public static async Task HidePanel()
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
		[QName("弹窗模式"),UnityEngine.Serialization.FormerlySerializedAs("blockInput")]
		public bool isWindow = true;
		[QName("控制TimeScale")]
		public float timeScale = -1;
#if QTween
		[QName("显示动画")]
		public QTweenComponent showAnim;
#endif
		[QName("父页面"),QPopup(nameof(QUIPanelPrefab) + "." + nameof(QUIPanelPrefab.LoadAll))]
		
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
			IsShow = Group.alpha >= 0.9f && gameObject.activeSelf;
			QUIManager.OnCurrentWindowChange += Fresh;
			QUIManager.ResisterPanel(this, ParentPanel);

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
					if (timeScale >= 0)
					{
						QTime.ChangeScale(gameObject, timeScale);
					}
					if (isWindow)
					{
						QUIManager.WindowPush(this);
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
				if (isWindow)
				{
					QUIManager.WindowRemove(this);
				}
			}
			catch (System.Exception e)
			{
				QDebug.LogError(this + " " + nameof(OnDisable) + " 出错：" + e);
			}
		}


		private void Fresh(QUIPanel window)
		{
			if (this == null) return;
			var value = window == null || this.Equals(window) || transform.ParentHas(window.RectTransform);
			if (value)
			{
				if (IsShow)
				{
					Group.interactable = true;
					if (isWindow)
					{
						Group.blocksRaycasts = true;
					}
					OnFresh();
				}
			}
			else
			{
				Group.interactable = false;
				if (isWindow)
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
		protected virtual async Task Switch(bool IsShow)
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
				if (Application.IsPlaying(this))
				{
					gameObject.SetActive(IsShow);
				}
				Group.interactable = IsShow;
				if (isWindow)
				{
					Group.blocksRaycasts = IsShow;
				}
				Group.alpha = IsShow ? 1 : 0;
			}
			if (!this.IsShow)
			{
				gameObject.SetActive(false);
			}
		}
		public override async Task ShowAsync()
		{
			await Switch(true).Run();
		}

		public override async Task HideAsync()
		{
			await Switch(false).Run();
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
}
