using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [CustomEditor(typeof(Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings))]
    public class CBRenderTextureInputSettingsEditor : InputEditor
    {
        static Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource m_SupportedSources = Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.MainCamera | Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.ActiveCameras | Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.TaggedCamera;
        string[] m_MaskedSourceNames;
        ResolutionSelector m_ResSelector;

        SerializedProperty m_Source;
        SerializedProperty m_CameraTag;
        SerializedProperty m_RenderSize;
        SerializedProperty m_RenderAspect;
        SerializedProperty m_FlipFinalOutput;
        SerializedProperty m_Transparency;
        SerializedProperty m_CaptureUI;

        protected void OnEnable()
        {
            if (this.target == null)
                return;


            var pf = new PropertyFinder<Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>(this.serializedObject);
            this.m_Source = pf.Find(w => w.source);
            this.m_CameraTag = pf.Find(w => w.m_CameraTag);

            this.m_RenderSize = pf.Find(w => w.m_OutputSize);
            this.m_RenderAspect = pf.Find(w => w.m_AspectRatio);
            this.m_FlipFinalOutput = pf.Find( w => w.m_FlipFinalOutput );
            this.m_Transparency = pf.Find(w => w.m_AllowTransparency);
            this.m_CaptureUI = pf.Find(w => w.m_CaptureUI);

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


            if (inputType != Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.RenderTexture)
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

                if(inputType == Unity_Technologies.Recorder.Framework.Core.Engine.EImageSource.ActiveCameras)
                {
                    this.AddProperty(this.m_CaptureUI, () => EditorGUILayout.PropertyField(this.m_CaptureUI, new GUIContent("Capture UI")));
                }
            }

            this.AddProperty(this.m_Transparency, () => EditorGUILayout.PropertyField(this.m_Transparency, new GUIContent("Capture alpha")));

            //if (Verbose.enabled)
            {
                //using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(this.m_FlipFinalOutput, new GUIContent("Flip image vertically"));
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
