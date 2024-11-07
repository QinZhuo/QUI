using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTool.UI {
	/// <summary>
	/// 场景预加载UI配置 
	/// </summary>
	public class QUISceneSetting : QSingletonBehaviour<QUISceneSetting> {
		public bool autoLoadAsync = false;
		[QName("加载UI")]
		[QObject(typeof(GameObject))]
		public List<string> PanelList = new List<string>();
		private bool isLoading = false;
		protected void Start() {
			base.Awake();
			if (autoLoadAsync) {
				LoadAsync();
			}
		}
		public void Load() {
			if (!isLoading) {
				foreach (var uiKey in PanelList) {
					var prefab = Resources.Load<GameObject>(uiKey);
					if (prefab == null) {
						QDebug.LogError("不存在UI预制体 [" + uiKey + "]");
						return;
					}
					Instantiate(prefab).GetComponent<QUI>(true);
				}
				PanelList.Clear();
			}

		}
		public void LoadAsync() {
			QDebug.Begin("异步加载场景UI");
			if (!isLoading) {
				isLoading = true;
				foreach (var uiKey in PanelList) {
					var prefab = Resources.LoadAsync<GameObject>(uiKey);
					prefab.completed += result => {
						PanelList.Remove(uiKey);
						if (prefab.asset is GameObject gameObject) {
							Instantiate(gameObject).GetComponent<QUI>(true);
							if (PanelList.Count == 0) {
								isLoading = false;
								QDebug.End("异步加载场景UI");
							}
						}
						else {
							QDebug.LogError("不存在UI预制体 [" + uiKey + "]");
						}
						
					};
				}
			}


		}
	}
}
