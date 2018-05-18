using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder
{
    public class OggEncoder : AudioEncoder
    {
        fcAPI.fcOggContext m_ctx;
        fcAPI.fcOggConfig m_config;

        public override void Release() { this.m_ctx.Release(); }
        public override bool IsValid() { return this.m_ctx; }
        public override Type type { get { return Type.Ogg; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcOggIsSupported())
            {
                Debug.LogError("Ogg encoder is not available on this platform.");
                return;
            }

            this.m_config = (fcAPI.fcOggConfig)config;
            this.m_config.sampleRate = AudioSettings.outputSampleRate;
            this.m_config.numChannels = fcAPI.fcGetNumAudioChannels();
            this.m_ctx = fcAPI.fcOggCreateContext(ref this.m_config);

            var path = outPath + ".ogg";
            var stream = fcAPI.fcCreateFileStream(path);
            fcAPI.fcOggAddOutputStream(this.m_ctx, stream);
            stream.Release();
        }

        public override void AddAudioSamples(float[] samples)
        {
            if(this.m_ctx)
            {
                fcAPI.fcOggAddAudioSamples(this.m_ctx, samples, samples.Length);
            }
        }
    }
}
