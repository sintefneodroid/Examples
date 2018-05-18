using System;
using System.Collections.Generic;
using System.IO;
using ImageConversion = UnityEngine.ImageConversion;

namespace Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine
{
    public class RenderTextureSampler : Core.Engine.BaseRenderTextureInput
    {
        UnityEngine.Shader superShader;
        UnityEngine.Shader accumulateShader;
        UnityEngine.Shader normalizeShader;

        Core.Engine.TextureFlipper m_VFlipper;

        UnityEngine.RenderTexture m_renderRT;
        UnityEngine.RenderTexture[] m_accumulateRTs = new UnityEngine.RenderTexture[2];
        int m_renderWidth, m_renderHeight;

        UnityEngine.Material m_superMaterial;
        UnityEngine.Material m_accumulateMaterial;
        UnityEngine.Material m_normalizeMaterial;

        class HookedCamera
        {
            public UnityEngine.Camera camera;
            public UnityEngine.RenderTexture textureBackup;
        }

        List<HookedCamera> m_hookedCameras;

        UnityEngine.Vector2[] m_samples;

        RenderTextureSamplerSettings rtsSettings
        {
            get { return (RenderTextureSamplerSettings)this.settings; }
        }

        void GenerateSamplesMSAA(UnityEngine.Vector2[] samples, ESuperSamplingCount sc)
        {
            switch (sc)
            {
                case ESuperSamplingCount.x1:
                    samples[0] = new UnityEngine.Vector2(0.0f, 0.0f);
                    break;
                case ESuperSamplingCount.x2:
                    samples[0] = new UnityEngine.Vector2(4.0f, 4.0f);
                    samples[1] = new UnityEngine.Vector2(-4.0f, -4.0f);
                    break;
                case ESuperSamplingCount.x4:
                    samples[0] = new UnityEngine.Vector2(-2.0f, -6.0f);
                    samples[1] = new UnityEngine.Vector2(6.0f, -2.0f);
                    samples[2] = new UnityEngine.Vector2(-6.0f, 2.0f);
                    samples[3] = new UnityEngine.Vector2(2.0f, 6.0f);
                    break;
                case ESuperSamplingCount.x8:
                    samples[0] = new UnityEngine.Vector2(1.0f, -3.0f);
                    samples[1] = new UnityEngine.Vector2(-1.0f, 3.0f);
                    samples[2] = new UnityEngine.Vector2(5.0f, 1.0f);
                    samples[3] = new UnityEngine.Vector2(-3.0f, -5.0f);

                    samples[4] = new UnityEngine.Vector2(-5.0f, 5.0f);
                    samples[5] = new UnityEngine.Vector2(-7.0f, -1.0f);
                    samples[6] = new UnityEngine.Vector2(3.0f, 7.0f);
                    samples[7] = new UnityEngine.Vector2(7.0f, -7.0f);
                    break;
                case ESuperSamplingCount.x16:
                    samples[0] = new UnityEngine.Vector2(1.0f, 1.0f);
                    samples[1] = new UnityEngine.Vector2(-1.0f, -3.0f);
                    samples[2] = new UnityEngine.Vector2(-3.0f, 2.0f);
                    samples[3] = new UnityEngine.Vector2(4.0f, -1.0f);

                    samples[4] = new UnityEngine.Vector2(-5.0f, -2.0f);
                    samples[5] = new UnityEngine.Vector2(2.0f, 5.0f);
                    samples[6] = new UnityEngine.Vector2(5.0f, 3.0f);
                    samples[7] = new UnityEngine.Vector2(3.0f, -5.0f);

                    samples[8] = new UnityEngine.Vector2(-2.0f, 6.0f);
                    samples[9] = new UnityEngine.Vector2(0.0f, -7.0f);
                    samples[10] = new UnityEngine.Vector2(-4.0f, -6.0f);
                    samples[11] = new UnityEngine.Vector2(-6.0f, 4.0f);

                    samples[12] = new UnityEngine.Vector2(-8.0f, 0.0f);
                    samples[13] = new UnityEngine.Vector2(7.0f, -4.0f);
                    samples[14] = new UnityEngine.Vector2(6.0f, 7.0f);
                    samples[15] = new UnityEngine.Vector2(-7.0f, -8.0f);
                    break;
                default:
                    UnityEngine.Debug.LogError("Not expected sample count: " + sc);
                    return;
            }
            const float oneOverSixteen = 1.0f / 16.0f;
            UnityEngine.Vector2 halfHalf = new UnityEngine.Vector2(0.5f, 0.5f);
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = samples[i] * oneOverSixteen + halfHalf;
            }
        }

        public override void BeginRecording(Core.Engine.RecordingSession session)
        {
            this.superShader = UnityEngine.Shader.Find("Hidden/Volund/BS4SuperShader");
            this.accumulateShader = UnityEngine.Shader.Find("Hidden/BeautyShot/Accumulate");
            this.normalizeShader = UnityEngine.Shader.Find("Hidden/BeautyShot/Normalize");

            if( this.rtsSettings.m_FlipFinalOutput )
                this.m_VFlipper = new Core.Engine.TextureFlipper();

            // Below here is considered 'void Start()', but we run it for directly "various reasons".
            if (this.rtsSettings.m_OutputSize > this.rtsSettings.m_RenderSize)
                throw new UnityEngine.UnityException("Upscaling is not supported! Output dimension must be smaller or equal to render dimension.");

            // Calculate aspect and render/output sizes
            // Clamp size to 16K, which is the min always supported size in d3d11
            // Force output to divisible by two as x264 doesn't approve of odd image dimensions.
            var aspect = Core.Engine.AspectRatioHelper.GetRealAR(this.rtsSettings.m_AspectRatio);
            this.m_renderHeight = (int)this.rtsSettings.m_RenderSize;
            this.m_renderWidth = UnityEngine.Mathf.Min(16 * 1024, UnityEngine.Mathf.RoundToInt(this.m_renderHeight * aspect));
            this.outputHeight = (int)this.rtsSettings.m_OutputSize;
            this.outputWidth = UnityEngine.Mathf.Min(16 * 1024, UnityEngine.Mathf.RoundToInt(this.outputHeight * aspect));
            if (this.rtsSettings.m_ForceEvenSize)
            {
                this.outputWidth = (this.outputWidth + 1) & ~1;
                this.outputHeight = (this.outputHeight + 1) & ~1;
            }

            this.m_superMaterial = new UnityEngine.Material(this.superShader);
            this.m_superMaterial.hideFlags = UnityEngine.HideFlags.DontSave;

            this.m_accumulateMaterial = new UnityEngine.Material(this.accumulateShader);
            this.m_accumulateMaterial.hideFlags = UnityEngine.HideFlags.DontSave;

            this.m_normalizeMaterial = new UnityEngine.Material(this.normalizeShader);
            this.m_normalizeMaterial.hideFlags = UnityEngine.HideFlags.DontSave;

            this.m_renderRT = new UnityEngine.RenderTexture(this.m_renderWidth, this.m_renderHeight, 24, UnityEngine.RenderTextureFormat.DefaultHDR, UnityEngine.RenderTextureReadWrite.Linear);
            this.m_renderRT.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            for (int i = 0; i < 2; ++i)
            {
                this.m_accumulateRTs[i] = new UnityEngine.RenderTexture(this.m_renderWidth, this.m_renderHeight, 0, UnityEngine.RenderTextureFormat.DefaultHDR, UnityEngine.RenderTextureReadWrite.Linear);
                this.m_accumulateRTs[i].wrapMode = UnityEngine.TextureWrapMode.Clamp;
                this.m_accumulateRTs[i].Create();
            }
            var rt = new UnityEngine.RenderTexture(this.outputWidth, this.outputHeight, 0, UnityEngine.RenderTextureFormat.DefaultHDR, UnityEngine.RenderTextureReadWrite.Linear);
            rt.Create();
            this.outputRT = rt;
            this.m_samples = new UnityEngine.Vector2[(int)this.rtsSettings.m_SuperSampling];
            this.GenerateSamplesMSAA(this.m_samples, this.rtsSettings.m_SuperSampling);

            this.m_hookedCameras = new List<HookedCamera>();
        }

        public override void NewFrameStarting(Core.Engine.RecordingSession session)
        {
            switch (this.rtsSettings.source)
            {
                case Core.Engine.EImageSource.ActiveCameras:
                {
                    bool sort = false;

                    // Find all cameras targetting Display
                    foreach (var cam in UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.Camera>())
                    {
                        var hookedCam = this.m_hookedCameras.Find((x) => cam == x.camera);
                        if (hookedCam != null)
                        {
                            // Should we keep it?
                            if (cam.targetDisplay != 0 || !cam.enabled)
                            {
                                Core.Engine.UnityHelpers.Destroy(cam.targetTexture);
                                cam.targetTexture = hookedCam.textureBackup;
                                this.m_hookedCameras.Remove(hookedCam);
                            }
                            continue;
                        }

                        if (!cam.enabled || !cam.gameObject.activeInHierarchy || cam.targetDisplay != 0)
                            continue;

                        hookedCam = new HookedCamera() { camera = cam, textureBackup = cam.targetTexture };
                        var camRT = new UnityEngine.RenderTexture((int)(this.m_renderWidth * cam.rect.width), (int)(this.m_renderHeight * cam.rect.height), 24, UnityEngine.RenderTextureFormat.DefaultHDR, UnityEngine.RenderTextureReadWrite.Linear);
                        cam.targetTexture = camRT;
                        this.m_hookedCameras.Add(hookedCam);
                        sort = true;
                    }

                    if (sort)
                    {
                        this.m_hookedCameras.Sort((x, y) => x.camera.depth < y.camera.depth ? -1 : x.camera.depth > y.camera.depth ? 1 : 0);
                    }
                    break;
                }
                case Core.Engine.EImageSource.MainCamera:
                {
                    var cam = UnityEngine.Camera.main;
                    if (this.m_hookedCameras.Count > 0)
                    {
                        if (this.m_hookedCameras[0].camera != cam)
                        {
                            this.m_hookedCameras[0].camera.targetTexture = this.m_hookedCameras[0].textureBackup;
                            this.m_hookedCameras.Clear();
                        }
                        else
                            break;
                    }
                    if (!cam.enabled)
                        break;

                    var hookedCam = new HookedCamera() { camera = cam, textureBackup = cam.targetTexture };
                    cam.targetTexture = this.m_renderRT;
                    this.m_hookedCameras.Add(hookedCam);
                    break;
                }
                case Core.Engine.EImageSource.TaggedCamera:
                {
                    UnityEngine.GameObject[] taggedObjs;
                    var tag = (this.settings as RenderTextureSamplerSettings).m_CameraTag;
                    try
                    {
                        taggedObjs = UnityEngine.GameObject.FindGameObjectsWithTag(tag);
                    }
                    catch (UnityEngine.UnityException)
                    {
                        UnityEngine.Debug.LogWarning("No camera has the requested target tag:" + tag);
                        taggedObjs = new UnityEngine.GameObject[0];
                    }

                    // Remove un-tagged cameras form list
                    for (int i = this.m_hookedCameras.Count - 1; i >= 0; i--)
                    {
                        if (this.m_hookedCameras[i].camera.gameObject.tag != tag)
                        {
                            // un-hook it
                            this.m_hookedCameras[i].camera.targetTexture = this.m_hookedCameras[i].textureBackup;
                            this.m_hookedCameras.RemoveAt(i);
                        }
                    }

                    // Add newly tagged cameras
                    for (var i = 0; i < taggedObjs.Length; i++)
                    {
                        var found = false;
                        var cam = taggedObjs[i].transform.GetComponent<UnityEngine.Camera>();
                        if (cam != null && cam.enabled)
                        {
                            for (var j = 0; j < this.m_hookedCameras.Count; j++)
                            {
                                if (this.m_hookedCameras[j].camera == taggedObjs[i].transform.GetComponent<UnityEngine.Camera>())
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                var hookedCam = new HookedCamera() { camera = cam, textureBackup = cam.targetTexture };
                                cam.targetTexture = this.m_renderRT;
                                this.m_hookedCameras.Add(hookedCam);

                            }
                        }
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.m_hookedCameras != null)
                {
                    foreach (var c in this.m_hookedCameras)
                    {
                        if (c != null)
                        {
                            if (c.camera.rect.width == 1f && c.camera.rect.height == 1f)
                                Core.Engine.UnityHelpers.Destroy(c.camera.targetTexture);
                            c.camera.targetTexture = c.textureBackup;
                        }
                    }
                    this.m_hookedCameras.Clear();
                }

                Core.Engine.UnityHelpers.Destroy(this.m_renderRT);
                foreach (var rt in this.m_accumulateRTs)
                    Core.Engine.UnityHelpers.Destroy(rt);
                Core.Engine.UnityHelpers.Destroy(this.m_superMaterial);
                Core.Engine.UnityHelpers.Destroy(this.m_accumulateMaterial);
                Core.Engine.UnityHelpers.Destroy(this.m_normalizeMaterial);
                if(this.m_VFlipper != null)
                    this.m_VFlipper.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void NewFrameReady(Core.Engine.RecordingSession session)
        {
            this.PerformSubSampling();

            if (this.rtsSettings.m_RenderSize == this.rtsSettings.m_OutputSize)
            {
                // Blit with normalization if sizes match.
                this.m_normalizeMaterial.SetFloat("_NormalizationFactor", 1.0f / (float)this.rtsSettings.m_SuperSampling);
                this.m_normalizeMaterial.SetInt("_ApplyGammaCorrection", UnityEngine.QualitySettings.activeColorSpace == UnityEngine.ColorSpace.Linear && this.rtsSettings.m_ColorSpace == UnityEngine.ColorSpace.Gamma ? 1 : 0);
                UnityEngine.Graphics.Blit(this.m_renderRT, this.outputRT, this.m_normalizeMaterial);
            }
            else
            {
                // Ideally we would use a separable filter here, but we're massively bound by readback and disk anyway for hi-res.
                this.m_superMaterial.SetVector("_Target_TexelSize", new UnityEngine.Vector4(1f / this.outputWidth, 1f / this.outputHeight, this.outputWidth, this.outputHeight));
                this.m_superMaterial.SetFloat("_KernelCosPower", this.rtsSettings.m_SuperKernelPower);
                this.m_superMaterial.SetFloat("_KernelScale", this.rtsSettings.m_SuperKernelScale);
                this.m_superMaterial.SetFloat("_NormalizationFactor", 1.0f / (float)this.rtsSettings.m_SuperSampling);
                this.m_superMaterial.SetInt("_ApplyGammaCorrection", UnityEngine.QualitySettings.activeColorSpace == UnityEngine.ColorSpace.Linear && this.rtsSettings.m_ColorSpace == UnityEngine.ColorSpace.Gamma ? 1 : 0);
                UnityEngine.Graphics.Blit(this.m_renderRT, this.outputRT, this.m_superMaterial);
            }

            if (this.rtsSettings.m_FlipFinalOutput)
                this.m_VFlipper.Flip(this.outputRT);
        }

        void ShiftProjectionMatrix(UnityEngine.Camera camera, UnityEngine.Vector2 sample)
        {
            camera.ResetProjectionMatrix();
            UnityEngine.Matrix4x4 projectionMatrix = camera.projectionMatrix;
            float dx = sample.x / this.m_renderWidth;
            float dy = sample.y / this.m_renderHeight;
            projectionMatrix.m02 += dx;
            projectionMatrix.m12 += dy;
            camera.projectionMatrix = projectionMatrix;
        }

        bool CameraUsingPartialViewport(UnityEngine.Camera cam)
        {
            return cam.rect.width != 1 || cam.rect.height != 1 || cam.rect.x != 0 || cam.rect.y != 0;
        }

        void PerformSubSampling()
        {
            UnityEngine.RenderTexture accumulateInto = null;
            this.m_renderRT.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            this.m_renderRT.filterMode = UnityEngine.FilterMode.Point;

            int x = 0;
            UnityEngine.Graphics.SetRenderTarget(this.m_accumulateRTs[0]);
            UnityEngine.GL.Clear(false, true, UnityEngine.Color.black);

            foreach (var hookedCam in this.m_hookedCameras)
            {
                var cam = hookedCam.camera;

                for (int i = 0, n = (int)this.rtsSettings.m_SuperSampling; i < n; i++)
                {
                    var oldProjectionMatrix = cam.projectionMatrix;
                    var oldRect = cam.rect;
                    cam.rect  =new UnityEngine.Rect(0f,0f,1f,1f);
                    this.ShiftProjectionMatrix(cam, this.m_samples[i] - new UnityEngine.Vector2(0.5f, 0.5f));
                    cam.Render();
                    cam.projectionMatrix = oldProjectionMatrix;
                    cam.rect = oldRect;

                    accumulateInto = this.m_accumulateRTs[(x + 1) % 2];
                    var accumulatedWith = this.m_accumulateRTs[x % 2];
                    this.m_accumulateMaterial.SetTexture("_PreviousTexture", accumulatedWith);

                    if (this.CameraUsingPartialViewport(cam))
                    {
                        this.m_accumulateMaterial.SetFloat("_OfsX", cam.rect.x );
                        this.m_accumulateMaterial.SetFloat("_OfsY", cam.rect.y );
                        this.m_accumulateMaterial.SetFloat("_Width", cam.rect.width );
                        this.m_accumulateMaterial.SetFloat("_Height", cam.rect.height );
                        this.m_accumulateMaterial.SetFloat("_Scale", cam.targetTexture.width / (float)this.m_renderRT.width );
                    }
                    else
                    {
                        this.m_accumulateMaterial.SetFloat("_OfsX", 0 );
                        this.m_accumulateMaterial.SetFloat("_OfsY", 0 );
                        this.m_accumulateMaterial.SetFloat("_Width", 1 );
                        this.m_accumulateMaterial.SetFloat("_Height", 1 );
                        this.m_accumulateMaterial.SetFloat("_Scale", 1 );
                    }
                    this.m_accumulateMaterial.SetInt("_Pass", i);
                    UnityEngine.Graphics.Blit(cam.targetTexture, accumulateInto, this.m_accumulateMaterial);
                    x++;
                }
            }

            UnityEngine.Graphics.Blit(accumulateInto, this.m_renderRT);
        }

        void SaveRT(UnityEngine.RenderTexture input)
        {
            if (input == null) return;

            var width = input.width;
            var height = input.height;
            
            var tex = new UnityEngine.Texture2D(width, height, UnityEngine.TextureFormat.RGBA32 , false);
            var backupActive = UnityEngine.RenderTexture.active;
            UnityEngine.RenderTexture.active = input;
            tex.ReadPixels(new UnityEngine.Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            UnityEngine.RenderTexture.active = backupActive;

            byte[] bytes;
            bytes = ImageConversion.EncodeToPNG(tex);

            Core.Engine.UnityHelpers.Destroy(tex);

            File.WriteAllBytes("Recorder/DebugDump.png", bytes);
        }
    }
}
