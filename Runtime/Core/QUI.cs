using QTool.Inspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
#if QTween
using QTool.Tween;
#endif
namespace QTool.UI {
	[RequireComponent(typeof(CanvasGroup))]
	/// <summary>
	/// 基础UI
	/// </summary>
	public class QUI : MonoBehaviour {

		#region 基础属性
		[QName("唯一窗口"), UnityEngine.Serialization.FormerlySerializedAs("isModalWindow")]
		public bool isTopWindow = false;
		public bool IsShow { internal set; get; }
		public RectTransform RectTransform {
			get {
				return transform as RectTransform;
			}
		}
		public CanvasGroup Group {
			get {
				if (_group == null) {
					_group = GetComponent<CanvasGroup>();
				}
				return _group;
			}
		}
		private CanvasGroup _group;
#if QTween
		[QName("显示动画")]
		public QTweenComponent showAnim;
#endif
		#endregion
		#region 基本生命周期
		protected virtual void Awake() {
			if (!IsShow) {
				IsShow = Group.alpha >= 0.9f;
			}
			QUIManager.OnWindowChange += Fresh;
			QUIManager.Add(this);
			if (gameObject.activeSelf != IsShow) {
				gameObject.SetActive(IsShow);
			}
		}
		protected virtual void OnDestroy() {
			QUIManager.OnWindowChange -= Fresh;
			QUIManager.Remove(this);
		}

		[QName("显示")]
		public void Show() {
			Switch(true);
		}
		public void Show(Action callback) {
#if QTween
			if (showAnim != null) {
				UnityAction action = null;
				action = () => {
					showAnim.OnShow.RemoveListener(action);
					callback?.Invoke();
				};
				showAnim.OnShow.AddListener(action);
				Show();
			}
			else
#endif
			{
				Show();
				callback?.Invoke();
			}
		}
		public void Hide(Action callback) {
#if QTween
			if (showAnim != null) {
				UnityAction action = null;
				action = () => {
					showAnim.OnHide.RemoveListener(action);
					callback?.Invoke();
				};
				showAnim.OnHide.AddListener(action);
				Hide();
			}
			else
#endif
			{
				Hide();
				callback?.Invoke();
			}
		}
		[QName("隐藏")]
		public void Hide() {
			Switch(false);
		}
		public void Switch(bool show) {
			if (this == null)
				return;
			this.IsShow = show;
			if (show) {
				gameObject.SetActive(true);
			}
			Action OnComplete = null;
			OnComplete = () => {
				if (show != IsShow) {
					return;
				}
#if QTween
				if (showAnim != null) {
					showAnim.Anim.OnCompleteEvent -= OnComplete;
				}
#endif
				if (this != null && Application.IsPlaying(this)) {
					gameObject.SetActive(show);
				}
				gameObject.SetDirty();
			};
#if QTween
			if (showAnim != null) {
				showAnim?.Anim.OnComplete(OnComplete);
				showAnim.Play(show);
			}
			else
#endif
			{
				Group.interactable = show;
				if (show) {
					Group.blocksRaycasts = show;
				}
				Group.alpha = show ? 1 : 0;
				OnComplete();
			}
		}
		protected virtual void OnShow() {
			if (Application.isPlaying) {
				if (isTopWindow) {
					QUIManager.WindowStackPush(this);
				}
				OnFresh();
			}
		}
		protected virtual void OnHide() {
			if (isTopWindow) {
				QUIManager.WindowStackRemove(this);
			}
		}
		protected virtual void OnEnable() {
			OnShow();
		}
		protected virtual void OnDisable() {
			OnHide();
		}
		private void Fresh(QUI window) {
			if (this == null)
				return;
			var value = window == null || this.Equals(window) || transform.ParentHas(window.RectTransform);
			if (value) {
				if (IsShow) {
					Group.interactable = true;
					OnFresh();
				}
			}
			else {
				Group.interactable = false;
			}
		}
		/// <summary>
		/// 堆栈窗口更改时调用 刷新数据
		/// </summary>
		public virtual void OnFresh() {

		}
		protected virtual void Complete() {
#if QTween
			showAnim?.Complete();
#endif
		}
		public void ShowAndComplete() {
			Show();
			Complete();
		}
		public void HideAndComplete() {
			Hide();
			Complete();
		}
#endregion
	}

	public abstract class QUI<T> : QUI where T : QUI<T> {
		#region 静态逻辑
		public static T Instance { get; private set; }
		public static bool Exists => Instance != null;
		public static bool PanelIsShow {
			get {
				if (Instance == null) {
					return false;
				}
				else {
					return Instance.IsShow;
				}
			}
		}
		public static void ShowPanel() {
			if (!PanelIsShow) {
				Instance.Show();
			}
		}
		public static void HidePanel() {
			if (PanelIsShow) {
				Instance.Hide();
			}
		}
		#endregion
		#region 基本生命周期
		protected override void Awake() {
			if (this is T panel) {
				Instance = panel;
			}
			else {
				Debug.LogError("页面类型不匹配：" + Instance + ":" + typeof(T));
			}
			base.Awake();
		}
		protected override void OnDestroy() {
			if (Instance == this) {
				Instance = null;
			}
			base.OnDestroy();
		}
		#endregion
	}
}
