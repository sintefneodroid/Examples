using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Extensions.FCIntegration.GIF.GIFRecorderSettings))]
    public class GIFRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GifEncoderSettings"), new GUIContent("Encoding"), true);
        }

        protected override EFieldDisplayState GetFieldDisplayState(SerializedProperty property)
        {
            if (property.name == "m_AllowTransparency"  )
                return EFieldDisplayState.Hidden;

            return base.GetFieldDisplayState(property);
        }

    }
}
