using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder
{
    public class GifEncoder : MovieEncoder
    {
        fcAPI.fcGifContext m_ctx;
        fcAPI.fcGifConfig m_config;

        public override void Release() { this.m_ctx.Release(); }
        public override bool IsValid() { return this.m_ctx; }
        public override Type type { get { return Type.Gif; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcGifIsSupported())
            {
                Debug.LogError("Gif encoder is not available on this platform.");
                return;
            }

            this.m_config = (fcAPI.fcGifConfig)config;
            this.m_config.numColors = Mathf.Clamp(this.m_config.numColors, 1, 256);
            this.m_ctx = fcAPI.fcGifCreateContext(ref this.m_config);

            var path = outPath + ".gif";
            var stream = fcAPI.fcCreateFileStream(path);
            fcAPI.fcGifAddOutputStream(this.m_ctx, stream);
            stream.Release();
        }

        public override void AddVideoFrame(byte[] frame, fcAPI.fcPixelFormat format, double timestamp)
        {
            if (this.m_ctx)
            {
                fcAPI.fcGifAddFramePixels(this.m_ctx, frame, format, timestamp);
            }
        }

        public override void AddAudioSamples(float[] samples)
        {
            // not supported
        }
    }
}
