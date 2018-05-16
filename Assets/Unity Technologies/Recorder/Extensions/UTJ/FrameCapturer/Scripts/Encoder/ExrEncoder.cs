using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder
{
    public class ExrEncoder : MovieEncoder
    {
        static readonly string[] s_channelNames = { "R", "G", "B", "A" };
        fcAPI.fcExrContext m_ctx;
        fcAPI.fcExrConfig m_config;
        string m_outPath;
        int m_frame;

        public override void Release() { this.m_ctx.Release(); }
        public override bool IsValid() { return this.m_ctx; }
        public override Type type { get { return Type.Exr; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcExrIsSupported())
            {
                Debug.LogError("Exr encoder is not available on this platform.");
                return;
            }

            this.m_config = (fcAPI.fcExrConfig)config;
            this.m_ctx = fcAPI.fcExrCreateContext(ref this.m_config);
            this.m_outPath = outPath;
            this.m_frame = 0;
        }

        public override void AddVideoFrame(byte[] frame, fcAPI.fcPixelFormat format, double timestamp = -1.0)
        {
            if (this.m_ctx)
            {
                string path = this.m_outPath + "_" + this.m_frame.ToString("0000") + ".exr";
                int channels = System.Math.Min(this.m_config.channels, (int)format & 7);

                fcAPI.fcExrBeginImage(this.m_ctx, path, this.m_config.width, this.m_config.height);
                for (int i = 0; i < channels; ++i)
                {
                    fcAPI.fcExrAddLayerPixels(this.m_ctx, frame, format, i, s_channelNames[i]);
                }
                fcAPI.fcExrEndImage(this.m_ctx);
            }
            ++this.m_frame;
        }

        public override void AddAudioSamples(float[] samples)
        {
            // not supported
        }

    }
}
