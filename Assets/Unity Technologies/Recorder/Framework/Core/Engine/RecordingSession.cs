using System;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public class RecordingSession : IDisposable
    {
        public Recorder m_Recorder;
        public UnityEngine.GameObject m_RecorderGO;

        public double m_CurrentFrameStartTS;
        public double m_RecordingStartTS;
        int m_FrameIndex = 0;
        int m_InitialFrame = 0;
        int m_FirstRecordedFrameCount = -1;
        float m_FPSTimeStart;
        float m_FPSNextTimeStart;
        int m_FPSNextFrameCount;

        public DateTime m_SessionStartTS;

        public RecorderSettings settings
        {
            get { return this.m_Recorder.settings; }
        }

        public bool recording
        {
            get { return this.m_Recorder.recording; }
        }

        public int frameIndex
        {
            get { return this.m_FrameIndex; }
        }

        public int RecordedFrameSpan
        {
            get { return this.m_FirstRecordedFrameCount == -1 ? 0 : UnityEngine.Time.renderedFrameCount - this.m_FirstRecordedFrameCount; }
        }

        public float recorderTime
        {
            get { return (float)(this.m_CurrentFrameStartTS - this.settings.m_StartTime); }
        }

        void AllowInBackgroundMode()
        {
            if (!UnityEngine.Application.runInBackground)
            {
                UnityEngine.Application.runInBackground = true;
                if (Verbose.enabled)
                    UnityEngine.Debug.Log("Recording sessions is enabling Application.runInBackground!");
            }
        }

        public bool SessionCreated()
        {
            try
            {
                this.AllowInBackgroundMode();
                this.m_RecordingStartTS = (UnityEngine.Time.time / UnityEngine.Time.timeScale);
                this.m_SessionStartTS = DateTime.Now;
                this.m_Recorder.SessionCreated(this);
                return true;

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                return false;
            }
        }


        public bool BeginRecording()
        {
            try
            {
                if (!this.settings.isPlatformSupported)
                {
                    UnityEngine.Debug.LogError(string.Format("Recorder {0} does not support current platform", this.m_Recorder.GetType().Name));
                    return false;
                }

                this.AllowInBackgroundMode();

                this.m_RecordingStartTS = (UnityEngine.Time.time / UnityEngine.Time.timeScale);
                this.m_Recorder.SignalInputsOfStage(ERecordingSessionStage.BeginRecording, this);

                if (!this.m_Recorder.BeginRecording(this))
                    return false;
                this.m_InitialFrame = UnityEngine.Time.renderedFrameCount;
                this.m_FPSTimeStart = UnityEngine.Time.unscaledTime;

                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                return false;
            }
        }

        public virtual void EndRecording()
        {
            try
            {
                this.m_Recorder.SignalInputsOfStage(ERecordingSessionStage.EndRecording, this);
                this.m_Recorder.EndRecording(this);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void RecordFrame()
        {
            try
            {
                this.m_Recorder.SignalInputsOfStage(ERecordingSessionStage.NewFrameReady, this);
                if (!this.m_Recorder.SkipFrame(this))
                {
                    this.m_Recorder.RecordFrame(this);
                    this.m_Recorder.recordedFramesCount++;
                    if (this.m_Recorder.recordedFramesCount == 1)
                        this.m_FirstRecordedFrameCount = UnityEngine.Time.renderedFrameCount;
                }
                this.m_Recorder.SignalInputsOfStage(ERecordingSessionStage.FrameDone, this);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            // Note: This is not great when multiple recorders are simultaneously active...
            if (this.m_Recorder.settings.m_FrameRateMode == FrameRateMode.Variable ||
                (this.m_Recorder.settings.m_FrameRateMode == FrameRateMode.Constant && this.m_Recorder.settings.m_SynchFrameRate))
            {
                var frameCount = UnityEngine.Time.renderedFrameCount - this.m_InitialFrame;
                var frameLen = 1.0f / this.m_Recorder.settings.m_FrameRate;
                var elapsed = UnityEngine.Time.unscaledTime - this.m_FPSTimeStart;
                var target = frameLen * (frameCount + 1);
                var sleep = (int)((target - elapsed) * 1000);

                if (sleep > 2)
                {
                    if (Verbose.enabled)
                        UnityEngine.Debug.Log(string.Format("Recording session info => dT: {0:F1}s, Target dT: {1:F1}s, Retarding: {2}ms, fps: {3:F1}", elapsed, target, sleep, frameCount / elapsed));
                    System.Threading.Thread.Sleep(Math.Min(sleep, 1000));
                }
                else if (sleep < -frameLen)
                    this.m_InitialFrame--;
                else if (Verbose.enabled)
                    UnityEngine.Debug.Log(string.Format("Recording session info => fps: {0:F1}", frameCount / elapsed));

                // reset every 30 frames
                if (frameCount % 50 == 49)
                {
                    this.m_FPSNextTimeStart = UnityEngine.Time.unscaledTime;
                    this.m_FPSNextFrameCount = UnityEngine.Time.renderedFrameCount;
                }
                if (frameCount % 100 == 99)
                {
                    this.m_FPSTimeStart = this.m_FPSNextTimeStart;
                    this.m_InitialFrame = this.m_FPSNextFrameCount;
                }
            }
            this.m_FrameIndex++;
        }

        public void PrepareNewFrame()
        {
            try
            {
                this.AllowInBackgroundMode();

                this.m_CurrentFrameStartTS = (UnityEngine.Time.time / UnityEngine.Time.timeScale) - this.m_RecordingStartTS;
                this.m_Recorder.SignalInputsOfStage(ERecordingSessionStage.NewFrameStarting, this);
                this.m_Recorder.PrepareNewFrame(this);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void Dispose()
        {
            if (this.m_Recorder != null)
            {
                try
                {
                    if (this.recording)
                        this.EndRecording();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }

                UnityHelpers.Destroy(this.m_Recorder);
                UnityHelpers.Destroy(this.m_RecorderGO);
            }
        }
    }
}
