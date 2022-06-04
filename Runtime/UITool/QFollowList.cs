using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
	[RequireComponent(typeof(QObjectList))]
	public class QFollowList :UIPanel<QFollowList>
	{
		public QObjectList objectList;
		protected override void Reset()
		{
			base.Reset();
			objectList = GetComponent<QObjectList>();
		}
		public QFollowUI GetUI(Transform target)
		{
			if (target == null)
			{
				Debug.LogError("follow目标为空");
				return null;
			}
			var obj= objectList[target.GetHashCode().ToString()];
			var followUI = obj.GetComponent<QFollowUI>();
			if(followUI!=null)
			{
				followUI.target = target;
				return followUI;
			}
			else
			{
				throw new System.Exception(target + " 目标不存在脚本 " + typeof(QFollowUI));
			}
		}
	}

}
