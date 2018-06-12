using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.FCIntegration.Editor;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.EXR.Editor
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Extensions.FCIntegration.EXR.EXRRecorderSettings))]
    public class EXRRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_ExrEncoderSettings"), new GUIContent("Encoding"), true);
        }

    }
}
