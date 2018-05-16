using UnityEngine;
using UnityEditor;

namespace UTJ.FrameCapturer
{
    [CustomPropertyDrawer(typeof(Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.MovieEncoderConfigs))]
    class MovieEncoderConfigsDrawer : PropertyDrawer
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
            switch ((Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.MovieEncoder.Type)type.intValue)
            {
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.MovieEncoder.Type.Png:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("pngEncoderSettings"), true);
                    break;
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.MovieEncoder.Type.Exr:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("exrEncoderSettings"), true);
                    break;
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.MovieEncoder.Type.Gif:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("gifEncoderSettings"), true);
                    break;
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.MovieEncoder.Type.WebM:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("webmEncoderSettings"), true);
                    break;
                case Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder.MovieEncoder.Type.MP4:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("mp4EncoderSettings"), true);
                    break;
            }
            EditorGUI.indentLevel--;
        }
    }
}
