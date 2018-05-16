using System.Collections.Generic;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.EXR
{
    [ExecuteInEditMode]
    public class EXRRecorderSettings : BaseFCRecorderSettings
    {

        public UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcExrConfig m_ExrEncoderSettings = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcExrConfig.default_value;

        EXRRecorderSettings()
        {
            this.m_BaseFileName.pattern = "image_<0000>.<ext>";
        }

        public override List<Framework.Core.Engine.RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<Framework.Core.Engine.RecorderInputSetting>() { this.NewInputSettingsObj<Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Pixels") };
        }

    }
}
