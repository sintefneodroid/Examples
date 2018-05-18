using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings))]
    public class RenderTextureSamplerEditor : InputEditor
    {
        static Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource m_SupportedSources = Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.ActiveCameras | Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.MainCamera | Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.TaggedCamera;
        string[] m_MaskedSourceNames;
        SerializedProperty m_Source;
        SerializedProperty m_RenderSize;
        SerializedProperty m_FinalSize;
        SerializedProperty m_AspectRatio;
        SerializedProperty m_SuperSampling;
        SerializedProperty m_CameraTag;
        SerializedProperty m_FlipFinalOutput;
        ResolutionSelector m_ResSelector;

        protected void OnEnable()
        {
            if (this.target == null)
                return;

            var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings>(this.serializedObject);
            this.m_Source = pf.Find(w => w.source);
            this.m_RenderSize = pf.Find(w => w.m_RenderSize);
            this.m_AspectRatio = pf.Find(w => w.m_AspectRatio);
            this.m_SuperSampling = pf.Find(w => w.m_SuperSampling);
            this.m_FinalSize = pf.Find(w => w.m_OutputSize);
            this.m_CameraTag = pf.Find(w => w.m_CameraTag);
            this.m_FlipFinalOutput = pf.Find( w => w.m_FlipFinalOutput );
            this.m_ResSelector = new ResolutionSelector();
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
                    index = EditorGUILayout.Popup("Object(s) of interest", index, this.m_MaskedSourceNames);

                    if (check.changed) this.m_Source.intValue = EnumHelper.GetEnumValueFromMaskedIndex<Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource>(index, (int)m_SupportedSources);
                }
            });
            
            var inputType = (Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource)this.m_Source.intValue;

            if ((Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource)this.m_Source.intValue == Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.TaggedCamera)
            {
                ++EditorGUI.indentLevel;
                this.AddProperty(this.m_CameraTag, () => EditorGUILayout.PropertyField(this.m_CameraTag, new GUIContent("Tag")));
                --EditorGUI.indentLevel;
            }

            this.AddProperty(this.m_AspectRatio, () => EditorGUILayout.PropertyField(this.m_AspectRatio, new GUIContent("Aspect Ratio")));
            this.AddProperty(this.m_SuperSampling, () => EditorGUILayout.PropertyField(this.m_SuperSampling, new GUIContent("Super sampling")));

            var renderSize = this.m_RenderSize;
            this.AddProperty(
                    this.m_RenderSize, () =>
            {
                
                if (inputType != Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.RenderTexture)
                {
                    EditorGUILayout.PropertyField(this.m_RenderSize, new GUIContent("Rendering resolution"));
                    if (this.m_FinalSize.intValue > renderSize.intValue) this.m_FinalSize.intValue = renderSize.intValue;
                }
            });

            this.AddProperty(
                    this.m_FinalSize, () =>
            {
                this.m_ResSelector.OnInspectorGUI( (this.target as Unity_Technologies.Recorder.Framework.Core.Engine.ImageInputSettings).maxSupportedSize, this.m_FinalSize );
                if (this.m_FinalSize.intValue == (int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.Window) this.m_FinalSize.intValue = (int)Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.x720p_HD;
                if (this.m_FinalSize.intValue > renderSize.intValue)
                    renderSize.intValue = this.m_FinalSize.intValue;
            });

            EditorGUILayout.PropertyField(this.m_FlipFinalOutput, new GUIContent("Flip image vertically"));
            if (Unity_Technologies.Recorder.Framework.Core.Engine.Verbose.enabled)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextField("Color Space", (this.target as Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings).m_ColorSpace.ToString());
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }

}
