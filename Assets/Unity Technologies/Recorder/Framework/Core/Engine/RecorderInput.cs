using System;

namespace Unity_Technologies.Recorder.Framework.Core.Engine
{

    /// <summary>
    /// What is this: 
    /// Motivation  : 
    /// Notes: 
    /// </summary>    
    public class RecorderInput : IDisposable
    {
        public int SourceID { get; set; }
        public RecorderInputSetting settings { get; set; }

        ~RecorderInput()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
        }

        public virtual void SessionCreated(RecordingSession session) {}

        public virtual void BeginRecording(RecordingSession session) {}

        public virtual void NewFrameStarting(RecordingSession session) {}

        public virtual void NewFrameReady(RecordingSession session) {}

        public virtual void FrameDone(RecordingSession session) {}

        public virtual void EndRecording(RecordingSession session) {}
    }
}
