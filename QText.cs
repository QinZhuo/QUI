using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    public class QText : Text
    {
        private static readonly Regex regex= new Regex(@"<(.+?)=(.+?)/>", RegexOptions.Singleline);
        private static readonly Regex _regex = new Regex( @"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />", RegexOptions.Singleline);
        string lastText="";
        public override string text
        {
            get
            {
                return base.text;
            }
            set
            {
                if (!lastText.Equals(value))
                {
                    lastText = value;
                    base.text = Fix(value);
                  
                }

            }
        }
        public string Fix(string text)
        {
            var newText = "";
            int lastEndIndex = 0;
            Debug.LogError("fix:" + regex.Matches(text).Count);
            foreach (Match match in regex.Matches(text))
            {
                newText += text.Substring(lastEndIndex, match.Index - lastEndIndex);
                lastEndIndex = match.Index + match.Length;
              
                switch (match.Groups[1].Value.ToLower().Trim())
                {
                    case "sprite":
                        newText += string.Format( "<quad name={0} size={1} width=1 />",match.Groups[2].Value.Trim(),fontSize);
                        Debug.LogError("fix:[" + match.Groups[1].Value.Trim().ToLower() + "]"+":["+ match.Groups[2].Value.Trim() + "]");
                        break;
                    default:
                        break;
                }
            }
            newText += text.Substring(lastEndIndex, text.Length - lastEndIndex);
            return newText;
        }
        [ContextMenu("Test")]
        public void Test()
        {
            text = "N< sprite=1 />NN< sprite=1 />NN< sprite=1 />NN< sprite=1 />NN< sprite=1 />N";
        }
        private readonly List<int> vIndex = new List<int>();
        public GameObject imagePrefab;
        List<Image> spriteList=new List<Image>();
        public override void SetAllDirty()
        {
            base.SetAllDirty();
            UpdateQuadImage();
        }
        public void UpdateQuadImage()
        {
            base.text = Fix(lastText);
            if (!Application.isPlaying) return;
                vIndex.Clear();
            foreach (var item in spriteList)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);

                }
            }
         
            spriteList.Clear();
            Debug.LogError("chek" + _regex.Matches(text).Count);
            int indexOffset = 0;
            foreach (Match match in _regex.Matches(text))
            {
                var index = match.Index +indexOffset;
                indexOffset -= match.Length-1;
                vIndex.Add(index*6+4);

            
                var sprite = match.Groups[1].Value;
             //   var size = float.Parse(match.Groups[2].Value);
                var img = Resources.Load<Sprite>(sprite);
                var obj = new GameObject(sprite, typeof(Image));
                var image = obj.GetComponent<Image>();
                image.rectTransform.SetParent(rectTransform);
                image.rectTransform.localPosition = Vector3.zero;
                image.sprite = img;
                image.rectTransform.sizeDelta = new Vector2(fontSize, fontSize);
                spriteList.Add(image);
                Debug.LogError("sprie [" + sprite + "][" + index + "]");
            }
        }
        protected override void OnDestroy()
        {
            foreach (var item in spriteList)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);

                }
            }

            spriteList.Clear();
        }
        List<UIVertex> vList = new List<UIVertex>();
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
     
            toFill.GetUIVertexStream(vList);
            //if (!Application.isPlaying) return;
            for (int i = 0; i < spriteList.Count; i++)
            {
                var endIndex = vIndex[i];

                var rt = spriteList[i].rectTransform;
                var size = rt.sizeDelta;
                Debug.LogError("index" + endIndex + "/" + vList.Count);
                if (endIndex < vList.Count)
                {
                    Debug.LogError("pos" + vList[endIndex].position.x + "," + vList[endIndex].position.y);
                    rt.anchoredPosition = new Vector2(vList[endIndex].position.x + size.x / 2
                        , vList[endIndex].position.y + size.x / 2);
                }
                else
                {
                    Debug.LogError("outIndex"+ endIndex + "/"+vList.Count);
                }
            }
        }
        //[Obsolete]
        //protected override void OnPopulateMesh(Mesh m)
        //{
        //    base.OnPopulateMesh(m);

        //    Debug.LogError("mesh" );
        //    if (!Application.isPlaying) return;
        //    var verts = m.vertices;
        //    for (int i = 0; i < vIndex.Count; i++)
        //    {
        //        var endIndex = vIndex[i];
             
        //        var rt = spriteList[i].rectTransform;
        //        var size = rt.sizeDelta;
        //        if (endIndex < verts.Length)
        //        {
        //            Debug.LogError("pos" + verts[endIndex].x + "," + verts[endIndex].y);
        //            rt.anchoredPosition = new Vector2(verts[endIndex].x + size.x / 2
        //                , verts[endIndex].y + size.y / 2);
        //        }
        //    }
        //}
    }
}
