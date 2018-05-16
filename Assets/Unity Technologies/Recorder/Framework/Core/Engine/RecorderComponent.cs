using System.Collections;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    [UnityEngine.ExecuteInEditMode]
    public class RecorderComponent : UnityEngine.MonoBehaviour
    {
        public bool autoExitPlayMode { get; set; }
        public RecordingSession session { get; set; }

        public void Update()
        {
            if (this.session != null && this.session.recording)
                this.session.PrepareNewFrame();
        }

        IEnumerator RecordFrame()
        {
            yield return new UnityEngine.WaitForEndOfFrame();
            if (this.session != null && this.session.recording)
            {
                this.session.RecordFrame();

                switch (this.session.m_Recorder.settings.m_DurationMode)
                {
                    case DurationMode.Manual:
                        break;
                    case DurationMode.SingleFrame:
                    {
                        if (this.session.m_Recorder.recordedFramesCount == 1)
                            this.enabled = false;
                        break;
                    }
                    case DurationMode.FrameInterval:
                    {
                        if (this.session.frameIndex > this.session.settings.m_EndFrame)
                            this.enabled = false;
                        break;
                    }
                    case DurationMode.TimeInterval:
                    {
                        if (this.session.settings.m_FrameRateMode == FrameRateMode.Variable)
                        {
                            if (this.session.m_CurrentFrameStartTS >= this.session.settings.m_EndTime)
                                this.enabled = false;
                        }
                        else
                        {
                            var expectedFrames = (this.session.settings.m_EndTime - this.session.settings.m_StartTime) * this.session.settings.m_FrameRate;
                            if (this.session.RecordedFrameSpan >= expectedFrames)
                                this.enabled = false;
                        }
                        break;
                    }
                }
            }
        }

        public void LateUpdate()
        {
            if (this.session != null && this.session.recording)
                this.StartCoroutine(this.RecordFrame());
        }

        public void OnDisable()
        {
            if (this.session != null)
            {
                this.session.Dispose();
                this.session = null;

#if UNITY_EDITOR
                if (this.autoExitPlayMode)
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

        public void OnDestroy()
        {
            if (this.session != null)
                this.session.Dispose();
        }
    }
}
