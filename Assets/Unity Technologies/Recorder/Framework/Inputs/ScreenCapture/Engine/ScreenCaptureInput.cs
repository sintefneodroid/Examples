#if UNITY_2017_3_OR_NEWER

using UnityEngine;

namespace Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine
{
    public class ScreenCaptureInput : Core.Engine.RecorderInput
    {
        bool m_ModifiedResolution;

        public Texture2D image { get; private set; }

        public ScreenCaptureInputSettings scSettings
        {
            get { return (ScreenCaptureInputSettings)this.settings; }
        }

        public int outputWidth { get; protected set; }
        public int outputHeight { get; protected set; }

        public override void NewFrameReady(Core.Engine.RecordingSession session)
        {
            this.image = UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();
        }

        public override void BeginRecording(Core.Engine.RecordingSession session)
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
#if UNITY_EDITOR
            switch (this.scSettings.m_OutputSize)
            {
                case Core.Engine.EImageDimension.Window:
                {
                    CBRenderTexture.Engine.GameViewSize.GetGameRenderSize(out screenWidth, out screenHeight);
                    this.outputWidth = screenWidth;
                    this.outputHeight = screenHeight;

                    if (this.scSettings.m_ForceEvenSize)
                    {
                        this.outputWidth = (this.outputWidth + 1) & ~1;
                        this.outputHeight = (this.outputHeight + 1) & ~1;
                    }
                    break;
                }

                default:
                {
                    this.outputHeight = (int)this.scSettings.m_OutputSize;
                    this.outputWidth = (int)(this.outputHeight * Core.Engine.AspectRatioHelper.GetRealAR(this.scSettings.m_AspectRatio));

                    if (this.scSettings.m_ForceEvenSize)
                    {
                        this.outputWidth = (this.outputWidth + 1) & ~1;
                        this.outputHeight = (this.outputHeight + 1) & ~1;
                    }

                    break;
                }
            }

            int w, h;
            CBRenderTexture.Engine.GameViewSize.GetGameRenderSize(out w, out h);
            if (w != this.outputWidth || h != this.outputHeight)
            {
                var size = CBRenderTexture.Engine.GameViewSize.SetCustomSize(this.outputWidth, this.outputHeight) ?? CBRenderTexture.Engine.GameViewSize.AddSize(this.outputWidth, this.outputHeight);
                if (CBRenderTexture.Engine.GameViewSize.m_ModifiedResolutionCount == 0)
                    CBRenderTexture.Engine.GameViewSize.BackupCurrentSize();
                else
                {
                    if (size != CBRenderTexture.Engine.GameViewSize.currentSize)
                    {
                        Debug.LogError("Requestion a resultion change while a recorder's input has already requested one! Undefined behaviour.");
                    }
                }
                CBRenderTexture.Engine.GameViewSize.m_ModifiedResolutionCount++;
                this.m_ModifiedResolution = true;
                CBRenderTexture.Engine.GameViewSize.SelectSize(size);
            }
#endif

        }

        public override void FrameDone(Core.Engine.RecordingSession session)
        {
            Core.Engine.UnityHelpers.Destroy(this.image);
            this.image = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if UNITY_EDITOR
                if (this.m_ModifiedResolution)
                {
                    CBRenderTexture.Engine.GameViewSize.m_ModifiedResolutionCount--;
                    if (CBRenderTexture.Engine.GameViewSize.m_ModifiedResolutionCount == 0)
                        CBRenderTexture.Engine.GameViewSize.RestoreSize();
                }
#endif
            }

            base.Dispose(disposing);
        }
    }
}

#endif