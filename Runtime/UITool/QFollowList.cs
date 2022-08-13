using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
	[RequireComponent(typeof(QObjectList))]
	public class QFollowList :UIPanel<QFollowList>
	{
		public QObjectList objectList;
		protected void Reset()
		{
			objectList = GetComponent<QObjectList>();
		}
		public static void Push(GameObject obj)
		{
			Instance.objectList.Push(obj);
		}
		public static QFollowUI GetQFollowUI(Transform target)
		{
			if (target == null)
			{
				Debug.LogError("follow目标为空");
				return null;
			}
			if (Instance == null)
			{
				return null;
			}
			var obj=Instance.objectList[target.GetHashCode().ToString()];
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
