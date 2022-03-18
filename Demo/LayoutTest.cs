using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool;
namespace QTool.UI
{

    public class LayoutTest : UIPanel<LayoutTest>
    {
        public QObjectList testList;
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                var view = testList[i.ToString()];
                view.transform.localScale = Vector3.zero;
            }

        }
    }
}
