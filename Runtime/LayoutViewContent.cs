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
    
        public List<GameObject> GetObjList(int index)
        {
            for (int i = objLists.Count; i <= index; i++)
            {
                objLists.Add(new List<GameObject>());
            }
            return objLists[index];
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
                    GetObjList(index).Add(view);
                    view.name = name;
                    _count++;
                     OnCreate?.Invoke(view);
                    Layout();
                }
                return view;
            }
        }
        List<List<GameObject>> objLists;
        public virtual void Clear()
        {
            for (int i = 0; i < objLists.Count; i++)
            {
                for (int j = objLists[i].Count-1; j >=0; j--)
                {
                    var view = objLists[i][j];
                    Push(view, i);
                }
            }
            OnClear?.Invoke();
            Layout();
        }
        private int _count = 0;
        public int Count
        {
            get
            {
                return _count;
            }
        }
        public void Push(GameObject view,int i=0)
        {
            _count--;
            GetPool(i).Push(view);
            GetObjList(i).Remove(view);
            OnPush?.Invoke(view);
        }
        public event System.Action<GameObject> OnPush;
        public event System.Action<GameObject> OnCreate;
        public event System.Action OnClear;
    }

}