using System.Collections.Generic;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.Camera360.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine;

namespace Unity_Technologies.Recorder.Framework.Recorders.ImageRecorder.Engine
{

    public enum PNGRecordeOutputFormat
    {
        PNG,
        JPEG,
        EXR
    }

    [ExecuteInEditMode]
    public class ImageRecorderSettings : RecorderSettings
    {
        public PNGRecordeOutputFormat m_OutputFormat = PNGRecordeOutputFormat.JPEG;

        ImageRecorderSettings()
        {
            this.m_BaseFileName.pattern = "image_<0000>.<ext>";
        }

        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<RecorderInputSetting>()
            {
                this.NewInputSettingsObj<CBRenderTextureInputSettings>("Pixels") 
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

        public override bool SelfAdjustSettings()
        {
            if (this.inputsSettings.Count == 0 )
                return false;

            var adjusted = false;

            if (this.inputsSettings[0] is RenderTextureSamplerSettings)
            {
                var input = (RenderTextureSamplerSettings)this.inputsSettings[0];
                var colorSpace = this.m_OutputFormat == PNGRecordeOutputFormat.EXR ? ColorSpace.Linear : ColorSpace.Gamma;
                if (input.m_ColorSpace != colorSpace)
                {
                    input.m_ColorSpace = colorSpace;
                    adjusted = true;
                }
            }

            if (this.inputsSettings[0] is ImageInputSettings)
            {
                var iis = (ImageInputSettings)this.inputsSettings[0];
                if (iis.maxSupportedSize != EImageDimension.x4320p_8K)
                {
                    iis.maxSupportedSize = EImageDimension.x4320p_8K;
                    adjusted = true;
                }
            }

            return adjusted;
        }

        public override List<InputGroupFilter> GetInputGroups()
        {
            return new List<InputGroupFilter>()
            {
                new InputGroupFilter()
                {
                    title = "Pixels", typesFilter = new List<InputFilter>()
                    {
#if UNITY_2017_3_OR_NEWER
                        new TInputFilter<ScreenCaptureInputSettings>("Screen"),
#endif
                        new TInputFilter<CBRenderTextureInputSettings>("Camera(s)"),
#if UNITY_2018_1_OR_NEWER
                        new TInputFilter<Camera360InputSettings>("360 View (feature preview)"),
#endif
                        new TInputFilter<RenderTextureSamplerSettings>("Sampling"),
                        new TInputFilter<RenderTextureInputSettings>("Render Texture"),
                    }
                }
            };
        }
    }
}
