using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.FCIntegration.Editor;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.PNG.Editor
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Extensions.FCIntegration.PNG.PNGRecorderSettings))]
    public class PngRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_PngEncoderSettings"), new GUIContent("Encoding"), true);
        }
    }
}
