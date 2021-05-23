using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace QTool.UI {
    [AddComponentMenu("QUI/QMask", 0)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class QMask : UIBehaviour
    {
        public RectTransform MaskRoot
        {
            get
            {
                return _maskRoot == null ? rectTransform : _maskRoot;
            }
        }
        public RectTransform _maskRoot;
        [NonSerialized]
        private RectTransform m_RectTransform;
        [SerializeField]
        private bool _reverse;
        public bool Reverse
        {
            get
            {
                return _reverse;
            }
            set
            {
                if (value != _reverse)
                {
                    _reverse = value;
                    graphic.SetMaterialDirty();
                }
            }
        }
        public RectTransform rectTransform
        {
            get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
        }

        [SerializeField]
        private bool m_ShowMaskGraphic = false;

        /// <summary>
        /// Show the graphic that is associated with the Mask render area.
        /// </summary>
        public bool showMaskGraphic
        {
            get { return m_ShowMaskGraphic; }
            set
            {
                if (m_ShowMaskGraphic == value)
                    return;

                m_ShowMaskGraphic = value;
                if (graphic != null)
                    graphic.SetMaterialDirty();
            }
        }

        [NonSerialized]
        private Graphic m_Graphic;

        /// <summary>
        /// The graphic associated with the Mask.
        /// </summary>
        public Graphic graphic
        {
            get { return m_Graphic ?? (m_Graphic = GetComponent<Graphic>()); }
        }

        [NonSerialized]
        private Material m_MaskMaterial;

        [NonSerialized]
        private Material m_UnmaskMaterial;

        protected QMask()
        { }

        public virtual bool MaskEnabled() { return IsActive() && graphic != null; }

        private void OnTransformChildrenChanged()
        {
            CheckChildrenMaskTarget();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckChildrenMaskTarget();
        }
        protected override void OnDestroy()
        {
            Clear();
        }
        public void CheckChildrenMaskTarget()
        {
            foreach (var child in MaskRoot.GetComponentsInChildren<MaskableGraphic>())
            {
                if (child == graphic) continue;
                var target = child.GetComponent<QMaskTarget>();
                if (target == null)
                {
                    target = child.gameObject.AddComponent<QMaskTarget>();
                }
                Add(target);
            }
            //for (int i = 0; i < MaskRoot.childCount; i++)
            //{
            //    var child = MaskRoot.GetChild(i);
              
            //}
        }
        public void Clear()
        {
            ForeachMaskTarget((target) => { Remove(target); });
            targetList.Clear();
        }
        public void Add(QMaskTarget target)
        {
            target.mask = this;
            targetList.AddCheckExist(target);
        }
        public void Remove(QMaskTarget target)
        {
            targetList.Remove(target);

#if UNITY_EDITOR
            DestroyImmediate(target);
#else
            Destroy(target);
#endif
        }
        List<QMaskTarget> targetList = new List<QMaskTarget>();
        public void ForeachMaskTarget(Action<QMaskTarget> action)
        {
            for (int i = targetList.Count-1; i >= 0; i--)
            {
                action?.Invoke(targetList[i]);
            }
        }
     
        protected override void OnDisable()
        {

            base.OnDisable();

        }
        public bool MaskActive
        {
            get
            {
                return IsActive();
            }
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
                return;
            graphic.enabled = m_ShowMaskGraphic;
        }

#endif



    }
}