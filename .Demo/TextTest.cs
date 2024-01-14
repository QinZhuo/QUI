using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    [ExecuteInEditMode]
    public class TextTest : MonoBehaviour
    {
        public RectTransform rect;

        // Update is called once per frame
        void Update()
        {
            if (rect != null)
            {
                var text= GetComponent<Text>();
				if (text!=null)
				{
					rect.transform.position = text.GetPos(text.text.Length - 1);
				}
            }
        }
    }

}
