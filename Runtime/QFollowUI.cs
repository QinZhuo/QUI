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
            transform.position = Camera.main.WorldToViewportPoint(target.position);
        }
    }

}
