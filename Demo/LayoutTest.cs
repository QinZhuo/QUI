using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTool.UI;
public class LayoutTest : UIPanel<LayoutTest>
{
    public QElementList testList;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            var view= testList[i.ToString()];
        }
       
    }

   
}
