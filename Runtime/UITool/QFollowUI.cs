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
                transform.position = Camera.main.WorldToScreenPoint(target.position+ offset + Vector3.up* transform.GetBounds().max.y*(useBoundsHeight ? 1:0));
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
