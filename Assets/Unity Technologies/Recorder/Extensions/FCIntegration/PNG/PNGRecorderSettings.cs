using System.Collections.Generic;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.PNG
{
    [ExecuteInEditMode]
    public class PNGRecorderSettings : BaseFCRecorderSettings
    {
        public UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcPngConfig m_PngEncoderSettings = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcPngConfig.default_value;

        PNGRecorderSettings()
        {
            this.m_BaseFileName.pattern = "image_<0000>.<ext>";
        }

        public override List<Framework.Core.Engine.RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<Framework.Core.Engine.RecorderInputSetting>() { this.NewInputSettingsObj<Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Pixels") };
        }
    }
}
