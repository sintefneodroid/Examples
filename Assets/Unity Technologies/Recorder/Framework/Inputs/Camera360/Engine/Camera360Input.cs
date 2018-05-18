#if UNITY_2018_1_OR_NEWER

using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity_Technologies.Recorder.Framework.Core.Engine;

#if UNITY_EDITOR
#endif

namespace Unity_Technologies.Recorder.Framework.Inputs.Camera360.Engine
{
    public class Camera360Input : BaseRenderTextureInput
    {
        bool m_ModifiedResolution;
        Shader m_shCopy;
        TextureFlipper m_VFlipper = new TextureFlipper();

        UnityEngine.RenderTexture m_Cubemap1;
        UnityEngine.RenderTexture m_Cubemap2;

        public Camera360InputSettings settings360
        {
            get { return (Camera360InputSettings)this.settings; }
        }

        Camera targetCamera { get; set; }

        public Shader copyShader
        {
            get
            {
                if (this.m_shCopy == null)
                {
                    this.m_shCopy = Shader.Find("Hidden/Recorder/Inputs/CBRenderTexture/CopyFB");
                }
                return this.m_shCopy;
            }

            set { this.m_shCopy = value; }
        }

        public override void BeginRecording(RecordingSession session)
        {
            if (this.settings360.m_FlipFinalOutput)
                this.m_VFlipper = new TextureFlipper();
            this.outputWidth = this.settings360.m_OutputWidth;
            this.outputHeight = this.settings360.m_OutputHeight;
        }

        public override void NewFrameStarting(RecordingSession session)
        {
            switch (this.settings360.source)
            {
                case EImageSource.MainCamera:
                {
                    if (this.targetCamera != Camera.main )
                        this.targetCamera = Camera.main;
                    break;
                }

                case EImageSource.TaggedCamera:
                {
                    var tag = this.settings360.m_CameraTag;

                    if (this.targetCamera == null || this.targetCamera.gameObject.tag != tag )
                    {
                        try
                        {
                            var cams = GameObject.FindGameObjectsWithTag(tag);
                            if (cams.Length > 0)
                                Debug.LogWarning("More than one camera has the requested target tag:" + tag);
                            this.targetCamera = cams[0].transform.GetComponent<Camera>();
                            
                        }
                        catch (UnityException)
                        {
                            Debug.LogWarning("No camera has the requested target tag:" + tag);
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

        public override void NewFrameReady(RecordingSession session)
        {
            var eyesEyeSepBackup = this.targetCamera.stereoSeparation;
            var eyeMaskBackup = this.targetCamera.stereoTargetEye;
            if (this.settings360.m_RenderStereo)
            {
                this.targetCamera.stereoSeparation = this.settings360.m_StereoSeparation;
                this.targetCamera.stereoTargetEye = StereoTargetEyeMask.Both;
                this.targetCamera.RenderToCubemap(this.m_Cubemap1, 63, Camera.MonoOrStereoscopicEye.Left);
                this.targetCamera.stereoSeparation = this.settings360.m_StereoSeparation;
                this.targetCamera.stereoTargetEye = StereoTargetEyeMask.Both;
                this.targetCamera.RenderToCubemap(this.m_Cubemap2, 63, Camera.MonoOrStereoscopicEye.Right);
            }
            else
            {
                this.targetCamera.RenderToCubemap(this.m_Cubemap1, 63, Camera.MonoOrStereoscopicEye.Mono);
            }
            
            if (this.settings360.m_RenderStereo)
            {
                this.m_Cubemap1.ConvertToEquirect(this.outputRT, Camera.MonoOrStereoscopicEye.Left);
                this.m_Cubemap2.ConvertToEquirect(this.outputRT, Camera.MonoOrStereoscopicEye.Right);
            }
            else
            {
                this.m_Cubemap1.ConvertToEquirect(this.outputRT, Camera.MonoOrStereoscopicEye.Mono);
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
                    UnityHelpers.Destroy(this.m_Cubemap1);
                if( this.m_Cubemap2 )
                    UnityHelpers.Destroy(this.m_Cubemap2);

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

            this.outputRT = new UnityEngine.RenderTexture(this.outputWidth, this.outputHeight, 24, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Tex2D,
                antiAliasing = 1
            };
            this.m_Cubemap1 = new UnityEngine.RenderTexture(this.settings360.m_MapSize, this.settings360.m_MapSize, 24, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube ,
                
            };
            this.m_Cubemap2 = new UnityEngine.RenderTexture(this.settings360.m_MapSize, this.settings360.m_MapSize, 24, RenderTextureFormat.ARGB32)
            {
                dimension = TextureDimension.Cube 
            };

            return true;
        }

    }
}

#endif