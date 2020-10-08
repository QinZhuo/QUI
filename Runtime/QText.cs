using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    public interface IReplaceUI
    {
        RectTransform rectTransform { get; }
        TextRepalceTag ReplaceTag { set; }
    }
    public class TextRepalceTag
    {
        public bool show=true;
        public int oneLineIndex;
        public int multLineIndex;
        public int GetIndex(bool oneLine)
        {
            return (oneLine ? oneLineIndex : multLineIndex) * 6 ;
        }
        public string Left
        {
            get
            {
                return "<color=#00000000>";
            }
        } 
        public string ReplaceText
        {
            get
            {
                return Left + "<quad></color>";
            }
        }
        public IReplaceUI ui;
        public Vector2 size;
        public string key;
        public string value;
        public void Clear()
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy((ui as MonoBehaviour).gameObject);
            }
          
        }
    }
    public class QText : Text
    {
        private static readonly Regex regex = new Regex(@"{(.+?):(.+?)}", RegexOptions.Singleline);
        //  private static readonly Regex _regex = new Regex( @"<quad name=(.+?) size=(\d*\.?\d+%?)/>", RegexOptions.Singleline);
       
        public string oText = "";
        public override string text
        {
            set
            {
                if (!oText.Equals(value))
                {
                    base.text = Fix(value);
                }

            }
        }

        public int oneLineIndex = 0;

        public void Fresh()
        {
            if (Application.isPlaying)
            {
                foreach (var tag in tagList)
                {
                    tag.ui.rectTransform.sizeDelta = tag.size;
                    tag.ui.rectTransform.gameObject.SetActive(tag.show);
                }
            }
        }
        protected override void Start()
        {
            base.text = Fix(oText);
        }
        private void Update()
        {
            if (Dirty)
            {
                Fresh();
                Dirty = false;
            }
        }

        public string Fix(string text)
        {
            oText = text;
          
            var newText = "";
            int lastEndIndex = 0;
            ClearTag();
            oneLineIndex = 0;
            var multLineIndex = 0;
            foreach (Match match in regex.Matches(text))
            {
                var lastText = text.Substring(lastEndIndex, match.Index - lastEndIndex);
                lastEndIndex = match.Index + match.Length;
                newText += lastText;

                oneLineIndex += lastText.Length;
                multLineIndex += lastText.Length;

                var key = match.Groups[1].Value.ToLower().Trim();
                var value = match.Groups[2].Value.Trim();
               
                
                var tag = new TextRepalceTag
                {
                    key = key,
                    value = value,
                    oneLineIndex = oneLineIndex,
                };
                if (Application.isPlaying)
                {
                    var ui = Instantiate(replacePrefab, transform).GetComponent<IReplaceUI>();
                    tag.ui = ui;
                    tag.ui.ReplaceTag = tag;
                    tag.ui.rectTransform.sizeDelta = tag.size;
                }
                tag.multLineIndex = multLineIndex+tag.Left.Length;
                tagList.Add(tag);
                oneLineIndex += 1;
                multLineIndex += tag.ReplaceText.Length;
                newText += tag.ReplaceText;
            }
            var t = text.Substring(lastEndIndex, text.Length - lastEndIndex); ;
            newText += t;
            oneLineIndex += t.Length;
            return newText;
        }
        [ContextMenu("Test")]
        public void Test()
        {
            text = "N{sprite:1}N";
        }
        public void ClearTag()
        {
            foreach (var t in tagList)
            {
                t.Clear();
            }
            tagList.Clear();
        }
        private readonly List<TextRepalceTag> tagList = new List<TextRepalceTag>();
        public GameObject replacePrefab;
    
       
        protected override void OnDestroy()
        {
            ClearTag();
        }
        //private void OnDrawGizmosSelected()
        //{
        //    foreach (var v in vList)
        //    {
        //        Gizmos.color = Color.Lerp(Color.red, Color.clear, 0.9f);
        //        Gizmos.DrawSphere(v.position + transform.position, 1f);
        //    }
        //}
        public bool Dirty=false;
        List<UIVertex> vList = new List<UIVertex>();
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            if (!Application.isPlaying) return;
            toFill.GetUIVertexStream(vList);
            var oneLine = vList.Count == oneLineIndex * 6;
        
            foreach (var tag in tagList)
            {
                var startIndex = tag.GetIndex(oneLine);
                var show = startIndex + 5 < vList.Count;
                if (tag.show != show)
                {
                    Dirty = true;
                    tag.show = show;
                }
                if (!show)
                {
                    continue;
                }
                var rt = tag.ui.rectTransform;
                if (rt == null)
                {
                    continue;
                }
                var leftDown = vList[startIndex + 4].position;
                var rightUp = vList[startIndex + 1].position;
                var center = (leftDown + rightUp) / 2;
                var size = rightUp - leftDown;
                if (startIndex < vList.Count)
                {
                    rt.anchoredPosition= center;
                    if (!rt.sizeDelta.Equals(size))
                    {
                        Dirty = true;
                        tag.size = size;
                    }
                   
                }
            }
        }

    }
}

