using System;
using UnityEngine;
using UnityEngine.Rendering;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
#endif

namespace Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine
{
    public class CBRenderTextureInput : BaseRenderTextureInput
    {
        struct CanvasBackup
        {
            public Camera camera;
            public Canvas canvas;
        }

        bool m_ModifiedResolution;
        Shader m_shCopy;
        Material m_CopyMaterial;
        TextureFlipper m_VFlipper = new TextureFlipper();
        Mesh m_quad;
        CommandBuffer m_cbCopyFB;
        CommandBuffer m_cbCopyGB;
        CommandBuffer m_cbClearGB;
        CommandBuffer m_cbCopyVelocity;
        Camera m_Camera;
        bool m_cameraChanged;
        Camera m_UICamera;

        CanvasBackup[] m_CanvasBackups;

        public CBRenderTextureInputSettings cbSettings
        {
            get { return (CBRenderTextureInputSettings)this.settings; }
        }

        public Camera targetCamera
        {
            get { return this.m_Camera; }

            set
            {
                if (this.m_Camera != value)
                {
                    this.ReleaseCamera();
                    this.m_Camera = value;
                    this.m_cameraChanged = true;
                }
            }
        }

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

        public Material copyMaterial
        {
            get
            {
                if (this.m_CopyMaterial == null)
                {
                    this.m_CopyMaterial = new Material(this.copyShader);
                    this.copyMaterial.EnableKeyword("OFFSCREEN");
                    if (this.cbSettings.m_AllowTransparency)
                        this.m_CopyMaterial.EnableKeyword("TRANSPARENCY_ON");
                }
                return this.m_CopyMaterial;
            }
        }

        public override void BeginRecording(RecordingSession session)
        {
            if (this.cbSettings.m_FlipFinalOutput)
                this.m_VFlipper = new TextureFlipper();

            this.m_quad = CreateFullscreenQuad();
            switch (this.cbSettings.source)
            {
                case EImageSource.ActiveCameras:
                case EImageSource.MainCamera:
                case EImageSource.TaggedCamera:
                {
                    var screenWidth = Screen.width;
                    var screenHeight = Screen.height;
#if UNITY_EDITOR
                    switch (this.cbSettings.m_OutputSize)
                    {
                        case EImageDimension.Window:
                        {
                            GameViewSize.GetGameRenderSize(out screenWidth, out screenHeight);
                            this.outputWidth = screenWidth;
                            this.outputHeight = screenHeight;

                            if (this.cbSettings.m_ForceEvenSize)
                            {
                                this.outputWidth = (this.outputWidth + 1) & ~1;
                                this.outputHeight = (this.outputHeight + 1) & ~1;
                            }

                            break;
                        }

                        default:
                        {
                            this.outputHeight = (int)this.cbSettings.m_OutputSize;
                            this.outputWidth = (int)(this.outputHeight * AspectRatioHelper.GetRealAR(this.cbSettings.m_AspectRatio));

                            if (this.cbSettings.m_ForceEvenSize)
                            {
                                this.outputWidth = (this.outputWidth + 1) & ~1;
                                this.outputHeight = (this.outputHeight + 1) & ~1;
                            }

                            var size = GameViewSize.SetCustomSize(this.outputWidth, this.outputHeight);
                            if (size == null)
                                size = GameViewSize.AddSize(this.outputWidth, this.outputHeight);

                            if (GameViewSize.m_ModifiedResolutionCount == 0)
                                GameViewSize.BackupCurrentSize();
                            else
                            {
                                if (size != GameViewSize.currentSize)
                                {
                                    Debug.LogError("Requestion a resultion change while a recorder's input has already requested one! Undefined behaviour.");
                                }
                            }
                            GameViewSize.m_ModifiedResolutionCount++;
                            this.m_ModifiedResolution = true;
                            GameViewSize.SelectSize(size);
                            break;
                        }
                    }
#endif
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (this.cbSettings.m_CaptureUI)
            {
                var uiGO = new GameObject();
                uiGO.name = "UICamera";
                uiGO.transform.parent = session.m_RecorderGO.transform;

                this.m_UICamera = uiGO.AddComponent<Camera>();
                this.m_UICamera.cullingMask = 1 << 5;
                this.m_UICamera.clearFlags = CameraClearFlags.Depth;
                this.m_UICamera.renderingPath = RenderingPath.DeferredShading;
                this.m_UICamera.targetTexture = this.outputRT;
                this.m_UICamera.enabled = false;
            }
        }

        public override void NewFrameStarting(RecordingSession session)
        {
            switch (this.cbSettings.source)
            {
                case EImageSource.ActiveCameras:
                {
                    if (this.targetCamera == null)
                    {
                        var displayGO = new GameObject();
                        displayGO.name = "CameraHostGO-" + displayGO.GetInstanceID();
                        displayGO.transform.parent = session.m_RecorderGO.transform;
                        var camera = displayGO.AddComponent<Camera>();
                        camera.clearFlags = CameraClearFlags.Nothing;
                        camera.cullingMask = 0;
                        camera.renderingPath = RenderingPath.DeferredShading;
                        camera.targetDisplay = 0;
                        camera.rect = new Rect(0, 0, 1, 1);
                        camera.depth = float.MaxValue;

                        this.targetCamera = camera;
                    }
                    break;
                }

                case EImageSource.MainCamera:
                {
                    if (this.targetCamera != Camera.main )
                    {
                        this.targetCamera = Camera.main;
                        this.m_cameraChanged = true;
                    }
                    break;
                }
                case EImageSource.TaggedCamera:
                {
                    var tag = (this.settings as CBRenderTextureInputSettings).m_CameraTag;

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

            var newTexture = this.PrepFrameRenderTexture();

            // initialize command buffer
            if (this.m_Camera != null && (this.m_cameraChanged || newTexture))
            {
                if (this.m_cbCopyFB != null)
                {
                    this.m_Camera.RemoveCommandBuffer(CameraEvent.AfterEverything, this.m_cbCopyFB);
                    this.m_cbCopyFB.Release();
                }

                var tid = Shader.PropertyToID("_TmpFrameBuffer");
                this.m_cbCopyFB = new CommandBuffer { name = "Recorder: copy frame buffer" };
                this.m_cbCopyFB.GetTemporaryRT(tid, -1, -1, 0, FilterMode.Bilinear);
                this.m_cbCopyFB.Blit(BuiltinRenderTextureType.CurrentActive, tid);
                this.m_cbCopyFB.SetRenderTarget(this.outputRT);
                this.m_cbCopyFB.DrawMesh(this.m_quad, Matrix4x4.identity, this.copyMaterial, 0, 0);
                this.m_cbCopyFB.ReleaseTemporaryRT(tid);
                this.m_Camera.AddCommandBuffer(CameraEvent.AfterEverything, this.m_cbCopyFB);

                this.m_cameraChanged = false;
            }

            if (Math.Abs(1-this.targetCamera.rect.width) > float.Epsilon || Math.Abs(1 - this.targetCamera.rect.height) > float.Epsilon)
            {
                Debug.LogWarning( string.Format("Recording output of camera '{0}' who's rectangle does not cover the viewport: resulting image will be up-sampled with associated quality degradation!", this.targetCamera.gameObject.name));
            }
        }

        public override void NewFrameReady(RecordingSession session)
        {
            if (this.cbSettings.m_CaptureUI)
            {
                // Find canvases
                var canvases = Object.FindObjectsOfType<Canvas>();
                if (this.m_CanvasBackups == null || this.m_CanvasBackups.Length != canvases.Length)
                    this.m_CanvasBackups = new CanvasBackup[canvases.Length];

                // Hookup canvase to UI camera
                for (var i = 0; i < canvases.Length; i++)
                {
                    var canvas = canvases[i];
                    if (canvas.isRootCanvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        this.m_CanvasBackups[i].camera = canvas.worldCamera;
                        this.m_CanvasBackups[i].canvas = canvas;
                        canvas.renderMode = RenderMode.ScreenSpaceCamera;
                        canvas.worldCamera = this.m_UICamera;
                    }
                    else
                    {
                        // Mark this canvas as null so we can skip it when restoring.
                        // The array might contain invalid data from a previous frame.
                        this.m_CanvasBackups[i].canvas = null;
                    }
                }

                this.m_UICamera.Render();

                // Restore canvas settings
                for (var i = 0; i < this.m_CanvasBackups.Length; i++)
                {
                    // Skip those canvases that are not roots canvases or are 
                    // not using ScreenSpaceOverlay as a render mode.
                    if (this.m_CanvasBackups[i].canvas == null)
                        continue;
                        
                    this.m_CanvasBackups[i].canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    this.m_CanvasBackups[i].canvas.worldCamera = this.m_CanvasBackups[i].camera;
                }
            }

            if( this.cbSettings.m_FlipFinalOutput )
                this.m_VFlipper.Flip(this.outputRT);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ReleaseCamera();
                UnityHelpers.Destroy(this.m_UICamera);
#if UNITY_EDITOR
                if (this.m_ModifiedResolution)
                {
                    GameViewSize.m_ModifiedResolutionCount --;
                    if(GameViewSize.m_ModifiedResolutionCount == 0 )
                        GameViewSize.RestoreSize();
                }
#endif
                if( this.m_VFlipper!=null )
                    this.m_VFlipper.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void ReleaseCamera()
        {
            if (this.m_cbCopyFB != null)
            {
                if (this.m_Camera != null)
                    this.m_Camera.RemoveCommandBuffer(CameraEvent.AfterEverything, this.m_cbCopyFB);

                this.m_cbCopyFB.Release();
                this.m_cbCopyFB = null;
            }

            if (this.m_CopyMaterial != null)
                UnityHelpers.Destroy(this.m_CopyMaterial);
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

            this.outputRT = new UnityEngine.RenderTexture(this.outputWidth, this.outputHeight, 0, RenderTextureFormat.ARGB32)
            {
                wrapMode = TextureWrapMode.Repeat
            };
            this.outputRT.Create();
            if (this.m_UICamera != null)
            {
                this.m_UICamera.targetTexture = this.outputRT;
            }

            return true;
        }

        public static Mesh CreateFullscreenQuad()
        {
            var vertices = new Vector3[4]
            {
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, -1.0f, 0.0f),
                new Vector3(1.0f, -1.0f, 0.0f),
            };
            var indices = new[] { 0, 1, 2, 2, 3, 0 };

            var r = new Mesh
            {
                vertices = vertices,
                triangles = indices
            };
            return r;
        }
    }
}
