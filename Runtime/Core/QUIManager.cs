using System.Collections.Generic;
using UnityEngine;
using System;
namespace QTool.UI {

	/// <summary>
	/// UI管理器
	/// </summary>
	public static class QUIManager {
		private static QDictionary<string, QUI> UIDic = new QDictionary<string, QUI>();
		public static event Action<QUI> OnWindowChange;
		/// <summary>
		/// 注册UI到管理器
		/// </summary>
		internal static void Add(QUI panel) {
			var name = panel.QName();
			panel.name = name;
			UIDic[name] = panel;
		}
		internal static void Remove(QUI panel) {
			var key = panel.name;
			if (!key.IsNull() && UIDic.ContainsKey(key)) {
				var ui = UIDic[key];
				if (ui == panel) {
					UIDic.RemoveKey(key);
				}
			}
		}
		public static List<QUI> WindowStack { get; private set; } = new List<QUI>();
		internal static void WindowStackPush(QUI window) {
			if (WindowStack.StackPeek() == window) {
				return;
			}
			if (WindowStack.Contains(window)) {
				WindowStack.Remove(window);
			}
			WindowStack.Push(window);
			OnWindowChange?.Invoke(WindowStack.StackPeek());
		}

		internal static void WindowStackRemove(QUI window) {
			if (WindowStack.Count == 0 || !WindowStack.Contains(window))
				return;
			WindowStack.Remove(window);
			OnWindowChange?.Invoke(WindowStack.StackPeek());
		}
	
		public static void Show(this string key) {
			if (IsShow(key))
				return;
			if (UIDic.ContainsKey(key)) {
				UIDic[key].Show();
			}
		}
		public static void Hide(this string key) {
			if (!IsShow(key))
				return;
			if (UIDic.ContainsKey(key)) {
				UIDic[key].Hide();
			}
		}
		public static bool IsShow(this string key) {
			if (UIDic.ContainsKey(key))
				return UIDic[key.ToString()].GetComponent<QUI>().IsShow;
			return false;
		}
		public static void Switch(this string key, bool show) {
			if (show == key.IsShow()) {
				return;
			}
			if (show) {
				key.Show();
			}
			else {
				key.Hide();
			}
		}
	}
}
