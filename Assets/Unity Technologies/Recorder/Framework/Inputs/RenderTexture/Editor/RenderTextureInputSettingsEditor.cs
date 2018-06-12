using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Editor;

namespace Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Editor
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine.RenderTextureInputSettings))]
    public class RenderTextureInputSettingsEditor : InputEditor
    {
        SerializedProperty m_SourceRTxtr;

        protected void OnEnable()
        {
            if (this.target == null)
                return;

            var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine.RenderTextureInputSettings>(this.serializedObject);
            this.m_SourceRTxtr = pf.Find(w => w.m_SourceRTxtr);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.m_SourceRTxtr, new GUIContent("Source"));
            using (new EditorGUI.DisabledScope(true))
            {
                var res = "N/A";
                if (this.m_SourceRTxtr.objectReferenceValue != null)
                {
                    var renderTexture = (UnityEngine.RenderTexture)this.m_SourceRTxtr.objectReferenceValue;
                    res = string.Format("{0} , {1}", renderTexture.width, renderTexture.height);
                }
                EditorGUILayout.TextField("Resolution", res);
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}