using UnityEngine;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder
{
    public class WaveEncoder : AudioEncoder
    {
        fcAPI.fcWaveContext m_ctx;
        fcAPI.fcWaveConfig m_config;

        public override void Release() { this.m_ctx.Release(); }
        public override bool IsValid() { return this.m_ctx; }
        public override Type type { get { return Type.Wave; } }

        public override void Initialize(object config, string outPath)
        {
            if (!fcAPI.fcWaveIsSupported())
            {
                Debug.LogError("Wave encoder is not available on this platform.");
                return;
            }

            this.m_config = (fcAPI.fcWaveConfig)config;
            this.m_config.sampleRate = AudioSettings.outputSampleRate;
            this.m_config.numChannels = fcAPI.fcGetNumAudioChannels();
            this.m_ctx = fcAPI.fcWaveCreateContext(ref this.m_config);

            var path = outPath + ".wave";
            var stream = fcAPI.fcCreateFileStream(path);
            fcAPI.fcWaveAddOutputStream(this.m_ctx, stream);
            stream.Release();
        }

        public override void AddAudioSamples(float[] samples)
        {
            if(this.m_ctx)
            {
                fcAPI.fcWaveAddAudioSamples(this.m_ctx, samples, samples.Length);
            }
        }
    }
}
