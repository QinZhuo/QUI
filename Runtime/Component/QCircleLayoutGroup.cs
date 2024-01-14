using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
	public class QCircleLayoutGroup : LayoutGroup
	{
		private Vector2 Center
		{
			get
			{
				return rectTransform.rect.size / 2;
			}
		}
		private Vector2 Size
		{
			get
			{
				return rectTransform.rect.size;
			}
		}


		[UnityEngine.Serialization.FormerlySerializedAs("distance"), SerializeField]
		private float _spacing = 20;
		public float Spacing { get => _spacing; set => _spacing = value; }
		[QName("控制旋转")]
		public bool rotate = false;
		private Vector2[] childPositions = new Vector2[0];
		private Quaternion[] childRotations = new Quaternion[0];
		public override void CalculateLayoutInputVertical()
		{
			FreshCirclePos();
		}
		public override void SetLayoutHorizontal()
		{
		}
		public override void SetLayoutVertical()
		{
			SetChildren();
		}
		private void SetChildren()
		{
			for (int i = 0; i < rectChildren.Count; i++)
			{
				var child = rectChildren[i];
				SetChildAlongAxis(child, 0, childPositions[i].x);
				SetChildAlongAxis(child, 1, childPositions[i].y);
				if (rotate)
				{
					rectChildren[i].rotation = childRotations[i];
				}
			}
		}
		private void FreshCirclePos()
		{
			var Spacing = this.Spacing;
			if (Spacing <= 0)
			{
				Spacing = 360f / rectChildren.Count;
			}
			var startAngle = 0f;
			var reverse = false;
			switch (childAlignment)
			{
				case TextAnchor.MiddleLeft:
				case TextAnchor.UpperLeft:
					reverse = true;
					startAngle = -90 + padding.right;
					break;
				case TextAnchor.LowerLeft:
					startAngle = 90 + padding.right;
					break;
				case TextAnchor.UpperRight:
				case TextAnchor.MiddleRight:
					startAngle = -90 + padding.left;
					break;
				case TextAnchor.LowerRight:
					reverse = true;
					startAngle = 90 + padding.left;
					break;

				case TextAnchor.MiddleCenter:
				case TextAnchor.UpperCenter:
					startAngle = -(90 + (rectChildren.Count - 1) * Spacing / 2) + padding.left + padding.right;
					break;
				case TextAnchor.LowerCenter:
					startAngle = 90 - (rectChildren.Count - 1) * Spacing / 2 + padding.left + padding.right;
					break;
				default:
					break;
			}
			childPositions = new Vector2[rectChildren.Count];
			childRotations = new Quaternion[rectChildren.Count];
			var start = startAngle / 360 * Mathf.PI * 2;
			
			var offset = Spacing / 360 * Mathf.PI * 2;
			
			for (int i = 0; i < rectChildren.Count; i++)
			{
				SetAngle(i, start + (reverse ? -1 : 1) * i * offset);
			}
		}
		private void SetAngle(int i, float angle)
		{
			var pos = new Vector2(Mathf.Cos(angle) * Size.x, Mathf.Sin(angle) * Size.y) / 2 - rectChildren[i].sizeDelta / 2;
			childPositions[i] = Center + pos;
			if (rotate)
			{
				childRotations[i] = Quaternion.Euler(0, 0, -angle / Mathf.PI * 180);
			}
		}
	}
}
