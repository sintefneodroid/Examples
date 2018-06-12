using UnityEditor;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Editor.Misc
{
    [CustomPropertyDrawer(typeof(Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Misc.DataPath))]
    class DataPathDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool ro = property.FindPropertyRelative("m_readOnly").boolValue;
            if(ro) { EditorGUI.BeginDisabledGroup(true); }

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float buttonWidth = 22;
            float rootWidth = 70;
            float leafWidth = position.width - rootWidth - 5 - buttonWidth;
            var rootRect = new Rect(position.x, position.y, rootWidth, position.height);
            var leafRect = new Rect(position.x + rootWidth + 5, position.y, leafWidth, position.height);
            var buttonRect = new Rect(position.x + rootWidth + 5 + leafWidth, position.y, buttonWidth, position.height);

            var pRoot = property.FindPropertyRelative("m_root");
            var pLeaf = property.FindPropertyRelative("m_leaf");
            EditorGUI.PropertyField(rootRect, pRoot, GUIContent.none);
            EditorGUI.PropertyField(leafRect, pLeaf, GUIContent.none);
            if (GUI.Button(buttonRect, "..."))
            {
                var tmp = new Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Misc.DataPath((Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Misc.DataPath.Root)pRoot.intValue, pLeaf.stringValue);
                var path = EditorUtility.OpenFolderPanel("Select Directory", tmp.GetFullPath(), "");
                if (path.Length > 0)
                {
                    var newPath = new Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Misc.DataPath(path);
                    pRoot.intValue = (int)newPath.root;
                    pLeaf.stringValue = newPath.leaf;
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();

            if (ro) { EditorGUI.EndDisabledGroup(); }
        }
    }
}
