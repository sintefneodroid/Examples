#if UNITY_2018_1_OR_NEWER

using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Inputs.Camera360.Engine.Camera360InputSettings))]
    public class Camera360InputEditor : InputEditor
    {
        static Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource m_SupportedSources = Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.MainCamera | Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.TaggedCamera;
        string[] m_MaskedSourceNames;

        SerializedProperty m_Source;
        SerializedProperty m_CameraTag;
        SerializedProperty m_FlipFinalOutput;
        SerializedProperty m_StereoSeparation;
        SerializedProperty m_CubeMapSz;
        SerializedProperty m_OutputWidth;
        SerializedProperty m_OutputHeight;
        SerializedProperty m_RenderStereo;

        protected void OnEnable()
        {
            if (this.target == null)
                return;

            var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Inputs.Camera360.Engine.Camera360InputSettings>(this.serializedObject);
            this.m_Source = pf.Find(w => w.source);
            this.m_CameraTag = pf.Find(w => w.m_CameraTag);

            this.m_StereoSeparation = pf.Find(w => w.m_StereoSeparation);
            this.m_FlipFinalOutput = pf.Find( w => w.m_FlipFinalOutput );
            this.m_CubeMapSz = pf.Find( w => w.m_MapSize );
            this.m_OutputWidth = pf.Find(w => w.m_OutputWidth);
            this.m_OutputHeight = pf.Find(w => w.m_OutputHeight);
            this.m_RenderStereo = pf.Find(w => w.m_RenderStereo);
        }

        public override void OnInspectorGUI()
        {
            this.AddProperty(
                    this.m_Source, () =>
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    if (this.m_MaskedSourceNames == null) this.m_MaskedSourceNames = EnumHelper.MaskOutEnumNames<Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource>((int)m_SupportedSources);
                    var index = EnumHelper.GetMaskedIndexFromEnumValue<Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource>(this.m_Source.intValue, (int)m_SupportedSources);
                    index = EditorGUILayout.Popup("Source", index, this.m_MaskedSourceNames);

                    if (check.changed) this.m_Source.intValue = EnumHelper.GetEnumValueFromMaskedIndex<Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource>(index, (int)m_SupportedSources);
                }
            });

            var inputType = (Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource)this.m_Source.intValue;
            if ((Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource)this.m_Source.intValue == Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.TaggedCamera )
            {
                ++EditorGUI.indentLevel;
                this.AddProperty(this.m_CameraTag, () => EditorGUILayout.PropertyField(this.m_CameraTag, new GUIContent("Tag")));
                --EditorGUI.indentLevel;
            }

            this.AddProperty(
                    this.m_OutputWidth, () =>
            {
                this.AddProperty(this.m_OutputWidth, () => EditorGUILayout.PropertyField(this.m_OutputWidth, new GUIContent("Output width")));
            });

            this.AddProperty(
                    this.m_OutputHeight, () =>
            {
                this.AddProperty(this.m_OutputWidth, () => EditorGUILayout.PropertyField(this.m_OutputHeight, new GUIContent("Output height")));
            });

            this.AddProperty(
                    this.m_CubeMapSz, () =>
            {
                this.AddProperty(this.m_CubeMapSz, () => EditorGUILayout.PropertyField(this.m_CubeMapSz, new GUIContent("Cube map width")));
            });

            this.AddProperty(
                    this.m_RenderStereo, () =>
            {
                this.AddProperty(this.m_RenderStereo, () => EditorGUILayout.PropertyField(this.m_RenderStereo, new GUIContent("Render in Stereo")));
            });

            this.AddProperty(
                    this.m_StereoSeparation, () =>
            {
                ++EditorGUI.indentLevel;
                using (new EditorGUI.DisabledScope(!this.m_RenderStereo.boolValue))
                {
                    this.AddProperty(this.m_StereoSeparation, () => EditorGUILayout.PropertyField(this.m_StereoSeparation, new GUIContent("Stereo Separation")));
                }
                --EditorGUI.indentLevel;
            });

            if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(this.m_FlipFinalOutput, new GUIContent("Flip output"));
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif