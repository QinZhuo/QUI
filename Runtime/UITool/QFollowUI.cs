using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
    public class QFollowUI : MonoBehaviour
    {
        public Transform target;
		[ViewName("使用包围盒高度")]
		public bool useBoundsHeight=false;
		public Vector3 offset=Vector3.zero;
        private void LateUpdate()
        {
            if (target != null)
            {
				var runtimeOffset = offset;
				if (useBoundsHeight)
				{
					var bounds = transform.GetBounds();
					runtimeOffset += bounds.size.y*Vector3.up;
				}
				transform.position = Camera.main.WorldToScreenPoint(target.position+ runtimeOffset);
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
