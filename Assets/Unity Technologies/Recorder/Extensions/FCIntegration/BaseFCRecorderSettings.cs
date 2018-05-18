using System;
using System.Collections.Generic;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.RenderTexture.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration
{
    public abstract class BaseFCRecorderSettings : RecorderSettings
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

        public override RecorderInputSetting NewInputSettingsObj(Type type, string title )
        {
            var obj = base.NewInputSettingsObj(type, title);
            if (type == typeof(CBRenderTextureInputSettings))
            {
                var settings = (CBRenderTextureInputSettings)obj;
                settings.m_FlipFinalOutput = true;
            }
            else if (type == typeof(RenderTextureSamplerSettings))
            {
                var settings = (RenderTextureSamplerSettings)obj;
                settings.m_FlipFinalOutput = true;
            }

            return obj ;
        }

        public override List<InputGroupFilter> GetInputGroups()
        {
            return new List<InputGroupFilter>()
            {
                new InputGroupFilter()
                {
                    title = "Pixels", typesFilter = new List<InputFilter>()
                    {
                        new TInputFilter<CBRenderTextureInputSettings>("Camera(s)"),
                        new TInputFilter<RenderTextureSamplerSettings>("Sampling"),
                        new TInputFilter<RenderTextureInputSettings>("Render Texture"),
                    }
                }
            };
        }

        public override bool SelfAdjustSettings()
        {
            if (this.inputsSettings.Count == 0 )
                return false;

            var adjusted = false;

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


    }
}
