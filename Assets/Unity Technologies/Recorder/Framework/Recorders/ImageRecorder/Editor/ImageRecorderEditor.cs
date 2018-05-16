using System;
using UnityEngine;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Recorders.ImageRecorder.Engine.ImageRecorderSettings))]
    public class ImageRecorderEditor : RecorderEditor
    {
        SerializedProperty m_OutputFormat;
        
        [MenuItem("Tools/Recorder/Video")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Recorders.ImageRecorder.Engine.ImageRecorderSettings>(serializedObject);
            m_OutputFormat = pf.Find(w => w.m_OutputFormat);
        }

        protected override void OnEncodingGroupGui()
        {
            // hiding this group by not calling parent class's implementation.  
        }

        protected override void OnOutputGui()
        {
            AddProperty(m_OutputFormat, () => EditorGUILayout.PropertyField(m_OutputFormat, new GUIContent("Output format")));
            base.OnOutputGui();
        }

        protected override EFieldDisplayState GetFieldDisplayState(SerializedProperty property)
        {
            if (property.name == "m_AllowTransparency")
            {
                return (target as Unity_Technologies.Recorder.Framework.Recorders.ImageRecorder.Engine.ImageRecorderSettings).m_OutputFormat == Unity_Technologies.Recorder.Framework.Recorders.ImageRecorder.Engine.PNGRecordeOutputFormat.JPEG ? EFieldDisplayState.Hidden : EFieldDisplayState.Enabled;
            }

            return base.GetFieldDisplayState(property);
        }
    }
}
