using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    public static class ColorExtend
    {
        public static string ToHtmlStringRGB(this Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
        public static Color ChangeAlpha(this Color color,float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }
    }
    public static class TextExtend
    {
        public static Vector3 GetPos(this Text text,int index)
        {
            string textStr = text.text;
            Vector3 charPos = Vector3.zero;
            if (index <= textStr.Length && index > 0)
            {
                TextGenerator textGen = new TextGenerator(textStr.Length);
                var size = text.rectTransform.rect.size;
                textGen.Populate(textStr, text.GetGenerationSettings(size));

                int newLine = textStr.Substring(0, index).Split('\n').Length - 1;
                int whiteSpace = textStr.Substring(0, index).Split(' ').Length - 1;
                int indexOfTextQuad = (index * 4) + (newLine * 4) - 4;
                if (indexOfTextQuad < textGen.vertexCount)
                {
                    charPos = (textGen.verts[indexOfTextQuad].position +
                        textGen.verts[indexOfTextQuad + 1].position +
                        textGen.verts[indexOfTextQuad + 2].position +
                        textGen.verts[indexOfTextQuad + 3].position) / 4f;
                }
            }
            charPos /= text.GetComponentInParent<Canvas>().scaleFactor;//适应不同分辨率的屏幕
            charPos = text.transform.TransformPoint(charPos);//转换为世界坐标
            return charPos;
        }
        public static bool CheckRichTextOneLine(this string text,int count)
        {
            if (text.Length == count)
            {
                if (text.Contains("<") && text.Contains(">"))
                {
                    return false;
                }
            }
            return true;
        }
       
        public static int RichTextLength(this string text)
        {
            var trueText = text.RemoveText('<', '>');
            return trueText.Length;
        }
        public static int ToRichTextIndex(this string text,int index)
        {
            var trueIndex = 0;
            bool ignore=false;
            for (int i = 0; i <=index; i++)
            {
                var c = text[i];
                if (ignore)
                {
                    if ('>'.Equals(c))
                    {
                        ignore = false;
                     //   Debug.LogError("end"+i+"/"+text.Length);
                    }
                }
                else
                {
                    if ('<'.Equals(c))
                    {
                        ignore = true;
                     //   Debug.LogError("ignore"+i+"/"+text.Length);
                    }
                    else
                    {
                        trueIndex++;
                    }
                }
            }
            Debug.LogError(index + " => " + trueIndex);
            return trueIndex;
        }
        public static string RemoveText(this string text,char start,char end)
        {
            var endText = text;
            var startIndex = endText.IndexOf(start);
            var endIndex = endText.IndexOf(end);
            while (startIndex>=0&&endIndex>=0)
            {
                Debug.LogError(startIndex + "-" + endIndex);
                endText = endText.Remove(startIndex, endIndex - startIndex+1);
                startIndex = endText.IndexOf(start);
                endIndex = endText.IndexOf(end);
            }
            return endText;
        }
    }
    public class QGradientUI : BaseMeshEffect
    {
        public enum LerpDir
        {
            TopDown,
            LeftRight,
            TextCount,
        }
        public LerpDir lerpDir = LerpDir.LeftRight;
        [SerializeField]
        private Gradient _gradientColor = new Gradient();
        public Gradient GradientColor
        {
            get
            {
                return _gradientColor;
            }
            set
            {
                if (graphic != null)
                {
                    Fresh();
                    _gradientColor = value;
                }
            }
        }
        private Text _text;
        Text text
        {
            get
            {
                return _text ?? (_text = GetComponent<Text>()); 
            }
        }
        public void SetAlphaPos(float t,float delay, params int[] index)
        {
            if (index.Length == 0) return;
            var alphaKeys = GradientColor.alphaKeys;

            for (int i = 0; i < index.Length; i++)
            {

                alphaKeys[index[i]].time =Mathf.Lerp(-(index.Length-1)*delay,1, t )+ delay* i;
            }
            GradientColor.SetKeys(GradientColor.colorKeys, alphaKeys); ;
            Fresh();
        }
        public void Fresh()
        {
            graphic.SetVerticesDirty();
        }
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
      //  float length = 0;
        float GetValue(UIVertex v, int index)
        {
            switch (lerpDir)
            {
                case LerpDir.TopDown:
                    return v.position.y;
                case LerpDir.LeftRight:
                    return v.position.x;
                case LerpDir.TextCount:
                    var cIndex = index / 4;
                    var offset = index - cIndex*4;
                    if (textOneLine)
                    {
                        cIndex= index / 4;
                    }
                    else
                    {
                        cIndex= text.text.ToRichTextIndex(index / 4);
                    }
                    if (offset == 0 || offset == 3)
                    {
                        return cIndex*2;
                    }
                    else
                    {
                        return cIndex * 2 + 1;
                    }
                default:
                    return 0;
            }
        }
        bool textOneLine = false;
        void Init(VertexHelper vh)
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;
            if(lerpDir== LerpDir.TextCount&&text!=null)
            {
                textOneLine = text.text.CheckRichTextOneLine(vh.currentVertCount);
            }
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i); 
                var value = GetValue(v, i);
                if (value > maxValue)
                {
                    maxValue = value; 
                }
                if (value < minValue)
                {
                    minValue = value;
                }
            }
        }
        UIVertex v = new UIVertex();
        public void LerpColor(VertexHelper vh)
        {
            var length = maxValue - minValue;

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                var t = (GetValue(v, i) - minValue) / length;
                var newColor = v.color* GradientColor.Evaluate(t);
                if (v.color != newColor)
                {
                    v.color = newColor;
                    vh.SetUIVertex(v, i);
                }
            }
        }
        public override void ModifyMesh(VertexHelper vh)
        {

            if (!IsActive() || vh.currentVertCount == 0)
            {
                return;
            }
            Init(vh);
            LerpColor(vh);
        }


    }
}
