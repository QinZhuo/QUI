using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
namespace QTool.UI
{

	[ExecuteInEditMode]
	public class QGradientControl : MonoBehaviour
	{
		public QGradientUI ui;
		public float ViewPos
		{
			get => _viewPos;
			set
			{
				if (_viewPos != value)
				{
					_viewPos = value;
					ui.SetAlphaPos(_viewPos, delay, 1, 2);
				}
			}
		}
		[QName("显示内容"), Range(0, 1), SerializeField, FormerlySerializedAs("lerpT")]
		private float _viewPos;
		public float delay = 0.1f;
		private void Reset()
		{
			ui = GetComponent<QGradientUI>();
		}
#if UNITY_EDITOR
		void OnValidate()
		{
			ui.SetAlphaPos(_viewPos, delay, 1, 2);
		}
#endif
	}
}
