using System;
using System.Collections.Generic;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{
    public enum ERecordingSessionStage
    {
        BeginRecording,
        NewFrameStarting,
        NewFrameReady,
        FrameDone,
        EndRecording,
        SessionCreated
    }

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public abstract class Recorder : UnityEngine.ScriptableObject
    {
        static int sm_CaptureFrameRateCount;
        bool m_ModifiedCaptureFR;

        public int recordedFramesCount { get; set; }
        
        protected List<RecorderInput> m_Inputs;

        public virtual void Awake()
        {
            sm_CaptureFrameRateCount = 0;
        }

        public virtual void Reset()
        {
            this.recordedFramesCount = 0;
            this.recording = false;
        }

        protected virtual void OnDestroy()
        {
            if (this.m_ModifiedCaptureFR )
            {
                sm_CaptureFrameRateCount--;
                if (sm_CaptureFrameRateCount == 0)
                {
                    UnityEngine.Time.captureFramerate = 0;
                    if (Verbose.enabled)
                        UnityEngine.Debug.Log("Recorder resetting 'CaptureFrameRate' to zero");
                }
            }
        }

        public abstract RecorderSettings settings { get; set; }

        public virtual void SessionCreated(RecordingSession session)
        {
            if (Verbose.enabled)
                UnityEngine.Debug.Log(string.Format("Recorder {0} session created", this.GetType().Name));

            this.settings.SelfAdjustSettings(); // ignore return value.

            var fixedRate = this.settings.m_FrameRateMode == FrameRateMode.Constant ? (int)this.settings.m_FrameRate : 0;
            if (fixedRate > 0)
            {
                if (UnityEngine.Time.captureFramerate != 0 && fixedRate != UnityEngine.Time.captureFramerate )
                    UnityEngine.Debug.LogError(string.Format("Recorder {0} is set to record at a fixed rate and another component has already set a conflicting value for [Time.captureFramerate], new value being applied : {1}!", this.GetType().Name, fixedRate));
                else if( UnityEngine.Time.captureFramerate == 0 && Verbose.enabled )
                    UnityEngine.Debug.Log("Frame recorder set fixed frame rate to " + fixedRate);

                UnityEngine.Time.captureFramerate = (int)fixedRate;

                sm_CaptureFrameRateCount++;
                this.m_ModifiedCaptureFR = true;
            }

            this.m_Inputs = new List<RecorderInput>();
            foreach (var inputSettings in this.settings.inputsSettings)
            {
                var input = Activator.CreateInstance(inputSettings.inputType) as RecorderInput;
                input.settings = inputSettings;
                this.m_Inputs.Add(input);
                this.SignalInputsOfStage(ERecordingSessionStage.SessionCreated, session);
            }
        }

        public virtual bool BeginRecording(RecordingSession session)
        {
            if (this.recording)
                throw new Exception("Already recording!");

            if (Verbose.enabled)
                UnityEngine.Debug.Log(string.Format("Recorder {0} starting to record", this.GetType().Name));
         
            return this.recording = true;
        }

        public virtual void EndRecording(RecordingSession ctx)
        {
            if (!this.recording)
                return;
            this.recording = false;

            if (this.m_ModifiedCaptureFR )
            {
                this.m_ModifiedCaptureFR = false;
                sm_CaptureFrameRateCount--;
                if (sm_CaptureFrameRateCount == 0)
                {
                    UnityEngine.Time.captureFramerate = 0;
                    if (Verbose.enabled)
                        UnityEngine.Debug.Log("Recorder resetting 'CaptureFrameRate' to zero");
                }
            }

            foreach (var input in this.m_Inputs)
            {
                if (input is IDisposable)
                    (input as IDisposable).Dispose();
            }

            if(Verbose.enabled)
                UnityEngine.Debug.Log(string.Format("{0} recording stopped, total frame count: {1}", this.GetType().Name, this.recordedFramesCount));
        }
        public abstract void RecordFrame(RecordingSession ctx);
        public virtual void PrepareNewFrame(RecordingSession ctx)
        {
        }

        public virtual bool SkipFrame(RecordingSession ctx)
        {
            return !this.recording 
                || (ctx.frameIndex % this.settings.m_CaptureEveryNthFrame) != 0 
                || ( this.settings.m_DurationMode == DurationMode.TimeInterval && ctx.m_CurrentFrameStartTS < this.settings.m_StartTime )
                || ( this.settings.m_DurationMode == DurationMode.FrameInterval && ctx.frameIndex < this.settings.m_StartFrame )
                || ( this.settings.m_DurationMode == DurationMode.SingleFrame && ctx.frameIndex < this.settings.m_StartFrame );
        }

        public bool recording { get; protected set; }

        public void SignalInputsOfStage(ERecordingSessionStage stage, RecordingSession session)
        {
            if (this.m_Inputs == null)
                return;

            switch (stage)
            {
                case ERecordingSessionStage.SessionCreated:
                    foreach( var input in this.m_Inputs )
                        input.SessionCreated(session);
                    break;
                case ERecordingSessionStage.BeginRecording:
                    foreach( var input in this.m_Inputs )
                        input.BeginRecording(session);
                    break;
                case ERecordingSessionStage.NewFrameStarting:
                    foreach( var input in this.m_Inputs )
                        input.NewFrameStarting(session);
                    break;
                case ERecordingSessionStage.NewFrameReady:
                    foreach( var input in this.m_Inputs )
                        input.NewFrameReady(session);
                    break;
                case ERecordingSessionStage.FrameDone:
                    foreach( var input in this.m_Inputs )
                        input.FrameDone(session);
                    break;
                case ERecordingSessionStage.EndRecording:
                    foreach( var input in this.m_Inputs )
                        input.EndRecording(session);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage", stage, null);
            }
        }
    }
}
