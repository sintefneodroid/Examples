using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.WEBM
{
    [ExecuteInEditMode]
    public class WEBMRecorderSettings : BaseFCRecorderSettings
    {
        public UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcWebMConfig m_WebmEncoderSettings = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcWebMConfig.default_value;
        public bool m_AutoSelectBR;

        WEBMRecorderSettings()
        {
            this.m_BaseFileName.pattern = "movie.<ext>";
            this.m_AutoSelectBR = true;
        }

        public override List<Framework.Core.Engine.RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<Framework.Core.Engine.RecorderInputSetting>()
            {
                this.NewInputSettingsObj<Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Pixels") 
            };
        }

        public override Framework.Core.Engine.RecorderInputSetting NewInputSettingsObj(Type type, string title )
        {
            var obj = base.NewInputSettingsObj(type, title);
            if (type == typeof(Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings))
                (obj as Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings).m_ForceEvenSize = true;

            return obj ;
        }

    }
}
