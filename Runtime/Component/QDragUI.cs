using QTool;
using QTool.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(QScreenLimitUI))]
public class QDragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public UnityEvent<int, int> onDragEnd;
	public LayoutElement layoutElement;
	private int startLayoutIndex = 0;
	private int startIndex = 0;
	private void Reset() {
		layoutElement = GetComponent<LayoutElement>();
	}
	public void OnBeginDrag(PointerEventData eventData) {
		layoutElement.ignoreLayout = true;
		startLayoutIndex = transform.GetSiblingIndex();
		startIndex = GetIndex();
		transform.SetAsLastSibling();
	}

	public int GetIndex() {
		if (transform.parent == null)
			return -1;
		var index = 0;
		for (int i = 0; i < transform.parent.childCount; i++) {
			var drag = transform.parent.GetChild(i).GetComponent<QDragUI>();
			if (drag != null && drag.gameObject.activeSelf) {
				if (this == drag) {
					return index;
				}
				index++;
			}
		}
		return -1;
	}
	public void OnDrag(PointerEventData eventData) {
		transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData) {
		var rect = transform as RectTransform;
		var endIndex = 0;
		for (int i = 0; i < transform.childCount; i++) {
			var view = transform.GetChild(i)?.GetComponent<QDragUI>();
			if (view != null && view != this) {
				rect.ContainsScreenPoint(view.transform.position);
				endIndex = view.GetIndex();
			}
		}
		layoutElement.ignoreLayout = false;
		transform.SetSiblingIndex(endIndex - startIndex + startLayoutIndex);
		QDebug.Log(nameof(QDragUI) + ":" + startIndex + "=>" + endIndex);
		onDragEnd.Invoke(startIndex, endIndex);
	}
}
