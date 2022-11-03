using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
    public class QUIPanelButtons : QObjectList
	{
		public static QUIPanelButtons Instance { get; private set; }
		private void Awake()
		{
			Instance = this;
		}
	}

}
