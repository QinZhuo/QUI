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

		Color.RGBToHSV(m_Color, out var h, out var s, out var v);
		foreach (var modifier in modifiers)
		{
			Color.RGBToHSV(modifier.targetColor, out var th, out var ts, out var tv);
			if (s > 0)
			{
				modifier.hue = Mathf.Repeat(h - th + 0.5f, 1) - 0.5f;
			}
			if (v > 0)
			{
				modifier.saturation = (s - ts) / 2;
			}
			modifier.value = (v - tv)/2;
		}
	}
}
#endif
