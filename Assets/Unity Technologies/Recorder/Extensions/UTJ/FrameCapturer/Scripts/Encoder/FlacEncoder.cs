using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder
{
    public class FlacEncoder : AudioEncoder
    {
        fcAPI.fcFlacContext m_ctx;
        fcAPI.fcFlacConfig m_config;

        public override void Release() { this.m_ctx.Release(); }
        public override bool IsValid() { return this.m_ctx; }
        public override Type type { get { return Type.Flac; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcFlacIsSupported())
            {
                Debug.LogError("Flac encoder is not available on this platform.");
                return;
            }

            this.m_config = (fcAPI.fcFlacConfig)config;
            this.m_config.sampleRate = AudioSettings.outputSampleRate;
            this.m_config.numChannels = fcAPI.fcGetNumAudioChannels();
            this.m_ctx = fcAPI.fcFlacCreateContext(ref this.m_config);

            var path = outPath + ".flac";
            var stream = fcAPI.fcCreateFileStream(path);
            fcAPI.fcFlacAddOutputStream(this.m_ctx, stream);
            stream.Release();
        }

        public override void AddAudioSamples(float[] samples)
        {
            if (this.m_ctx)
            {
                fcAPI.fcFlacAddAudioSamples(this.m_ctx, samples, samples.Length);
            }
        }
    }
}
