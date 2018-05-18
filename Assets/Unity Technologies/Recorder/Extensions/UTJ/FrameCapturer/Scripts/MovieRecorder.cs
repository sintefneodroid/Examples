using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR

#endif


namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts
{
    [AddComponentMenu("UTJ/FrameCapturer/Movie Recorder")]
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class MovieRecorder : RecorderBase
    {
        #region inner_types
        public enum CaptureTarget
        {
            FrameBuffer,
            RenderTexture,
        }
        #endregion


        #region fields
        [SerializeField] Encoder.MovieEncoderConfigs m_encoderConfigs = new Encoder.MovieEncoderConfigs(Encoder.MovieEncoder.Type.WebM);
        [SerializeField] CaptureTarget m_captureTarget = CaptureTarget.FrameBuffer;
        [SerializeField] RenderTexture m_targetRT;
        [SerializeField] bool m_captureVideo = true;
        [SerializeField] bool m_captureAudio = true;

        [SerializeField] Shader m_shCopy;
        Material m_matCopy;
        Mesh m_quad;
        CommandBuffer m_cb;
        RenderTexture m_scratchBuffer;
        Encoder.MovieEncoder m_encoder;
        #endregion


        #region properties
        public CaptureTarget captureTarget
        {
            get { return this.m_captureTarget; }
            set { this.m_captureTarget = value; }
        }
        public RenderTexture targetRT
        {
            get { return this.m_targetRT; }
            set { this.m_targetRT = value; }
        }
        public bool captureAudio
        {
            get { return this.m_captureAudio; }
            set { this.m_captureAudio = value; }
        }
        public bool captureVideo
        {
            get { return this.m_captureVideo; }
            set { this.m_captureVideo = value; }
        }

        public bool supportVideo { get { return this.m_encoderConfigs.supportVideo; } }
        public bool supportAudio { get { return this.m_encoderConfigs.supportAudio; } }
        public RenderTexture scratchBuffer { get { return this.m_scratchBuffer; } }
        #endregion


        public override bool BeginRecording()
        {
            if (this.m_recording) { return false; }
            if (this.m_shCopy == null)
            {
                Debug.LogError("MovieRecorder: copy shader is missing!");
                return false;
            }
            if (this.m_captureTarget == CaptureTarget.RenderTexture && this.m_targetRT == null)
            {
                Debug.LogError("MovieRecorder: target RenderTexture is null!");
                return false;
            }

            this.m_outputDir.CreateDirectory();
            if (this.m_quad == null) this.m_quad = Encoder.fcAPI.CreateFullscreenQuad();
            if (this.m_matCopy == null) this.m_matCopy = new Material(this.m_shCopy);

            var cam = this.GetComponent<Camera>();
            if (cam.targetTexture != null)
            {
                this.m_matCopy.EnableKeyword("OFFSCREEN");
            }
            else
            {
                this.m_matCopy.DisableKeyword("OFFSCREEN");
            }

            // create scratch buffer
            {
                int captureWidth = cam.pixelWidth;
                int captureHeight = cam.pixelHeight;
                this.GetCaptureResolution(ref captureWidth, ref captureHeight);
                if (this.m_encoderConfigs.format == Encoder.MovieEncoder.Type.MP4 ||
                    this.m_encoderConfigs.format == Encoder.MovieEncoder.Type.WebM)
                {
                    captureWidth = (captureWidth + 1) & ~1;
                    captureHeight = (captureHeight + 1) & ~1;
                }

                this.m_scratchBuffer = new RenderTexture(captureWidth, captureHeight, 0, RenderTextureFormat.ARGB32);
                this.m_scratchBuffer.wrapMode = TextureWrapMode.Repeat;
                this.m_scratchBuffer.Create();
            }

            // initialize encoder
            {
                int targetFramerate = 60;
                if(this.m_framerateMode == FrameRateMode.Constant)
                {
                    targetFramerate = this.m_targetFramerate;
                }
                string outPath = this.m_outputDir.GetFullPath() + "/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                this.m_encoderConfigs.captureVideo = this.m_captureVideo;
                this.m_encoderConfigs.captureAudio = this.m_captureAudio;
                this.m_encoderConfigs.Setup(this.m_scratchBuffer.width, this.m_scratchBuffer.height, 3, targetFramerate);
                this.m_encoder = Encoder.MovieEncoder.Create(this.m_encoderConfigs, outPath);
                if (this.m_encoder == null || !this.m_encoder.IsValid())
                {
                    this.EndRecording();
                    return false;
                }
            }

            // create command buffer
            {
                int tid = Shader.PropertyToID("_TmpFrameBuffer");
                this.m_cb = new CommandBuffer();
                this.m_cb.name = "MovieRecorder: copy frame buffer";

                if(this.m_captureTarget == CaptureTarget.FrameBuffer)
                {
                    this.m_cb.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Bilinear);
                    this.m_cb.Blit(BuiltinRenderTextureType.CurrentActive, tid);
                    this.m_cb.SetRenderTarget(this.m_scratchBuffer);
                    this.m_cb.DrawMesh(this.m_quad, Matrix4x4.identity, this.m_matCopy, 0, 0);
                    this.m_cb.ReleaseTemporaryRT(tid);
                }
                else if(this.m_captureTarget == CaptureTarget.RenderTexture)
                {
                    this.m_cb.SetRenderTarget(this.m_scratchBuffer);
                    this.m_cb.SetGlobalTexture("_TmpRenderTarget", this.m_targetRT);
                    this.m_cb.DrawMesh(this.m_quad, Matrix4x4.identity, this.m_matCopy, 0, 0);
                }
                cam.AddCommandBuffer(CameraEvent.AfterEverything, this.m_cb);
            }

            base.BeginRecording();
            Debug.Log("MovieRecorder: BeginRecording()");
            return true;
        }

        public override void EndRecording()
        {
            if (this.m_encoder != null)
            {
                this.m_encoder.Release();
                this.m_encoder = null;
            }
            if (this.m_cb != null)
            {
                this.GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.AfterEverything, this.m_cb);
                this.m_cb.Release();
                this.m_cb = null;
            }
            if (this.m_scratchBuffer != null)
            {
                this.m_scratchBuffer.Release();
                this.m_scratchBuffer = null;
            }

            if (this.m_recording)
            {
                Debug.Log("MovieRecorder: EndRecording()");
            }
            base.EndRecording();
        }


        #region impl
#if UNITY_EDITOR
        void Reset()
        {
            this.m_shCopy = Encoder.fcAPI.GetFrameBufferCopyShader();
        }
#endif // UNITY_EDITOR

        IEnumerator OnPostRender()
        {
            if (this.m_recording && this.m_encoder != null && Time.frameCount % this.m_captureEveryNthFrame == 0)
            {
                yield return new WaitForEndOfFrame();

                double timestamp = Time.unscaledTime - this.m_initialTime;
                if (this.m_framerateMode == FrameRateMode.Constant)
                {
                    timestamp = 1.0 / this.m_targetFramerate * this.m_recordedFrames;
                }

                Encoder.fcAPI.fcLock(this.m_scratchBuffer, TextureFormat.RGB24, (data, fmt) =>
                {
                    this.m_encoder.AddVideoFrame(data, fmt, timestamp);
                });
                ++this.m_recordedFrames;
            }
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
