using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder
{
    public class MP4Encoder : MovieEncoder
    {
        fcAPI.fcMP4Context m_ctx;
        fcAPI.fcMP4Config m_config;

        public override void Release() { this.m_ctx.Release(); }
        public override bool IsValid() { return this.m_ctx; }
        public override Type type { get { return Type.MP4; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcMP4OSIsSupported())
            {
                Debug.LogError("MP4 encoder is not available on this platform.");
                return;
            }

            this.m_config = (fcAPI.fcMP4Config)config;
            this.m_config.audioSampleRate = AudioSettings.outputSampleRate;
            this.m_config.audioNumChannels = fcAPI.fcGetNumAudioChannels();

            var path = outPath + ".mp4";
            this.m_ctx = fcAPI.fcMP4OSCreateContext(ref this.m_config, path);
        }

        public override void AddVideoFrame(byte[] frame, fcAPI.fcPixelFormat format, double timestamp)
        {
            if (this.m_ctx && this.m_config.video)
            {
                fcAPI.fcMP4AddVideoFramePixels(this.m_ctx, frame, format, timestamp);
            }
        }

        public override void AddAudioSamples(float[] samples)
        {
            if (this.m_ctx && this.m_config.audio)
            {
                fcAPI.fcMP4AddAudioSamples(this.m_ctx, samples, samples.Length);
            }
        }
    }
}
