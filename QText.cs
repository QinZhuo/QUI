using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace QTool.UI
{
    public class QText : MonoBehaviour
    {
        private static readonly Regex _regex = new Regex(@"{(.+?)=(.+?)}", RegexOptions.Singleline);
        [ContextMenu("Test")]
        public void Test()
        {

        }
    }
}
