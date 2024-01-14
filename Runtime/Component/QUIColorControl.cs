using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool;
#if UIEffect
using Coffee.UIEffects;
public class QUIColorControl : QKeyColor
{
	public UIHsvModifier[] modifiers;
	private void Reset()
	{
		modifiers = GetComponentsInChildren<UIHsvModifier>();
		if (modifiers.Length > 0)
		{
			m_Color = modifiers.Get(0).targetColor;
		}
	}
	protected override void OnValidate()
	{
		base.OnValidate();
		foreach (var modifier in modifiers)
		{
			modifier.hue = Mathf.Repeat(m_Color.ToH() - modifier.targetColor.ToH() + 0.5f, 1) - 0.5f; 
		}
	}
}
#endif
