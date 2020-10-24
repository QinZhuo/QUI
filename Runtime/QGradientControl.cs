using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace QTool.UI
{

    [ExecuteInEditMode]
    public class QGradientControl : MonoBehaviour
    {
        public QGradientUI ui;
        [Range(0,1)]
        public float lerpT;
        public float delay = 0.1f;
        private void Reset()
        {
            ui = GetComponent<QGradientUI>();
        }

        // Update is called once per frame
        void Update()
        {
            ui.SetAlphaPos(lerpT, delay, 1, 2);
        }
    }

}