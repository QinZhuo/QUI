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
			throw new System.Exception("未实现QUIPanel Set函数[" + typeof(TObj) + "]" + obj);
		}
		public virtual Task ShowAsync()
		{
			gameObject.SetActive(true);
			return Task.CompletedTask;
		}

		public virtual Task HideAsync()
		{
			gameObject.SetActive(false);
			return Task.CompletedTask;
		}

		public virtual void ResetUI()
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
		#region 单例逻辑
		private static T _instance;
		protected static T Instance
		{
			get
			{
				if (_instance != null || !Application.isPlaying) return _instance;
				_instance = QUIManager.Get(typeof(T).Name) as T;
				return _instance;
			}
		}
		#endregion
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
		#region 基础属性
		[QName("初始显示")]
		public bool showOnStart = false;
		[QName("遮挡点击")]
		public bool blockInput = true;
		[QName("控制TimeScale")]
		public float timeScale = -1;
#if QTween
		[QName("显示动画")]
		public QTweenComponent showAnim;
#endif
		[QName("父页面"),QPopup(nameof(QUIPanelPrefab) + "." + nameof(QUIPanelPrefab.LoadAll))]
		
		public string ParentPanel = "";
		public ActionEvent OnShowAction;
		public ActionEvent OnHideAction;
		CanvasGroup _group;
		public CanvasGroup Group => _group ??= GetComponent<CanvasGroup>();

		#endregion
		#region 基本生命周期
		
		
		protected virtual void Awake()
		{
			if (this is T panel)
			{
				_instance = this as T;
			}
			else
			{
				Debug.LogError("页面类型不匹配：" + _instance + ":" + typeof(T));
			}
			IsShow = Group.alpha >= 0.9f&&gameObject.activeSelf;
			SceneManager.sceneLoaded += OnSceneChanged;
			QUIManager.WindowChange += Fresh;
			QUIManager.ResisterPanel(this, ParentPanel);
			if (ParentPanel.IsNull()&&IsShow&&gameObject.activeInHierarchy)
			{
				OnFresh();
			}
		}
		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
			SceneManager.sceneLoaded -= OnSceneChanged;
			QUIManager.WindowChange -= Fresh;
			QUIManager.Remove(this);
		}

		/// <summary>
		/// 切换场景时调用默认隐藏拥有 ParentPanel 的页面
		/// </summary>
		protected virtual void OnSceneChanged(Scene scene, LoadSceneMode mode)
		{
			if (this == null) return;
			if (!string.IsNullOrEmpty(ParentPanel)&& mode== LoadSceneMode.Single)
			{
				if(QSceneUISetting.Instance == null||!QSceneUISetting.Instance.PanelList.Contains(name))
				{
					if (PanelIsShow)
					{
						OnHide();
					}
					Destroy(gameObject);
				}
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
					if (blockInput)
					{
						Group.blocksRaycasts = true;
					}
					OnFresh();
				}
			}
			else
			{
				Group.interactable = false;
				if (blockInput)
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
			if (gameObject.activeSelf != IsShow)
			{
				gameObject.SetActive(IsShow);
			}
		}
	



	
		/// <summary>
		/// 显示或隐藏页面时 最开始调用
		/// </summary>
		protected virtual async Task StartShow(bool IsShow)
		{
			if (this == null) return;
			this.IsShow = IsShow;
			if (IsShow)
			{
				OnShow();
			}
#if QTween
			if (showAnim != null)
			{
				if (!gameObject.activeSelf)
				{
					if (IsShow)
					{
						gameObject.SetActive(true);
					}
				}
				var animTask=showAnim.PlayAsync(IsShow);
				if(await animTask.IsCancel())
				{
					return;
				}
				if (animTask.Exception != null)
				{
					Debug.LogError("播放页面" + this + "动画出错 " + animTask.Exception);
				}
				if (IsShow != base.IsShow) {
					return;
				}
				if (this!=null&&Application.IsPlaying(this))
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
				if (blockInput)
				{
					Group.blocksRaycasts = IsShow;
				}
				Group.alpha = IsShow ? 1 : 0;
			}
			if (!this.IsShow)
			{
				OnHide();
			}
		}
		public override async Task ShowAsync()
		{
			await StartShow(true).Run();
		}

		public override async Task HideAsync()
		{
			await StartShow(false).Run();
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
					OnFresh();
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
			if (this != null)
			{
				QTime.RevertScale(gameObject);
			}
			if (blockInput)
			{
				QUIManager.WindowRemove(this);
			}
		}

		#endregion
		
	}
}
