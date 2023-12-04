using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
	[ExecuteInEditMode]
	public class QRangeLimitUI : MonoBehaviour
	{
		private void Update()
		{
			var parentRect = transform.parent as RectTransform;
			var rect = transform as RectTransform;
			var offset = rect.UpRight() - parentRect.UpRight();
			rect.anchoredPosition -= new Vector2(offset.x > 0 ? offset.x : 0, offset.y > 0 ? offset.y : 0);
			offset = rect.DownLeft() - parentRect.DownLeft();
			rect.anchoredPosition -= new Vector2(offset.x < 0 ? offset.x : 0, offset.y < 0 ? offset.y : 0);
		}
	}
}
