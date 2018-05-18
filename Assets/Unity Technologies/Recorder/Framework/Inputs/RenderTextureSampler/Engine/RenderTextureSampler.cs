using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Engine;

namespace Unity_Technologies.Recorder.Framework.Inputs.RenderTextureSampler.Engine
{
    public class RenderTextureSampler : BaseRenderTextureInput
    {
        Shader superShader;
        Shader accumulateShader;
        Shader normalizeShader;

        TextureFlipper m_VFlipper;

        UnityEngine.RenderTexture m_renderRT;
        UnityEngine.RenderTexture[] m_accumulateRTs = new UnityEngine.RenderTexture[2];
        int m_renderWidth, m_renderHeight;

        Material m_superMaterial;
        Material m_accumulateMaterial;
        Material m_normalizeMaterial;

        class HookedCamera
        {
            public Camera camera;
            public UnityEngine.RenderTexture textureBackup;
        }

        List<HookedCamera> m_hookedCameras;

        Vector2[] m_samples;

        RenderTextureSamplerSettings rtsSettings
        {
            get { return (RenderTextureSamplerSettings)this.settings; }
        }

        void GenerateSamplesMSAA(Vector2[] samples, ESuperSamplingCount sc)
        {
            switch (sc)
            {
                case ESuperSamplingCount.x1:
                    samples[0] = new Vector2(0.0f, 0.0f);
                    break;
                case ESuperSamplingCount.x2:
                    samples[0] = new Vector2(4.0f, 4.0f);
                    samples[1] = new Vector2(-4.0f, -4.0f);
                    break;
                case ESuperSamplingCount.x4:
                    samples[0] = new Vector2(-2.0f, -6.0f);
                    samples[1] = new Vector2(6.0f, -2.0f);
                    samples[2] = new Vector2(-6.0f, 2.0f);
                    samples[3] = new Vector2(2.0f, 6.0f);
                    break;
                case ESuperSamplingCount.x8:
                    samples[0] = new Vector2(1.0f, -3.0f);
                    samples[1] = new Vector2(-1.0f, 3.0f);
                    samples[2] = new Vector2(5.0f, 1.0f);
                    samples[3] = new Vector2(-3.0f, -5.0f);

                    samples[4] = new Vector2(-5.0f, 5.0f);
                    samples[5] = new Vector2(-7.0f, -1.0f);
                    samples[6] = new Vector2(3.0f, 7.0f);
                    samples[7] = new Vector2(7.0f, -7.0f);
                    break;
                case ESuperSamplingCount.x16:
                    samples[0] = new Vector2(1.0f, 1.0f);
                    samples[1] = new Vector2(-1.0f, -3.0f);
                    samples[2] = new Vector2(-3.0f, 2.0f);
                    samples[3] = new Vector2(4.0f, -1.0f);

                    samples[4] = new Vector2(-5.0f, -2.0f);
                    samples[5] = new Vector2(2.0f, 5.0f);
                    samples[6] = new Vector2(5.0f, 3.0f);
                    samples[7] = new Vector2(3.0f, -5.0f);

                    samples[8] = new Vector2(-2.0f, 6.0f);
                    samples[9] = new Vector2(0.0f, -7.0f);
                    samples[10] = new Vector2(-4.0f, -6.0f);
                    samples[11] = new Vector2(-6.0f, 4.0f);

                    samples[12] = new Vector2(-8.0f, 0.0f);
                    samples[13] = new Vector2(7.0f, -4.0f);
                    samples[14] = new Vector2(6.0f, 7.0f);
                    samples[15] = new Vector2(-7.0f, -8.0f);
                    break;
                default:
                    Debug.LogError("Not expected sample count: " + sc);
                    return;
            }
            const float oneOverSixteen = 1.0f / 16.0f;
            Vector2 halfHalf = new Vector2(0.5f, 0.5f);
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = samples[i] * oneOverSixteen + halfHalf;
            }
        }

        public override void BeginRecording(RecordingSession session)
        {
            this.superShader = Shader.Find("Hidden/Volund/BS4SuperShader");
            this.accumulateShader = Shader.Find("Hidden/BeautyShot/Accumulate");
            this.normalizeShader = Shader.Find("Hidden/BeautyShot/Normalize");

            if( this.rtsSettings.m_FlipFinalOutput )
                this.m_VFlipper = new TextureFlipper();

            // Below here is considered 'void Start()', but we run it for directly "various reasons".
            if (this.rtsSettings.m_OutputSize > this.rtsSettings.m_RenderSize)
                throw new UnityException("Upscaling is not supported! Output dimension must be smaller or equal to render dimension.");

            // Calculate aspect and render/output sizes
            // Clamp size to 16K, which is the min always supported size in d3d11
            // Force output to divisible by two as x264 doesn't approve of odd image dimensions.
            var aspect = AspectRatioHelper.GetRealAR(this.rtsSettings.m_AspectRatio);
            this.m_renderHeight = (int)this.rtsSettings.m_RenderSize;
            this.m_renderWidth = Mathf.Min(16 * 1024, Mathf.RoundToInt(this.m_renderHeight * aspect));
            this.outputHeight = (int)this.rtsSettings.m_OutputSize;
            this.outputWidth = Mathf.Min(16 * 1024, Mathf.RoundToInt(this.outputHeight * aspect));
            if (this.rtsSettings.m_ForceEvenSize)
            {
                this.outputWidth = (this.outputWidth + 1) & ~1;
                this.outputHeight = (this.outputHeight + 1) & ~1;
            }

            this.m_superMaterial = new Material(this.superShader);
            this.m_superMaterial.hideFlags = HideFlags.DontSave;

            this.m_accumulateMaterial = new Material(this.accumulateShader);
            this.m_accumulateMaterial.hideFlags = HideFlags.DontSave;

            this.m_normalizeMaterial = new Material(this.normalizeShader);
            this.m_normalizeMaterial.hideFlags = HideFlags.DontSave;

            this.m_renderRT = new UnityEngine.RenderTexture(this.m_renderWidth, this.m_renderHeight, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
            this.m_renderRT.wrapMode = TextureWrapMode.Clamp;
            for (int i = 0; i < 2; ++i)
            {
                this.m_accumulateRTs[i] = new UnityEngine.RenderTexture(this.m_renderWidth, this.m_renderHeight, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
                this.m_accumulateRTs[i].wrapMode = TextureWrapMode.Clamp;
                this.m_accumulateRTs[i].Create();
            }
            var rt = new UnityEngine.RenderTexture(this.outputWidth, this.outputHeight, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
            rt.Create();
            this.outputRT = rt;
            this.m_samples = new Vector2[(int)this.rtsSettings.m_SuperSampling];
            this.GenerateSamplesMSAA(this.m_samples, this.rtsSettings.m_SuperSampling);

            this.m_hookedCameras = new List<HookedCamera>();
        }

        public override void NewFrameStarting(RecordingSession session)
        {
            switch (this.rtsSettings.source)
            {
                case EImageSource.ActiveCameras:
                {
                    bool sort = false;

                    // Find all cameras targetting Display
                    foreach (var cam in Resources.FindObjectsOfTypeAll<Camera>())
                    {
                        var hookedCam = this.m_hookedCameras.Find((x) => cam == x.camera);
                        if (hookedCam != null)
                        {
                            // Should we keep it?
                            if (cam.targetDisplay != 0 || !cam.enabled)
                            {
                                UnityHelpers.Destroy(cam.targetTexture);
                                cam.targetTexture = hookedCam.textureBackup;
                                this.m_hookedCameras.Remove(hookedCam);
                            }
                            continue;
                        }

                        if (!cam.enabled || !cam.gameObject.activeInHierarchy || cam.targetDisplay != 0)
                            continue;

                        hookedCam = new HookedCamera() { camera = cam, textureBackup = cam.targetTexture };
                        var camRT = new UnityEngine.RenderTexture((int)(this.m_renderWidth * cam.rect.width), (int)(this.m_renderHeight * cam.rect.height), 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
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
                case EImageSource.MainCamera:
                {
                    var cam = Camera.main;
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
                case EImageSource.TaggedCamera:
                {
                    GameObject[] taggedObjs;
                    var tag = (this.settings as RenderTextureSamplerSettings).m_CameraTag;
                    try
                    {
                        taggedObjs = GameObject.FindGameObjectsWithTag(tag);
                    }
                    catch (UnityException)
                    {
                        Debug.LogWarning("No camera has the requested target tag:" + tag);
                        taggedObjs = new GameObject[0];
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
                        var cam = taggedObjs[i].transform.GetComponent<Camera>();
                        if (cam != null && cam.enabled)
                        {
                            for (var j = 0; j < this.m_hookedCameras.Count; j++)
                            {
                                if (this.m_hookedCameras[j].camera == taggedObjs[i].transform.GetComponent<Camera>())
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
                                UnityHelpers.Destroy(c.camera.targetTexture);
                            c.camera.targetTexture = c.textureBackup;
                        }
                    }
                    this.m_hookedCameras.Clear();
                }

                UnityHelpers.Destroy(this.m_renderRT);
                foreach (var rt in this.m_accumulateRTs)
                    UnityHelpers.Destroy(rt);
                UnityHelpers.Destroy(this.m_superMaterial);
                UnityHelpers.Destroy(this.m_accumulateMaterial);
                UnityHelpers.Destroy(this.m_normalizeMaterial);
                if(this.m_VFlipper != null)
                    this.m_VFlipper.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void NewFrameReady(RecordingSession session)
        {
            this.PerformSubSampling();

            if (this.rtsSettings.m_RenderSize == this.rtsSettings.m_OutputSize)
            {
                // Blit with normalization if sizes match.
                this.m_normalizeMaterial.SetFloat("_NormalizationFactor", 1.0f / (float)this.rtsSettings.m_SuperSampling);
                this.m_normalizeMaterial.SetInt("_ApplyGammaCorrection", QualitySettings.activeColorSpace == ColorSpace.Linear && this.rtsSettings.m_ColorSpace == ColorSpace.Gamma ? 1 : 0);
                Graphics.Blit(this.m_renderRT, this.outputRT, this.m_normalizeMaterial);
            }
            else
            {
                // Ideally we would use a separable filter here, but we're massively bound by readback and disk anyway for hi-res.
                this.m_superMaterial.SetVector("_Target_TexelSize", new Vector4(1f / this.outputWidth, 1f / this.outputHeight, this.outputWidth, this.outputHeight));
                this.m_superMaterial.SetFloat("_KernelCosPower", this.rtsSettings.m_SuperKernelPower);
                this.m_superMaterial.SetFloat("_KernelScale", this.rtsSettings.m_SuperKernelScale);
                this.m_superMaterial.SetFloat("_NormalizationFactor", 1.0f / (float)this.rtsSettings.m_SuperSampling);
                this.m_superMaterial.SetInt("_ApplyGammaCorrection", QualitySettings.activeColorSpace == ColorSpace.Linear && this.rtsSettings.m_ColorSpace == ColorSpace.Gamma ? 1 : 0);
                Graphics.Blit(this.m_renderRT, this.outputRT, this.m_superMaterial);
            }

            if (this.rtsSettings.m_FlipFinalOutput)
                this.m_VFlipper.Flip(this.outputRT);
        }

        void ShiftProjectionMatrix(Camera camera, Vector2 sample)
        {
            camera.ResetProjectionMatrix();
            Matrix4x4 projectionMatrix = camera.projectionMatrix;
            float dx = sample.x / this.m_renderWidth;
            float dy = sample.y / this.m_renderHeight;
            projectionMatrix.m02 += dx;
            projectionMatrix.m12 += dy;
            camera.projectionMatrix = projectionMatrix;
        }

        bool CameraUsingPartialViewport(Camera cam)
        {
            return cam.rect.width != 1 || cam.rect.height != 1 || cam.rect.x != 0 || cam.rect.y != 0;
        }

        void PerformSubSampling()
        {
            UnityEngine.RenderTexture accumulateInto = null;
            this.m_renderRT.wrapMode = TextureWrapMode.Clamp;
            this.m_renderRT.filterMode = FilterMode.Point;

            int x = 0;
            Graphics.SetRenderTarget(this.m_accumulateRTs[0]);
            GL.Clear(false, true, Color.black);

            foreach (var hookedCam in this.m_hookedCameras)
            {
                var cam = hookedCam.camera;

                for (int i = 0, n = (int)this.rtsSettings.m_SuperSampling; i < n; i++)
                {
                    var oldProjectionMatrix = cam.projectionMatrix;
                    var oldRect = cam.rect;
                    cam.rect  =new Rect(0f,0f,1f,1f);
                    this.ShiftProjectionMatrix(cam, this.m_samples[i] - new Vector2(0.5f, 0.5f));
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
                    Graphics.Blit(cam.targetTexture, accumulateInto, this.m_accumulateMaterial);
                    x++;
                }
            }

            Graphics.Blit(accumulateInto, this.m_renderRT);
        }

        void SaveRT(UnityEngine.RenderTexture input)
        {
            if (input == null) return;

            var width = input.width;
            var height = input.height;
            
            var tex = new Texture2D(width, height, TextureFormat.RGBA32 , false);
            var backupActive = UnityEngine.RenderTexture.active;
            UnityEngine.RenderTexture.active = input;
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            UnityEngine.RenderTexture.active = backupActive;

            byte[] bytes;
            bytes = ImageConversion.EncodeToPNG(tex);

            UnityHelpers.Destroy(tex);

            File.WriteAllBytes("Recorder/DebugDump.png", bytes);
        }
    }
}
