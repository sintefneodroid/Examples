using System;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Extensions.FCIntegration.PNG.PNGRecorderSettings))]
    public class PngRecorderSettingsEditor : RecorderEditorBase
    {
        protected override void OnEncodingGroupGui()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PngEncoderSettings"), new GUIContent("Encoding"), true);
        }
    }
}
