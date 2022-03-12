using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace QTool.UI
{
    
    public class QForceLayout : MonoBehaviour
    {
        [System.Serializable]
        public class ActionEvent : UnityEvent
        {
        }
        public ActionEvent OnLayout;
        [ContextMenu("布局")]
        public void Layout()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(WaitLayout());
            }
        }
        public int DelayLayout = 1;
        IEnumerator WaitLayout()
        {
            for (int i = 0; i < DelayLayout; i++)
            {
                yield return null;
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
                OnLayout?.Invoke();

            }

        }

    }
}

