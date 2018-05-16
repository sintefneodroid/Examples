using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine.RenderTextureInputSettings))]
    public class RenderTextureInputSettingsEditor : InputEditor
    {
        SerializedProperty m_SourceRTxtr;

        protected void OnEnable()
        {
            if (target == null)
                return;

            var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine.RenderTextureInputSettings>(serializedObject);
            m_SourceRTxtr = pf.Find(w => w.m_SourceRTxtr);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_SourceRTxtr, new GUIContent("Source"));
            using (new EditorGUI.DisabledScope(true))
            {
                var res = "N/A";
                if (m_SourceRTxtr.objectReferenceValue != null)
                {
                    var renderTexture = (RenderTexture)m_SourceRTxtr.objectReferenceValue;
                    res = string.Format("{0} , {1}", renderTexture.width, renderTexture.height);
                }
                EditorGUILayout.TextField("Resolution", res);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}