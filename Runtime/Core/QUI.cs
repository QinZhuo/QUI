using QTool.Inspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using QTool.Reflection;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEditor;
using UnityEngine.UI;





#if QTween
using QTool.Tween;
#endif
namespace QTool.UI
{
	/// <summary>
	/// 基础UI 自动注册事件
	/// </summary>
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
			QUIManager.Add(this);
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
		public bool IsShow { internal set; get; }
		public RectTransform RectTransform
		{
			get
			{
				return transform as RectTransform;
			}
		}
#if UNITY_EDITOR
		private bool CanGenerate => GetType() != typeof(QUI);
		const string UICodeStart = "//UICodeStart\n";
		const string UICodeEnd = "//UICodeEnd\n";
		[QName("UI字段生成", nameof(CanGenerate))]
		private void GenerateUICode()
		{
			var target = this;
			if (!CanGenerate) return;
			var script = UnityEditor.MonoScript.FromMonoBehaviour(this);
			var path = UnityEditor.AssetDatabase.GetAssetPath(script);
			var className = script.GetClass().Name;
			var data = QFileTool.Load(path);
			var oldUICode = "";
			if (data.Contains(UICodeStart) && data.Contains(UICodeEnd))
			{
				oldUICode = data.GetBlockValue(UICodeStart, UICodeEnd);
			}
			else
			{
				var classIndex = data.IndexOf("class " + className);
				var index = data.IndexOf('{', classIndex);
				var startIndex = data.Substring(0, index).LastIndexOf('\n') + 1;
				var t = data.Substring(startIndex, index - startIndex) + "    ";
				data = data.Insert(index + 1, "\n" + t + UICodeStart + t + "\n" + t + UICodeEnd);
				oldUICode = data.GetBlockValue(UICodeStart, UICodeEnd);
			}
			var start = oldUICode.Substring(oldUICode.LastIndexOf('\n'));
			var newUICode = QTool.BuildString(writer =>
			{
				var texts = gameObject.GetComponentsInChildren<Text>();
				foreach (var item in texts)
				{
					writer.Write(string.Format("{0}public Text {1}=null;\n", start, item.name.Replace(" ", "_")));
				}
			});
			newUICode += start;
			if (oldUICode != newUICode)
			{
				data = data.Replace(UICodeStart + oldUICode + UICodeEnd, UICodeStart + newUICode + UICodeEnd);
				Debug.LogError(data);
			}
			QFileTool.Save(path, data);
			AssetDatabase.Refresh();
		}
#endif
	}

	[RequireComponent(typeof(CanvasGroup))]
	public abstract class QUI<T> : QUI where T : QUI<T>
	{
		#region 静态逻辑
		private static T _instance;
		public static T Instance
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
		[QName("模态窗"), UnityEngine.Serialization.FormerlySerializedAs("isWindow")]
		public bool isModalWindow = false;
#if QTween
		[QName("显示动画")]
		public QTweenComponent showAnim;
#endif
		[QName("父页面"), QPopup(nameof(UI_Prefab) + "." + nameof(UI_Prefab.LoadAll))]

		public string ParentPanel = "";
		public CanvasGroup Group => _group == null ? _group = GetComponent<CanvasGroup>() : _group;
		private CanvasGroup _group;
		#endregion
		#region 基本生命周期

		private int initIndex = -1;
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
			#region 动态创建时设置父物体并隐藏
			if (transform.parent == null)
			{
				if (!ParentPanel.IsNull())
				{
					var parent = QUIManager.Load(ParentPanel)?.transform;
					if (parent == null)
					{
						QDebug.LogWarning("找不到父页面[" + ParentPanel + "]");
					}
					else if (parent != this)
					{
						var scale = RectTransform.localScale;
						RectTransform.SetParent(parent, false);
						RectTransform.localScale = scale;
						if (RectTransform.anchorMin == Vector2.zero && RectTransform.anchorMax == Vector2.one)
						{
							RectTransform.offsetMin = Vector2.zero;
							RectTransform.offsetMax = Vector2.zero;
						}
					}
				}
				if (transform.parent == null)
				{
					transform.parent = FindFirstObjectByType<Canvas>()?.transform;
				}
				HideAndComplete();
			}
			else if (IsShow)
			{
				initIndex = transform.GetSiblingIndex();
			}
			#endregion
			QUIManager.Add(this);
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
		protected virtual void OnShow()
		{
			try
			{
				if (initIndex < 0)
				{
					transform.SetAsLastSibling();
				}
				else
				{
					transform.SetSiblingIndex(initIndex);
					initIndex = -1;
				}
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
		protected virtual void OnHide()
		{
			try
			{
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
		protected virtual void OnEnable()
		{
			OnShow();
		}
		protected virtual void OnDisable()
		{
			OnHide();
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
					gameObject.SetActive(false);
				}
			}
			gameObject.SetDirty();
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

	public abstract class QUI<T, TData> : QUI<T> where T : QUI<T, TData>
	{
		public virtual TData Data { get; protected set; }
		protected override void Awake()
		{
			base.Awake();
			QEventManager.Register<TData>(GetType().Name + "_" + nameof(Show) + "_" + typeof(TData).Name, Show);
		}
		public virtual void Show(TData data)
		{
			Data = data;
			Show();
		}
		protected override void OnHide()
		{
			base.OnHide();
			Data = default;
		}

	}
}
