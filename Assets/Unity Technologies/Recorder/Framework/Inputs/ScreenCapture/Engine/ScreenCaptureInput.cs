#if UNITY_2017_3_OR_NEWER

using UnityEngine;
using Unity_Technologies.Recorder.Framework.Core.Engine;
using Unity_Technologies.Recorder.Framework.Inputs.CBRenderTexture.Engine;

namespace Unity_Technologies.Recorder.Framework.Inputs.ScreenCapture.Engine
{
    public class ScreenCaptureInput : RecorderInput
    {
        bool m_ModifiedResolution;

        public Texture2D image { get; private set; }

        public ScreenCaptureInputSettings scSettings
        {
            get { return (ScreenCaptureInputSettings)this.settings; }
        }

        public int outputWidth { get; protected set; }
        public int outputHeight { get; protected set; }

        public override void NewFrameReady(RecordingSession session)
        {
            this.image = UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();
        }

        public override void BeginRecording(RecordingSession session)
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
#if UNITY_EDITOR
            switch (this.scSettings.m_OutputSize)
            {
                case EImageDimension.Window:
                {
                    GameViewSize.GetGameRenderSize(out screenWidth, out screenHeight);
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
                    this.outputWidth = (int)(this.outputHeight * AspectRatioHelper.GetRealAR(this.scSettings.m_AspectRatio));

                    if (this.scSettings.m_ForceEvenSize)
                    {
                        this.outputWidth = (this.outputWidth + 1) & ~1;
                        this.outputHeight = (this.outputHeight + 1) & ~1;
                    }

                    break;
                }
            }

            int w, h;
            GameViewSize.GetGameRenderSize(out w, out h);
            if (w != this.outputWidth || h != this.outputHeight)
            {
                var size = GameViewSize.SetCustomSize(this.outputWidth, this.outputHeight) ?? GameViewSize.AddSize(this.outputWidth, this.outputHeight);
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
            }
#endif

        }

        public override void FrameDone(RecordingSession session)
        {
            UnityHelpers.Destroy(this.image);
            this.image = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if UNITY_EDITOR
                if (this.m_ModifiedResolution)
                {
                    GameViewSize.m_ModifiedResolutionCount--;
                    if (GameViewSize.m_ModifiedResolutionCount == 0)
                        GameViewSize.RestoreSize();
                }
#endif
            }

            base.Dispose(disposing);
        }
    }
}

#endif