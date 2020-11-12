using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    public class LayoutViewContent : ForceLayout
    {

        public QTool.ObjectPool<GameObject> GetPool(int index)
        {
            return PoolManager.GetPool(viewPrefab[index]);
        }
        private void Awake()
        {
            objList = new List<GameObject>[viewPrefab.Length];
            for (int i = 0; i < objList.Length; i++)
            {
                objList[i] = new List<GameObject>();
            }
            
           
        }
        public GameObject[] viewPrefab;
        public virtual GameObject this[string name, int index = 0]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    Debug.LogError(this + "索引为空[" + name + "]");
                    return null;
                }
                var trans = transform.Find(name);
                var view = (trans != null && trans.gameObject.activeSelf) ? trans.gameObject : null;
                if (view == null)
                {
                    if (trans != null)
                    {
                        view = GetPool(index).Get(trans.gameObject);
                    }
                    else
                    {
                        view = GetPool(index).Get();
                    }
                  
                    var sacle = view.transform.localScale;
                    view.transform.SetParent(transform);
                    view.transform.localScale = sacle;
                    view.transform.localRotation = Quaternion.identity;
                    view.transform.SetAsLastSibling();
                    objList[index].Add(view);
                    view.name = name;
                   
                    OnCreate?.Invoke(view);
                    Layout();
                }
                return view;
            }
        }
        List<GameObject>[] objList;
        public virtual void Clear()
        {
            if(objList==null)return;
            for (int i = 0; i < objList.Length; i++)
            {
                for (int j = objList[i].Count-1; j >=0; j--)
                {
                    var view = objList[i][j];
                    Push(view, i);
                }
            }
            OnClear?.Invoke();
            Layout();
        }
        public void Push(GameObject view,int i=0)
        {
            GetPool(i).Push(view);
            objList[i].Remove(view);
            OnPush?.Invoke(view);
        }
        public event System.Action<GameObject> OnPush;
        public event System.Action<GameObject> OnCreate;
        public event System.Action OnClear;
    }

}