using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity_Technologies.Recorder.Extensions.UTJ.FrameCapturer.Scripts
{

    [AddComponentMenu("UTJ/FrameCapturer/GBuffer Recorder")]
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class GBufferRecorder : RecorderBase
    {
        #region inner_types
        [Serializable]
        public struct FrameBufferConponents
        {
            public bool frameBuffer;
            public bool fbColor;
            public bool fbAlpha;

            public bool GBuffer;
            public bool gbAlbedo;
            public bool gbOcclusion;
            public bool gbSpecular;
            public bool gbSmoothness;
            public bool gbNormal;
            public bool gbEmission;
            public bool gbDepth;
            public bool gbVelocity;

            public static FrameBufferConponents defaultValue
            {
                get
                {
                    var ret = new FrameBufferConponents
                    {
                        frameBuffer = true,
                        fbColor = true,
                        fbAlpha = true,
                        GBuffer = true,
                        gbAlbedo = true,
                        gbOcclusion = true,
                        gbSpecular = true,
                        gbSmoothness = true,
                        gbNormal = true,
                        gbEmission = true,
                        gbDepth = true,
                        gbVelocity = true,
                    };
                    return ret;
                }
            }
        }

        class BufferRecorder
        {
            RenderTexture m_rt;
            int m_channels;
            int m_targetFramerate = 30;
            string m_name;
            Encoder.MovieEncoder m_encoder;

            public BufferRecorder(RenderTexture rt, int ch, string name, int tf)
            {
                this.m_rt = rt;
                this.m_channels = ch;
                this.m_name = name;
            }

            public bool Initialize(Encoder.MovieEncoderConfigs c, Misc.DataPath p)
            {
                string path = p.GetFullPath() + "/" + this.m_name;
                c.Setup(this.m_rt.width, this.m_rt.height, this.m_channels, this.m_targetFramerate);
                this.m_encoder = Encoder.MovieEncoder.Create(c, path);
                return this.m_encoder != null && this.m_encoder.IsValid();
            }

            public void Release()
            {
                if(this.m_encoder != null)
                {
                    this.m_encoder.Release();
                    this.m_encoder = null;
                }
            }

            public void Update(double time)
            {
                if (this.m_encoder != null)
                {
                    Encoder.fcAPI.fcLock(this.m_rt, (data, fmt) =>
                    {
                        this.m_encoder.AddVideoFrame(data, fmt, time);
                    });
                }
            }
        }

        #endregion


        #region fields
        [SerializeField] Encoder.MovieEncoderConfigs m_encoderConfigs = new Encoder.MovieEncoderConfigs(Encoder.MovieEncoder.Type.Exr);
        [SerializeField] FrameBufferConponents m_fbComponents = FrameBufferConponents.defaultValue;

        [SerializeField] Shader m_shCopy;
        Material m_matCopy;
        Mesh m_quad;
        CommandBuffer m_cbCopyFB;
        CommandBuffer m_cbCopyGB;
        CommandBuffer m_cbClearGB;
        CommandBuffer m_cbCopyVelocity;
        RenderTexture[] m_rtFB;
        RenderTexture[] m_rtGB;
        List<BufferRecorder> m_recorders = new List<BufferRecorder>();
        #endregion


        #region properties
        public FrameBufferConponents fbComponents
        {
            get { return this.m_fbComponents; }
            set { this.m_fbComponents = value; }
        }

        public Encoder.MovieEncoderConfigs encoderConfigs { get { return this.m_encoderConfigs; } }
        #endregion



        public override bool BeginRecording()
        {
            if (this.m_recording) { return false; }
            if (this.m_shCopy == null)
            {
                Debug.LogError("GBufferRecorder: copy shader is missing!");
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

            int captureWidth = cam.pixelWidth;
            int captureHeight = cam.pixelHeight;
            this.GetCaptureResolution(ref captureWidth, ref captureHeight);
            if (this.m_encoderConfigs.format == Encoder.MovieEncoder.Type.MP4 ||
                this.m_encoderConfigs.format == Encoder.MovieEncoder.Type.WebM)
            {
                captureWidth = (captureWidth + 1) & ~1;
                captureHeight = (captureHeight + 1) & ~1;
            }

            if (this.m_fbComponents.frameBuffer)
            {
                this.m_rtFB = new RenderTexture[2];
                for (int i = 0; i < this.m_rtFB.Length; ++i)
                {
                    this.m_rtFB[i] = new RenderTexture(captureWidth, captureHeight, 0, RenderTextureFormat.ARGBHalf);
                    this.m_rtFB[i].filterMode = FilterMode.Point;
                    this.m_rtFB[i].Create();
                }

                int tid = Shader.PropertyToID("_TmpFrameBuffer");
                this.m_cbCopyFB = new CommandBuffer();
                this.m_cbCopyFB.name = "GBufferRecorder: Copy FrameBuffer";
                this.m_cbCopyFB.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Point);
                this.m_cbCopyFB.Blit(BuiltinRenderTextureType.CurrentActive, tid);
                this.m_cbCopyFB.SetRenderTarget(new RenderTargetIdentifier[] { this.m_rtFB[0], this.m_rtFB[1] }, this.m_rtFB[0]);
                this.m_cbCopyFB.DrawMesh(this.m_quad, Matrix4x4.identity, this.m_matCopy, 0, 0);
                this.m_cbCopyFB.ReleaseTemporaryRT(tid);
                cam.AddCommandBuffer(CameraEvent.AfterEverything, this.m_cbCopyFB);
            }
            if (this.m_fbComponents.GBuffer)
            {
                this.m_rtGB = new RenderTexture[8];
                for (int i = 0; i < this.m_rtGB.Length; ++i)
                {
                    this.m_rtGB[i] = new RenderTexture(captureWidth, captureHeight, 0, RenderTextureFormat.ARGBHalf);
                    this.m_rtGB[i].filterMode = FilterMode.Point;
                    this.m_rtGB[i].Create();
                }

                // clear gbuffer (Unity doesn't clear emission buffer - it is not needed usually)
                this.m_cbClearGB = new CommandBuffer();
                this.m_cbClearGB.name = "GBufferRecorder: Cleanup GBuffer";
                if (cam.allowHDR)
                {
                    this.m_cbClearGB.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                }
                else
                {
                    this.m_cbClearGB.SetRenderTarget(BuiltinRenderTextureType.GBuffer3);
                }
                this.m_cbClearGB.DrawMesh(this.m_quad, Matrix4x4.identity, this.m_matCopy, 0, 3);
                this.m_matCopy.SetColor("_ClearColor", cam.backgroundColor);

                // copy gbuffer
                this.m_cbCopyGB = new CommandBuffer();
                this.m_cbCopyGB.name = "GBufferRecorder: Copy GBuffer";
                this.m_cbCopyGB.SetRenderTarget(new RenderTargetIdentifier[] {
                    this.m_rtGB[0], this.m_rtGB[1], this.m_rtGB[2], this.m_rtGB[3], this.m_rtGB[4], this.m_rtGB[5], this.m_rtGB[6]
                }, this.m_rtGB[0]);
                this.m_cbCopyGB.DrawMesh(this.m_quad, Matrix4x4.identity, this.m_matCopy, 0, 2);
                cam.AddCommandBuffer(CameraEvent.BeforeGBuffer, this.m_cbClearGB);
                cam.AddCommandBuffer(CameraEvent.BeforeLighting, this.m_cbCopyGB);

                if (this.m_fbComponents.gbVelocity)
                {
                    this.m_cbCopyVelocity = new CommandBuffer();
                    this.m_cbCopyVelocity.name = "GBufferRecorder: Copy Velocity";
                    this.m_cbCopyVelocity.SetRenderTarget(this.m_rtGB[7]);
                    this.m_cbCopyVelocity.DrawMesh(this.m_quad, Matrix4x4.identity, this.m_matCopy, 0, 4);
                    cam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_cbCopyVelocity);
                    cam.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
                }
            }

            int framerate = this.m_targetFramerate;
            if (this.m_fbComponents.frameBuffer) {
                if (this.m_fbComponents.fbColor) this.m_recorders.Add(new BufferRecorder(this.m_rtFB[0], 4, "FrameBuffer", framerate));
                if (this.m_fbComponents.fbAlpha) this.m_recorders.Add(new BufferRecorder(this.m_rtFB[1], 1, "Alpha", framerate));
            }
            if (this.m_fbComponents.GBuffer)
            {
                if (this.m_fbComponents.gbAlbedo)      { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[0], 3, "Albedo", framerate)); }
                if (this.m_fbComponents.gbOcclusion)   { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[1], 1, "Occlusion", framerate)); }
                if (this.m_fbComponents.gbSpecular)    { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[2], 3, "Specular", framerate)); }
                if (this.m_fbComponents.gbSmoothness)  { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[3], 1, "Smoothness", framerate)); }
                if (this.m_fbComponents.gbNormal)      { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[4], 3, "Normal", framerate)); }
                if (this.m_fbComponents.gbEmission)    { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[5], 3, "Emission", framerate)); }
                if (this.m_fbComponents.gbDepth)       { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[6], 1, "Depth", framerate)); }
                if (this.m_fbComponents.gbVelocity)    { this.m_recorders.Add(new BufferRecorder(this.m_rtGB[7], 2, "Velocity", framerate)); }
            }
            foreach (var rec in this.m_recorders)
            {
                if (!rec.Initialize(this.m_encoderConfigs, this.m_outputDir))
                {
                    this.EndRecording();
                    return false;
                }
            }

            base.BeginRecording();

            Debug.Log("GBufferRecorder: BeginRecording()");
            return true;
        }

        public override void EndRecording()
        {
            foreach (var rec in this.m_recorders) { rec.Release(); }
            this.m_recorders.Clear();

            var cam = this.GetComponent<Camera>();
            if (this.m_cbCopyFB != null)
            {
                cam.RemoveCommandBuffer(CameraEvent.AfterEverything, this.m_cbCopyFB);
                this.m_cbCopyFB.Release();
                this.m_cbCopyFB = null;
            }
            if (this.m_cbClearGB != null)
            {
                cam.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, this.m_cbClearGB);
                this.m_cbClearGB.Release();
                this.m_cbClearGB = null;
            }
            if (this.m_cbCopyGB != null)
            {
                cam.RemoveCommandBuffer(CameraEvent.BeforeLighting, this.m_cbCopyGB);
                this.m_cbCopyGB.Release();
                this.m_cbCopyGB = null;
            }
            if (this.m_cbCopyVelocity != null)
            {
                cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_cbCopyVelocity);
                this.m_cbCopyVelocity.Release();
                this.m_cbCopyVelocity = null;
            }

            if (this.m_rtFB != null)
            {
                foreach (var rt in this.m_rtFB) { rt.Release(); }
                this.m_rtFB = null;
            }
            if (this.m_rtGB != null)
            {
                foreach (var rt in this.m_rtGB) { rt.Release(); }
                this.m_rtGB = null;
            }

            if (this.m_recording)
            {
                Debug.Log("GBufferRecorder: EndRecording()");
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
            if (this.m_recording)
            {
                yield return new WaitForEndOfFrame();

                //double timestamp = Time.unscaledTime - m_initialTime;
                double timestamp = 1.0 / this.m_targetFramerate * this.m_recordedFrames;
                foreach (var rec in this.m_recorders) { rec.Update(timestamp); }

                ++this.m_recordedFrames;
            }
            this.m_frame++;
        }
        #endregion
    }

}
