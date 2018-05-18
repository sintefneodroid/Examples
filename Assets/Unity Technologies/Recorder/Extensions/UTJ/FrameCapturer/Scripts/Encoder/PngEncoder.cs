using System;
using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder
{
    public class PngEncoder : MovieEncoder
    {
        fcAPI.fcPngContext m_ctx;
        fcAPI.fcPngConfig m_config;
        string m_outPath;
        int m_frame;


        public override void Release() { this.m_ctx.Release(); }
        public override bool IsValid() { return this.m_ctx; }
        public override Type type { get { return Type.Png; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcPngIsSupported())
            {
                Debug.LogError("Png encoder is not available on this platform.");
                return;
            }

            this.m_config = (fcAPI.fcPngConfig)config;
            this.m_ctx = fcAPI.fcPngCreateContext(ref this.m_config);
            this.m_outPath = outPath;
            this.m_frame = 0;
        }

        public override void AddVideoFrame(byte[] frame, fcAPI.fcPixelFormat format, double timestamp = -1.0)
        {
            if (this.m_ctx)
            {
                string path = this.m_outPath + "_" + this.m_frame.ToString("0000") + ".png";
                int channels = Math.Min(this.m_config.channels, (int)format & 7);
                fcAPI.fcPngExportPixels(this.m_ctx, path, frame, this.m_config.width, this.m_config.height, format, channels);
            }
            ++this.m_frame;
        }

        public override void AddAudioSamples(float[] samples)
        {
            // not supported
        }

    }
}
