using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration
{
    public abstract class BaseFCRecorderSettings : Framework.Core.Engine.RecorderSettings
    {
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

        public override bool isPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsEditor || 
                        Application.platform == RuntimePlatform.WindowsPlayer ||
                        Application.platform == RuntimePlatform.OSXEditor ||
                        Application.platform == RuntimePlatform.OSXPlayer ||
                        Application.platform == RuntimePlatform.LinuxEditor ||
                        Application.platform == RuntimePlatform.LinuxPlayer;
            }
        }

        public override Framework.Core.Engine.RecorderInputSetting NewInputSettingsObj(Type type, string title )
        {
            var obj = base.NewInputSettingsObj(type, title);
            if (type == typeof(Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings))
            {
                var settings = (Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings)obj;
                settings.m_FlipFinalOutput = true;
            }
            else if (type == typeof(Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings))
            {
                var settings = (Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings)obj;
                settings.m_FlipFinalOutput = true;
            }

            return obj ;
        }

        public override List<Framework.Core.Engine.InputGroupFilter> GetInputGroups()
        {
            return new List<Framework.Core.Engine.InputGroupFilter>()
            {
                new Framework.Core.Engine.InputGroupFilter()
                {
                    title = "Pixels", typesFilter = new List<Framework.Core.Engine.InputFilter>()
                    {
                        new Framework.Core.Engine.TInputFilter<Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Camera(s)"),
                        new Framework.Core.Engine.TInputFilter<Framework.Inputs.RenderTextureSampler.Engine.RenderTextureSamplerSettings>("Sampling"),
                        new Framework.Core.Engine.TInputFilter<Framework.Inputs.RenderTexture.Engine.RenderTextureInputSettings>("Render Texture"),
                    }
                }
            };
        }

        public override bool SelfAdjustSettings()
        {
            if (this.inputsSettings.Count == 0 )
                return false;

            var adjusted = false;

            if (this.inputsSettings[0] is Framework.Core.Engine.ImageInputSettings)
            {
                var iis = (Framework.Core.Engine.ImageInputSettings)this.inputsSettings[0];
                if (iis.maxSupportedSize != Framework.Core.Engine.EImageDimension.x4320p_8K)
                {
                    iis.maxSupportedSize = Framework.Core.Engine.EImageDimension.x4320p_8K;
                    adjusted = true;
                }
            }
            return adjusted;
        }


    }
}
