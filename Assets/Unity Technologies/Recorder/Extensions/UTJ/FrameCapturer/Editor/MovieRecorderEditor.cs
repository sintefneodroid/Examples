using System;
using UnityEditor;
using UnityEngine;

namespace UTJ.FrameCapturer
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.MovieRecorder))]
    public class MovieRecorderEditor : RecorderBaseEditor
    {
        public virtual void VideoConfig()
        {
            var recorder = this.target as Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.MovieRecorder;
            var so = this.serializedObject;

            EditorGUILayout.PropertyField(so.FindProperty("m_captureTarget"));
            if(recorder.captureTarget == Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.MovieRecorder.CaptureTarget.RenderTexture)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(so.FindProperty("m_targetRT"));
                EditorGUI.indentLevel--;
            }

            this.ResolutionControl();
            EditorGUILayout.PropertyField(so.FindProperty("m_captureEveryNthFrame"));
        }


        public virtual void AudioConfig()
        {
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            var recorder = this.target as Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.MovieRecorder;
            var so = this.serializedObject;

            this.CommonConfig();

            EditorGUILayout.Space();

            if (recorder.supportVideo && !recorder.supportAudio)
            {
                this.VideoConfig();
            }
            else if (!recorder.supportVideo && recorder.supportAudio)
            {
                this.AudioConfig();
            }
            else if (recorder.supportVideo && recorder.supportAudio)
            {
                EditorGUILayout.PropertyField(so.FindProperty("m_captureVideo"));
                if (recorder.captureVideo)
                {
                    EditorGUI.indentLevel++;
                    this.VideoConfig();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(so.FindProperty("m_captureAudio"));
                if (recorder.captureAudio)
                {
                    EditorGUI.indentLevel++;
                    this.AudioConfig();
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space();
            this.FramerateControl();
            EditorGUILayout.Space();
            this.RecordingControl();

            so.ApplyModifiedProperties();
        }
    }
}
