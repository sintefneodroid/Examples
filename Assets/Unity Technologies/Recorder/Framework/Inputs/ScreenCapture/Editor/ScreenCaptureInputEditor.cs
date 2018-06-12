#if UNITY_2017_3_OR_NEWER

using UnityEditor;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Editor;

namespace Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Editor
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInputSettings))]
    public class ScreenCaptureInputEditor : InputEditor
    {
        SerializedProperty m_RenderSize;
        SerializedProperty m_RenderAspect;
        ResolutionSelector m_ResSelector;

        protected void OnEnable()
        {
            if (this.target == null)
                return;

            var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInputSettings>(this.serializedObject);
            this.m_RenderSize = pf.Find(w => w.m_OutputSize);
            this.m_RenderAspect = pf.Find(w => w.m_AspectRatio);

            this.m_ResSelector = new ResolutionSelector();
        }

        public override void OnInspectorGUI()
        {
            this.AddProperty(
                    this.m_RenderSize, () =>
            {
                this.m_ResSelector.OnInspectorGUI((this.target as Unity_Technologies.Recorder.Framework.Core.Engine.ImageInputSettings).maxSupportedSize, this.m_RenderSize);
            });

            if (this.m_RenderSize.intValue > (int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window)
            {
                this.AddProperty(this.m_RenderAspect, () => EditorGUILayout.PropertyField(this.m_RenderAspect, new GUIContent("Aspect Ratio")));
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif