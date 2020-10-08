using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace QTool.UI
{
    public class ResourcesImageReplace : Image,IReplaceUI
    {
        public TextRepalceTag ReplaceTag { set =>  sprite = Resources.Load<Sprite>(value.value); }
    }

}
