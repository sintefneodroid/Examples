using System;
using UnityEngine;
using Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts.Encoder;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts
{
    [AddComponentMenu("UTJ/FrameCapturer/Audio Recorder")]
    [RequireComponent(typeof(AudioListener))]
    [ExecuteInEditMode]
    public class AudioRecorder : RecorderBase
    {
        #region fields
        [SerializeField] AudioEncoderConfigs m_encoderConfigs = new AudioEncoderConfigs();
        AudioEncoder m_encoder;
        #endregion


        public override bool BeginRecording()
        {
            if (this.m_recording) { return false; }

            this.m_outputDir.CreateDirectory();

            // initialize encoder
            {
                var outPath = this.m_outputDir.GetFullPath() + "/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                this.m_encoderConfigs.Setup();
                this.m_encoder = AudioEncoder.Create(this.m_encoderConfigs, outPath);
                if (this.m_encoder == null || !this.m_encoder.IsValid())
                {
                    this.EndRecording();
                    return false;
                }
            }

            base.BeginRecording();
            Debug.Log("AudioMRecorder: BeginRecording()");
            return true;
        }

        public override void EndRecording()
        {
            if (this.m_encoder != null)
            {
                this.m_encoder.Release();
                this.m_encoder = null;
            }

            if (this.m_recording)
            {
                Debug.Log("AudioMRecorder: EndRecording()");
            }
            base.EndRecording();
            
        }


        #region impl
        void LateUpdate()
        {
            ++this.m_frame;
        }

        void OnAudioFilterRead(float[] samples, int channels)
        {
            if (this.m_recording && this.m_encoder != null)
            {
                this.m_encoder.AddAudioSamples(samples);
                this.m_recordedSamples += samples.Length;
            }
        }
        #endregion
    }
}
