using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QTool;
using UnityEngine.Serialization;
using QTool.Inspector;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
namespace QTool.UI
{

	/// <summary>
	/// UI管理器
	/// </summary>
	public static class QUIManager
	{
		#region 管理
		private static QDictionary<string, QUI> UIList = new QDictionary<string, QUI>();
		public static event System.Action<QUI> OnCurrentWindowChange;
		/// <summary>
		/// 注册UI到管理器
		/// </summary>
		internal static void Add(QUI panel)
		{
			UIList[panel.QName()] = panel;
		}
		internal static void Remove(QUI panel)
		{
			var key = panel.name;
			if (!key.IsNull() && UIList.ContainsKey(key))
			{
				var ui = UIList[key];
				if (ui == panel)
				{
					UIList.RemoveKey(key);
				}
			}
		}
		public static List<QUI> ModalWindowStack { get; private set; } = new List<QUI>();
		internal static void ModalWindowPush(QUI window)
		{
			if (ModalWindowStack.StackPeek() == window)
			{
				return;
			}
			if (ModalWindowStack.Contains(window))
			{
				ModalWindowStack.Remove(window);
			}
			ModalWindowStack.Push(window);
			OnCurrentWindowChange?.Invoke(ModalWindowStack.StackPeek());
		}

		internal static void ModalWindowRemove(QUI window)
		{
			if (ModalWindowStack.Count == 0 || !ModalWindowStack.Contains(window)) return;
			ModalWindowStack.Remove(window);
			OnCurrentWindowChange?.Invoke(ModalWindowStack.StackPeek());
		}
		internal static QUI Load(string key)
		{
			if (!QTool.IsPlaying) return null;
			if (UIList[key] == null)
			{
				QDebug.Begin("动态创建" + nameof(QUI) + "<" + key + ">");
				var prefab = UI_Prefab.Load(key);
				if (prefab == null)
				{
					QDebug.LogError("不存在UI预制体 [" + key + "]");
					return null;
				}
				var panel = Object.Instantiate(prefab).GetComponent<QUI>(true);
				panel.name = key;
				QDebug.End("动态创建" + nameof(QUI) + "<" + key + ">");
			}
			return UIList[key];
		}
		internal static async Task<QUI> LoadAsync(string key)
		{
			await UI_Prefab.LoadAsync(key);
			return Load(key);
		}
		#endregion
		#region 枚举接口
		public static void Show<TData>(this System.Enum key, TData data)
		{
			QEventManager.InvokeEvent(key.ToString() + "_" + nameof(Show) + "_" + typeof(TData).Name);
		}
		public static void Show(this System.Enum key)
		{
			if (IsShow(key)) return;
			Load(key.ToString()).Show();
		}
		public static void Hide(this System.Enum key)
		{
			if (!IsShow(key)) return;
			Load(key.ToString()).Hide();
		}
		public static bool IsShow(this System.Enum enumKey)
		{
			var key = enumKey?.ToString();
			if (UIList.ContainsKey(key)) return UIList[key.ToString()].GetComponent<QUI>().IsShow;
			return false;
		}
		public static async Task Switch(this System.Enum enumKey, bool show)
		{
			if (show == IsShow(enumKey))
			{
				return;
			}
			if (show)
			{
				await Load(enumKey.ToString()).ShowAsync();
			}
			else
			{
				await Load(enumKey.ToString()).HideAsync();
			}
		}
		#endregion
	}


	public class UI_Prefab : QPrefabLoader<UI_Prefab>
	{
	}
}
