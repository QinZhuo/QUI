using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool;
#if UIEffect
using Coffee.UIEffects;
public class QUIColorControl : MonoBehaviour
{
	[SerializeField]
	private Color m_Color = Color.green;
	public UIHsvModifier[] modifiers;
	public Color Color
	{
		get => m_Color; set
		{
			m_Color = value;
			OnValidate();
		}
	}
	private void Reset()
	{
		modifiers = GetComponentsInChildren<UIHsvModifier>();
		if (modifiers.Length > 0)
		{
			m_Color = modifiers.Get(0).targetColor;
		}
	}
	private void OnValidate()
	{
		foreach (var modifier in modifiers)
		{
			modifier.hue = Color.ToH() - modifier.targetColor.ToH();
		}
	}
}
#endif
