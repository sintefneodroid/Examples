using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Extensions.FCIntegration.WEBM.WEBMRecorderSettings))]
    public class WEBMRecorderSettingsEditor : RecorderEditorBase
    {
        SerializedProperty m_VideoEncoder;
        SerializedProperty m_VideoBitRateMode;
        SerializedProperty m_VideoBitRate;
        SerializedProperty m_VideoMaxTasks;
        SerializedProperty m_AutoSelectBR;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (this.target == null)
                return;

            var pf = new PropertyFinder<Unity_Technologies.Recorder.Extensions.FCIntegration.WEBM.WEBMRecorderSettings>(this.serializedObject);
            var encoding = pf.Find(w => w.m_WebmEncoderSettings);
            var settings = this.target as Unity_Technologies.Recorder.Extensions.FCIntegration.WEBM.WEBMRecorderSettings;
            this.m_VideoBitRateMode = encoding.FindPropertyRelative(() => settings.m_WebmEncoderSettings.videoBitrateMode);
            this.m_VideoBitRate = encoding.FindPropertyRelative(() => settings.m_WebmEncoderSettings.videoTargetBitrate);
            this.m_VideoMaxTasks = encoding.FindPropertyRelative(() => settings.m_WebmEncoderSettings.videoMaxTasks);
            this.m_VideoEncoder = encoding.FindPropertyRelative(() => settings.m_WebmEncoderSettings.videoEncoder);
            this.m_AutoSelectBR = pf.Find(w => w.m_AutoSelectBR);
        }

        protected override void OnEncodingGui()
        {
            EditorGUILayout.PropertyField(this.m_VideoEncoder, new GUIContent("Encoder"), true);            
            EditorGUILayout.PropertyField(this.m_VideoBitRateMode, new GUIContent("Bitrate mode"), true);   
            EditorGUILayout.PropertyField(this.m_AutoSelectBR, new GUIContent("Autoselect bitrate"), true);
                using (new EditorGUI.DisabledScope(this.m_AutoSelectBR.boolValue))                        
            EditorGUILayout.PropertyField(this.m_VideoBitRate, new GUIContent("Bitrate (bps)"), true);            
            EditorGUILayout.PropertyField(this.m_VideoMaxTasks, new GUIContent("Max tasks"), true);    
        }

        protected override EFieldDisplayState GetFieldDisplayState( SerializedProperty property)
        {
            if( property.name == "m_CaptureEveryNthFrame" )
                return EFieldDisplayState.Hidden;

            if (property.name == "m_AllowTransparency"  )
                return EFieldDisplayState.Hidden;

            return base.GetFieldDisplayState(property);
        }
    }
}
