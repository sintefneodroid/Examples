#if UNITY_2017_3_OR_NEWER

using System;
using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{

    public enum MediaRecorderOutputFormat
    {
        MP4,
        WEBM
    }

    [ExecuteInEditMode]
    public class MediaRecorderSettings : Unity_Technologies.Recorder.Framework.Core.Engine.RecorderSettings
    {
        public MediaRecorderOutputFormat m_OutputFormat = MediaRecorderOutputFormat.MP4;
#if UNITY_2018_1_OR_NEWER
        public VideoBitrateMode m_VideoBitRateMode = VideoBitrateMode.High;
#endif
        public bool m_AppendSuffix = false;

        MediaRecorderSettings()
        {
            this.m_BaseFileName.pattern = "movie.<ext>";
        }

        public override List<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting>()
            {
                    this.NewInputSettingsObj<Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Pixels"),
                    this.NewInputSettingsObj<AudioInputSettings>("Audio")
            };
        }

        public override bool ValidityCheck( List<string> errors )
        {
            var ok = base.ValidityCheck(errors);

            if( string.IsNullOrEmpty(this.m_DestinationPath.GetFullPath() ))
            {
                ok = false;
                errors.Add("Missing destination path.");
            } 
            if(  string.IsNullOrEmpty(this.m_BaseFileName.pattern))
            {
                ok = false;
                errors.Add("missing file name");
            }

            return ok;
        }

        public override Unity_Technologies.Recorder.Framework.Core.Engine.RecorderInputSetting NewInputSettingsObj(Type type, string title)
        {
            var obj = base.NewInputSettingsObj(type, title);
            if (type == typeof(Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings))
            {
                (obj as Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings).m_ForceEvenSize = true;
                (obj as Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings).m_FlipFinalOutput = Application.platform == RuntimePlatform.OSXEditor;
            }
            if (type == typeof(Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings))
            {
                (obj as Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings).m_ForceEvenSize = true;
            }
            if (type == typeof(Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInputSettings))
            {
                (obj as Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInputSettings).m_ForceEvenSize = true;
            }

            return obj ;
        }

        public override List<Unity_Technologies.Recorder.Framework.Core.Engine.InputGroupFilter> GetInputGroups()
        {
            return new List<Unity_Technologies.Recorder.Framework.Core.Engine.InputGroupFilter>()
            {
                new Unity_Technologies.Recorder.Framework.Core.Engine.InputGroupFilter()
                {
                    title = "Pixels",
                    typesFilter = new List<Unity_Technologies.Recorder.Framework.Core.Engine.InputFilter>()
                    {
                        new Unity_Technologies.Recorder.Framework.Core.Engine.TInputFilter<Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine.ScreenCaptureInputSettings>("Game View"),
                        new Unity_Technologies.Recorder.Framework.Core.Engine.TInputFilter<Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Specific Camera(s)"),
#if UNITY_2018_1_OR_NEWER
                        new Unity_Technologies.Recorder.Framework.Core.Engine.TInputFilter<Unity_Technologies.Recorder.Framework.Inputs.Camera360.Engine.Camera360InputSettings>("360 View (feature preview)"),
#endif
                        new Unity_Technologies.Recorder.Framework.Core.Engine.TInputFilter<Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings>("Sampling (off screen)"),
                        new Unity_Technologies.Recorder.Framework.Core.Engine.TInputFilter<Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine.RenderTextureInputSettings>("Render Texture Asset"),
                    }
                },
                new Unity_Technologies.Recorder.Framework.Core.Engine.InputGroupFilter()
                {
                    title = "Sound",
                    typesFilter = new List<Unity_Technologies.Recorder.Framework.Core.Engine.InputFilter>()
                    {
                        new Unity_Technologies.Recorder.Framework.Core.Engine.TInputFilter<AudioInputSettings>("Audio"),
                    }
                }
            };
        }

        public override bool SelfAdjustSettings()
        {
            if (this.inputsSettings.Count == 0 )
                return false;

            var adjusted = false;

            if (this.inputsSettings[0] is Unity_Technologies.Recorder.Framework.Core.Engine.ImageInputSettings)
            {
                var iis = (Unity_Technologies.Recorder.Framework.Core.Engine.ImageInputSettings)this.inputsSettings[0];
                var maxRes = this.m_OutputFormat == MediaRecorderOutputFormat.MP4 ? Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.x2160p_4K : Unity_Technologies.Recorder.Framework.Core.Engine.EImageDimension.x4320p_8K;
                if (iis.maxSupportedSize != maxRes)
                {
                    iis.maxSupportedSize = maxRes;
                    adjusted = true;
                }
            }

            return adjusted;
        }
       
    }
}

#endif