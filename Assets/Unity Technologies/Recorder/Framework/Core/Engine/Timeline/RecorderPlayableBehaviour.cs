using System;
using UnityEngine.Playables;

namespace Unity_Technologies.Recorder.Framework.Core.Engine.Timeline
{
    /// <summary>
    /// What is it: Implements a playable that records something triggered by a Timeline Recorder Clip.
    /// Motivation: Allow Timeline to trigger recordings
    /// 
    /// Notes: 
    ///     - Totally ignores the time info comming from the playable infrastructure. Only conciders scaled time.
    ///     - Does not support multiple OnGraphStart...
    ///     - It relies on WaitForEndOfFrameComponent to inform the Session object that it's time to record to frame.
    /// </summary>    
    public class RecorderPlayableBehaviour : PlayableBehaviour
    {
        PlayState m_PlayState = PlayState.Paused;
        public RecordingSession session { get; set; }
        WaitForEndOfFrameComponent endOfFrameComp;
        bool m_FirstOneSkipped;

        public Action OnEnd;

        public override void OnGraphStart(Playable playable)
        {
            if (this.session != null)
            {
                // does not support multiple starts...
                this.session.SessionCreated();
                this.m_PlayState = PlayState.Paused;
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            if (this.session != null && this.session.recording)
            {
                this.session.EndRecording();
                this.session.Dispose();
                this.session = null;

                if (this.OnEnd != null)
                    this.OnEnd();
            }
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (this.session != null && this.session.recording)
            {
                this.session.PrepareNewFrame();
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (this.session != null)
            {
                if (this.endOfFrameComp == null)
                {
                    this.endOfFrameComp = this.session.m_RecorderGO.AddComponent<WaitForEndOfFrameComponent>();
                    this.endOfFrameComp.m_playable = this;
                }
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (this.session == null)
                return;

            // Assumption: OnPlayStateChanged( PlayState.Playing ) ONLY EVER CALLED ONCE for this type of playable.
            this.m_PlayState = PlayState.Playing;
            this.session.BeginRecording();                
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (this.session == null)
                return;

            if (this.session.recording && this.m_PlayState == PlayState.Playing)
            {
                this.session.EndRecording();
                this.session.Dispose();
                this.session = null;

                if (this.OnEnd != null)
                    this.OnEnd();
            }

            this.m_PlayState = PlayState.Paused;
        }

        public void FrameEnded()
        {
            if (this.session != null && this.session.recording)
                this.session.RecordFrame();
        }
    }
}
