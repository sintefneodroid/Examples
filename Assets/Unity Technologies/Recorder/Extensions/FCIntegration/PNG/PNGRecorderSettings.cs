using System.Collections.Generic;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine;

namespace Unity_Technologies.Recorder.Extensions.FCIntegration.PNG
{
    [ExecuteInEditMode]
    public class PNGRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcPngConfig m_PngEncoderSettings = fcAPI.fcPngConfig.default_value;

        PNGRecorderSettings()
        {
            this.m_BaseFileName.pattern = "image_<0000>.<ext>";
        }

        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<RecorderInputSetting>() { this.NewInputSettingsObj<CBRenderTextureInputSettings>("Pixels") };
        }
    }
}
