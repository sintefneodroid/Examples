using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.FCIntegration.Editor;
using Unity_Technologies.Recorder.Framework.Core.Editor;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.GIF.Editor
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Extensions.FCIntegration.GIF.GIFRecorderSettings))]
    public class GIFRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_GifEncoderSettings"), new GUIContent("Encoding"), true);
        }

        protected override EFieldDisplayState GetFieldDisplayState(SerializedProperty property)
        {
            if (property.name == "m_AllowTransparency"  )
                return EFieldDisplayState.Hidden;

            return base.GetFieldDisplayState(property);
        }

    }
}
