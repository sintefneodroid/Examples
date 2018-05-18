using System;
using System.Collections.Generic;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.WEBM
{
    [ExecuteInEditMode]
    public class WEBMRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcWebMConfig m_WebmEncoderSettings = fcAPI.fcWebMConfig.default_value;
        public bool m_AutoSelectBR;

        WEBMRecorderSettings()
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
                (obj as CBRenderTextureInputSettings).m_ForceEvenSize = true;

            return obj ;
        }

    }
}
