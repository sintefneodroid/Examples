using System.Collections.Generic;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.GIF
{
    [ExecuteInEditMode]
    public class GIFRecorderSettings : BaseFCRecorderSettings
    {
        public UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcGifConfig m_GifEncoderSettings = UTJ.FrameCapturer.Scripts.Encoder.fcAPI.fcGifConfig.default_value;

        GIFRecorderSettings()
        {
            this.m_BaseFileName.pattern = "image.<ext>";
        }

        public override List<Framework.Core.Engine.RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<Framework.Core.Engine.RecorderInputSetting>() { this.NewInputSettingsObj<Framework.Inputs.CBRenderTexture.Engine.CBRenderTextureInputSettings>("Pixels") };
        }
    }
}
