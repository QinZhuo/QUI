using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace QTool.UI
{
    [CustomEditor(typeof(QText))]
    [CanEditMultipleObjects]
    public class QTextEditor : UnityEditor.UI.TextEditor
    {
        QText text
        {
            get
            {
                return target as QText;
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("原始文本");
            text.text = EditorGUILayout.TextArea(text.oText,GUILayout.Height(50));
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}

