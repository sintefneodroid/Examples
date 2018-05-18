using UnityEngine;
using UnityEditor;

namespace UTJ.FrameCapturer
{
    [CustomPropertyDrawer(typeof(Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.AudioEncoderConfigs))]
    class AudioEncoderConfigsDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0.0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = property.FindPropertyRelative("format");
            EditorGUILayout.PropertyField(type);
            EditorGUI.indentLevel++;
            switch ((Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.AudioEncoder.Type)type.intValue)
            {
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.AudioEncoder.Type.Wave:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("waveEncoderSettings"), true);
                    break;
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.AudioEncoder.Type.Ogg:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("oggEncoderSettings"), true);
                    break;
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.AudioEncoder.Type.Flac:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("flacEncoderSettings"), true);
                    break;
            }
            EditorGUI.indentLevel--;
        }
    }
}
