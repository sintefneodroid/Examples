using System;
using System.Collections.Generic;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.MP4
{
    [ExecuteInEditMode]
    public class MP4RecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcMP4Config m_MP4EncoderSettings = fcAPI.fcMP4Config.default_value;
        public bool m_AutoSelectBR;

        MP4RecorderSettings()
        {
            this.m_BaseFileName.pattern = "movie.<ext>";
            this.m_AutoSelectBR = true;
        }

        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<RecorderInputSetting>()
            {
                this.NewInputSettingsObj<CBRenderTextureInputSettings>("Pixels") 
            };
        }

        public override RecorderInputSetting NewInputSettingsObj(Type type, string title )
        {
            var obj = base.NewInputSettingsObj(type, title);
            if (type == typeof(CBRenderTextureInputSettings))
            {
                var settings = (CBRenderTextureInputSettings)obj;
                settings.m_ForceEvenSize = true;
                settings.m_FlipFinalOutput = true;
            }

            return obj ;
        }

        public override bool isPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer;
            }
        }

        public override bool SelfAdjustSettings()
        {
            if (this.inputsSettings.Count == 0 )
                return false;

            var adjusted = false;

            if (this.inputsSettings[0] is ImageInputSettings)
            {
                var iis = (ImageInputSettings)this.inputsSettings[0];
                if (iis.maxSupportedSize != EImageDimension.x2160p_4K)
                {
                    iis.maxSupportedSize = EImageDimension.x2160p_4K;
                    adjusted = true;
                }
            }
            return adjusted;
        }

    }
}
