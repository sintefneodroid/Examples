using System.Collections.Generic;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.GIF
{
    [ExecuteInEditMode]
    public class GIFRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcGifConfig m_GifEncoderSettings = fcAPI.fcGifConfig.default_value;

        GIFRecorderSettings()
        {
            this.m_BaseFileName.pattern = "image.<ext>";
        }

        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<RecorderInputSetting>() { this.NewInputSettingsObj<CBRenderTextureInputSettings>("Pixels") };
        }
    }
}
