using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{
    public class QFollowUI : MonoBehaviour
    {
        public Transform target;
        private void LateUpdate()
        {
            if (target != null)
            {
                transform.position = Camera.main.WorldToScreenPoint(target.position);
            }
        }
    }

}
