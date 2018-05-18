#if UNITY_2018_1_OR_NEWER

using System;
using UnityEngine.Rendering;
#if UNITY_EDITOR
#endif

namespace Unity_Technologies.Recorder.Framework.Inputs.Camera360.Engine
{
    public class Camera360Input : Core.Engine.BaseRenderTextureInput
    {
        bool m_ModifiedResolution;
        UnityEngine.Shader m_shCopy;
        Core.Engine.TextureFlipper m_VFlipper = new Core.Engine.TextureFlipper();

        UnityEngine.RenderTexture m_Cubemap1;
        UnityEngine.RenderTexture m_Cubemap2;

        public Camera360InputSettings settings360
        {
            get { return (Camera360InputSettings)this.settings; }
        }

        UnityEngine.Camera targetCamera { get; set; }

        public UnityEngine.Shader copyShader
        {
            get
            {
                if (this.m_shCopy == null)
                {
                    this.m_shCopy = UnityEngine.Shader.Find("Hidden/Recorder/Inputs/CBRenderTexture/CopyFB");
                }
                return this.m_shCopy;
            }

            set { this.m_shCopy = value; }
        }

        public override void BeginRecording(Core.Engine.RecordingSession session)
        {
            if (this.settings360.m_FlipFinalOutput)
                this.m_VFlipper = new Core.Engine.TextureFlipper();
            this.outputWidth = this.settings360.m_OutputWidth;
            this.outputHeight = this.settings360.m_OutputHeight;
        }

        public override void NewFrameStarting(Core.Engine.RecordingSession session)
        {
            switch (this.settings360.source)
            {
                case Core.Engine.EImageSource.MainCamera:
                {
                    if (this.targetCamera != UnityEngine.Camera.main )
                        this.targetCamera = UnityEngine.Camera.main;
                    break;
                }

                case Core.Engine.EImageSource.TaggedCamera:
                {
                    var tag = this.settings360.m_CameraTag;

                    if (this.targetCamera == null || this.targetCamera.gameObject.tag != tag )
                    {
                        try
                        {
                            var cams = UnityEngine.GameObject.FindGameObjectsWithTag(tag);
                            if (cams.Length > 0)
                                UnityEngine.Debug.LogWarning("More than one camera has the requested target tag:" + tag);
                            this.targetCamera = cams[0].transform.GetComponent<UnityEngine.Camera>();
                            
                        }
                        catch (UnityEngine.UnityException)
                        {
                            UnityEngine.Debug.LogWarning("No camera has the requested target tag:" + tag);
                            this.targetCamera = null;
                        }
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.PrepFrameRenderTexture();

        }

        public override void NewFrameReady(Core.Engine.RecordingSession session)
        {
            var eyesEyeSepBackup = this.targetCamera.stereoSeparation;
            var eyeMaskBackup = this.targetCamera.stereoTargetEye;
            if (this.settings360.m_RenderStereo)
            {
                this.targetCamera.stereoSeparation = this.settings360.m_StereoSeparation;
                this.targetCamera.stereoTargetEye = UnityEngine.StereoTargetEyeMask.Both;
                this.targetCamera.RenderToCubemap(this.m_Cubemap1, 63, UnityEngine.Camera.MonoOrStereoscopicEye.Left);
                this.targetCamera.stereoSeparation = this.settings360.m_StereoSeparation;
                this.targetCamera.stereoTargetEye = UnityEngine.StereoTargetEyeMask.Both;
                this.targetCamera.RenderToCubemap(this.m_Cubemap2, 63, UnityEngine.Camera.MonoOrStereoscopicEye.Right);
            }
            else
            {
                this.targetCamera.RenderToCubemap(this.m_Cubemap1, 63, UnityEngine.Camera.MonoOrStereoscopicEye.Mono);
            }
            
            if (this.settings360.m_RenderStereo)
            {
                this.m_Cubemap1.ConvertToEquirect(this.outputRT, UnityEngine.Camera.MonoOrStereoscopicEye.Left);
                this.m_Cubemap2.ConvertToEquirect(this.outputRT, UnityEngine.Camera.MonoOrStereoscopicEye.Right);
            }
            else
            {
                this.m_Cubemap1.ConvertToEquirect(this.outputRT, UnityEngine.Camera.MonoOrStereoscopicEye.Mono);
            }
            
            if (this.settings360.m_FlipFinalOutput)
                this.m_VFlipper.Flip(this.outputRT);
                
            this.targetCamera.stereoSeparation = eyesEyeSepBackup;
            this.targetCamera.stereoTargetEye = eyeMaskBackup;
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if( this.m_Cubemap1 )
                    Core.Engine.UnityHelpers.Destroy(this.m_Cubemap1);
                if( this.m_Cubemap2 )
                    Core.Engine.UnityHelpers.Destroy(this.m_Cubemap2);

                if( this.m_VFlipper!=null )
                    this.m_VFlipper.Dispose();
            }

            base.Dispose(disposing);
        }

        bool PrepFrameRenderTexture()
        {
            if (this.outputRT != null)
            {
                if (this.outputRT.IsCreated() && this.outputRT.width == this.outputWidth && this.outputRT.height == this.outputHeight)
                {
                    return false;
                }

                this.ReleaseBuffer();
            }

            this.outputRT = new UnityEngine.RenderTexture(this.outputWidth, this.outputHeight, 24, UnityEngine.RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Tex2D,
                antiAliasing = 1
            };
            this.m_Cubemap1 = new UnityEngine.RenderTexture(this.settings360.m_MapSize, this.settings360.m_MapSize, 24, UnityEngine.RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube ,
                
            };
            this.m_Cubemap2 = new UnityEngine.RenderTexture(this.settings360.m_MapSize, this.settings360.m_MapSize, 24, UnityEngine.RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube 
            };

            return true;
        }

    }
}

#endif