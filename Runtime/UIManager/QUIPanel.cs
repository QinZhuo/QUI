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
		public abstract void Switch(bool show);
		public abstract void Show();
		public abstract Task ShowAsync();
		public abstract void Hide();
		public abstract Task HideAsync();
		public abstract void ResetUI();
		public abstract bool IsShow { protected set; get; }
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
		public static async Task<T> GetInstance()
		{
			return await QUIManager.GetUI(typeof(T).Name) as T;
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
		[ViewName("初始显示")]
		public bool showOnStart = false;
		[ViewName("时间控制")]
		public float timeScale = -1;
		[ViewName("遮挡点击")]
		public bool blockInput = false;
		protected virtual void FreshWindow(UIPanel window)
		{
			if (group == null) return;

			var value = window == null || this.Equals(window) || transform.HasParentIs(window.RectTransform);
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
			if (!string.IsNullOrEmpty(ParentPanel))
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

			if (group == null)
			{
				group = GetComponent<CanvasGroup>();
			}
			IsShow = group.alpha >= 0.9f;

			SceneManager.activeSceneChanged += OnSceneChange;
			QUIManager.WindowChange += FreshWindow;
			QUIManager.ResisterPanel(name.Contains("(Clone)") ? name.Substring(0, name.IndexOf("(Clone)")) : name, GetComponent<RectTransform>(), ParentPanel);
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
				group.interactable = IsShow;
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
				QUIManager.WindowPush(this);
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


		public static async Task SwitchPanel(bool switchBool)
		{
			if (switchBool)
			{
				await ShowPanel();
			}
			else
			{
				HidePanel();
			}
		}

		public static async Task ShowPanel()
		{
			await GetInstance();
			if (Application.isPlaying)
			{
				Instance?.Show();
			}
		}
		public static void HidePanel()
		{
			if (PanelIsShow)
			{
				Instance.Hide();
			}
		}
		[ViewButton("显示")]
		public override void Show()
		{
			_ = ShowAsync();
		}
		//List<object> showObj = new List<object>();

		public override void Switch(bool show)
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

		public override async Task ShowAsync()
		{
			IsShow = true;
			await RunAnim();
		}
		[ViewButton("隐藏")]
		public override void Hide()
		{
			//showObj.Clear();
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
