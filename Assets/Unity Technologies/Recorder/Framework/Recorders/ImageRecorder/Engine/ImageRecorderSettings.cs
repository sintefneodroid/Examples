using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Recorders.ImageRecorder.Engine
{

    public enum PNGRecordeOutputFormat
    {
        PNG,
        JPEG,
        EXR
    }

    [UnityEngine.ExecuteInEditMode]
    public class ImageRecorderSettings : Core.Engine.RecorderSettings
    {
        public PNGRecordeOutputFormat m_OutputFormat = PNGRecordeOutputFormat.JPEG;

        ImageRecorderSettings()
        {
            this.m_BaseFileName.pattern = "image_<0000>.<ext>";
        }

        public override List<Core.Engine.RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<Core.Engine.RecorderInputSetting>()
            {
                this.NewInputSettingsObj<Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Pixels") 
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

            bool adjusted = false;

            if (this.inputsSettings[0] is Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings)
            {
                var input = (Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings)this.inputsSettings[0];
                var colorSpace = this.m_OutputFormat == PNGRecordeOutputFormat.EXR ? UnityEngine.ColorSpace.Linear : UnityEngine.ColorSpace.Gamma;
                if (input.m_ColorSpace != colorSpace)
                {
                    input.m_ColorSpace = colorSpace;
                    adjusted = true;
                }
            }

            if (this.inputsSettings[0] is Core.Engine.ImageInputSettings)
            {
                var iis = (Core.Engine.ImageInputSettings)this.inputsSettings[0];
                if (iis.maxSupportedSize != Core.Engine.EImageDimension.x4320p_8K)
                {
                    iis.maxSupportedSize = Core.Engine.EImageDimension.x4320p_8K;
                    adjusted = true;
                }
            }

            return adjusted;
        }

        public override List<Core.Engine.InputGroupFilter> GetInputGroups()
        {
            return new List<Core.Engine.InputGroupFilter>()
            {
                new Core.Engine.InputGroupFilter()
                {
                    title = "Pixels", typesFilter = new List<Core.Engine.InputFilter>()
                    {
#if UNITY_2017_3_OR_NEWER
                        new Core.Engine.TInputFilter<Inputs.ScreenCapture.Engine.ScreenCaptureInputSettings>("Screen"),
#endif
                        new Core.Engine.TInputFilter<Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Camera(s)"),
#if UNITY_2018_1_OR_NEWER
                        new Core.Engine.TInputFilter<Inputs.Camera360.Engine.Camera360InputSettings>("360 View (feature preview)"),
#endif
                        new Core.Engine.TInputFilter<Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings>("Sampling"),
                        new Core.Engine.TInputFilter<Inputs.RenderTexture.Engine.RenderTextureInputSettings>("Render Texture"),
                    }
                }
            };
        }
    }
}
