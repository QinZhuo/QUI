using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
    public class QFollowUI : MonoBehaviour
    {
		[SerializeField]
        private Transform target;
		public Transform Target
		{
			get => target;
			set
			{
				target = value;
				if (target!=null&& useBoundsHeight)
				{
					bounds = target.GetBounds();
				}
			}
		}
		public bool worldPosition = false;	
		[QName("使用包围盒高度")]
		public bool useBoundsHeight=false;
		Bounds bounds;
		public Vector3 offset=Vector3.zero;
        private void LateUpdate()
        {
            if (target != null&&target.gameObject.activeInHierarchy)
            {
				var runtimeOffset = offset;
				if (useBoundsHeight)
				{
					runtimeOffset += bounds.size.y*Vector3.up;
				}
				var position = target.position + runtimeOffset;
				transform.position = worldPosition? position: Camera.main.WorldToScreenPoint(position);
            }
        }
		public void Recover()
		{
			if (QFollowList.PanelIsShow)
			{
				QFollowList.Push(gameObject);
			}
		}
    }

}
